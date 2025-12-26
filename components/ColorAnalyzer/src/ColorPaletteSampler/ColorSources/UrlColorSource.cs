// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A <see cref="ColorSource"/> that that loads pixel data from a url.
/// </summary>
public class UrlColorSource : ColorSource
{
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Source"/> property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(string), typeof(UrlColorSource), new PropertyMetadata(null, OnSourceChanged));

    /// <summary>
    /// Gets or sets the url source sampled for a color palette.
    /// </summary>
    public string? Source
    {
        get => (string?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <inheritdoc/>
    public override async Task<Stream?> GetPixelDataAsync(int requestedSamples)
    {
        if (Source is null)
            return null;

        var stream = await RandomAccessStreamReference.CreateFromUri(new Uri(Source)).OpenReadAsync();
#if !HAS_UNO
        var decoder = await BitmapDecoder.CreateAsync(stream);
        var pixelData = await decoder.GetPixelDataAsync();
        var bytes = pixelData.DetachPixelData();
        return new MemoryStream(bytes);
#else
        // NOTE: This assumes raw pixel data.
        // TODO: Uses some form of image processing
        return stream.AsStream();
#endif
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not UrlColorSource source)
            return;

        source.InvokeSourceUpdated();
    }
}
