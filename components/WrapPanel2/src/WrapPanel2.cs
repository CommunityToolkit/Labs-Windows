// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A panel that arranges its children in a grid-like fashion, stretching them to fill available space.
/// </summary>
public partial class WrapPanel2 : Panel
{
    private List<RowSpec>? _rowSpecs;
    private double _longestRowSize = 0;

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        _rowSpecs = [];
        _longestRowSize = 0;

        // Define XY/UV coordinate variables
        var uvAvailableSize = new UVCoord(availableSize.Width, availableSize.Height, Orientation);

        RowSpec currentRowSpec = default;

        var elements = Children.Where(static e => e.Visibility is Visibility.Visible);

        // Do nothing if the panel is empty
        if (!elements.Any())
        {
            return new Size(0, 0);
        }

        foreach (var child in elements)
        {
            // Measure the child's desired size and get layout
            child.Measure(availableSize);
            var uvDesiredSize = new UVCoord(child.DesiredSize, Orientation);
            var layoutLength = GetLayoutLength(child);

            // Attempt to add the child to the current row/column
            var spec = new RowSpec(layoutLength, uvDesiredSize);
            if (!currentRowSpec.TryAdd(spec, ItemSpacing, uvAvailableSize.U))
            {
                // If the overflow behavior is drop, just end the row here.
                if (OverflowBehavior is OverflowBehavior.Drop)
                    break;

                // Could not add to current row/column
                // Start a new row/column
                _rowSpecs.Add(currentRowSpec);
                _longestRowSize = Math.Max(_longestRowSize, currentRowSpec.Measure(ItemSpacing));
                currentRowSpec = spec;
            }
        }

        // Add the final row/column
        _rowSpecs.Add(currentRowSpec);
        _longestRowSize = Math.Max(_longestRowSize, currentRowSpec.Measure(ItemSpacing));

        // Calculate final desired size
        var uvSize = new UVCoord(0, 0, Orientation)
        {
            U = IsMainAxisStretch(uvAvailableSize.U) ? uvAvailableSize.U : _longestRowSize,
            V = _rowSpecs.Sum(static rs => rs.MaxOffAxisSize) + (LineSpacing * (_rowSpecs.Count - 1))
        };

        // Clamp to available size and return
        uvSize.U = Math.Min(uvSize.U, uvAvailableSize.U);
        uvSize.V = Math.Min(uvSize.V, uvAvailableSize.V);
        return uvSize.Size;
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(Size finalSize)
    {
        // Do nothing if there are no rows/columns
        if (_rowSpecs is null || _rowSpecs.Count is 0)
            return new Size(0, 0);

        // Create XY/UV coordinate variables
        var pos = new UVCoord(0, 0, Orientation);
        var uvFinalSize = new UVCoord(finalSize, Orientation);

        // Adjust the starting position based on off-axis alignment
        var contentHeight = _rowSpecs.Sum(static rs => rs.MaxOffAxisSize) + (LineSpacing * (_rowSpecs.Count - 1));
        pos.V = GetStartByAlignment(GetOffAlignment(), contentHeight, uvFinalSize.V);

        var childQueue = new Queue<UIElement>(Children.Where(static e => e.Visibility is Visibility.Visible));

        foreach (var row in _rowSpecs)
        {
            // Arrange the row/column
            ArrangeRow(ref pos, row, uvFinalSize, childQueue);
        }

        // "Arrange" remaning children by rendering them with zero size
        while (childQueue.TryDequeue(out var child))
        {
            // Arrange with zero size
            child.Arrange(new Rect(0, 0, 0, 0));
        }

        return finalSize;
    }

    private void ArrangeRow(ref UVCoord pos, RowSpec row, UVCoord uvFinalSize, Queue<UIElement> childQueue)
    {
        var spacingTotalSize = ItemSpacing * (row.ItemsCount - 1);
        var remainingSpace = uvFinalSize.U - row.ReservedSpace - spacingTotalSize;
        var portionSize = row.MinPortionSize;

        // Determine if the desired alignment is stretched.
        // Or if fixed row lengths are in use.
        bool stretch = IsMainAxisStretch(uvFinalSize.U) || FixedRowLengths;

        // Calculate portion size if stretching
        // Same logic applies for matching row lengths, since the size was determined during measure
        if (stretch)
        {
            portionSize = remainingSpace / row.PortionsSum;
        }

        // Reset the starting U position
        pos.U = 0;

        // Adjust the starting position if not stretching
        // Also do this if there are no star-sized items in the row/column and no forced streching is in use.
        if (!stretch || (row.PortionsSum is 0 && ForcedStretchMethod is ForcedStretchMethod.None))
        {
            var rowSize = row.Measure(ItemSpacing);
            pos.U = GetStartByAlignment(GetAlignment(), rowSize, uvFinalSize.U);
        }

        // Set a flag for if the row is being forced to stretch
        bool forceStretch = row.PortionsSum is 0 && ForcedStretchMethod is not ForcedStretchMethod.None;

        // Setup portionSize for forced stretching
        if (forceStretch)
        {
            portionSize = ForcedStretchMethod switch
            {
                // The first child's size will be overridden to 1*
                // Change portion size to fill remaining space plus its original size
                ForcedStretchMethod.First =>
                    remainingSpace + GetChildSize(childQueue.Peek()),

                // The last child's size will be overridden to 1*
                // Change portion size to fill remaining space plus its original size
                ForcedStretchMethod.Last =>
                    remainingSpace + GetChildSize(childQueue.ElementAt(row.ItemsCount - 1)),

                // All children's sizes will be overridden to 1*
                // Change portion size to evenly distribute remaining space
                ForcedStretchMethod.Equal =>
                    (uvFinalSize.U - spacingTotalSize) / row.ItemsCount,

                // All children's sizes will be overridden to star sizes proportional to their original size
                // Change portion size to distribute remaining space proportionally
                ForcedStretchMethod.Proportional =>
                    (uvFinalSize.U - spacingTotalSize) / row.ReservedSpace,

                // Default case (should not be hit)
                _ => row.MinPortionSize,
            };
        }

        // Arrange each child in the row/column
        for (int i = 0; i < row.ItemsCount; i++)
        {
            // Get the next child
            var child = childQueue.Dequeue();

            // Sanity check
            if (child is null)
                return;

            // Determine the child's size
            var size = GetChildSize(child, i, row, portionSize, forceStretch);

            // NOTE: The arrange method is still in X/Y coordinate system
            child.Arrange(new Rect(pos.X, pos.Y, size.X, size.Y));

            // Advance the position
            pos.U += size.U + ItemSpacing;
        }

        // Advance to the next row/column
        pos.V += row.MaxOffAxisSize + LineSpacing;
    }

    private UVCoord GetChildSize(UIElement child, int indexInRow, RowSpec row, double portionSize, bool forceStretch)
    {
        // Get layout and desired size
        var layoutLength = GetLayoutLength(child);
        var uvDesiredSize = new UVCoord(child.DesiredSize, Orientation);

        // Override the layout based on the forced stretch method if necessary
        if (forceStretch)
        {
            var oneStar = new GridLength(1, GridUnitType.Star);
            layoutLength = ForcedStretchMethod switch
            {
                // Override the first item's layout to 1*
                ForcedStretchMethod.First when indexInRow is 0 => oneStar,

                // Override the last item's layout to 1*
                ForcedStretchMethod.Last when indexInRow == (row.ItemsCount - 1) => oneStar,

                // Override all item's layouts to 1*
                ForcedStretchMethod.Equal => oneStar,

                // Override all item's layouts to star sizes proportional to their original size
                ForcedStretchMethod.Proportional => layoutLength.GridUnitType switch
                {
                    GridUnitType.Auto => new GridLength(uvDesiredSize.U, GridUnitType.Star),
                    GridUnitType.Pixel or _ => new GridLength(layoutLength.Value, GridUnitType.Star),
                },

                // If the above conditions aren't met, do nothing
                _ => layoutLength,
            };
        }

        // Determine the child's U size
        double uSize = layoutLength.GridUnitType switch
        {
            GridUnitType.Auto => uvDesiredSize.U,
            GridUnitType.Pixel => layoutLength.Value,
            GridUnitType.Star => layoutLength.Value * portionSize,
            _ => uvDesiredSize.U,
        };

        // Return the final size
        return new UVCoord(0, 0, Orientation)
        {
            U = uSize,
            V = row.MaxOffAxisSize
        };
    }

    private static double GetStartByAlignment(Alignment alignment, double size, double availableSize)
    {
        return alignment switch
        {
            Alignment.Start => 0,
            Alignment.Center => (availableSize / 2) - (size / 2),
            Alignment.End => availableSize - size,
            _ => 0,
        };
    }

    private Alignment GetAlignment()
    {
        return Orientation switch
        {
            Orientation.Horizontal => HorizontalAlignment switch
            {
                HorizontalAlignment.Left => Alignment.Start,
                HorizontalAlignment.Center => Alignment.Center,
                HorizontalAlignment.Right => Alignment.End,
                HorizontalAlignment.Stretch => Alignment.Stretch,
                _ => Alignment.Start,
            },
            Orientation.Vertical => VerticalAlignment switch
            {
                VerticalAlignment.Top => Alignment.Start,
                VerticalAlignment.Center => Alignment.Center,
                VerticalAlignment.Bottom => Alignment.End,
                VerticalAlignment.Stretch => Alignment.Stretch,
                _ => Alignment.Start,
            },
            _ => Alignment.Start,
        };
    }

    private Alignment GetOffAlignment()
    {
        return Orientation switch
        {
            Orientation.Horizontal => VerticalAlignment switch
            {
                VerticalAlignment.Top => Alignment.Start,
                VerticalAlignment.Center => Alignment.Center,
                VerticalAlignment.Bottom => Alignment.End,
                VerticalAlignment.Stretch => Alignment.Stretch,
                _ => Alignment.Start,
            },
            Orientation.Vertical => HorizontalAlignment switch
            {
                HorizontalAlignment.Left => Alignment.Start,
                HorizontalAlignment.Center => Alignment.Center,
                HorizontalAlignment.Right => Alignment.End,
                HorizontalAlignment.Stretch => Alignment.Stretch,
                _ => Alignment.Start,
            },
            _ => Alignment.Start,
        };
    }

    /// <summary>
    /// Determine if the desired alignment is stretched.
    /// Don't stretch if infinite space is available though. Attempting to divide infinite space will result in a crash.
    /// </summary>
    private bool IsMainAxisStretch(double availableSize) => GetAlignment() is Alignment.Stretch && !double.IsInfinity(availableSize);

    private double GetChildSize(UIElement child)
    {
        var childLayout = GetLayoutLength(child);

        return childLayout.GridUnitType switch
        {
            GridUnitType.Auto => new UVCoord(child.DesiredSize, Orientation).U,
            GridUnitType.Pixel => childLayout.Value,
            _ => 0,
        };
    }

    private enum Alignment
    {
        Start,
        Center,
        End,
        Stretch
    }
}
