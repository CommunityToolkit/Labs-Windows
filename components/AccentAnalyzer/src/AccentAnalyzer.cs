// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Numerics;
using System.Windows.Input;
using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

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

    /// <summary>
    /// Initialize an instance of the <see cref="AccentAnalyzer"/> class.
    /// </summary>
    public AccentAnalyzer()
    {
        AccentUpdateCommand = new Command(UpdateAccent);
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
}
