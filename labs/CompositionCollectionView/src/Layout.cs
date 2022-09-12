// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using static CommunityToolkit.Labs.WinUI.CompositionCollectionView.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public interface ILayout
{
    Action<ILayout, ILayout>? LayoutReplaced { get; set; }

    void Activate(Panel panel);
}

public abstract partial class Layout<TId, TItem> : ILayout, IDisposable
{
    public enum ElementAlignment { Start, Center, End };

    public Layout(Func<TId, FrameworkElement> elementFactory, Action<string>? log)
    {
        ElementFactory = elementFactory;
        _log = log;

        var compositor = Window.Current.Compositor;
        Properties = new BindableCompositionPropertySet(compositor.CreatePropertySet());
        AnimatableNodes = new AnimatableCompositionNodeSet(compositor);
    }

    public Layout(Layout<TId, TItem> sourceLayout)
    {
        ParentLayout = sourceLayout;

        ElementFactory = sourceLayout.ElementFactory;

        _uiRoot = sourceLayout._uiRoot;
        Properties = sourceLayout.Properties;
        AnimatableNodes = sourceLayout.AnimatableNodes;

        HorizontalAlignment = sourceLayout.HorizontalAlignment;

        _log = sourceLayout._log;
    }

    public void Activate(Panel panel)
    {
        var rootPanelVisual = InitializeRootContainer(panel);

        _uiRoot = new(
            panel,
            rootPanelVisual);

        Activate();

        Visual InitializeRootContainer(Panel root)
        {
            var rootContainer = ElementCompositionPreview.GetElementVisual(root);
            rootContainer.Size = new Vector2((float)root.ActualWidth, (float)root.ActualHeight);
            return rootContainer;
        }
    }

    private void Activate()
    {
        //The parent layout should only be accessible before we transition to the current layout,
        //once we activate the current layout we dispose and stop referencing it
        ParentLayout?.Dispose();
        ParentLayout = null;
        IsActive = true;

        RootPanel.SizeChanged += RootSizeChanged;

        foreach (var behavior in _behaviors)
        {
            behavior.Configure(this);
        }

        OnActivated();

        foreach (var behavior in _behaviors)
        {
            behavior.OnActivated();
        }
    }

    private void Deactivate()
    {
        OnDeactivated();

        IsActive = false;

        RootPanel.SizeChanged -= RootSizeChanged;

        foreach (var behavior in _behaviors)
        {
            behavior.OnDeactivated();
        }
    }

    private void RootSizeChanged(object sender, SizeChangedEventArgs e)
    {
        OnElementsUpdated();
    }

    //Public properties

    public IEnumerable<TId> Source => Elements.Keys;
    public Action<ILayout, ILayout>? LayoutReplaced { get; set; }

    //Public methods
    public T TransitionTo<T>(Func<Layout<TId, TItem>, T> factory) where T : Layout<TId, TItem>
    {
        System.Diagnostics.Debug.Assert(IsActive);

        Deactivate();

        var newLayout = factory(this);

        foreach (var behavior in _behaviors)
        {
            newLayout.AddBehavior(behavior);
        }

        newLayout.Activate();

        TransferElements();

        LayoutReplaced?.Invoke(this, newLayout);

        return newLayout;

        void TransferElements()
        {
            foreach (var (id, element) in Elements)
            {
                element.ReasignTo(newLayout);
                newLayout.Elements.Add(id, element);
                CleanupElement(element);
                CleanupElementBehaviors(element);
            }

            //Configure the animations after all the elements have been added to the new layout,
            //to allow elements to depend on each other
            foreach (var (id, element) in Elements)
            {
                newLayout.ConfigureElement(element);
                newLayout.ConfigureElementBehaviors(element);

                var currentPosition = GetElementPositionValue(element);
                var currentScale = GetElementScaleValue(element);
                var currentOrientation = GetElementOrientationValue(element);
                var currentOpacity = GetElementOpacityValue(element);

                var transition = newLayout.GetElementTransitionEasingFunction(element);
                if (transition is not null)
                {
                    TaskCompletionSource<bool> tsc = new();
                    newLayout.StopElementAnimation(element);

                    var progressAnimation = Compositor.CreateScalarKeyFrameAnimation();
                    progressAnimation.Duration = TimeSpan.FromMilliseconds(transition.Length);
                    progressAnimation.StopBehavior = AnimationStopBehavior.SetToFinalValue;
                    progressAnimation.InsertKeyFrame(0, 0f, transition.EasingFunction);
                    progressAnimation.InsertKeyFrame(1, 1f, transition.EasingFunction);

                    var animProgressNode = new AnimatableScalarCompositionNode(Compositor);

                    element.Visual.StartAnimation(Offset, ExpressionFunctions.Lerp(currentPosition, newLayout.GetElementPositionNode(element), animProgressNode.Reference));
                    var scale = newLayout.GetElementScaleNode(element);
                    element.Visual.StartAnimation(Scale, ExpressionFunctions.Lerp(new Vector3(currentScale), ExpressionFunctions.Vector3(scale, scale, scale), animProgressNode.Reference));
                    element.Visual.StartAnimation(AnimationConstants.Orientation, ExpressionFunctions.Slerp(currentOrientation, newLayout.GetElementOrientationNode(element), animProgressNode.Reference));
                    element.Visual.StartAnimation(Opacity, ExpressionFunctions.Lerp(currentOpacity, newLayout.GetElementOpacityNode(element), animProgressNode.Reference));

                    var batch = Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                    batch.Completed += (object _, CompositionBatchCompletedEventArgs _1) =>
                    {
                        if (newLayout.Elements.ContainsKey(element.Id))
                        {
                            newLayout.ConfigureElementAnimation(element);
                        }
                        tsc.SetResult(true);

                        batch.Dispose();
                        animProgressNode.Dispose();
                        progressAnimation.Dispose();
                    };

                    animProgressNode.Animate(progressAnimation);

                    batch.End();
                }
                else
                {
                    newLayout.ConfigureElementAnimation(element);
                }

                newLayout.UpdateElement(element);
            }

            Elements.Clear();

            newLayout.OnElementsUpdated();
        }
    }

    public ElementReference<TId, TItem>? GetElement(TId obId) =>
        Elements.TryGetValue(obId, out var element) ? element : null;

    //Protected properties
    protected ScalarNode ViewportWidthNode => RootPanelVisual.GetReference().Size.X;
    protected ScalarNode ViewportHeightNode => RootPanelVisual.GetReference().Size.Y;

    //Customizable behaviors
    protected virtual void OnActivated() { }
    protected virtual void OnDeactivated() { }
    protected virtual void OnElementsUpdated() { }

    protected virtual void ConfigureElement(ElementReference<TId, TItem> element) { }
    protected virtual void CleanupElement(ElementReference<TId, TItem> element) { }
    public abstract Vector3Node GetElementPositionNode(ElementReference<TId, TItem> element);
    public virtual ScalarNode GetElementScaleNode(ElementReference<TId, TItem> element) => 1;

    public virtual ScalarNode GetElementOpacityNode(ElementReference<TId, TItem> element) => 1;
    public virtual QuaternionNode GetElementOrientationNode(ElementReference<TId, TItem> element) => Quaternion.Identity;
    protected virtual Transition? GetElementTransitionEasingFunction(ElementReference<TId, TItem> element) => null;

    //Invoked per-element as part of a source update, before its animation has been updated. The user is intended to update the composition property set/animation nodes
    public virtual void UpdateElementData(ElementReference<TId, TItem> element) { }
    //Invoked per-element as part of a source update, after its animation has been updated
    public virtual void UpdateElement(ElementReference<TId, TItem> element) { }


    //Internal implementation, the copy constructor should copy the properties we want to maintain across layouts
    protected Func<TId, FrameworkElement> ElementFactory { get; init; }

    private record UIRoot(Panel RootPanel, Visual RootPanelVisual);
    private UIRoot? _uiRoot;

    private UIRoot GetVisualProperties()
    {
        if (_uiRoot is null)
        {
            throw new InvalidOperationException("Tried to use this layout when it hasn't been activated yet.");
        }
        return _uiRoot;
    }

    public Panel RootPanel => GetVisualProperties().RootPanel;
    public Visual RootPanelVisual => GetVisualProperties().RootPanelVisual;
    public Compositor Compositor => GetVisualProperties().RootPanelVisual.Compositor;
    protected Dictionary<TId, ElementReference<TId, TItem>> Elements { get; } = new Dictionary<TId, ElementReference<TId, TItem>>();
    public BindableCompositionPropertySet Properties { private init; get; }
    public AnimatableCompositionNodeSet AnimatableNodes { private init; get; }
    //public bool IsUserInteractingWithTracker { get; private set; } = false;

    //internal Func<bool>? _shouldAcceptTouchInput;
    internal Action<string>? _log;


    //private PointerEventHandler? _rootPointerPressedHandler;
    //private PointerEventHandler? _rootPointerReleasedHandler;
    //private PointerEventHandler? _rootPointerExitedHandler;
    //private PointerEventHandler? _rootPointerCanceledHandler;
    //private PointerEventHandler? _rootPointerCaptureLostHandler;


    public Layout<TId, TItem>? ParentLayout { get; private set; }

    protected List<LayoutBehavior<TId, TItem>> _behaviors = new List<LayoutBehavior<TId, TItem>>();

    //internal static Dictionary<uint, LayoutPointer<TId>> ActiveTouchPointers { get; } = new();

    public bool IsActive { get; private set; } = false;

    private bool disposedValue;

    private void ConfigureElementBehaviors(ElementReference<TId, TItem> elementReference)
    {
        foreach (var behavior in _behaviors)
        {
            behavior.ConfigureElement(elementReference);
        }
    }

    private void CleanupElementBehaviors(ElementReference<TId, TItem> elementReference)
    {
        foreach (var behavior in _behaviors)
        {
            behavior.CleanupElement(elementReference);
        }
    }

    public void ConfigureElementAnimation(ElementReference<TId, TItem> element, Visual? proxyVisual = null)
    {
        var visual = proxyVisual ?? element.Visual;

        visual.StartAnimation(Offset, GetElementPositionNode(element));

        var scale = GetElementScaleNode(element);
        visual.StartAnimation(Scale, ExpressionFunctions.Vector3(scale, scale, scale));

        visual.StartAnimation(AnimationConstants.Orientation, GetElementOrientationNode(element));

        visual.StartAnimation(Opacity, GetElementOpacityNode(element));
    }

    public void StopElementsAnimation()
    {
        foreach (var (_, element) in Elements)
        {
            StopElementAnimation(element);
        }
    }

    public void StopElementAnimation(ElementReference<TId, TItem> element)
    {
        element.Visual.StopAnimation(Offset);
        element.Visual.StopAnimation(Scale);
        element.Visual.StopAnimation(Opacity);
        element.Visual.StopAnimation(AnimationConstants.Orientation);
    }

    public IEnumerable<ElementReference<TId, TItem>> GetElements() => Elements.Values;

    bool _isUpdatingSource = false;
    private record SourceUpdate(IDictionary<TId, TItem> UpdatedElements, Action? Callback);
    Queue<SourceUpdate> _pendingSourceUpdates = new();

    public void UpdateSource(IDictionary<TId, TItem> source, Action? updateCallback = null)
    {
        if (!_isUpdatingSource)
        {
            _isUpdatingSource = true;
            ProcessSourceUpdate(source, updateCallback);
        }
        else
        {
            _pendingSourceUpdates.Enqueue(new SourceUpdate(source, updateCallback));
        }
    }

    public async void ProcessSourceUpdate(IDictionary<TId, TItem> updatedElements, Action? updateCallback)
    {
        List<Task<bool>> elementUpdateTask = new();

        System.Diagnostics.Debug.WriteLine("Process source update");

        HashSet<TId> processedElements = new();

        foreach (var (id, element) in Elements.ToArray())
        {
            if (!updatedElements.ContainsKey(id))
            {
                DestroyElement(element, id);
            }
            else
            {
                UpdateAndTransitionElement(element, updatedElements[id]);
            }
        }
        foreach (var (id, model) in updatedElements)
        {
            if (processedElements.Contains(id))
            {
                continue;
            }
            InstantiateElement(id, model);
        }

        updateCallback?.Invoke();

        await Task.WhenAll(elementUpdateTask);

        if (_pendingSourceUpdates.Count > 0)
        {
            var update = _pendingSourceUpdates.Dequeue();
            ProcessSourceUpdate(update.UpdatedElements, update.Callback);
        }
        else
        {
            _isUpdatingSource = false;
        }

        OnElementsUpdated();

        void InstantiateElement(TId id, TItem item)
        {
            var element = ElementFactory(id);
            RootPanel.Children.Add(element);

            var elementReference = new ElementReference<TId, TItem>(id, item, element,/* source, tracker, trackerOwner,*/ this);
            UpdateElementData(elementReference);
            Elements[id] = elementReference;

            ConfigureElement(elementReference);
            ConfigureElementBehaviors(elementReference);

            ConfigureElementAnimation(elementReference);
            UpdateElement(elementReference);
        }

        void DestroyElement(ElementReference<TId, TItem> element, TId id)
        {
            RootPanel.Children.Remove(element.Container);
            Elements.Remove(id);
            element.Dispose();
        }

        void UpdateAndTransitionElement(ElementReference<TId, TItem> element, TItem newData)
        {
            var currentPosition = GetElementPositionValue(element);
            var currentScale = GetElementScaleValue(element);
            var currentOrientation = GetElementOrientationValue(element);
            var currentOpacity = GetElementOpacityValue(element);

            element.Model = newData;
            UpdateElementData(element);

            var transition = GetElementTransitionEasingFunction(element);
            if (transition is not null)
            {
                TaskCompletionSource<bool> tsc = new();
                StopElementAnimation(element);

                var progressAnimation = Compositor.CreateScalarKeyFrameAnimation();
                progressAnimation.Duration = TimeSpan.FromMilliseconds(transition.Length);
                progressAnimation.StopBehavior = AnimationStopBehavior.SetToFinalValue;
                progressAnimation.InsertKeyFrame(0, 0f, transition.EasingFunction);
                progressAnimation.InsertKeyFrame(1, 1f, transition.EasingFunction);

                var animProgressNode = new AnimatableScalarCompositionNode(Compositor);

                element.Visual.StartAnimation(Offset, ExpressionFunctions.Lerp(currentPosition, GetElementPositionNode(element), animProgressNode.Reference));
                var scale = GetElementScaleNode(element);
                element.Visual.StartAnimation(Scale, ExpressionFunctions.Lerp(new Vector3(currentScale), ExpressionFunctions.Vector3(scale, scale, scale), animProgressNode.Reference));
                element.Visual.StartAnimation(AnimationConstants.Orientation, ExpressionFunctions.Slerp(currentOrientation, GetElementOrientationNode(element), animProgressNode.Reference));
                element.Visual.StartAnimation(Opacity, ExpressionFunctions.Lerp(currentOpacity, GetElementOpacityNode(element), animProgressNode.Reference));

                var batch = Compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (object _, CompositionBatchCompletedEventArgs _1) =>
                {
                    if (IsActive)
                    {
                        ConfigureElementAnimation(element);
                    }
                    tsc.SetResult(true);

                    batch.Dispose();
                    animProgressNode.Dispose();
                    progressAnimation.Dispose();
                };

                animProgressNode.Animate(progressAnimation);

                batch.End();

                elementUpdateTask.Add(tsc.Task);
            }


            UpdateElement(element);
            processedElements.Add(element.Id);
        }
    }

    public record Transition(uint Length, CompositionEasingFunction EasingFunction);

    public virtual ElementAlignment HorizontalAlignment { get; set; }

    protected void AddBehavior(LayoutBehavior<TId, TItem> behavior)
    {
        if (IsActive)
        {
            behavior.Configure(this);
        }
        _behaviors.Add(behavior);
    }

    public T GetBehavior<T>() where T : LayoutBehavior<TId, TItem> =>
        _behaviors.OfType<T>().First();

    public T? TryGetBehavior<T>() where T : LayoutBehavior<TId, TItem> =>
        _behaviors.OfType<T>().FirstOrDefault();

    protected void RemoveBehavior(LayoutBehavior<TId, TItem> behavior)
    {
        _behaviors.Remove(behavior);
    }

    //todo add trackerPosition
    public Vector3 GetElementPositionValue(ElementReference<TId, TItem> element, Vector3? trackerPosition = null) => GetElementPositionNode(element).Evaluate();
    public float GetElementScaleValue(ElementReference<TId, TItem> element) => GetElementScaleNode(element).Evaluate();
    public Quaternion GetElementOrientationValue(ElementReference<TId, TItem> element) => GetElementOrientationNode(element).Evaluate();
    public float GetElementOpacityValue(ElementReference<TId, TItem> element) => GetElementOpacityNode(element).Evaluate();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (IsActive)
                {
                    //We only want to dipose these fields if the layout is still active
                    //If it isn't, that means other layout owns them now
                    Properties.Dispose();
                    //InteractionSource.Dispose();
                    //Tracker.Dispose();
                    AnimatableNodes.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Layout()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
