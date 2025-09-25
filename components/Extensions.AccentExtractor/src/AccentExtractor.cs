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
/// 
/// </summary>
public partial class AccentExtractor : DependencyObject
{
    private partial class Command : ICommand
    {
        private Action _action;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        public Command(Action action)
        {
            _action = action;
        }

        /// <inheritdoc/>
        public bool CanExecute(object? parameter) => true;
        
        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            _action();
        }
    }

    private UIElement? _source;

    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="PrimaryAccentColor"/> property.
    /// </summary>
    public static readonly DependencyProperty PrimaryAccentColorProperty =
        DependencyProperty.Register(nameof(PrimaryAccentColor), typeof(Color), typeof(AccentExtractor), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="SecondaryAccentColor"/> property.
    /// </summary>
    public static readonly DependencyProperty SecondaryAccentColorProperty =
        DependencyProperty.Register(nameof(SecondaryAccentColor), typeof(Color), typeof(AccentExtractor), new PropertyMetadata(Colors.Transparent));
    
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="TertiaryAccentColor"/> property.
    /// </summary>
    public static readonly DependencyProperty TertiaryAccentColorProperty =
        DependencyProperty.Register(nameof(TertiaryAccentColor), typeof(Color), typeof(AccentExtractor), new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// Initialize an instance of the <see cref="AccentExtractor"/> class.
    /// </summary>
    public AccentExtractor()
    {
        AccentUpdateCommand = new Command(UpdateAccent);
    }

    /// <summary>
    /// Gets or sets the primary accent color as extracted from the <see cref="Source"/>.
    /// </summary>
    public Color PrimaryAccentColor
    {
        get => (Color)GetValue(PrimaryAccentColorProperty);
        set => SetValue(PrimaryAccentColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the secondary accent color as extracted from the <see cref="Source"/>.
    /// </summary>
    public Color SecondaryAccentColor
    {
        get => (Color)GetValue(SecondaryAccentColorProperty);
        set => SetValue(SecondaryAccentColorProperty, value);
    }

    /// <summary>
    /// Gets or sets the tertiary accent color as extracted from the <see cref="Source"/>.
    /// </summary>
    public Color TertiaryAccentColor
    {
        get => (Color)GetValue(TertiaryAccentColorProperty);
        set => SetValue(TertiaryAccentColorProperty, value);
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
        // Colorfulness is defined by David Hasler and Sabine Susstrunk's paper on measuring colorfulness
        // https://infoscience.epfl.ch/server/api/core/bitstreams/77f5adab-e825-4995-92db-c9ff4cd8bf5a/content
        var clusters = KMeansCluster(colors, 5);
        var colorfulness = clusters.Select(color => (color, FindColorfulness(color)));

        // Select the accent color and convert to color
        var accentColors = colorfulness
            .OrderByDescending(x => x.Item2)
            .Select(x => x.color * 255)
            .Select(x => Color.FromArgb(255, (byte)x.X, (byte)x.Y, (byte)x.Z));

        // Ensure tertiary population
        var primary = accentColors.First() ;
        var secondary = accentColors.ElementAtOrDefault(1);
        var tertiary = accentColors.ElementAtOrDefault(2);

        // Set the accent color on the UI thread
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() => UpdateAccentColors(primary, secondary, tertiary));
    }

    private void UpdateAccentColors(Color primary, Color secondary, Color tertiary)
    {
        PrimaryAccentColor = primary;
        SecondaryAccentColor = secondary;
        TertiaryAccentColor = tertiary;
    }

    private void SetSource(UIElement? source)
    {
        _source = source;

        // If true, calculate the accent color immediately.
        _ = UpdateAccentAsync();
    }
}
