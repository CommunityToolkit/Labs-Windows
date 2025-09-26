// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINAPPSDK
global using Windows.UI.Xaml.Media.Imaging;
#else
global using Microsoft.UI;
global using Microsoft.UI.Dispatching;
global using Microsoft.UI.Xaml.Media.Imaging;
using System;

#endif

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
        // This is ridiculous, but the event must be here since it's part of the ICommand interface,
        // however without supression it causes an error that prevents the CI from building.
        #pragma warning disable CS0067

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        #pragma warning restore CS0067

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
        const int sampleCount = 4096;
        const int k = 8;

        // Retreive pixel samples from source
        var samples = await SampleSourcePixelColorsAsync(sampleCount);

        // Failed to retreive pixel data. Cancel
        if (samples.Length == 0)
            return;

        // Cluster samples in RGB floating-point color space
        // With Euclidean Squared distance function
        // The accumulate accent color infos
        var clusters = KMeansCluster(samples, k, out var sizes);
        var colorData = clusters
            .Select((color, i) => new AccentColorInfo(color, (float)sizes[i] / samples.Length));

        // Update accent colors property
        // Not a dependency property, so don't update form 
#if !WINDOWS_UWP
        AccentColors = [..colorData];
#else
        AccentColors = colorData.ToList();
#endif

        // Select accent colors
        Color primary, secondary, tertiary, baseColor;
        (primary, secondary, tertiary, baseColor) = SelectAccents(colorData);

        // Get dominant color by prominence
#if NET6_0_OR_GREATER
        var dominantColor = colorData
            .MaxBy(x => x.Prominence).Color;
#else
        var dominantColor = colorData
            .OrderByDescending((x) => x.Prominence)
            .First().Color;
#endif

        // Evaluate colorfulness
        // TODO: Should this be weighted by cluster sizes?
        var overallColorfulness = FindColorfulness(clusters);
        
        // Update using the color data
        UpdateAccentProperties(primary, secondary, tertiary, baseColor, dominantColor, overallColorfulness);
    }

    private (Color primary, Color secondary, Color tertiary, Color baseColor) SelectAccents(IEnumerable<AccentColorInfo> colorData)
    {
        // Select accent colors
        var accentColors = colorData
            .OrderByDescending(x => x.Colorfulness)
            .Take(3)
            .Select(x => x.Color);

        // Get primary/secondary/tertiary accents
        var primary = accentColors.First();
        var secondary = accentColors.ElementAtOrDefault(1);
        var tertiary = accentColors.ElementAtOrDefault(2);

        // Get base color
        var baseColor = accentColors.Last();

        // Return palette
        return (primary, secondary, tertiary, baseColor);
    }

    private async Task<Vector3[]> SampleSourcePixelColorsAsync(int sampleCount)
    {
        // Ensure the source is populated
        if (Source is null)
            return [];

        // Grab actual size
        // If actualSize is 0, replace with 1:1 aspect ratio
        var actualSize = Source.ActualSize;
        actualSize = actualSize != Vector2.Zero ? actualSize : Vector2.One;
        
        // Calculate size of scaled rerender using the actual size
        // scaled down to the sample count, maintaining aspect ration
        var actualArea = actualSize.X * actualSize.Y;
        var scale = MathF.Sqrt(sampleCount / actualArea);
        var scaledSize = actualSize * scale;
        
        // Rerender the UIElement to a bitmap of about sampleCount pixels
        var bitmap = new RenderTargetBitmap();
        await bitmap.RenderAsync(Source, (int)scaledSize.X, (int)scaledSize.Y);

        // Create a stream from the bitmap
        var pixels = await bitmap.GetPixelsAsync();
        var pixelByteStream = pixels.AsStream();

        // Something went wrong
        if (pixelByteStream.Length == 0)
            return [];

        // Read the stream into a a color array
        const int bytesPerPixel = 4;
        Vector3[] samples = new Vector3[(int)pixelByteStream.Length / bytesPerPixel];

        // Iterate through the stream reading a pixel (4 bytes) at a time
        // and storing them as a Vector3. Opacity info is dropped.
        int colorIndex = 0;
#if NET7_0_OR_GREATER
        Span<byte> pixelBytes = stackalloc byte[bytesPerPixel];
        while (pixelByteStream.Read(pixelBytes) == bytesPerPixel)
#else
        byte[] pixelBytes = new byte[bytesPerPixel];
        while(pixelByteStream.Read(pixelBytes, 0, bytesPerPixel) == bytesPerPixel)
#endif
        {
            // Skip fully transparent pixels
            if (pixelBytes[3] == 0)
                continue;

            // Take the red, green, and blue channels to make a floating-point space color.
            samples[colorIndex] = new Vector3(pixelBytes[2], pixelBytes[1], pixelBytes[0]) / byte.MaxValue;
            colorIndex++;
        }

        // If we skipped any pixels, trim the span
#if !WINDOWS_UWP
        samples = samples[..colorIndex];
#else
        Array.Resize(ref samples, colorIndex);
#endif

        return samples;
    }
}
