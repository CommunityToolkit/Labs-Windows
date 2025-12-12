// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A <see cref="ColorPaletteSelector"/> based on the least "colorful" color.
/// </summary>
public class BaseColorPaletteSelector : ColorPaletteSelector
{
    /// <inheritdoc/>
    public override void SelectColors(IEnumerable<PaletteColor> palettes)
    {
        // Get base color
        SelectedColors = palettes
            .Select(x => x.Color)
            .OrderBy(ColorExtensions.FindColorfulness)
            .ToList()
            .EnsureMinColorCount(MinColorCount);
    }
}
