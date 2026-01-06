// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A struct containing palettized color info.
/// </summary>
public readonly struct PaletteColor
{ 
    internal PaletteColor(Color color, float sampleFraction)
    {
        Color = color;
        Weight = sampleFraction;
    }

    /// <summary>
    /// Gets the color of the <see cref="PaletteColor"/>.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Gets the fraction of the image the color covers.
    /// </summary>
    /// <remarks>
    /// Multiply by 100 to get the percentage of the image the color represents.
    /// </remarks>
    public float Weight { get; }
}
