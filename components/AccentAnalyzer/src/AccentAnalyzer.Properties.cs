// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
using Windows.UI;

#if !WINAPPSDK
using Windows.System;
#endif


namespace CommunityToolkit.WinUI.Helpers;

public partial class AccentAnalyzer
{
    private UIElement? _source;

    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="PrimaryAccentColor"/> property.
    /// </summary>
    public static readonly DependencyProperty PrimaryAccentColorProperty =
        DependencyProperty.Register(nameof(PrimaryAccentColor), typeof(Color), typeof(AccentAnalyzer), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="SecondaryAccentColor"/> property.
    /// </summary>
    public static readonly DependencyProperty SecondaryAccentColorProperty =
        DependencyProperty.Register(nameof(SecondaryAccentColor), typeof(Color), typeof(AccentAnalyzer), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="TertiaryAccentColor"/> property.
    /// </summary>
    public static readonly DependencyProperty TertiaryAccentColorProperty =
        DependencyProperty.Register(nameof(TertiaryAccentColor), typeof(Color), typeof(AccentAnalyzer), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="BaseColor"/> property.
    /// </summary>
    public static readonly DependencyProperty BaseColorProperty =
        DependencyProperty.Register(nameof(BaseColor), typeof(Color), typeof(AccentAnalyzer), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="DominantColor"/> property.
    /// </summary>
    public static readonly DependencyProperty DominantColorProperty =
        DependencyProperty.Register(nameof(DominantColor), typeof(Color), typeof(AccentAnalyzer), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Colorfulness"/> property.
    /// </summary>
    public static readonly DependencyProperty ColorfulnessProperty =
        DependencyProperty.Register(nameof(Colorfulness), typeof(float), typeof(AccentAnalyzer), new PropertyMetadata(0f));

    /// <summary>
    /// An event fired when the accent properties are updated.
    /// </summary>
    public event EventHandler? AccentsUpdated;

    /// <summary>
    /// Gets the primary accent color as extracted from the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// The most "colorful" found in the image.
    /// </remarks>
    public Color PrimaryAccentColor
    {
        get => (Color)GetValue(PrimaryAccentColorProperty);
        private set => SetValue(PrimaryAccentColorProperty, value);
    }

    /// <summary>
    /// Gets the secondary accent color as extracted from the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// The second most "colorful" color found in the image.
    /// </remarks>
    public Color SecondaryAccentColor
    {
        get => (Color)GetValue(SecondaryAccentColorProperty);
        private set => SetValue(SecondaryAccentColorProperty, value);
    }

    /// <summary>
    /// Gets the tertiary accent color as extracted from the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// The third most "colorful" color found in the image.
    /// </remarks>
    public Color TertiaryAccentColor
    {
        get => (Color)GetValue(TertiaryAccentColorProperty);
        private set => SetValue(TertiaryAccentColorProperty, value);
    }
    
    /// <summary>
    /// Gets the base color as extracted from the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// The least "colorful" color found in the image.
    /// </remarks>
    public Color BaseColor
    {
        get => (Color)GetValue(BaseColorProperty);
        private set => SetValue(BaseColorProperty, value);
    }
    
    /// <summary>
    /// Gets the dominant color as extracted from the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// The color that takes up the most of the image.
    /// </remarks>
    public Color DominantColor
    {
        get => (Color)GetValue(DominantColorProperty);
        private set => SetValue(DominantColorProperty, value);
    }
    
    /// <summary>
    /// Gets the "colorfulness" of the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// Colorfulness is defined by David Hasler and Sabine Susstrunk's paper on measuring colorfulness
    /// <seealso href="https://infoscience.epfl.ch/server/api/core/bitstreams/77f5adab-e825-4995-92db-c9ff4cd8bf5a/content"/>
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

    /// <summary>
    /// Gets a command that executes an accent update.
    /// </summary>
    public ICommand AccentUpdateCommand { get; }

    /// <summary>
    /// Gets or sets the <see cref="UIElement"/>
    /// </summary>
    public UIElement? Source
    {
        get => _source;
        set => SetSource(value);
    }

    private void SetSource(UIElement? source)
    {
        _source = source;
        _ = UpdateAccentAsync();
    }
    
    private void UpdateAccentProperties(Color primary, Color secondary, Color tertiary, Color baseColor, Color dominantColor, float colorfulness)
    {
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            PrimaryAccentColor = primary;
            SecondaryAccentColor = secondary;
            TertiaryAccentColor = tertiary;
            DominantColor = dominantColor;
            BaseColor = baseColor;
            Colorfulness = colorfulness;

            AccentsUpdated?.Invoke(this, EventArgs.Empty);
        });
    }
}
