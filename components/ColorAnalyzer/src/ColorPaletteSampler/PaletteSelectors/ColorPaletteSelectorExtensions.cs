// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// Extension methods for <see cref="ColorPaletteSelector"/>.
/// </summary>
public static class ColorPaletteSelectorExtensions
{
    /// <summary>
    /// Extends the list of colors to ensure it meets the minimum count by repeating the <paramref name="index"/>th color.
    /// </summary>
    /// <param name="colors">The list of colors to extend</param>
    /// <param name="minCount">The minimum number of colors required</param>
    /// <param name="index">The index of the item to repeat</param>
    public static IList<Color> EnsureMinColorCount(this IList<Color> colors, int minCount, int index = 0)
    {
        // If we already have enough colors, do nothing.
        if (colors.Count >= minCount)
            return colors;

        var nthColor = colors[index];
        while (colors.Count < minCount)
        {
            colors.Add(nthColor);
        }

        return colors;
    }
}
