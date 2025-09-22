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
    public event EventHandler? MarqueeBegan;

    /// <summary>
    /// Event raised when the Marquee stops scrolling for any reason.
    /// </summary>
    public event EventHandler? MarqueeStopped;

    /// <summary>
    /// Event raised when the Marquee completes scrolling.
    /// </summary>
    public event EventHandler? MarqueeCompleted;
    
    private void Marquee_Loaded(object sender, RoutedEventArgs e)
    {
        // While loaded, detach the loaded event and attach the unloaded event
        this.Loaded -= this.Marquee_Loaded;
        this.Unloaded += Marquee_Unloaded;

        // Attach other events
        if (_marqueeContainer is not null)
        {
            _marqueeContainer.SizeChanged += Container_SizeChanged;
        }

        if (_marqueeStoryboard is not null)
        {
            _marqueeStoryboard.Completed += StoryBoard_Completed;
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
        _marqueeContainer.Clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
        };

        // The marquee should run when the size changes in case the text gets cutoff
        StartMarquee();
    }

    private void StoryBoard_Completed(object? sender, object e)
    {
        StopMarquee(true);
        MarqueeCompleted?.Invoke(this, EventArgs.Empty);
    }
}
