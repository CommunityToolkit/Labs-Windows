// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Input;
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
    /// Gets or sets the <see cref="UIElement"/> source for accent color analysis.
    /// </summary>
    public UIElement? Source
    {
        get => (UIElement)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets the primary accent color as extracted from the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// The most "colorful" found in the image.
    /// </remarks>
    public Color PrimaryAccentColor
    {
        get => (Color)GetValue(PrimaryAccentColorProperty);
        protected set => SetValue(PrimaryAccentColorProperty, value);
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
        protected set => SetValue(SecondaryAccentColorProperty, value);
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
        protected set => SetValue(TertiaryAccentColorProperty, value);
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
        protected set => SetValue(BaseColorProperty, value);
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
        protected set => SetValue(DominantColorProperty, value);
    }

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

    /// <summary>
    /// Gets a command that executes an accent update.
    /// </summary>
    public ICommand AccentUpdateCommand { get; }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AccentAnalyzer analyzer)
            return;

        _ = analyzer.UpdateAccentAsync();
    }
}
