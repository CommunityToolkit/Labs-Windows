// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Text;

namespace CommunityToolkit.Labs.WinUI.MarqueeTextRns;

/// <summary>
/// A Control that displays Text in a Marquee style.
/// </summary>
public partial class MarqueeText
{
    private static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(MarqueeText), new PropertyMetadata(null, TextPropertyChanged));

    private static readonly DependencyProperty SecondaryTextProperty =
        DependencyProperty.Register(nameof(SecondaryText), typeof(string), typeof(MarqueeText), new PropertyMetadata(null));

    private static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(MarqueeText), new PropertyMetadata(32d, PropertyChanged));

    private static readonly DependencyProperty RepeatBehaviorProperty =
        DependencyProperty.Register(nameof(RepeatBehavior), typeof(RepeatBehavior), typeof(MarqueeText), new PropertyMetadata(new RepeatBehavior(1), PropertyChanged));

    private static readonly DependencyProperty StartOnTextChangedProperety =
        DependencyProperty.Register(nameof(StartOnTextChanged), typeof(bool), typeof(MarqueeText), new PropertyMetadata(false));

    private static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(MarqueeDirection), typeof(MarqueeText), new PropertyMetadata(MarqueeDirection.Left, DirectionPropertyChanged));

    private static readonly DependencyProperty TickerStartPositionProperty =
        DependencyProperty.Register(nameof(TickerStartPosition), typeof(double), typeof(MarqueeText), new PropertyMetadata(0));

    private static readonly DependencyProperty TickerEndPositionProperty =
        DependencyProperty.Register(nameof(TickerEndPosition), typeof(double), typeof(MarqueeText), new PropertyMetadata(0));

    private static readonly DependencyProperty TickerAnimationDurationProperty =
        DependencyProperty.Register(nameof(TickerAnimationDuration), typeof(TimeSpan), typeof(MarqueeText), new PropertyMetadata(TimeSpan.Zero));

    #if !HAS_UNO
    private static readonly DependencyProperty TextDecorationsProperty =
        DependencyProperty.Register(nameof(TextDecorations), typeof(TextDecorations), typeof(MarqueeText), new PropertyMetadata(TextDecorations.None));
    #endif

    /// <summary>
    /// Gets or sets the text being displayed in Marquee.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets a secondary text field used for binding the secondary text block.
    /// </summary>
    /// <remarks>
    /// When the <see cref="TextProperty"/> is updated, this 
    /// </remarks>
    public string SecondaryText
    {
        get => (string)GetValue(SecondaryTextProperty);
        private set => SetValue(SecondaryTextProperty, value);
    }

    /// <summary>
    /// Gets or sets the speed the text moves in the Marquee.
    /// </summary>
    /// <remarks>
    /// Ignored if the behavior is <see cref="MarqueeBehavior.Cycle"/>
    /// </remarks>
    public double Speed
    {
        get => (double)GetValue(SpeedProperty);
        set => SetValue(SpeedProperty, value);
    }

    /// <summary>
    /// Gets or sets whether or not the animation begin when the <see cref="TextProperty"/> is updated.
    /// </summary>
    public bool StartOnTextChanged
    {
        get => (bool)GetValue(StartOnTextChangedProperety);
        set => SetValue(StartOnTextChangedProperety, value);
    }

    /// <summary>
    /// Gets or sets whether or not to ignore if the text fits when running the animation.
    /// </summary>
    public bool IgnoreFitting => true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the marquee scroll repeats.
    /// </summary>
    public RepeatBehavior RepeatBehavior
    {
        get => (RepeatBehavior)GetValue(RepeatBehaviorProperty);
        set => SetValue(RepeatBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets the direction the Marquee should scroll
    /// </summary>
    public MarqueeDirection Direction
    {
        get => (MarqueeDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public double TickerStartPosition
    {
        get => (double)GetValue(TickerStartPositionProperty);
        set => SetValue(TickerStartPositionProperty, value);
    }

    public double TickerEndPosition
    {
        get => (double)GetValue(TickerEndPositionProperty);
        set => SetValue(TickerEndPositionProperty, value);
    }

    public TimeSpan TickerAnimationDuration
    {
        get => (TimeSpan)GetValue(TickerAnimationDurationProperty);
        set => SetValue(TickerAnimationDurationProperty, value);
    }

    /// <summary>
    /// Gets whether or not the marquee animation is playing.
    /// </summary>
    public bool IsRunning => _isActive;

    private bool IsDirectionHorizontal => Direction is MarqueeDirection.Left or MarqueeDirection.Right;

    private bool IsDirectionInverse => Direction is MarqueeDirection.Down or MarqueeDirection.Right;

    // Waiting on https://github.com/unoplatform/uno/issues/12929
    #if !HAS_UNO
    /// <summary>
    /// Gets or sets a value that indicates what decorations are applied to the text.
    /// </summary>
    public TextDecorations TextDecorations
    {
        get => (TextDecorations)GetValue(TextDecorationsProperty);
        set => SetValue(TextDecorationsProperty, value);
    }
    #endif

    private static void BehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MarqueeText control)
        {
            return;
        }

        bool active = control._isActive;
        var newBehavior = (MarqueeBehavior)e.NewValue;
        
        control.UpdateClipping();

        control.StopMarquee(false);
        if (active)
        {
            control.StartMarquee();
        }
    }

    private static void DirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MarqueeText control)
        {
            return;
        }

        bool active = control._isActive;
        var oldDirection = (MarqueeDirection)e.OldValue;
        var newDirection = (MarqueeDirection)e.NewValue;
        bool oldAxisX = oldDirection is MarqueeDirection.Left or MarqueeDirection.Right;
        bool newAxisX = newDirection is MarqueeDirection.Left or MarqueeDirection.Right;
        
        control.StopMarquee(false);

        VisualStateManager.GoToState(control, GetVisualStateName(newDirection), true);

        if (active)
        {
            control.StartMarquee();
        }
    }

    private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MarqueeText control)
        {
            return;
        }

        if (!control.StartOnTextChanged)
        {
            PropertyChanged(d, e);
            return;
        }
        
        if (!control._isActive)
        {
            // We can skip this if the animation is already
            // playing because that's smoother than starting a new animation.
            control.StartMarquee();
        }
    }

    private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MarqueeText control)
        {
            return;
        }

        control.UpdateAnimationProperties();
        control.ResumeAnimation();
    }

    private void UpdateAnimationProperties()
    {
        if (_marqueeContainer is null || _segment1 is null)
        {
            return;
        }

        TickerStartPosition = IsDirectionHorizontal ? _marqueeContainer.ActualWidth : _marqueeContainer.ActualHeight;
        TickerEndPosition = IsDirectionHorizontal ? -_segment1.ActualWidth : -_segment1.ActualHeight;
        TickerAnimationDuration = TimeSpan.FromSeconds(Math.Abs(TickerStartPosition - TickerEndPosition) / Speed);
    }
}
