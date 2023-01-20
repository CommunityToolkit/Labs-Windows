// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class EqualPanel : Panel
{
    private double maxItemWidth = 0;
    private double maxItemHeight = 0;

    public int Spacing { get; set; }
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

            return new Size((maxItemWidth * Children.Count) + (Spacing * Children.Count - 1), maxItemHeight);
        }
        else
        {
            return new Size(maxItemWidth, maxItemHeight);
        }
    }
}
