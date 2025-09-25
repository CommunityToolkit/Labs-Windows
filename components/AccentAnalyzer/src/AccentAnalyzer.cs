// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Numerics;
using System.Windows.Input;
using Windows.UI;

namespace CommunityToolkit.WinUI.Extensions;

/// <summary>
/// A resource that can be used to extract color palettes out of any UIElement.
/// </summary>
public partial class AccentAnalyzer : DependencyObject
{
    private partial class Command(Action action) : ICommand
    {
        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => true;
        
        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            action();
        }
    }

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
    /// Initialize an instance of the <see cref="AccentAnalyzer"/> class.
    /// </summary>
    public AccentAnalyzer()
    {
        AccentUpdateCommand = new Command(UpdateAccent);
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
    /// Gets the dominent color as extracted from the <see cref="Source"/>.
    /// </summary>
    /// <remarks>
    /// The most color that takes up the most of the image.
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
        set => SetValue(ColorfulnessProperty, value);
    }

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

    /// <summary>
    /// Update the accent
    /// </summary>
    public void UpdateAccent()
    {
        _ = UpdateAccentAsync();
    }

    private async Task UpdateAccentAsync()
    {
        // Rerender the UIElement to a 64x64 bitmap
        RenderTargetBitmap bitmap = new RenderTargetBitmap();
        await bitmap.RenderAsync(Source, 64, 64);

        // Create a stream from the bitmap
        var pixels = await bitmap.GetPixelsAsync();
        var stream = pixels.AsStream();

        if (stream.Length == 0)
            return;

        // Read the stream into a a color array
        int pos = 0;
        Span<Vector3> colors = new Vector3[(int)stream.Length / 4]; // This should be 4096 (64x64), but it's good to be safe.
        Span<byte> bytes = stackalloc byte[4];
        while (stream.Read(bytes) > 0)
        {
            // Safety check to avoid going out of bounds
            // This should never happen, but it's good to be safe.
            if (pos >= colors.Length)
                break;

            // Skip fully transparent pixels
            if (bytes[3] == 0)
                continue;

            colors[pos] = new Vector3(bytes[2], bytes[1], bytes[0]) / 255;
            pos++;
        }
        
        // If we skipped any pixels, trim the span
        colors = colors[..pos];

        // Determine most prominent colors and assess colorfulness
        int k = 6; // Should this be adjustable?
        var clusters = KMeansCluster(colors, k, out var sizes);
        var colorfulness = clusters.Select(color => (color, FindColorfulness(color)));

        // Select the accent color and convert to color
        var accentColors = colorfulness
            .OrderByDescending(x => x.Item2)
            .Select(x => x.color * 255)
            .Select(x => Color.FromArgb(255, (byte)x.X, (byte)x.Y, (byte)x.Z));

        // Get primary/secondary/tertiary accents
        var primary = accentColors.First();
        var secondary = accentColors.ElementAtOrDefault(1);
        var tertiary = accentColors.ElementAtOrDefault(2);
        var baseColor = accentColors.Last();

        // Get base color by prominence
        var dominant = clusters
            .Select((color, i) => (color, sizes[i]))
            .MaxBy(x => x.Item2).color * 255;
        var dominantColor = Color.FromArgb(255, (byte)dominant.X, (byte)dominant.Y, (byte)dominant.Z);

        // Evaluate colorfulness
        // TODO: Should this be weighted by cluster sizes?
        var overallColorfulness = FindColorfulness(clusters);

        // Set the various properties from the UI thread
        UpdateAccentProperties(primary, secondary, tertiary, baseColor, dominantColor, overallColorfulness);
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
        });
    }

    private void SetSource(UIElement? source)
    {
        _source = source;

        // If true, calculate the accent color immediately.
        _ = UpdateAccentAsync();
    }
}
