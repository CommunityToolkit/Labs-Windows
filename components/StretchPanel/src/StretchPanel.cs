// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A panel that arranges its children in a grid-like fashion, stretching them to fill available space.
/// </summary>
public partial class StretchPanel : Panel
{
    private List<RowSpec>? _rowSpecs;

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        _rowSpecs = [];

        // Define XY/UV coordinate variables
        var uvAvailableSize = new UVCoord(availableSize.Width, availableSize.Height, Orientation);
        var uvSpacing = new UVCoord(HorizontalSpacing, VerticalSpacing, Orientation);

        double largestRow = 0;
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
            if (!currentRowSpec.TryAdd(spec, uvSpacing.U, uvAvailableSize.U))
            {
                // Could not add to current row/column
                // Start a new row/column
                _rowSpecs.Add(currentRowSpec);
                largestRow = Math.Max(largestRow, currentRowSpec.Measure(uvSpacing.U));
                currentRowSpec = spec;
            }
        }

        // Add the final row/column
        _rowSpecs.Add(currentRowSpec);
        largestRow = Math.Max(largestRow, currentRowSpec.Measure(uvSpacing.U));

        // Determine if the desired alignment is stretched.
        // Don't stretch if infinite space is available though. Attempting to divide infinite space will result in a crash.
        bool stretch = GetAlignment() is Alignment.Stretch && !double.IsInfinity(uvAvailableSize.U);

        // Calculate final desired size
        var uvSize = new UVCoord(0, 0, Orientation)
        {
            U = stretch ? uvAvailableSize.U : largestRow,
            V = _rowSpecs.Sum(static rs => rs.MaxOffAxisSize) + (uvSpacing.V * (_rowSpecs.Count - 1))
        };

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
        var uvSpacing = new UVCoord(HorizontalSpacing, VerticalSpacing, Orientation);

        // Adjust the starting position based on off-axis alignment
        var contentHeight = _rowSpecs.Sum(static rs => rs.MaxOffAxisSize) + (uvSpacing.V * (_rowSpecs.Count - 1));
        pos.V = GetStartByAlignment(GetOffAlignment(), contentHeight, uvFinalSize.V);

        var elements = Children.Where(static e => e.Visibility is Visibility.Visible);
        foreach (var row in _rowSpecs)
        {
            var spacingTotalSize = uvSpacing.U * (row.ItemsCount - 1);
            var portionSize = row.MinPortionSize;

            // Determine if the desired alignment is stretched.
            bool stretch = GetAlignment() is Alignment.Stretch && !double.IsInfinity(uvFinalSize.U);

            // Calculate portion size if stretching
            if (stretch)
            {
                portionSize = (uvFinalSize.U - row.ReservedSpace - spacingTotalSize) / row.PortionsSum;
            }

            // Reset U position
            pos.U = 0;

            // Adjust the starting position if not stretching
            // Also do this if there are no star-sized items in the row/column
            if (!stretch || row.PortionsSum is 0)
            {
                // Determine the offset based on alignment
                var rowSize = row.Measure(uvSpacing.U);
                pos.U = GetStartByAlignment(GetAlignment(), rowSize, uvFinalSize.U);
            }

            for (int i = 0; i < row.ItemsCount; i++)
            {
                // Get the next child
                var child = elements.ElementAt(0);
                elements = elements.Skip(1);

                // Sanity check
                if (child is null)
                {
                    return finalSize;
                }

                // Get layout and desired size
                var layoutLength = GetLayoutLength(child);
                var uvDesiredSize = new UVCoord(child.DesiredSize, Orientation);

                // Determine the child's U size
                double uSize = layoutLength.GridUnitType switch
                {
                    GridUnitType.Auto => uvDesiredSize.U,
                    GridUnitType.Pixel => layoutLength.Value,
                    GridUnitType.Star => layoutLength.Value * portionSize,
                    _ => uvDesiredSize.U,
                };

                // Arrange the child
                var size = new UVCoord(0, 0, Orientation)
                {
                    U = uSize,
                    V = row.MaxOffAxisSize
                };

                // NOTE: The arrange method is still in X/Y coordinate system
                child.Arrange(new Rect(pos.X, pos.Y, size.X, size.Y));

                // Advance the position
                pos.U += uSize + uvSpacing.U;
            }

            // Advance to the next row/column
            pos.V += row.MaxOffAxisSize + uvSpacing.V;
        }

        return finalSize;
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

    private enum Alignment
    {
        Start,
        Center,
        End,
        Stretch
    }

    /// <summary>
    /// A struct representing the specifications of a row or column in the panel.
    /// </summary>
    private struct RowSpec
    {
        public RowSpec(GridLength layout, UVCoord desiredSize)
        {
            switch (layout.GridUnitType)
            {
                case GridUnitType.Auto:
                    ReservedSpace = desiredSize.U;
                    break;
                case GridUnitType.Pixel:
                    ReservedSpace = layout.Value;
                    break;
                case GridUnitType.Star:
                    PortionsSum = layout.Value;
                    MinPortionSize = desiredSize.U / layout.Value;
                    break;
            }

            MaxOffAxisSize = desiredSize.V;
            ItemsCount = 1;
        }

        /// <summary>
        /// Gets the total reserved space for spacing in the row/column.
        /// </summary>
        /// <remarks>
        /// Items with a fixed size or auto size contribute to this value.
        /// </remarks>
        public double ReservedSpace { get; private set; }

        /// <summary>
        /// Gets the sum of portions in the row/column.
        /// </summary>
        /// <remarks>
        /// Items with a star-sized length contribute to this value.
        /// </remarks>
        public double PortionsSum { get; private set; }

        /// <summary>
        /// Gets the maximum width/height of items in the row/column.
        /// </summary>
        /// <remarks>
        /// Width in vertical orientation, height in horizontal orientation.
        /// </remarks>
        public double MaxOffAxisSize { get; private set; }

        /// <summary>
        /// Gets the minimum size of a portion in the row/column.
        /// </summary>
        public double MinPortionSize { get; private set; }

        /// <summary>
        /// Gets the number of items in the row/column.
        /// </summary>
        public int ItemsCount { get; private set; }

        public bool TryAdd(RowSpec addend, double spacing, double maxSize)
        {
            // Check if adding the new spec would exceed the maximum size
            var sum = this + addend;
            if (sum.Measure(spacing) > maxSize)
                return false;

            // Update the current spec to include the new spec
            this = sum;
            return true;
        }

        public readonly double Measure(double spacing)
        {
            var totalSpacing = (ItemsCount - 1) * spacing;
            var totalSize = ReservedSpace + totalSpacing;

            // Add star-sized items if applicable
            if (!double.IsNaN(MinPortionSize) && !double.IsInfinity(MinPortionSize))
                totalSize += MinPortionSize * PortionsSum;

            return totalSize;
        }

        public static RowSpec operator +(RowSpec a, RowSpec b)
        {
            var combined = new RowSpec
            {
                ReservedSpace = a.ReservedSpace + b.ReservedSpace,
                PortionsSum = a.PortionsSum + b.PortionsSum,
                MinPortionSize = Math.Max(a.MinPortionSize, b.MinPortionSize),
                MaxOffAxisSize = Math.Max(a.MaxOffAxisSize, b.MaxOffAxisSize),
                ItemsCount = a.ItemsCount + b.ItemsCount
            };
            return combined;
        }
    }

    /// <summary>
    /// A struct for mapping X/Y coordinates to an orientation adjusted U/V coordinate system.
    /// </summary>
    private struct UVCoord(double x, double y, Orientation orientation)
    {
        private readonly bool _horizontal = orientation is Orientation.Horizontal;

        public UVCoord(Size size, Orientation orientation) : this(size.Width, size.Height, orientation)
        {
        }

        public double X { get; set; } = x;

        public double Y { get; set; } = y;

        public double U
        {
            readonly get => _horizontal ? X : Y;
            set 
            {
                if (_horizontal)
                {
                    X = value;
                }
                else
                {
                    Y = value;
                }
            }
        }

        public double V
        {
            readonly get => _horizontal ? Y : X;
            set
            {
                if (_horizontal)
                {
                    Y = value;
                }
                else
                {
                    X = value;
                }
            }
        }

        public readonly Size Size => new(X, Y);
    }
}
