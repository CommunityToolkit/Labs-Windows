// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using TreeView = Microsoft.UI.Xaml.Controls.TreeView;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Row item of <see cref="DataTable"/>.
/// </summary>
public partial class DataRow : Panel
{
    // TODO: Create our own helper class here for the Header as well vs. straight-Grid.
    // TODO: WeakReference?
    private DataTable? _parentTable;

    private bool _isTreeView;
    private double _treePadding;

    /// <summary>
    /// Constructor.
    /// </summary>
    public DataRow()
    {
        Unloaded += this.DataRow_Unloaded;
    }

    private void DataRow_Unloaded(object sender, RoutedEventArgs e)
    {
        // Remove our references on unloaded
        _parentTable?.Rows.Remove(this);
        _parentTable = null;
    }

    private DataTable? InitializeParentHeaderConnection()
    {
        // TODO: Think about this expression instead...
        //       Drawback: Can't have Grid between table and header
        //       Positive: don't have to restart climbing the Visual Tree if we don't find ItemsPresenter...
        ////var parent = this.FindAscendant<FrameworkElement>(static (element) => element is ItemsPresenter or Grid);

        // TODO: Investigate what a scenario with an ItemsRepeater would look like (with a StackLayout, but
        //       using DataRow as the item's panel inside)
        Panel? panel = null;

        // 1a. Get parent ItemsPresenter to find header
        if (this.FindAscendant<ItemsPresenter>() is ItemsPresenter itemsPresenter)
        {
            if (itemsPresenter.Header is DependencyObject header)
            {
                panel = header.FindDescendantOrSelf<DataTable>();
            }

            // Check if we're in a TreeView
            _isTreeView = itemsPresenter.FindAscendant<TreeView>() is TreeView;
        }

        // Cache actual datatable reference
        if (panel is DataTable table)
        {
            _parentTable = table;
            _parentTable.Rows.Add(this); // Add us to the row list.
        }

        return _parentTable;
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        // We should probably only have to do this once ever?
        _parentTable ??= InitializeParentHeaderConnection();

        double maxHeight = 0;

        // If we don't have a DataTable, just layout children like a horizontal StackPanel.
        if (_parentTable is null)
        {
            double totalWidth = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (child?.Visibility != Visibility.Visible)
                    continue;

                child.Measure(availableSize);

                totalWidth += child.DesiredSize.Width;
                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }

            return new Size(totalWidth, maxHeight);
        }
        // Handle DataTable Parent
        else
        {
            int maxChildCount = Math.Min(_parentTable.Children.Count, Children.Count);

            // Measure all children which have corresponding visible DataColumns.
            for (int i = 0; i < maxChildCount; i++)
            {
                var child = Children[i];
                var column = _parentTable.Children[i] as DataColumn;
                if (column?.Visibility != Visibility.Visible)
                    continue;

                if (column.IsAuto)
                {
                    child.Measure(availableSize);

                    // For TreeView in the first column, we want the header to expand to encompass
                    // the maximum indentation of the tree.
                    double padding = 0;
                    //// TODO: We only want/need to do this once? We may want to do if we're not an Auto column too...?
                    if (i == 0 && _isTreeView)
                    {
                        // Get our containing grid from TreeViewItem, start with our indented padding
                        var parentContainer = this.FindAscendant("MultiSelectGrid") as Grid;
                        if (parentContainer != null)
                        {
                            _treePadding = parentContainer.Padding.Left;
                            // We assume our 'DataRow' is in the last child slot of the Grid, need to know
                            // how large the other columns are.
                            for (int j = 0; j < parentContainer.Children.Count - 1; j++)
                            {
                                // TODO: We may need to get the actual size here later in Arrange?
                                _treePadding += parentContainer.Children[j].DesiredSize.Width;
                            }
                        }
                        padding = _treePadding;
                    }

                    // TODO: Do we want this to ever shrink back?
                    var prev = column.MaxChildDesiredWidth;
                    column.MaxChildDesiredWidth = Math.Max(column.MaxChildDesiredWidth, child.DesiredSize.Width + padding);
                    if (column.MaxChildDesiredWidth != prev)
                    {
                        // If our measure has changed, then we have to invalidate the arrange of the DataTable
                        _parentTable.ColumnResized();
                    }

                }
                else if (column.IsAbsolute)
                {
                    child.Measure(new(column.DesiredWidth.Value, availableSize.Height));
                }
                else
                {
                    child.Measure(availableSize);
                }

                maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
            }

            // Return our parent's size as the desired size.
            return new Size(_parentTable.DesiredSize.Width, maxHeight);
        }
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        // If we don't have DataTable, just layout children like a horizontal StackPanel.
        if (_parentTable is null)
        {
            double x = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                if (child?.Visibility != Visibility.Visible)
                    continue;

                double width = child.DesiredSize.Width;

                child.Arrange(new Rect(x, 0, width, finalSize.Height));

                x += width;
            }

            return new Size(x, finalSize.Height);
        }
        // Handle DataTable Parent
        else
        {
            int maxChildCount = Math.Min(_parentTable.Children.Count, Children.Count);

            double columnSpacing = _parentTable.ColumnSpacing;
            double x = double.NaN;

            // Arrange all children which have corresponding visible DataColumns.
            for (int i = 0; i < maxChildCount; i++)
            {
                var column = _parentTable.Children[i] as DataColumn;
                if (column?.Visibility != Visibility.Visible)
                    continue;

                if (double.IsNaN(x))
                    x = 0;
                else
                    x += columnSpacing;

                // TODO: This is messy...
                double width = column.ActualWidth;

                var child = Children[i];

                // Note: For Auto, since we measured our children and bubbled that up to the DataTable layout,
                //       then the DataColumn size we grab above should account for the largest of our children.
                if (i == 0)
                {
                    child.Arrange(new Rect(x, 0, width, finalSize.Height));
                }
                else
                {
                    // If we're in a tree, remove the indentation from the layout of columns beyond the first.
                    child.Arrange(new Rect(x - _treePadding, 0, width, finalSize.Height));
                }

                x += width;
            }

            return new Size(x, finalSize.Height);
        }
    }
}
