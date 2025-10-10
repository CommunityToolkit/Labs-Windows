// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A <see cref="ColorPaletteSelector"/> based on the three most prominent colors.
/// </summary>
public class ColorWeightPaletteSelector : ColorPaletteSelector
{
    /// <inheritdoc/>
    public override void SelectColors(IEnumerable<PaletteColor> colors)
    {
        // Order by weight and ensure we have at least MinColorCount colors
        SelectedColors = colors
            .OrderByDescending(x => x.Weight)
            .Select(x => x.Color)
            .ToList()
            .EnsureMinColorCount(MinColorCount);
    }
}
