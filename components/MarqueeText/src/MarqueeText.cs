// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.MarqueeTextRns;

/// <summary>
/// A Control that displays Text in a Marquee style.
/// </summary>
[TemplatePart(Name = MarqueeContainerPartName, Type = typeof(Panel))]
[TemplatePart(Name = Segment1PartName, Type = typeof(FrameworkTemplate))]
[TemplatePart(Name = Segment2PartName, Type = typeof(FrameworkTemplate))]
[TemplatePart(Name = Segment2PartName, Type = typeof(FrameworkTemplate))]
[TemplatePart(Name = MarqueeTransformPartName, Type = typeof(TranslateTransform))]
[TemplatePart(Name = TransitionAnaimationPartName, Type = typeof(Storyboard))]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = LeftwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = RightwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = UpwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = DownwardsVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = TickerVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = LoopingVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = BouncingVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = TransitionVisualStateName)]
[ContentProperty(Name = nameof(Text))]

#if HAS_UNO
// See: https://github.com/CommunityToolkit/Labs-Windows/pull/275#issuecomment-1331113635
#pragma warning disable CA1001
#endif
public partial class MarqueeText : Control
{
    private const string MarqueeContainerPartName = "MarqueeContainer";
    private const string Segment1PartName = "Segment1";
    private const string Segment2PartName = "Segment2";
    private const string MarqueeTransformPartName = "MarqueeTransform";
    private const string TransitionAnaimationPartName = "TransitionAnimation";

    private const string MarqueeActiveState = "MarqueeActive";
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
    private const string TransitionVisualStateName = "Transition";

    private Panel? _marqueeContainer;
    private FrameworkElement? _segment1;
    private FrameworkElement? _segment2;
    private CompositeTransform? _marqueeTransform;
    private Storyboard? _transitionStoryboard;
    private Storyboard? _marqueeStoryboard;

    private bool _isActive;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarqueeText"/> class.
    /// </summary>
    public MarqueeText()
    {
        DefaultStyleKey = typeof(MarqueeText);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Explicit casting throws early when parts are missing from the template
        _marqueeContainer = (Panel)GetTemplateChild(MarqueeContainerPartName);
        _segment1 = (FrameworkElement)GetTemplateChild(Segment1PartName);
        _segment2 = (FrameworkElement)GetTemplateChild(Segment2PartName);
        _marqueeTransform = (CompositeTransform)GetTemplateChild(MarqueeTransformPartName);
        _transitionStoryboard = (Storyboard)GetTemplateChild(TransitionAnaimationPartName);

        _marqueeContainer.SizeChanged += Container_SizeChanged;

        // Swapping tabs in TabView caused errors where the control would unload and never reattach events.
        // Hotfix: Don't detach events. This should be fine because the GC will handle it.
        // However, more research is required.
        //Unloaded += MarqueeText_Unloaded;

        VisualStateManager.GoToState(this, GetVisualStateName(Direction), false);
        VisualStateManager.GoToState(this, GetVisualStateName(Behavior), false);
        StopMarquee();
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
    /// Begins the Marquee animation if not running.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when template parts are not supplied.</exception>
    public void StartMarquee()
    {
        bool initial = _isActive;
        _isActive = true;
        bool playing = UpdateAnimation(initial);

        // Invoke MarqueeBegan if Marquee is now playing and was not before
        if (playing && !initial)
        {
            MarqueeBegan?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Stops the Marquee animation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when template parts are not supplied.</exception>
    public void StopMarquee()
    {
        StopMarquee(_isActive);
    }

    private void StopMarquee(bool initialState)
    {
        // Set _isActive and update the animation to match
        _isActive = false;
        bool playing = UpdateAnimation(false);

        // Invoke MarqueeStopped if Marquee is not playing and was before
        if (!playing && initialState)
        {
            MarqueeStopped?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Updates the animation to match the current control state.
    /// </summary>
    /// <param name="resume">True if animation should resume from its current position, false if it should restart.</param>
    /// <exception cref="InvalidOperationException">Thrown when template parts are not supplied.</exception>
    /// <returns>True if the Animation is now playing.</returns>
    private bool UpdateAnimation(bool resume = true)
    {
        // Crucial template parts are missing!
        // This can happen during initialization of certain properties.
        // Gracefully return when this happens. Proper checks for these template parts happen in OnApplyTemplate.
        if (_marqueeContainer is null ||
            _marqueeTransform is null ||
            _segment1 is null ||
            _segment2 is null ||
            _transitionStoryboard is null)
        {
            return false;
        }

        // The marquee is stopped.
        // Update the animation to the stopped position.
        if (!_isActive)
        {
            _marqueeStoryboard?.Stop();
            VisualStateManager.GoToState(this, MarqueeStoppedState, false);

            return false;
        }

        // Get the size of the container and segment, based on the orientation.
        // Also track the property to adjust, also based on the orientation.
        double containerSize;
        double segmentSize;
        double value;
        string targetProperty;

        if (IsDirectionHorizontal)
        {
            // The direction is horizontal, so the sizes, value, and properties
            // are defined by width and X coordinates.
            containerSize = _marqueeContainer.ActualWidth;
            segmentSize = _segment1.ActualWidth;
            value = _marqueeTransform.TranslateX;
            targetProperty = "(CompositeTransform.TranslateX)";
        }
        else
        {
            // The direction is vertical, so the sizes, value, and properties
            // are defined by height and Y coordinates.
            containerSize = _marqueeContainer.ActualHeight;
            segmentSize = _segment1.ActualHeight;
            value = _marqueeTransform.TranslateY;
            targetProperty = "(CompositeTransform.TranslateY)";
        }

        if (IsLooping && segmentSize < containerSize)
        {
            // If the marquee is in looping mode and the segment is smaller
            // than the container, then the animation does not not need to play.

            // NOTE: Use resume as initial because _isActive is updated before
            // calling update animation. If _isActive were passed, it would allow for
            // MarqueeStopped to be invoked when the marquee was already stopped.
            StopMarquee(resume);
            
            return false;
        }

        // The start position is offset 100% if in ticker mode
        // Otherwise it's 0
        double start = IsTicker ? containerSize : 0;

        double end = Behavior switch
        {
            // (this value just needs to be non-Zero, the real number is handled in the Style)
            MarqueeBehavior.Transition => 20,
#if !HAS_UNO
            // When the end of the text reaches the border if in bouncing mode
            MarqueeBehavior.Bouncing => containerSize - segmentSize,
#endif
            // When the first set of text is 100% out of view
            MarqueeBehavior.Ticker or MarqueeBehavior.Looping or _ => -segmentSize,
        };

        // Swap the directions if inverse direction animation
        if (IsDirectionInverse)
        {
            // Swap the start and end to inverse direction for right or upwards
            (start, end) = (end, start);
        }

        // The distance is used for calculating the duration and the previous
        // animation progress if resuming
        double distance = Math.Abs(start - end);

        // If the distance is zero, don't play an animation
        if (distance is 0)
        {
            return false;
        }

        // Calculate the animation duration by dividing the distance by the speed
        TimeSpan duration = TimeSpan.FromSeconds(distance / Speed);
        
        // Unbind events from the old storyboard and stop it before disposal.
        if (_marqueeStoryboard is not null)
        {
            _marqueeStoryboard.Completed -= StoryBoard_Completed;
            _marqueeStoryboard?.Stop();
        }

        // Create new storyboard and animation
        _marqueeStoryboard = Behavior switch
        {
            MarqueeBehavior.Transition => _transitionStoryboard,
            _ => CreateMarqueeStoryboardAnimation(start, end, duration, targetProperty),
        };

        // Bind the storyboard completed event
        _marqueeStoryboard.Completed += StoryBoard_Completed;
        
        // Set the visual state to active and begin the animation
        VisualStateManager.GoToState(this, MarqueeActiveState, true);
        _marqueeStoryboard.Begin();
        
        // If resuming, seek the animation so the text resumes from its current position.
        if (resume)
        {
            double progress = Math.Abs(start - value) / distance;
            _marqueeStoryboard.Seek(TimeSpan.FromTicks((long)(duration.Ticks * progress)));
        }

        return true;
    }

    /// <remarks>
    /// This is method is used for all modes except transition.
    /// </remarks>
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
        var posAnim = new DoubleAnimationUsingKeyFrames
        {
            Duration = duration,
            RepeatBehavior = RepeatBehavior,
#if !HAS_UNO
            AutoReverse = IsBouncing,
#endif
        };
        
        // Set the animation target and target property
        Storyboard.SetTarget(posAnim, _marqueeTransform);
        Storyboard.SetTargetProperty(posAnim, targetProperty);

        // Create the key frames
        var posFrame0 = new LinearDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero),
            Value = start,
        };
        var posFrame1 = new LinearDoubleKeyFrame
        {
            KeyTime = KeyTime.FromTimeSpan(duration),
            Value = end,
        };

        // Add the key frames to the animation
        posAnim.KeyFrames.Add(posFrame0);
        posAnim.KeyFrames.Add(posFrame1);

        // Add the double animation to the storyboard
        marqueeStoryboard.Children.Add(posAnim);

        return marqueeStoryboard;
    }

    private void UpdateClipping()
    {
        if (_marqueeContainer is null)
        {
            return;
        }

        _marqueeContainer.Clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, ActualWidth, ActualHeight)
        };

        if (IsTransition)
        {
            // Don't clip in transition mode
            _marqueeContainer.Clip = null;
        }
    }
}

#if HAS_UNO
#pragma warning restore CA1001
#endif
