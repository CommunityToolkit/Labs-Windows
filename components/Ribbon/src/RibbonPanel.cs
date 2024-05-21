// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A panel which will set the <see cref="RibbonCollapsibleGroup"/> items in a collapsed state if there is not enough space to render them.
/// It is used by the <see cref="Ribbon"/> control.
/// </summary>
internal sealed class RibbonPanel : Panel
{
    private static readonly Size GroupAvailableSize = new(double.PositiveInfinity, double.PositiveInfinity);

    protected override Size MeasureOverride(Size availableSize)
    {
        // We try to limit the layout changes if the parent scrollviewer is sending values with small changes.
        availableSize.Width = Math.Floor(availableSize.Width);

        var childrenByPriority = Children.OrderBy(c => c is RibbonCollapsibleGroup collapsibleGroup ? collapsibleGroup.Priority : 0);
        var desiredSize = new Size();
        foreach (var child in childrenByPriority)
        {
            var collapsibleGroup = child as RibbonCollapsibleGroup;
            var requestedWidths = collapsibleGroup?.RequestedWidths;
            if (requestedWidths is null || collapsibleGroup?.State == Visibility.Collapsed)
            {
                child.Measure(GroupAvailableSize);
            }
            else
            {
                // Get the closest match to remainingWidth or use infinite size if we do not have any match.
                var remainingWidth = availableSize.Width - desiredSize.Width;
                //var requestedWidth = requestedWidths.LastOrDefault(w => w <= remainingWidth, defaultValue: double.PositiveInfinity);
                var matchingWidths = requestedWidths.Where(w => w <= remainingWidth);
                var requestedWidth = matchingWidths.Any() ? matchingWidths.Last() : double.PositiveInfinity;
                var fixedSize = new Size(requestedWidth, availableSize.Height);
                child.Measure(fixedSize);
            }

            desiredSize.Width += child.DesiredSize.Width;
            desiredSize.Height = Math.Max(desiredSize.Height, child.DesiredSize.Height);
        }

        if (desiredSize.Width > availableSize.Width)
        {
            // We need to collapse some groups.
            // If there is no priority order we assume that the last items are the one which should collapse first.
            var groups = Children.OfType<RibbonCollapsibleGroup>().Reverse().Where(g => g.State == Visibility.Visible).OrderByDescending(g => g.Priority);
            foreach (var group in groups)
            {
                group.State = Visibility.Collapsed;
                var previousSize = group.DesiredSize;
                group.Measure(GroupAvailableSize);
                var newSize = group.DesiredSize;

                if (newSize.Width < previousSize.Width)
                {
                    desiredSize.Width -= previousSize.Width - newSize.Width;
                    desiredSize.Height = Math.Max(desiredSize.Height, newSize.Height);
                }
                else
                {
                    // The collapsed size is bigger so keep using the visible state.
                    group.Visibility = Visibility.Visible;
                    group.Measure(GroupAvailableSize);
                }

                if (desiredSize.Width < availableSize.Width)
                {
                    // No need to collapse more groups.
                    break;
                }
            }
        }
        else if (desiredSize.Width < availableSize.Width)
        {
            // We have more space than needed, we check if we can expand some groups
            var groups = Children.OfType<RibbonCollapsibleGroup>().Where(g => g.State == Visibility.Collapsed).OrderBy(g => g.Priority);
            foreach (var group in groups)
            {
                var previousSize = group.DesiredSize;
                group.State = Visibility.Visible;

                var requestedWidths = group.RequestedWidths;
                if (requestedWidths is null)
                {
                    group.Measure(GroupAvailableSize);
                }
                else
                {
                    // Get the closest match to remainingWidth or use infinite size if we do not have any match.
                    var remainingWidth = availableSize.Width + previousSize.Width - desiredSize.Width;
                    //var requestedWidth = requestedWidths.LastOrDefault(w => w <= remainingWidth, defaultValue: double.PositiveInfinity);
                    var matchingWidths = requestedWidths.Where(w => w <= remainingWidth);
                    var requestedWidth = matchingWidths.Any() ? matchingWidths.Last() : double.PositiveInfinity;
                    var fixedSize = new Size(requestedWidth, availableSize.Height);
                    group.Measure(fixedSize);
                }

                var newSize = group.DesiredSize;
                var widthIncrease = newSize.Width - previousSize.Width;
                if (desiredSize.Width + widthIncrease > availableSize.Width)
                {
                    // Too wide, we revert the change
                    group.State = Visibility.Collapsed;
                    group.Measure(GroupAvailableSize);
                    break;
                }

                desiredSize.Width += widthIncrease;
                desiredSize.Height = Math.Max(desiredSize.Height, newSize.Height);
            }
        }

        return desiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var position = new Rect
        {
            Height = finalSize.Height
        };

        foreach (var child in Children)
        {
            position.Width = child.DesiredSize.Width;
            child.Arrange(position);

            position.X += position.Width;
        }

        return new Size(position.X, position.Height);
    }
}
