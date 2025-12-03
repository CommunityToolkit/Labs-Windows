// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class WrapPanel2
{
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
