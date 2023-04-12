// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A <see cref="DataTable"/> is a <see cref="Panel"/> which lays out <see cref="DataColumn"/>s based on
/// their configured properties (akin to <see cref="ColumnDefinition"/>); similar to a <see cref="Grid"/> with a single row.
/// </summary>
public partial class DataTable : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        double fixedWidth = 0;
        double proportionalUnits = 0;
        double autoSized = 0;

        double maxHeight = 0;

        var elements = Children.Where(e => e.Visibility == Visibility.Visible && e is DataColumn);

        // We only need to measure elements that are visible
        foreach (DataColumn column in elements)
        {
            if (column.DesiredWidth.IsStar)
            {
                proportionalUnits += column.DesiredWidth.Value;
            }
            else if (column.DesiredWidth.IsAbsolute)
            {
                fixedWidth += column.DesiredWidth.Value;
            }
        }

        // TODO: Handle infinite width?
        var proportionalAmount = (availableSize.Width - fixedWidth) / proportionalUnits;

        foreach (DataColumn column in elements)
        {
            if (column.DesiredWidth.IsStar)
            {
                column.Measure(new Size(proportionalAmount * column.DesiredWidth.Value, availableSize.Height));
            }
            else if (column.DesiredWidth.IsAbsolute)
            {
                column.Measure(new Size(column.DesiredWidth.Value, availableSize.Height));
            }
            else
            {
                // TODO: Technically this is using 'Auto' on the Header content
                // What the developer probably intends is it to be adjusted based on the contents of the rows...
                // To enable this scenario, we'll need to actually measure the contents of the rows for that column
                // in DataRow and figure out the maximum size to report back and adjust here in some sort of hand-shake
                // for the layout process... (i.e. get the data in the measure step, use it in the arrange step here,
                // then invalidate the child arranges [don't re-measure and cause loop]...)

                // For now, we'll just use the header content as a guideline to see if things work.
                column.Measure(new Size(availableSize.Width - fixedWidth - autoSized, availableSize.Height));

                // Keep track of already 'allotted' space
                autoSized += column.DesiredSize.Width;
            }

            maxHeight = Math.Max(maxHeight, column.DesiredSize.Height);
        }

        return new Size(availableSize.Width, maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double fixedWidth = 0;
        double proportionalUnits = 0;
        double autoSized = 0;

        var elements = Children.Where(e => e.Visibility == Visibility.Visible && e is DataColumn);

        // We only need to measure elements that are visible
        foreach (DataColumn column in elements)
        {
            if (column.DesiredWidth.IsStar)
            {
                proportionalUnits += column.DesiredWidth.Value;
            }
            else if (column.DesiredWidth.IsAbsolute)
            {
                fixedWidth += column.DesiredWidth.Value;
            }
            else
            {
                // Note: This is different that the measure step as we know the desired width now from the measure.
                autoSized += column.DesiredSize.Width;
            }
        }

        // TODO: Handle infinite width?
        var proportionalAmount = (finalSize.Width - fixedWidth - autoSized) / proportionalUnits;

        double width = 0;
        double x = 0;

        foreach (DataColumn column in elements)
        {
            if (column.DesiredWidth.IsStar)
            {
                width = proportionalAmount * column.DesiredWidth.Value;
                column.Arrange(new Rect(x, 0, width, finalSize.Height));
            }
            else if (column.DesiredWidth.IsAbsolute)
            {
                width = column.DesiredWidth.Value;
                column.Arrange(new Rect(x, 0, width, finalSize.Height));
            }
            else
            {
                // TODO: See Note in Measure step about how we need to coordinate with DataRow for true 'Auto' support

                // For now, we'll just use the header content as a guideline to see if things work.
                width = column.DesiredSize.Width;
                column.Arrange(new Rect(x, 0, width, finalSize.Height));
            }

            x += width;
        }

        return finalSize;
    }
}
