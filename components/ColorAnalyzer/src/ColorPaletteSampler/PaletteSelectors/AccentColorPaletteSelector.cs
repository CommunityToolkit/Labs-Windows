// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A <see cref="ColorPaletteSelector"/> based on the three most "colorful" colors.
/// </summary>
public class AccentColorPaletteSelector : ColorPaletteSelector
{
    /// <inheritdoc/>
    public override void SelectColors(IEnumerable<PaletteColor> palette)
    {
        // Select accent colors
        SelectedColors = palette
            .Select(x => x.Color)
            .OrderByDescending(ColorExtensions.FindColorfulness)
            .ToList()
            .EnsureMinColorCount(MinColorCount);
    }
}
