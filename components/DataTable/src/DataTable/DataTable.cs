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
    // TODO: Check with Sergio if there's a better structure here, as I don't need a Dictionary like ConditionalWeakTable
    internal HashSet<DataRow> Rows { get; private set; } = new();

    internal void ColumnResized()
    {
        InvalidateMeasure();

        foreach (var row in Rows)
        {
            row.InvalidateArrange();
        }
    }

    //// TODO: Would we want this named 'Spacing' instead if we support an Orientation in the future for columns being items instead of rows?
    /// <summary>
    /// Gets or sets the amount of space to place between columns within the table.
    /// </summary>
    public double ColumnSpacing
    {
        get { return (double)GetValue(ColumnSpacingProperty); }
        set { SetValue(ColumnSpacingProperty, value); }
    }

    /// <summary>
    /// Gets the <see cref="ColumnSpacing"/> <see cref="DependencyProperty"/>.
    /// </summary>
    public static readonly DependencyProperty ColumnSpacingProperty =
        DependencyProperty.Register(
            nameof(ColumnSpacing), typeof(double), typeof(DataTable),
            new PropertyMetadata(0d));

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        //Debug.WriteLine($"DataTable.MeasureOverride");

        int starRemains = 0;
        double starAmounts = 0;

        double columnSpacing = ColumnSpacing;
        double totalWidth = double.NaN;
        double maxHeight = 0;

        bool invokeRowsMeasures = false;

        for (int i = 0; i < Children.Count; i++)
        {
            // We only need to measure children that are visible
            var column = Children[i] as DataColumn;
            if (column?.Visibility != Visibility.Visible)
                continue;

            if (double.IsNaN(totalWidth))
                totalWidth = 0;
            else
                totalWidth += columnSpacing;

            double width = column.ActualCurrentWidth;

            if (column.IsStarProportion)
            {
                ++starRemains;
                starAmounts += column.DesiredWidth.Value;
                continue;
            }
            else if (column.IsStar)
            {
                starAmounts += column.DesiredWidth.Value;
                //Debug.WriteLine($"  Column[{i}] ({column.DesiredWidth}) width is fixed to: {width}");

                // If availableSize.Width is infinite, the column will also get infinite available width.
                column.Measure(new Size(width, availableSize.Height));
            }
            else if (column.IsAbsolute)
            {
                // column.CurrentWidth is already set in DesiredWidth_PropertyChanged.
                //Debug.WriteLine($"  Column[{i}] ({column.DesiredWidth}) width is fixed to: {width}");

                column.Measure(new Size(width, availableSize.Height));
            }
            else // (column.IsAuto)
            {
                if (column.IsAutoFit)
                {
                    // Calculate the best width of the header content.
                    column.Measure(new Size(double.PositiveInfinity, availableSize.Height));

                    width = column.DesiredSize.Width;
                    foreach (var row in Rows)
                    {
                        if (i < row.Children.Count)
                        {
                            var child = row.Children[i];

                            var childWidth = child.DesiredSize.Width;
                            if (i == 0)
                                childWidth += row.TreePadding;

                            width = Math.Max(width, childWidth);
                        }
                    }
                    //Debug.WriteLine($"  Column[{i}] ({column.DesiredWidth}) width is adjusted to: {width}");

                    // The column width of the corresponding cell in each row
                    // is taken into account in the next layout pass.
                    invokeRowsMeasures = true;

                    // Store the calculated column width as a negative value.
                    column.CurrentWidth = -width;
                }
                else
                {
                    //Debug.WriteLine($"  Column[{i}] ({column.DesiredWidth}) width is fixed to: {width}");

                    column.Measure(new Size(width, availableSize.Height));
                }
            }

            totalWidth += width;
            maxHeight = Math.Max(maxHeight, column.DesiredSize.Height);
        }

        if (double.IsNaN(totalWidth))
            return new Size(0, 0);

        double starUnit = Math.Max(0, availableSize.Width - totalWidth) / starAmounts;
        for (int i = 0; starRemains != 0; i++)
        {
            var column = Children[i] as DataColumn;
            if (column?.Visibility != Visibility.Visible)
                continue;

            if (column.IsStarProportion)
            {
                --starRemains;

                // If the column width needs to be calculated, get the proportion of the remained space.
                double width = starUnit * column.DesiredWidth.Value;
                //Debug.WriteLine($"  Column[{i}] ({column.DesiredWidth}) width is adjusted to: {width}");

                // If availableSize.Width is infinite, the column will also get infinite available width.
                column.Measure(new Size(width, availableSize.Height));

                // Store the calculated column width as a negative value.
                column.CurrentWidth = -width;

                totalWidth += width;
                maxHeight = Math.Max(maxHeight, column.DesiredSize.Height);
            }
        }

        if (invokeRowsMeasures)
        {
            foreach (var row in Rows)
                row.InvalidateMeasure();
        }

        return new Size(totalWidth, maxHeight);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        //Debug.WriteLine($"DataTable.ArrangeOverride");

        double columnSpacing = ColumnSpacing;
        double x = double.NaN;

        for (int i = 0; i < Children.Count; i++)
        {
            // We only need to measure children that are visible
            var column = Children[i] as DataColumn;
            if (column?.Visibility != Visibility.Visible)
                continue;

            if (double.IsNaN(x))
                x = 0;
            else
                x += columnSpacing;

            double width = column.ActualCurrentWidth;

            column.Arrange(new Rect(x, 0, width, finalSize.Height));

            x += width;
        }

        return finalSize;
    }
}
