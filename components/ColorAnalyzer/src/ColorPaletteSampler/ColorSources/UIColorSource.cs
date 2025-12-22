// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Numerics;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A <see cref="ColorSource"/> that samples the 
/// </summary>
public class UIColorSource : ColorSource
{
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Source"/> property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(UIElement), typeof(UIColorSource), new PropertyMetadata(null, OnSourceChanged));

    /// <summary>
    /// Gets or sets the <see cref="UIElement"/> source sampled for a color palette.
    /// </summary>
    public UIElement? Source
    {
        get => (UIElement)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <inheritdoc/>
    public override async Task<Stream?> GetPixelStreamAsync(int sampleCount)
    {
        // Ensure the source is populated
        if (Source is null)
            return null;

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
        return pixels.AsStream();
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not UIColorSource source)
            return;

        source.InvokeSourceUpdated();
    }
}
