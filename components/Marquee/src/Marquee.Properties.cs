// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Text;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A Control that displays Text in a Marquee style.
/// </summary>
public partial class Marquee
{
    private static readonly DependencyProperty AutoPlayProperty =
        DependencyProperty.Register(nameof(AutoPlay), typeof(bool), typeof(Marquee), new PropertyMetadata(false));
    
    private static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(Marquee), new PropertyMetadata(32d, PropertyChanged));

    private static readonly DependencyProperty RepeatBehaviorProperty =
        DependencyProperty.Register(nameof(RepeatBehavior), typeof(RepeatBehavior), typeof(Marquee), new PropertyMetadata(new RepeatBehavior(1), PropertyChanged));

    private static readonly DependencyProperty BehaviorProperty =
        DependencyProperty.Register(nameof(Behavior), typeof(MarqueeBehavior), typeof(Marquee), new PropertyMetadata(MarqueeBehavior.Ticker, BehaviorPropertyChanged));

    private static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(MarqueeDirection), typeof(Marquee), new PropertyMetadata(MarqueeDirection.Left, DirectionPropertyChanged));

    /// <summary>
    /// Gets or sets whether or not the Marquee plays immediately upon loading or updating a property.
    /// </summary>
    public bool AutoPlay
    {
        get => (bool)GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    /// <summary>
    /// Gets or sets the speed the text moves in the Marquee.
    /// </summary>
    public double Speed
    {
        get => (double)GetValue(SpeedProperty);
        set => SetValue(SpeedProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the marquee scroll repeats.
    /// </summary>
    public RepeatBehavior RepeatBehavior
    {
        get => (RepeatBehavior)GetValue(RepeatBehaviorProperty);
        set => SetValue(RepeatBehaviorProperty, value);
    }

    /// <summary>
    /// Gets or sets the marquee behavior.
    /// </summary>
    public MarqueeBehavior Behavior
    {
        get => (MarqueeBehavior)GetValue(BehaviorProperty);
        set => SetValue(BehaviorProperty, value);
    }

    private bool IsTicker => Behavior == MarqueeBehavior.Ticker;

    private bool IsLooping => Behavior == MarqueeBehavior.Looping;

#if !HAS_UNO
    private bool IsBouncing => Behavior == MarqueeBehavior.Bouncing;
#else
    private bool IsBouncing => false;
#endif

    /// <summary>
    /// Gets or sets a value indicating whether or not the marquee text wraps.
    /// </summary>
    /// <remarks>
    /// Wrapping text won't scroll if the text can already fit in the screen.
    /// </remarks>
    public MarqueeDirection Direction
    {
        get => (MarqueeDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    private bool IsDirectionHorizontal => Direction is MarqueeDirection.Left or MarqueeDirection.Right;

    private bool IsDirectionInverse => Direction is MarqueeDirection.Up or MarqueeDirection.Right;

    private static void BehaviorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Marquee control)
            return;

        var newBehavior = (MarqueeBehavior)e.NewValue;

        VisualStateManager.GoToState(control, GetVisualStateName(newBehavior), true);

        // It is always impossible to perform an on the fly behavior change.
        control.UpdateMarquee(false);

        if (control.AutoPlay)
        {
            control.StartMarquee();
        }
    }

    private static void DirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Marquee control)
            return;

        var oldDirection = (MarqueeDirection)e.OldValue;
        var newDirection = (MarqueeDirection)e.NewValue;
        bool oldAxisX = oldDirection is MarqueeDirection.Left or MarqueeDirection.Right;
        bool newAxisX = newDirection is MarqueeDirection.Left or MarqueeDirection.Right;

        VisualStateManager.GoToState(control, GetVisualStateName(newDirection), true);

        // If the axis changed we cannot update the animation on the fly.
        // Otherwise, the animation can be updated and resumed seamlessly
        control.UpdateMarquee(oldAxisX == newAxisX);

        if (control.AutoPlay)
        {
            control.StartMarquee();
        }
    }

    private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Marquee control)
            return;

        // It is always possible to update these properties on the fly.
        // NOTE: The RepeatBehavior will reset its count though. Can this be fixed?
        control.UpdateMarquee(true);

        if (control.AutoPlay)
        {
            control.StartMarquee();
        }
    }
}
