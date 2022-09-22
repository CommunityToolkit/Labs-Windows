// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#if !WINAPPSDK
#nullable enable
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace CommunityToolkit.Labs.WinUI;
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
                gesture.PreviewControl.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
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
            TrackerOwner.OnInertiaStateEntered += InertiaStateEntered;
            TrackerOwner.OnInteractingStateEntered += InteractingStateEntered;

            foreach (var gesture in _gestures)
            {
                RegisterGestureHandlers(gesture);
            }
        }

        override public void OnDeactivated()
        {
            TrackerOwner.OnInertiaStateEntered -= InertiaStateEntered;
            TrackerOwner.OnInteractingStateEntered -= InteractingStateEntered;

            for (var i = _gestures.Count - 1; i >= 0; i--)
            {
                RemoveGesture(_gestures[i]);
            }
        }

        private void InteractingStateEntered(InteractionTracker _, InteractionTrackerInteractingStateEnteredArgs _2)
        {
            IsUserInteracting = true;
        }

        private void InertiaStateEntered(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args)
        {
            IsUserInteracting = false;
        }

        public class InteractionTrackerOwner : IInteractionTrackerOwner
        {
            public delegate void CustomAnimationStateEnteredHandler(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args);
            public event CustomAnimationStateEnteredHandler? OnCustomAnimationStateEntered;

            public delegate void IdleStateEnteredHandler(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args);
            public event IdleStateEnteredHandler? OnIdleStateEntered;

            public delegate void InertiaStateEnteredHandler(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args);
            public event InertiaStateEnteredHandler? OnInertiaStateEntered;

            public delegate void InteractingStateEnteredHandler(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args);
            public event InteractingStateEnteredHandler? OnInteractingStateEntered;

            public delegate void RequestIgnoredHandler(InteractionTracker sender, InteractionTrackerRequestIgnoredArgs args);
            public event RequestIgnoredHandler? OnRequestIgnored;

            public delegate void ValuesChangedHandler(InteractionTracker sender, InteractionTrackerValuesChangedArgs args);
            public event ValuesChangedHandler? OnValuesChanged;

            public void CustomAnimationStateEntered(InteractionTracker sender, InteractionTrackerCustomAnimationStateEnteredArgs args) => OnCustomAnimationStateEntered?.Invoke(sender, args);

            public void IdleStateEntered(InteractionTracker sender, InteractionTrackerIdleStateEnteredArgs args) => OnIdleStateEntered?.Invoke(sender, args);

            public void InertiaStateEntered(InteractionTracker sender, InteractionTrackerInertiaStateEnteredArgs args) => OnInertiaStateEntered?.Invoke(sender, args);

            public void InteractingStateEntered(InteractionTracker sender, InteractionTrackerInteractingStateEnteredArgs args) => OnInteractingStateEntered?.Invoke(sender, args);

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

#endif
