// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System.Numerics;
using static CommunityToolkit.Labs.WinUI.AnimationConstants;


namespace CommunityToolkit.Labs.WinUI;
public abstract partial class CompositionCollectionLayout<TId, TItem> : ILayout, IDisposable where TId : notnull
{

    bool _isUpdatingSource = false;

    private record SourceUpdate(IDictionary<TId, TItem> UpdatedElements, TaskCompletionSource<bool> TaskCompletion, bool Animated);
    Queue<SourceUpdate> _pendingSourceUpdates = new();

    public async Task UpdateSource(IDictionary<TId, TItem> source, bool animate)
    {
        var tcs = new TaskCompletionSource<bool>();

        if (!_isUpdatingSource)
        {
            _isUpdatingSource = true;
            ProcessSourceUpdate(source, tcs, animate);
        }
        else
        {
            _pendingSourceUpdates.Enqueue(new SourceUpdate(source, tcs, animate));
        }

        await tcs.Task;
    }

    private HashSet<AnimatableScalarCompositionNode> _ongoingSourceUpdateAnimation = new();

    public async void ProcessSourceUpdate(IDictionary<TId, TItem> updatedElements, TaskCompletionSource<bool> taskCompletion, bool animate)
    {
        List<Task<bool>> elementUpdateTask = new();

        HashSet<TId> processedElements = new();

        foreach (var (id, element) in _elements.ToArray())
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

        OnElementsUpdated();

        taskCompletion.SetResult(true);

        if (elementUpdateTask.Any())
        {
            await Task.WhenAll(elementUpdateTask);
        }

        if (_pendingSourceUpdates.Count > 0)
        {
            var update = _pendingSourceUpdates.Dequeue();
            ProcessSourceUpdate(update.UpdatedElements, update.TaskCompletion, update.Animated);
        }
        else
        {
            _isUpdatingSource = false;
        }

        void InstantiateElement(TId id, TItem item)
        {
            var element = ElementFactory(id);
            RootPanel.Children.Add(element);

            var elementReference = new ElementReference<TId, TItem>(id, item, element,/* source, tracker, trackerOwner,*/ this);
            UpdateElementData(elementReference);
            _elements[id] = elementReference;

            ConfigureElement(elementReference);
            ConfigureElementBehaviors(elementReference);

            ConfigureElementAnimation(elementReference);
            UpdateElement(elementReference);
        }

        void DestroyElement(ElementReference<TId, TItem> element, TId id)
        {
            CleanupElement(element);
            CleanupElementBehaviors(element);
            RootPanel.Children.Remove(element.Container);
            _elements.Remove(id);
            element.Dispose();
        }

        void UpdateAndTransitionElement(ElementReference<TId, TItem> element, TItem newData)
        {
            if (animate && GetElementTransitionEasingFunction(element) is ElementTransition transition)
            {
                var currentPosition = GetElementPositionValue(element);
                var currentScale = GetElementScaleValue(element);
                var currentOrientation = GetElementOrientationValue(element);
                var currentOpacity = GetElementOpacityValue(element);

                TaskCompletionSource<bool> tsc = new();
                StopElementAnimation(element);

                element.Model = newData;
                UpdateElementData(element);

                var progressAnimation = Compositor.CreateScalarKeyFrameAnimation();
                progressAnimation.Duration = TimeSpan.FromMilliseconds(transition.Length);
                progressAnimation.StopBehavior = AnimationStopBehavior.SetToFinalValue;
                progressAnimation.InsertKeyFrame(0, 0f, transition.EasingFunction);
                progressAnimation.InsertKeyFrame(1, 1f, transition.EasingFunction);

                var animProgressNode = new AnimatableScalarCompositionNode(Compositor);
                _ongoingSourceUpdateAnimation.Add(animProgressNode);

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

                    _ongoingSourceUpdateAnimation.Remove(animProgressNode);
                };

                animProgressNode.Animate(progressAnimation);

                batch.End();

                elementUpdateTask.Add(tsc.Task);
            }
            else
            {
                element.Model = newData;
                UpdateElementData(element);
            }

            UpdateElement(element);
            processedElements.Add(element.Id);
        }
    }
}
