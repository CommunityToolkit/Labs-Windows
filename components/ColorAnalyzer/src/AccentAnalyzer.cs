// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINAPPSDK
global using Windows.UI.Xaml.Media.Imaging;
global using Windows.System;
#else
global using Microsoft.UI;
global using Microsoft.UI.Dispatching;
global using Microsoft.UI.Xaml.Media.Imaging;
#endif

using System.Numerics;
using System.Windows.Input;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A resource that can be used to extract color palettes out of any <see cref="UIElement"/>.
/// </summary>
public partial class AccentAnalyzer : DependencyObject
{
    /// <summary>
    /// Initialize an instance of the <see cref="AccentAnalyzer"/> class.
    /// </summary>
    public AccentAnalyzer()
    {
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
        
        // Evaluate colorfulness
        // TODO: Should this be weighted by cluster sizes?
        var overallColorfulness = FindColorfulness(clusters);

        // Select accent colors
        SelectAccentColors(colorData, overallColorfulness);

        // Update accent colors property
        // Not a dependency property, so no need to update from the UI Thread
#if !WINDOWS_UWP
        AccentColors = [..colorData];
#else
        AccentColors = colorData.ToList();
#endif

        // Update the colorfulness and invoke accents updated event,
        // both from the UI thread
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            Colorfulness = overallColorfulness;
            AccentsUpdated?.Invoke(this, EventArgs.Empty);
        });
    }

    /// <summary>
    /// This method takes the processed color information and selects the accent colors from it.
    /// </summary>
    /// <remarks>
    /// There is no guarentee that this method will be called from the UI Thread.
    /// Dependency properties should be updated using a dispatcher.
    /// </remarks>
    /// <param name="colorData">The analyzed accent color info from the image.</param>
    /// <param name="imageColorfulness">The overall colorfulness of the image.</param>
    protected virtual void SelectAccentColors(IEnumerable<AccentColorInfo> colorData, float imageColorfulness)
    {
        // Select accent colors
        var accentColors = colorData
            .OrderByDescending(x => x.Colorfulness)
            .Select(x => x.Color);

        // Get primary/secondary/tertiary accents
        var primary = accentColors.First();
        var secondary = accentColors.ElementAtOrDefault(1);
        secondary = secondary != default ? secondary : primary;
        var tertiary = accentColors.ElementAtOrDefault(2);
        tertiary = tertiary != default ? tertiary : secondary;

        // Get base color
        var baseColor = accentColors.Last();

        // Get dominant color by prominence
#if NET6_0_OR_GREATER
        var dominantColor = colorData
            .MaxBy(x => x.Prominence).Color;
#else
        var dominantColor = colorData
            .OrderByDescending((x) => x.Prominence)
            .First().Color;
#endif

        // Batch update the dependency properties in the UI Thread
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            PrimaryAccentColor = primary;
            SecondaryAccentColor = secondary;
            TertiaryAccentColor = tertiary;
            BaseColor = baseColor;
            DominantColor = dominantColor;
        });
    }

    private async Task<Vector3[]> SampleSourcePixelColorsAsync(int sampleCount)
    {
        // Ensure the source is populated
        if (Source is null)
            return [];

        // Grab actual size
        // If actualSize is 0, replace with 1:1 aspect ratio
        var sourceSize = Source.ActualSize;
        sourceSize = sourceSize != Vector2.Zero ? sourceSize : Vector2.One;
        
        // Calculate size of scaled rerender using the actual size
        // scaled down to the sample count, maintaining aspect ration
        var sourceArea = sourceSize.X * sourceSize.Y;
        var sampleScale = MathF.Sqrt(sampleCount / sourceArea);
        var sampleSize = sourceSize * sampleScale;

        // Rerender the UIElement to a bitmap of about sampleCount pixels
        // Note: RenderTargetBitmap is not supported with Uno Platform.
        var bitmap = new RenderTargetBitmap();
        await bitmap.RenderAsync(Source, (int)sampleSize.X, (int)sampleSize.Y);

        // Create a stream from the bitmap
        var pixels = await bitmap.GetPixelsAsync();
        var pixelByteStream = pixels.AsStream();

        // Something went wrong
        if (pixelByteStream.Length == 0)
            return [];

        // Read the stream into a a color array
        const int bytesPerPixel = 4;
        var samples = new Vector3[(int)pixelByteStream.Length / bytesPerPixel];

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
