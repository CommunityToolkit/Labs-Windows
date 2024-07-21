// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.MarqueeTextRns;

/// <summary>
/// A Control that displays Text in a Marquee style.
/// </summary>
public partial class MarqueeText
{
    /// <summary>
    /// Event raised when the Marquee begins scrolling.
    /// </summary>
    public event EventHandler? MarqueeBegan;

    /// <summary>
    /// Event raised when the Marquee stops scrolling for any reason.
    /// </summary>
    public event EventHandler? MarqueeStopped;

    /// <summary>
    /// Event raised when the Marquee completes scrolling.
    /// </summary>
    public event EventHandler? MarqueeCompleted;

    private void MarqueeText_Unloaded(object sender, RoutedEventArgs e)
    {
        this.Unloaded -= MarqueeText_Unloaded;

        if (_marqueeContainer is not null)
        {
            _marqueeContainer.SizeChanged -= Container_SizeChanged;
        }

        if (_marqueeStoryboard is not null)
        {
            _marqueeStoryboard.Completed -= StoryBoard_Completed;
        }
    }

    private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_marqueeContainer is null)
        {
            return;
        }
        
        // Clip the marquee within its bounds
        UpdateClipping();

        // Update the animation when the size changes
        // Unless in transition mode where the container size doesn't affect the animation.
        if (!IsTransition)
        {
            UpdateAnimation(true);
        }
    }

    private void StoryBoard_Completed(object? sender, object e)
    {
        StopMarquee(true);
        MarqueeCompleted?.Invoke(this, EventArgs.Empty);

        // Update the secondary text to match the new text
        if (IsTransition)
        {
            SecondaryText = Text;
        }
    }
}
