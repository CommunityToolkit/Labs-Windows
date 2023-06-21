// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class DataRow : Panel
{
    // TODO: Create our own helper class here for the Header as well vs. straight-Grid.
    // TODO: WeakReference?
    private Panel? _parentPanel;

    private Panel? FindParentHeader()
    {
        // TODO: Think about this expression instead...
        //       Drawback: Can't have Grid between table and header
        //       Positive: don't have to restart climbing the Visual Tree if we don't find ItemsPresenter...
        ////var parent = this.FindAscendant<FrameworkElement>(static (element) => element is ItemsPresenter or Grid);

        // TODO: Investigate what a scenario with an ItemsRepeater would look like (with a StackLayout, but using DataRow as the item's panel inside)

        // 1a. Get parent ItemsPresenter to find header
        if (this.FindAscendant<ItemsPresenter>() is ItemsPresenter itemsPresenter)
        {
            // 2. Quickly check if the header is just what we're looking for.
            if (itemsPresenter.Header is Grid or DataTable)
            {
                return itemsPresenter.Header as Panel;
            }

            // 3. Otherwise, try and find the inner thing we want.
            return itemsPresenter.FindDescendant<Panel>(static (element) => element is Grid or DataTable);
        }

        // 1b. If we can't find the ItemsPresenter, then we reach up outside to find the next thing we could use as a parent
        return this.FindAscendant<Panel>(static (element) => element is Grid or DataTable);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // We should probably only have to do this once ever?
        _parentPanel ??= FindParentHeader();
        
        if (Children.Count > 0)
        {
            // If we don't have a grid, just measure first child to get row height and take available space
            if (_parentPanel is null)
            {
                Children[0].Measure(availableSize);
                return new Size(availableSize.Width, Children[0].DesiredSize.Height);
            }
            else if (_parentPanel is DataTable table && table.IsAnyColumnAuto &&
                     table.Children.Count == Children.Count)
            {
                // Measure all children since we have a column that cares about it
                for (int i = 0; i < Children.Count; i++)
                {
                    if (table.Children[i] is DataColumn { DesiredWidth.GridUnitType: GridUnitType.Auto } col)
                    {
                        Children[i].Measure(availableSize);

                        // TODO: Do we want this to ever shrink back?
                        var prev = col.MaxChildDesiredWidth;
                        col.MaxChildDesiredWidth = Math.Max(col.MaxChildDesiredWidth, Children[i].DesiredSize.Width);
                        if (col.MaxChildDesiredWidth != prev)
                        {
                            // TODO: Clean this logic up?
                            // If our measure has changed, then we have to invalidate the arrange of the DataTable
                            _parentPanel.InvalidateArrange();
                        }
                    }
                }
            }
        }

        // Otherwise, return our parent's size as the desired size.
        return _parentPanel?.DesiredSize ?? new Size(availableSize.Width, 0);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int column = 0;
        double x = 0;

        double spacing = (_parentPanel as Grid)?.ColumnSpacing ?? 0; // TODO: Spacing for DataTable?

        double width = 0;

        if (_parentPanel != null)
        {
            foreach (UIElement child in Children.Where(static e => e.Visibility == Visibility.Visible))
            {
                if (_parentPanel is Grid grid &&
                    column < grid.ColumnDefinitions.Count)
                {
                    width = grid.ColumnDefinitions[column++].ActualWidth;                    
                }
                // TODO: Need to check Column visibility here as well...
                else if (_parentPanel is DataTable table &&
                    column < table.Children.Count)
                {
                    // TODO: This is messy...
                    width = (table.Children[column++] as DataColumn)?.ActualWidth ?? 0;
                }

                // Note: For Auto, since we measured our children and bubbled that up to the DataTable layout, then the DataColumn size we grab above should account for the largest of our children.
                child.Arrange(new Rect(x, 0, width, finalSize.Height));

                x += width + spacing;
            }

            return new Size(x - spacing, finalSize.Height);
        }

        return finalSize;
    }
}
