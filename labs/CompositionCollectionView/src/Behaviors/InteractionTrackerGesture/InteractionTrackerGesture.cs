// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using static CommunityToolkit.Labs.WinUI.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI;
public abstract class InteractionTrackerGesture<TId>
{
    public event EventHandler? GestureCompleted;

    protected BindableCompositionPropertySet LayoutProperties { get; init; }
    protected CompositionPropertySet GestureProperties { get; init; }

    const string InteractionInProgressId = "IIP";
    protected bool InteractionInProgress
    {
        get => GestureProperties.TryGetBoolean(InteractionInProgressId, out var inProgress) == CompositionGetValueStatus.Succeeded && inProgress;
        set => GestureProperties.InsertBoolean(InteractionInProgressId, value);
    }

    protected BooleanNode InteractionInProgressReference => GestureProperties.GetReference().GetBooleanProperty(InteractionInProgressId);

    const string CompletionInProgressId = "CIP";

    protected bool CompletionInProgress
    {
        get => GestureProperties.TryGetBoolean(CompletionInProgressId, out var inProgress) == CompositionGetValueStatus.Succeeded && inProgress;
        set => GestureProperties.InsertBoolean(CompletionInProgressId, value);
    }

    protected BooleanNode CompletionInProgressReference => GestureProperties.GetReference().GetBooleanProperty(CompletionInProgressId);

    const string InertiaInProgressId = "NIP";

    protected bool InertiaInProgress
    {
        get => GestureProperties.TryGetBoolean(InertiaInProgressId, out var inProgress) == CompositionGetValueStatus.Succeeded && inProgress;
        set => GestureProperties.InsertBoolean(InertiaInProgressId, value);
    }

    protected BooleanNode InertiaInProgressReference => GestureProperties.GetReference().GetBooleanProperty(InertiaInProgressId);

    private InteractionTrackerReferenceNode _tracker;

    protected DateTime InteractionStartedTime { get; private set; }

    protected InteractionTrackerGesture(Compositor compositor, InteractionTrackerReferenceNode tracker, BindableCompositionPropertySet layoutProperties)
    {
        LayoutProperties = layoutProperties;
        GestureProperties = compositor.CreatePropertySet();
        _tracker = tracker;
        InteractionInProgress = false;
        CompletionInProgress = false;
        InertiaInProgress = false;
    }

    private bool _isDisabled = false;

    //Disabling a gesture prevents it from processing tracker updates immediately, it can't be undone
    public void Disable() => _isDisabled = true;

    public void PauseAnimation()
    {
        if (PreviewControl is null) { return; }
        var visual = ElementCompositionPreview.GetElementVisual(PreviewControl);
        visual.StopAnimation(TransformMatrix);
        visual.StopAnimation(Opacity);
    }

    public void Restart()
    {
        InteractionInProgress = false;
        CompletionInProgress = false;

        if (PreviewControl is null) { return; }
        var visual = ElementCompositionPreview.GetElementVisual(PreviewControl);

        var visibility = GetPreviewVisibility(_tracker);
        var opacity = GetPreviewOpacity(_tracker);
        var transform = GetPreviewTransform(_tracker);

        visual.StartAnimation(TransformMatrix, transform);
        visual.StartAnimation(Opacity, ExpressionFunctions.Conditional(visibility,
            opacity,
            0));

        PreviewControl?.ResetVisualState();
    }

    protected void InvokeGestureCompleted()
    {
        InteractionInProgress = false;
        InertiaInProgress = false;
        CompletionInProgress = true;
        GestureCompleted?.Invoke(this, EventArgs.Empty);
    }
    public void ValuesChanged(InteractionTracker tracker, InteractionTrackerValuesChangedArgs args)
    {
        if (_isDisabled)
        {
            return;
        }
        OnValuesChanged(tracker, args);
    }

    public void InteractingStateEntered(InteractionTracker _, InteractionTrackerInteractingStateEnteredArgs _1, InteractionTrackerState _3)
    {
        InteractionInProgress = true;
        InertiaInProgress = false;
        InteractionStartedTime = DateTime.Now;
    }

    public void InertiaStateEntered(InteractionTracker tracker, InteractionTrackerInertiaStateEnteredArgs args, InteractionTrackerState _)
    {
        if (_isDisabled)
        {
            return;
        }
        InteractionInProgress = false;
        InertiaInProgress = true;
        OnInertiaStateEntered(tracker, args);
    }

    protected virtual void OnValuesChanged(InteractionTracker _, InteractionTrackerValuesChangedArgs _1) { }

    protected virtual void OnInertiaStateEntered(InteractionTracker _, InteractionTrackerInertiaStateEnteredArgs _1) { }

    protected abstract ScalarNode GetPreviewOpacity(InteractionTrackerReferenceNode tracker);
    protected abstract BooleanNode GetPreviewVisibility(InteractionTrackerReferenceNode tracker);
    protected abstract Matrix4x4Node GetPreviewTransform(InteractionTrackerReferenceNode tracker);

    public GesturePreviewControl? PreviewControl { get; set; }

}

public abstract class InteractionTrackerGesture<TId, TPanningGesturePreview> : InteractionTrackerGesture<TId> where TPanningGesturePreview : GesturePreviewControl, new()
{
    protected InteractionTrackerGesture(Compositor compositor, InteractionTrackerReferenceNode tracker, BindableCompositionPropertySet layoutProperties) : base(compositor, tracker, layoutProperties)
    {
        PreviewControl = new TPanningGesturePreview();
    }
}
