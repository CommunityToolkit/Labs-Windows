// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class DataTable : Panel
{
    // TODO: Create our own helper class here for the Header as well vs. straight-Grid.
    // TODO: WeakReference?
    private Grid? _parentGrid;

    private Grid? FindParentHeader()
    {
        // TODO: Think about this expression instead...
        //       Drawback: Can't have Grid between table and header
        //       Positive: don't have to restart climbing the Visual Tree if we don't find ItemsPresenter...
        ////var parent = this.FindAscendant<FrameworkElement>(static (element) => element is ItemsPresenter or Grid);

        // 1a. Get parent ItemsPresenter to find header
        if (this.FindAscendant<ItemsPresenter>() is ItemsPresenter itemsPresenter)
        {
            // 2. Quickly check if the header is just what we're looking for.
            if (itemsPresenter.Header is Grid grid)
            {
                return grid;
            }

            // 3. Otherwise, try and find the inner thing we want.
            return itemsPresenter.FindDescendant<Grid>();
        }

        // 1b. If we can't find the ItemsPresenter, then we reach up outside to find the next thing we could use as a parent
        return this.FindAscendant<Grid>();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // We should probably only have to do this once ever?
        _parentGrid ??= FindParentHeader();

        // If we don't have a grid, just measure first child to get row height
        if (_parentGrid is null && Children.Count > 0)
        {
            Children[0].Measure(availableSize);
            return new Size(availableSize.Width, Children[0].DesiredSize.Height);
        }

        // Otherwise, return our parent's size as the desired size.
        return _parentGrid?.DesiredSize ?? new Size(availableSize.Width, 0);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int column = 0;
        double x = 0;

        if (_parentGrid != null)
        {
            foreach (UIElement child in Children)
            {
                if (child.Visibility == Visibility.Visible &&
                    column < _parentGrid.ColumnDefinitions.Count)
                {
                    var width = _parentGrid.ColumnDefinitions[column++].ActualWidth;
                    child.Arrange(new Rect(x, 0, width, finalSize.Height));

                    x += width + _parentGrid.ColumnSpacing;
                }
            }

            return new Size(x - _parentGrid.ColumnSpacing, finalSize.Height);
        }

        return finalSize;
    }
}
