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
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = LeftwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = RightwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = UpwardsVisualStateName)]
[TemplateVisualState(GroupName = DirectionVisualStateGroupName, Name = DownwardsVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = TickerVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = LoopingVisualStateName)]
[TemplateVisualState(GroupName = BehaviorVisualStateGroupName, Name = BouncingVisualStateName)]
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

    private Panel? _marqueeContainer;
    private FrameworkElement? _segment1;
    private FrameworkElement? _segment2;
    private TranslateTransform? _marqueeTransform;
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

        _marqueeContainer = (Panel)GetTemplateChild(MarqueeContainerPartName);
        _segment1 = (FrameworkElement)GetTemplateChild(Segment1PartName);
        _segment2 = (FrameworkElement)GetTemplateChild(Segment2PartName);
        _marqueeTransform = (TranslateTransform)GetTemplateChild(MarqueeTransformPartName);

        _marqueeContainer.SizeChanged += Container_SizeChanged;

        // Swapping tabs in TabView caused errors where the control would unload and never reattach events.
        // Hotfix: Don't detach events. This should be fine because the GC will handle it.
        // However, more research is required.
        //Unloaded += MarqueeText_Unloaded;

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
    /// Begins the Marquee animation if not running.
    /// </summary>
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
    public void StopMarquee()
    {
        StopMarquee(_isActive);
    }

    private void StopMarquee(bool stopping)
    {
        _isActive = false;
        bool playing = UpdateAnimation(false);

        // Invoke MarqueeStopped if Marquee is not playing and was before
        if (!playing && stopping)
        {
            MarqueeStopped?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Updates the animation to match the current control state.
    /// </summary>
    /// <param name="resume">True if animation should resume from its current position, false if it should restart.</param>
    /// <returns>True if the Animation is now playing</returns>
    private bool UpdateAnimation(bool resume = true)
    {
        if (_marqueeContainer is null ||
            _marqueeTransform is null ||
            _segment1 is null ||
            _segment2 is null)
        {
            return false;
        }

        if (!_isActive)
        {
            VisualStateManager.GoToState(this, MarqueeStoppedState, false);

            return false;
        }

        // Get the size (width horizontal, height if vertical) of the
        // contain and segment.
        // Also track the property to adjust based on the orientation.
        double containerSize;
        double segmentSize;
        double value;
        string property;

        if (IsDirectionHorizontal)
        {
            containerSize = _marqueeContainer.ActualWidth;
            segmentSize = _segment1.ActualWidth;
            value = _marqueeTransform.X;
            property = "(TranslateTransform.X)";
        }
        else
        {
            containerSize = _marqueeContainer.ActualHeight;
            segmentSize = _segment1.ActualHeight;
            value = _marqueeTransform.Y;
            property = "(TranslateTransform.Y)";
        }

        if (IsLooping && segmentSize < containerSize)
        {
            // If the text segment is smaller than the area provided,
            // it does not need to run in looping mode.
            StopMarquee(resume);
            _segment2.Visibility = Visibility.Collapsed;
            return false;
        }

        // The start position is offset 100% if ticker
        // Otherwise it's 0
        double start = IsTicker ? containerSize : 0;
        // The end is when the end of the text reaches the border if bounding
        // Otherwise it is when the first set of text is 100% out of view
        double end = IsBouncing ? containerSize - segmentSize : -segmentSize;

        // The distance is used for calculating the duration and the progress if resuming
        double distance = Math.Abs(start - end);

        if (distance is 0)
        {
            return false;
        }

        // Swap the start and end to inverse direction for right or upwards
        if (IsDirectionInverse)
        {
            (start, end) = (end, start);
        }

        // The second segment of text should be hidden if the marquee is not in looping mode.
        _segment2.Visibility = IsLooping ? Visibility.Visible : Visibility.Collapsed;

        TimeSpan duration = TimeSpan.FromSeconds(distance / Speed);

        if (_marqueeStoryboard is not null)
        {
            _marqueeStoryboard.Completed -= StoryBoard_Completed;
        }

        _marqueeStoryboard = new Storyboard
        {
            Duration = duration,
            RepeatBehavior = RepeatBehavior,
#if !HAS_UNO
            AutoReverse = IsBouncing,
#endif
        };

        _marqueeStoryboard.Completed += StoryBoard_Completed;

        var animation = new DoubleAnimationUsingKeyFrames
        {
            Duration = duration,
            RepeatBehavior = RepeatBehavior,
#if !HAS_UNO
            AutoReverse = IsBouncing,
#endif
        };
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

        animation.KeyFrames.Add(frame1);
        animation.KeyFrames.Add(frame2);
        _marqueeStoryboard.Children.Add(animation);
        Storyboard.SetTarget(animation, _marqueeTransform);
        Storyboard.SetTargetProperty(animation, property);

        VisualStateManager.GoToState(this, MarqueeActiveState, true);
        _marqueeStoryboard.Begin();

        if (resume)
        {
            // Seek the animation so the text is in the same position.
            double progress = Math.Abs(start - value) / distance;
            _marqueeStoryboard.Seek(TimeSpan.FromTicks((long)(duration.Ticks * progress)));
        }

        return true;
    }
}

#if HAS_UNO
#pragma warning restore CA1001
#endif
