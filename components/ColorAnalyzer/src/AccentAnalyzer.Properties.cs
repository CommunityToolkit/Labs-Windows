// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

public partial class AccentAnalyzer
{
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Source"/> property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(UIElement), typeof(AccentAnalyzer), new PropertyMetadata(null, OnSourceChanged));

    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Colorfulness"/> property.
    /// </summary>
    public static readonly DependencyProperty ColorfulnessProperty =
        DependencyProperty.Register(nameof(Colorfulness), typeof(float), typeof(AccentAnalyzer), new PropertyMetadata(0f));

    /// <summary>
    /// An event fired when the accent properties are updated.
    /// </summary>
    public event EventHandler? PalettesUpdated;

    /// <summary>
    /// Gets or sets the <see cref="UIElement"/> source for accent color analysis.
    /// </summary>
    public UIElement? Source
    {
        get => (UIElement)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// The list of <see cref="ColorPalette"/> to update when the <see cref="Source"/> is set or changed.
    /// </summary>
    public IList<ColorPalette> Palettes { get; set; }

    /// <summary>
    /// Gets the "colorfulness" of the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// Colorfulness is defined by David Hasler and Sabine Susstrunk's paper on measuring colorfulness
    /// <seealso href="https://infoscience.epfl.ch/server/api/core/bitstreams/77f5adab-e825-4995-92db-c9ff4cd8bf5a/content"/>.
    ///
    /// An image with colors of high saturation and value will have a high colorfulness (around 1),
    /// meanwhile images that are mostly gray or white will have a low colorfulness (around 0).
    /// </remarks>
    public float Colorfulness
    {
        get => (float)GetValue(ColorfulnessProperty);
        private set => SetValue(ColorfulnessProperty, value);
    }

    /// <summary>
    /// Gets the set of <see cref="AccentColorInfo"/> extracted on last update.
    /// </summary>
    public IReadOnlyList<AccentColorInfo>? AccentColors { get; private set; }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AccentAnalyzer analyzer)
            return;

        _ = analyzer.UpdateAccentAsync();
    }
}
