// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System.Numerics;

namespace CommunityToolkit.Labs.WinUI;

//The possible states of the tracker as documented in https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.composition.interactions.interactiontracker?view=windows-app-sdk-1.1#interactiontracker-states-and-transitions
public enum InteractionTrackerState { Idle, Inertia, Interacting, CustomAnimation };

public class InteractionTrackerBehavior<TId, TItem> : CompositionCollectionLayoutBehavior<TId, TItem> where TId : notnull
{
    public VisualInteractionSource InteractionSource { get; init; }
    public InteractionTracker Tracker { get; init; }
    public InteractionTrackerOwner TrackerOwner { get; init; }

    public bool IsUserInteracting { get; private set; } = false;


    private List<InteractionTrackerGesture<TId>> _gestures = new List<InteractionTrackerGesture<TId>>();

    public InteractionTrackerBehavior(FrameworkElement root)
    {
        var rootVisual = ElementCompositionPreview.GetElementVisual(root);

        InteractionSource = InitializeInteractionSource();
        TrackerOwner = new InteractionTrackerOwner();
        Tracker = InitializeTracker();

        VisualInteractionSource InitializeInteractionSource()
        {
            var interactionSource = VisualInteractionSource.Create(rootVisual);
            interactionSource.ScaleSourceMode = InteractionSourceMode.EnabledWithInertia;
            interactionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithInertia;
            interactionSource.PositionYSourceMode = InteractionSourceMode.EnabledWithInertia;
            interactionSource.IsPositionXRailsEnabled = false;
            interactionSource.IsPositionYRailsEnabled = false;
            interactionSource.ManipulationRedirectionMode = VisualInteractionSourceRedirectionMode.CapableTouchpadAndPointerWheel;
            return interactionSource;
        }

        InteractionTracker InitializeTracker()
        {
            var tracker = InteractionTracker.CreateWithOwner(rootVisual.Compositor, TrackerOwner);
            tracker.InteractionSources.Add(InteractionSource);
            return tracker;
        }
    }

    public virtual void AddGesture(InteractionTrackerGesture<TId> gesture)
    {
        if (gesture.PreviewControl is { })
        {
            gesture.PreviewControl.HorizontalAlignment = HorizontalAlignment.Left;
            gesture.PreviewControl.VerticalAlignment = VerticalAlignment.Top;
            Layout.RootPanel.Children.Add(gesture.PreviewControl);
            gesture.Restart();
        }

        if (Layout.IsActive)
        {
            RegisterGestureHandlers(gesture);
        }

        _gestures.Add(gesture);
    }

    public void RemoveGesture(InteractionTrackerGesture<TId> gesture)
    {
        gesture.Disable();
        UnregisterGestureHandlers(gesture);

        if (gesture.PreviewControl is { })
        {
            Layout.RootPanel.Children.Remove(gesture.PreviewControl);
        }

        _gestures.Remove(gesture);
        if (gesture is IDisposable disposableGesture)
        {
            disposableGesture.Dispose();
        }
    }

    private void RegisterGestureHandlers(InteractionTrackerGesture<TId> gesture)
    {
        TrackerOwner.OnInteractingStateEntered += gesture.InteractingStateEntered;
        TrackerOwner.OnInertiaStateEntered += gesture.InertiaStateEntered;
        TrackerOwner.OnValuesChanged += gesture.ValuesChanged;
    }

    private void UnregisterGestureHandlers(InteractionTrackerGesture<TId> gesture)
    {
        TrackerOwner.OnInteractingStateEntered -= gesture.InteractingStateEntered;
        TrackerOwner.OnInertiaStateEntered -= gesture.InertiaStateEntered;
        TrackerOwner.OnValuesChanged -= gesture.ValuesChanged;
    }

    public virtual void RestartGestures()
    {
        foreach (var gesture in _gestures)
        {
            gesture.Restart();
        }
    }

    public void Disable()
    {
        InteractionSource.IsPositionXRailsEnabled = false;
        InteractionSource.IsPositionYRailsEnabled = false;

        InteractionSource.PositionXSourceMode = InteractionSourceMode.EnabledWithInertia;
        InteractionSource.PositionYSourceMode = InteractionSourceMode.EnabledWithInertia;
        Tracker.MaxPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        Tracker.MinPosition = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
    }

    override public void OnActivated()
    {
        foreach (var gesture in _gestures)
        {
            RegisterGestureHandlers(gesture);
        }
    }

    override public void OnDeactivated()
    {
        for (var i = _gestures.Count - 1; i >= 0; i--)
        {
            RemoveGesture(_gestures[i]);
        }
    }

    public class InteractionTrackerOwner : IInteractionTrackerOwner
    {
        public InteractionTrackerState CurrentState { get; private set; }

        public delegate void CustomAnimationStateEnteredHandler(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args, InteractionTrackerState previousState);
        public event CustomAnimationStateEnteredHandler? OnCustomAnimationStateEntered;

        public delegate void IdleStateEnteredHandler(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args, InteractionTrackerState previousState);
        public event IdleStateEnteredHandler? OnIdleStateEntered;

        public delegate void InertiaStateEnteredHandler(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args, InteractionTrackerState previousState);
        public event InertiaStateEnteredHandler? OnInertiaStateEntered;

        public delegate void InteractingStateEnteredHandler(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args, InteractionTrackerState previousState);
        public event InteractingStateEnteredHandler? OnInteractingStateEntered;

        public delegate void RequestIgnoredHandler(InteractionTracker sender, InteractionTrackerRequestIgnoredArgs args);
        public event RequestIgnoredHandler? OnRequestIgnored;

        public delegate void ValuesChangedHandler(InteractionTracker sender, InteractionTrackerValuesChangedArgs args);
        public event ValuesChangedHandler? OnValuesChanged;

        public void CustomAnimationStateEntered(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args)
        {
            OnCustomAnimationStateEntered?.Invoke(sender, args, CurrentState);
            CurrentState = InteractionTrackerState.CustomAnimation;
        }

        public void IdleStateEntered(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args)
        {
            OnIdleStateEntered?.Invoke(sender, args, CurrentState);
            CurrentState = InteractionTrackerState.Idle;
        }

        public void InertiaStateEntered(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args)
        {
            OnInertiaStateEntered?.Invoke(sender, args, CurrentState);
            CurrentState = InteractionTrackerState.Inertia;
        }

        public void InteractingStateEntered(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args)
        {
            OnInteractingStateEntered?.Invoke(sender, args, CurrentState);
            CurrentState = InteractionTrackerState.Interacting;
        }

        public void RequestIgnored(InteractionTracker sender, InteractionTrackerRequestIgnoredArgs args) => OnRequestIgnored?.Invoke(sender, args);

        public void ValuesChanged(InteractionTracker sender, InteractionTrackerValuesChangedArgs args) => OnValuesChanged?.Invoke(sender, args);

        public void Reset()
        {
            OnCustomAnimationStateEntered = null;
            OnIdleStateEntered = null;
            OnInertiaStateEntered = null;
            OnInteractingStateEntered = null;
            OnRequestIgnored = null;
            OnValuesChanged = null;
        }
    }
}
