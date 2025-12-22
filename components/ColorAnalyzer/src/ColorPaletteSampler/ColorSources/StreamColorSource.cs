// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Helpers;

namespace CommunityToolkit.WinUI.ColorAnalyzerRns.ColorPaletteSampler.ColorSources;

/// <summary>
/// A <see cref="ColorSource"/> that uses a <see cref="Stream"/> directly as a source.
/// </summary>
public class StreamColorSource : ColorSource
{
    /// <summary>
    /// Gets the <see cref="DependencyProperty"/> for the <see cref="Source"/> property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(UIElement), typeof(StreamColorSource), new PropertyMetadata(null, OnSourceChanged));

    /// <summary>
    /// Gets or sets the <see cref="UIElement"/> source sampled for a color palette.
    /// </summary>
    public Stream? Source
    {
        get => (Stream)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <inheritdoc/>
    public override async Task<Stream?> GetPixelStreamAsync(int sampleCount)
    {
        // TODO: Sample the data
        return Source;
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not StreamColorSource source)
            return;

        source.InvokeSourceUpdated();
    }
}
