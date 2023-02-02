// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public class EqualPanel : Panel
{
    public int Spacing { get; set; }

    private double maxItemWidth = 0;
    private double maxItemHeight = 0;

    protected override Size ArrangeOverride(Size finalSize)
    {
        var x = 0.0;
        foreach (var child in Children)
        {
            var newpos = new Rect(x, 0, maxItemWidth, maxItemHeight);
            child.Arrange(newpos);
            x += maxItemWidth + Spacing;
        }
        return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count > 0)
        {
            // The panel columns should have the same width: that of the item width the largest width.
            foreach (var item in Children)
            {
                item.Measure(availableSize);

                var desiredWidth = item.DesiredSize.Width;
                if (desiredWidth > maxItemWidth)
                    maxItemWidth = desiredWidth;

                var desiredHeight = item.DesiredSize.Height;
                if (desiredHeight > maxItemHeight)
                    maxItemHeight = desiredHeight;
            }

            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return new Size((maxItemWidth * Children.Count) + (Spacing * (Children.Count - 1)), maxItemHeight);
            }
            else
            {
                // The panel should equally split the available width when HorizontalAlignment is set to Stretch
                var availableWidth = availableSize.Width - (Spacing * (Children.Count - 1));
                maxItemWidth = availableWidth / Children.Count;
                return new Size(availableWidth, maxItemHeight);
            }
        }
        else
        {
            return new Size(maxItemWidth, maxItemHeight);
        }
    }
}
