// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System.Numerics;
using static CommunityToolkit.Labs.WinUI.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI;
public abstract partial class CompositionCollectionLayout<TId, TItem> : ILayout, IDisposable where TId : notnull
{
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


        foreach (var behavior in _behaviors)
        {
            behavior.OnDeactivated();
        }
    }

    public T TransitionTo<T>(Func<CompositionCollectionLayout<TId, TItem>, T> factory, bool animateTransition = true) where T : CompositionCollectionLayout<TId, TItem>
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("TransitionTo can only be used in active layouts. You might have already transitioned away from this layout.");
        }

        Deactivate();

        var newLayout = factory(this);

        foreach (var behavior in _behaviors)
        {
            newLayout.AddBehavior(behavior);
        }

        newLayout.Activate();

        TransferElements();

        LayoutReplaced?.Invoke(this, newLayout, animateTransition);

        return newLayout;

        void TransferElements()
        {
            foreach (var (id, element) in _elements)
            {
                element.ReasignTo(newLayout);
                newLayout._elements.Add(id, element);
                CleanupElement(element);
                CleanupElementBehaviors(element);
            }

            //Configure the animations after all the elements have been added to the new layout,
            //to allow elements to depend on each other
            foreach (var (id, element) in _elements)
            {
                newLayout.ConfigureElement(element);
                newLayout.ConfigureElementBehaviors(element);

                var currentPosition = GetElementPositionValue(element);
                var currentScale = GetElementScaleValue(element);
                var currentOrientation = GetElementOrientationValue(element);
                var currentOpacity = GetElementOpacityValue(element);

                if (animateTransition && newLayout.GetElementTransitionEasingFunction(element) is ElementTransition transition)
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
                        if (newLayout._elements.ContainsKey(element.Id))
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

            _elements.Clear();

            newLayout.OnElementsUpdated();
        }
    }
}
