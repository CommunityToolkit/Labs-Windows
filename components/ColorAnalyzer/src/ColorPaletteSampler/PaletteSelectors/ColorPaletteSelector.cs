// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A base class for selecting colors from a palette extracted by the <see cref="ColorPaletteSampler"/>.
/// </summary>
public abstract class ColorPaletteSelector : DependencyObject
{
    private IEnumerable<PaletteColor>? _palette;

    /// <summary>
    /// An attached property that defines the <see cref="IList{Color}"/> of colors selected from the palette.
    /// </summary>
    public static readonly DependencyProperty SelectedColorsProperty =
        DependencyProperty.Register(
            nameof(SelectedColors),
            typeof(IList<Color>),
            typeof(ColorPaletteSelector),
            new PropertyMetadata(null));

    /// <summary>
    /// An attached property that defines the minimum number of colors permitted to select from the palette.
    /// </summary>
    public static readonly DependencyProperty MinColorCountProperty =
        DependencyProperty.Register(
            nameof(MinColorCount),
            typeof(int),
            typeof(ColorPaletteSelector),
            new PropertyMetadata(1, OnMinColorCountChanged));

    /// <summary>
    /// Gets the list of colors selected from the palette.
    /// </summary>
    public IList<Color>? SelectedColors
    {
        get => (IList<Color>?)GetValue(SelectedColorsProperty);
        protected set => SetValue(SelectedColorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the minimum number of colors permitted to select from the palette.
    /// </summary>
    public int MinColorCount
    {
        get => (int)GetValue(MinColorCountProperty);
        set => SetValue(MinColorCountProperty, value);
    }

    /// <summary>
    /// Selects a set of colors from a palette to create a sub-group.
    /// </summary>
    /// <param name="palette">The color info extracted by the <see cref="ColorPaletteSampler"/>.</param>
    public virtual void SelectColors(IEnumerable<PaletteColor> palette)
    {
        _palette = palette;
    }

    private static void OnMinColorCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ColorPaletteSelector selector || selector._palette is null)
            return;

        selector.SelectColors(selector._palette);
    }
}
