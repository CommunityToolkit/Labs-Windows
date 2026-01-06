// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A Control that displays Text in a Marquee style.
/// </summary>
[TemplatePart(Name = MarqueeContainerPartName, Type = typeof(Panel))]
[TemplatePart(Name = Segment1PartName, Type = typeof(ContentPresenter))]
[TemplatePart(Name = Segment2PartName, Type = typeof(ContentPresenter))]
[TemplatePart(Name = MarqueeTransformPartName, Type = typeof(TranslateTransform))]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = LeftwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = RightwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = UpwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = DownwardsVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = TickerVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = LoopingVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = BouncingVisualStateName)]
public partial class Marquee : ContentControl
{
    private const string MarqueeContainerPartName = "MarqueeContainer";
    private const string Segment1PartName = "Segment1";
    private const string Segment2PartName = "Segment2";
    private const string MarqueeTransformPartName = "MarqueeTransform";

    private const string MarqueeActiveState = "MarqueeActive";
    private const string MarqueePausedState = "MarqueePaused";
    private const string MarqueeStoppedState = "MarqueeStopped";

    private const string DirectionVisualStateGroupName = "DirectionStateGroup";
    private const string LeftwardsVisualStateName = "Leftwards";
    private const string RightwardsVisualStateName = "Rightwards";
    private const string UpwardsVisualStateName = "Upwards";
    private const string DownwardsVisualStateName = "Downwards";

    private const string BehaviorVisualStateGroupName = "BehaviorStateGroup";
    private const string TickerVisualStateName = "Ticker";
    private const string LoopingVisualStateName = "Looping";
    private const string BouncingVisualStateName = "Bouncing";

    private Panel? _marqueeContainer;
    private ContentPresenter? _segment1;
    private ContentPresenter? _segment2;
    private TranslateTransform? _marqueeTransform;
    private Storyboard? _marqueeStoryboard;

    // Used to track if the marquee is active or not.
    // This signifies being mid animation. A paused marquee is still active!
    private bool _isActive = false;
    private bool _isPaused = false;

    // This is used to track the position when stopped.
    // If the animation update happens while running, this position
    // is lost and must be set when the animation stops.
    private double _stoppedPosition;
    private DependencyProperty? _animationProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Marquee"/> class.
    /// </summary>
    public Marquee()
    {
        DefaultStyleKey = typeof(Marquee);
    }

    /// <summary>
    /// Unsubscribes from the loaded event when the control is being disposed.
    /// </summary>
    ~Marquee()
    {
        Loaded -= this.Marquee_Loaded;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Explicit casting throws early when parts are missing from the template
        _marqueeContainer = (Panel)GetTemplateChild(MarqueeContainerPartName);
        _segment1 = (ContentPresenter)GetTemplateChild(Segment1PartName);
        _segment2 = (ContentPresenter)GetTemplateChild(Segment2PartName);
        _marqueeTransform = (TranslateTransform)GetTemplateChild(MarqueeTransformPartName);

        // Swapping tabs in TabView caused errors where the control would unload and never reattach events.
        // Fix: Track the loaded event. This should be fine because the GC will handle detaching the Loaded
        // event on disposal. However, more research is required.
        // As a result, all other events should be attached in the Loaded event handler.
        Loaded += this.Marquee_Loaded;

        VisualStateManager.GoToState(this, GetVisualStateName(Direction), false);
        VisualStateManager.GoToState(this, GetVisualStateName(Behavior), false);
    }

    private static string GetVisualStateName(MarqueeDirection direction)
    {
        return direction switch
        {
            MarqueeDirection.Left => LeftwardsVisualStateName,
            MarqueeDirection.Right => RightwardsVisualStateName,
            MarqueeDirection.Up => UpwardsVisualStateName,
            MarqueeDirection.Down => DownwardsVisualStateName,
            _ => LeftwardsVisualStateName,
        };
    }

    private static string GetVisualStateName(MarqueeBehavior behavior)
    {
        return behavior switch
        {
            MarqueeBehavior.Ticker => TickerVisualStateName,
            MarqueeBehavior.Looping => LoopingVisualStateName,
#if !HAS_UNO
            MarqueeBehavior.Bouncing => BouncingVisualStateName,
#endif
            _ => TickerVisualStateName,
        };
    }

    /// <summary>
    /// Begins the Marquee animation if not running or resumes if paused.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when template parts are not supplied.</exception>
    public void StartMarquee() => PlayMarquee(fromStart: false);

    /// <summary>
    /// Restarts the Marquee from the start of the animation regardless of current state.
    /// </summary>
    /// <remarks>
    /// <see cref="MarqueeStarted"/> will not be raised if the marquee was already active.
    /// </remarks>
    public void RestartMarquee() => PlayMarquee(fromStart: true);

    /// <summary>
    /// Resumes the Marquee animation if paused.
    /// </summary>
    public void ResumeMarquee()
    {
        // If not paused or not active, do nothing
        if (!_isPaused || !_isActive)
            return;

        // Resume the storyboard
        _isPaused = false;
        _marqueeStoryboard?.Resume();

        // Apply state transitions
        VisualStateManager.GoToState(this, MarqueeActiveState, false);
        MarqueeResumed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Pauses the Marquee animation.
    /// </summary>
    public void PauseMarquee()
    {
        // Log initial paused status
        bool wasPaused = _isPaused;

        // Ensure paused status
        _marqueeStoryboard?.Pause();
        _isPaused = true;

        if (!wasPaused)
        {
            // Apply state transitions
            VisualStateManager.GoToState(this, MarqueePausedState, false);
            MarqueePaused?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Stops the Marquee animation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when template parts are not supplied.</exception>
    public void StopMarquee()
    {
        bool wasStopped = !_isActive;

        // Ensure stopped status
        _marqueeStoryboard?.Stop();
        _isActive = false;
        _isPaused = false;

        // Set the transform to the stopped position if provided.
        if (_animationProperty is not null)
        {
            _marqueeTransform?.SetValue(_animationProperty, _stoppedPosition);
        }

        if (!wasStopped)
        {
            // Apply state transitions
            VisualStateManager.GoToState(this, MarqueeStoppedState, false);
            MarqueeStopped?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayMarquee(bool fromStart = false)
    {
        // Resume if paused and not playing from start
        if (!fromStart && _isPaused && _isActive)
        {
            ResumeMarquee();
            return;
        }

        // Do nothing if storyboard is null or already playing and not from start.
        if (_marqueeStoryboard is null || (_isActive && !fromStart))
            return;

        bool wasActive = _isActive;

        // Stop the storboard if it is already active and playing from start
        if (fromStart)
        {
            _marqueeStoryboard.Stop();
        }

        // Start the storyboard
        _marqueeStoryboard.Begin();

        // Update the status variables
        _isActive = true;
        _isPaused = false;

        if (!wasActive)
        {
            // Apply state transitions
            VisualStateManager.GoToState(this, MarqueeActiveState, false);
            MarqueeStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UpdateMarquee(bool onTheFly)
    {
        // Check for crucial template parts
        if (_marqueeTransform is null)
            return;

        // If the update cannot be made on the fly,
        // stop the marquee and reset the transform
        if (!onTheFly)
        {
            StopMarquee();
            _marqueeTransform.X = 0;
            _marqueeTransform.Y = 0;
        }

        // Apply the animation update
        bool hasAnimation = UpdateAnimation(out var seek);

        // If updating on the fly, and there is an animation,
        // seek to the correct position
        if (onTheFly && hasAnimation && _isActive)
        {
            _marqueeStoryboard?.Begin();
            _marqueeStoryboard?.Seek(seek);

            // Restore paused state if necessary
            if (_isPaused)
            {
                PauseMarquee();
            }
        }
    }

    /// <summary>
    /// Updates the animation to match the current control state.
    /// </summary>
    /// <remarks>
    /// When in looping mode, it is possible that no animation is necessary.
    /// </remarks>
    /// <param name="seekPoint">The seek point to resume the animation (if possible or appropriate.</param>
    /// <exception cref="InvalidOperationException">Thrown when template parts are not supplied.</exception>
    /// <returns>Returns whether or not an animation is necessary.</returns>
    private bool UpdateAnimation(out TimeSpan seekPoint)
    {
        seekPoint = TimeSpan.Zero;

        // Check for crucial template parts
        if (_marqueeContainer is null ||
            _marqueeTransform is null ||
            _segment1 is null ||
            _segment2 is null)
        {
            // Crucial template parts are missing!
            // This can happen during initialization of certain properties.
            // Gracefully return when this happens. Proper checks for these template parts happen in OnApplyTemplate.
            return false;
        }

        // Unbind events from the old storyboard
        if (_marqueeStoryboard is not null)
        {
            _marqueeStoryboard.Completed -= StoryBoard_Completed;
        }

        // Get the size of the container and segment, based on the orientation.
        // Also track the property to adjust, also based on the orientation.
        double containerSize;
        double segmentSize;
        double value;
        DependencyProperty dp;
        string targetProperty;

        if (IsDirectionHorizontal)
        {
            // The direction is horizontal, so the sizes, value, and properties
            // are defined by width and X coordinates.
            containerSize = _marqueeContainer.ActualWidth;
            segmentSize = _segment1.ActualWidth;
            value = _marqueeTransform.X;
            dp = TranslateTransform.XProperty;
            targetProperty = "(TranslateTransform.X)";
        }
        else
        {
            // The direction is vertical, so the sizes, value, and properties
            // are defined by height and Y coordinates.
            containerSize = _marqueeContainer.ActualHeight;
            segmentSize = _segment1.ActualHeight;
            value = _marqueeTransform.Y;
            dp = TranslateTransform.YProperty;
            targetProperty = "(TranslateTransform.Y)";
        }

        if (IsLooping && segmentSize < containerSize)
        {
            // If the marquee is in looping mode and the segment is smaller
            // than the container, then the animation does not not need to play.

            // Reset the transform to 0 and hide the second segment
            _marqueeContainer.SetValue(dp, 0);
            _segment2.Visibility = Visibility.Collapsed;

            _marqueeStoryboard?.Stop();
            _marqueeStoryboard = null;
            return false;
        }

        // The start position is offset 100% if in ticker mode
        // Otherwise it's 0
        double start = IsTicker ? containerSize + 1 : 0;

        // The end is when the end of the text reaches the border if in bouncing mode
        // Otherwise it is when the first set of text is 100% out of view
        double end = IsBouncing ? containerSize - segmentSize : -segmentSize;

        // The distance is used for calculating the duration and the previous
        // animation progress if resuming
        double distance = Math.Abs(start - end);

        // If the distance is zero, don't play an animation
        if (distance is 0)
        {
            _marqueeStoryboard?.Stop();
            _marqueeStoryboard = null;
            return false;
        }

        // Swap the start and end to inverse direction for right or upwards
        if (IsDirectionInverse)
        {
            (start, end) = (end, start);
        }

        // The second segment of text should be hidden if the marquee is not in looping mode
        _segment2.Visibility = IsLooping ? Visibility.Visible : Visibility.Collapsed;

        // Calculate the animation duration by dividing the distance by the speed
        TimeSpan duration = TimeSpan.FromSeconds(distance / Speed);

        // Create new storyboard and animation
        _marqueeStoryboard = CreateMarqueeStoryboardAnimation(start, end, duration, targetProperty);

        // Bind the storyboard completed event
        _marqueeStoryboard.Completed += StoryBoard_Completed;

        // NOTE: Can this be optimized to remove or reduce the need for this callback?
        // Invalidate the segment measures when the transform changes.
        // This forces virtualized panels to re-measure the segments
        _marqueeTransform.RegisterPropertyChangedCallback(dp, (sender, dp) =>
        {
            _segment1.InvalidateMeasure();
            _segment2.InvalidateMeasure();
        });

        // Calculate the seek point for seamless animation updates
        double progress = Math.Abs(start - value) / distance;
        seekPoint = TimeSpan.FromTicks((long)(duration.Ticks * progress));

        // Set the value of the transform to the start position if not active.
        // This puts the content in the correct starting position without using the animation.
        if (!_isActive)
        {
            _marqueeTransform.SetValue(dp, start);
        }

        // Set stopped position and animation property regardless of the active state.
        // This will be used when the animation stops.
        _stoppedPosition = start;
        _animationProperty = dp;

        return true;
    }

    private Storyboard CreateMarqueeStoryboardAnimation(double start, double end, TimeSpan duration, string targetProperty)
    {   
        // Initialize the new storyboard
        var marqueeStoryboard = new Storyboard
        {
            Duration = duration,
            RepeatBehavior = RepeatBehavior,
#if !HAS_UNO
            AutoReverse = IsBouncing,
#endif
        };
        
        // Create a new double animation, moving from [start] to [end] positions in [duration] time.
        var animation = new DoubleAnimationUsingKeyFrames
        {
            Duration = duration,
            RepeatBehavior = RepeatBehavior,
#if !HAS_UNO
            AutoReverse = IsBouncing,
#endif
        };

        // Create the key frames
        var frame1 = new DiscreteDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
            Value = start,
        };
        var frame2 = new EasingDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(duration),
            Value = end,
        };

        // Add the key frames to the animation
        animation.KeyFrames.Add(frame1);
        animation.KeyFrames.Add(frame2);

        // Add the double animation to the storyboard
        marqueeStoryboard.Children.Add(animation);
        
        // Set the storyboard target and target property
        Storyboard.SetTarget(animation, _marqueeTransform);
        Storyboard.SetTargetProperty(animation, targetProperty);

        return marqueeStoryboard;
    }

    private void ClipMarquee(double width = default, double height = default)
    {
        if (_marqueeContainer is null)
            return;

        width = width is default(double) ? _marqueeContainer.ActualWidth : width;
        height = height is default(double) ? _marqueeContainer.ActualHeight : height;

        // Clip the marquee within the bounds of the container
        _marqueeContainer.Clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, width, height)
        };
    }
}
