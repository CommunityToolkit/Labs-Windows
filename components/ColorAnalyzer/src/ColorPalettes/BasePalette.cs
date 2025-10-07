// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// 
/// </summary>
public class BasePalette : ColorPalette
{
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Base"/> property.
    /// </summary>
    public static readonly DependencyProperty BaseProperty =
        DependencyProperty.Register(nameof(Base), typeof(Color), typeof(AccentPalette), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the primary accent color.
    /// </summary>
    /// <remarks>
    /// The most "colorful" found in the palette.
    /// </remarks>
    public Color Base
    {
        get => (Color)GetValue(BaseProperty);
        protected set => SetValue(BaseProperty, value);
    }
    
    /// <inheritdoc/>
    public override void SelectColors(IEnumerable<AccentColorInfo> colors, float imageColorfulness)
    {
        // Get base color
        var accentColors = colors
            .OrderBy(x => x.Colorfulness)
            .Take(1)
            .Select(x => x.Color);

        Base = accentColors.First();
    }
}
