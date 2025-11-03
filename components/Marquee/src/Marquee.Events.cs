// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A Control that displays Text in a Marquee style.
/// </summary>
public partial class Marquee
{
    /// <summary>
    /// Event raised when the Marquee begins scrolling.
    /// </summary>
    public event EventHandler? MarqueeStarted;

    /// <summary>
    /// Event raised when the Marquee is stopped manually or completed.
    /// </summary>
    public event EventHandler? MarqueeStopped;

    /// <summary>
    /// Event raised when the Marquee is resumed from a pause.
    /// </summary>
    public event EventHandler? MarqueeResumed;

    /// <summary>
    /// Event raised when the Marquee is paused.
    /// </summary>
    public event EventHandler? MarqueePaused;

    /// <summary>
    /// Event raised when the Marquee completes scrolling.
    /// </summary>
    public event EventHandler? MarqueeCompleted;
    
    private void Marquee_Loaded(object sender, RoutedEventArgs e)
    {
        // While loaded, detach the loaded event and attach the unloaded event
        this.Loaded -= this.Marquee_Loaded;
        this.Unloaded += this.Marquee_Unloaded;

        // Attach other events
        if (_marqueeContainer is not null)
        {
            _marqueeContainer.SizeChanged += Container_SizeChanged;
        }

        if (_segment1 is not null)
        {
            _segment1.SizeChanged += Segment_SizeChanged;
        }

        if (_marqueeStoryboard is not null)
        {
            _marqueeStoryboard.Completed += StoryBoard_Completed;
        }

        // The size may have channged while unloaded.
        // Clip the marquee
        ClipMarquee();

        // Setup the animation
        UpdateMarquee(false);

        // The marquee should run when loaded if auto play is enabled
        if (AutoPlay)
        {
            StartMarquee();
        }
    }

    private void Marquee_Unloaded(object sender, RoutedEventArgs e)
    {
        // Restore the loaded event and detach the unloaded event 
        this.Loaded += Marquee_Loaded;
        this.Unloaded -= Marquee_Unloaded;

        if (_marqueeContainer is not null)
        {
            _marqueeContainer.SizeChanged -= Container_SizeChanged;
        }

        if (_segment1 is not null)
        {
            _segment1.SizeChanged -= Segment_SizeChanged;
        }

        if (_marqueeStoryboard is not null)
        {
            _marqueeStoryboard.Completed -= StoryBoard_Completed;
        }
    }

    private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_marqueeContainer is null)
            return;
        
        // Clip the marquee
        ClipMarquee(e.NewSize.Width, e.NewSize.Height);

        // Update animation on the fly
        UpdateMarquee(true);

        // The marquee should run when the size changes in case the text gets cutoff
        // and auto play is enabled.
        if (AutoPlay)
        {
            StartMarquee();
        }
    }

    private void Segment_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_segment1 is null)
            return;

        if (_marqueeContainer is null)
            return;

        // Cap the height of the container to the segment height
        _marqueeContainer.Height = _segment1.ActualHeight;

        // If the segment size changes, we need to update the storyboard,
        // and seek to the correct position to maintain a smooth animation.
        UpdateMarquee(true);
    }

    private void StoryBoard_Completed(object? sender, object e)
    {
        StopMarquee();
        MarqueeCompleted?.Invoke(this, EventArgs.Empty);
    }
}
