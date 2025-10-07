// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A <see cref="ColorPalette"/> based on the three most "colorful" colors.
/// </summary>
public class AccentPalette : ColorPalette
{
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Primary"/> property.
    /// </summary>
    public static readonly DependencyProperty PrimaryProperty =
        DependencyProperty.Register(nameof(Primary), typeof(Color), typeof(AccentPalette), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Secondary"/> property.
    /// </summary>
    public static readonly DependencyProperty SecondaryProperty =
        DependencyProperty.Register(nameof(Secondary), typeof(Color), typeof(AccentPalette), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Tertiary"/> property.
    /// </summary>
    public static readonly DependencyProperty TertiaryProperty =
        DependencyProperty.Register(nameof(Tertiary), typeof(Color), typeof(AccentPalette), new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// Gets the primary accent color.
    /// </summary>
    /// <remarks>
    /// The most "colorful" found in the palette.
    /// </remarks>
    public Color Primary
    {
        get => (Color)GetValue(PrimaryProperty);
        protected set => SetValue(PrimaryProperty, value);
    }

    /// <summary>
    /// Gets the secondary accent color.
    /// </summary>
    /// <remarks>
    /// The second most "colorful" color found in the palette.
    /// </remarks>
    public Color Secondary
    {
        get => (Color)GetValue(SecondaryProperty);
        protected set => SetValue(SecondaryProperty, value);
    }

    /// <summary>
    /// Gets the tertiary accent color.
    /// </summary>
    /// <remarks>
    /// The third most "colorful" color found in the palette.
    /// </remarks>
    public Color Tertiary
    {
        get => (Color)GetValue(TertiaryProperty);
        protected set => SetValue(TertiaryProperty, value);
    }

    /// <inheritdoc/>
    public override void SelectColors(IEnumerable<AccentColorInfo> colors, float imageColorfulness)
    {
        // Select accent colors
        var accentColors = colors
            .OrderByDescending(x => x.Colorfulness)
            .Select(x => x.Color);

        // Get primary/secondary/tertiary accents
        var primary = accentColors.First();
        var secondary = accentColors.ElementAtOrDefault(1);
        secondary = secondary != default ? secondary : primary;
        var tertiary = accentColors.ElementAtOrDefault(2);
        tertiary = tertiary != default ? tertiary : secondary;
        
        // Update DPs
        Primary = primary;
        Secondary = secondary;
        Tertiary = tertiary;
    }
}
