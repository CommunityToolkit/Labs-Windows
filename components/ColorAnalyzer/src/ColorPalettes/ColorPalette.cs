// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// An interface for 
/// </summary>
public abstract class ColorPalette : DependencyObject
{
    /// <summary>
    /// Selects the colors for the palette.
    /// </summary>
    /// <param name="colors">The color info extracted by the <see cref="AccentAnalyzer"/>.</param>
    /// <param name="imageColorfulness">The overall colorfulness of the palette.</param>
    public abstract void SelectColors(IEnumerable<AccentColorInfo> colors, float imageColorfulness);
}
