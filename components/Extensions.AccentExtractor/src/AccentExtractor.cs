// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Buffers;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace CommunityToolkit.WinUI.Extensions;

/// <summary>
/// 
/// </summary>
public static partial class AccentExtractor
{
    /// <summary>
    /// Attached <see cref="DependencyProperty"/> that enables or disables accent color calculation.
    /// </summary>
    public static readonly DependencyProperty CalculateAccentProperty =
        DependencyProperty.RegisterAttached("CalculateAccent", typeof(bool), typeof(AccentExtractor), new PropertyMetadata(false, OnCalculateAccentChanged));

    /// <summary>
    /// Attached <see cref="DependencyProperty"/> that holds the calculated accent color.
    /// </summary>
    public static readonly DependencyProperty AccentColorProperty =
        DependencyProperty.RegisterAttached("AccentColor", typeof(Color), typeof(AccentExtractor), new PropertyMetadata(Colors.Transparent));

    /// <summary>
    /// Gets a value indicating whether the accent calculation is enabled for the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="obj">The <see cref="UIElement"/> from which to retrieve the value.</param>
    /// <returns><see langword="true"/> if accent calculation is enabled for the specified <see cref="UIElement"/>; otherwise,
    /// <see langword="false"/>.</returns>
    public static bool GetCalculateAccent(UIElement obj) => (bool)obj.GetValue(CalculateAccentProperty);

    /// <summary>
    /// Sets a value indicating whether the accent calculation is enabled for the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="obj">The <see cref="UIElement"/> on which to assign the value.</param>
    /// <param name="value">The value to assign.</param>
    public static void SetCalculateAccent(UIElement obj, bool value) => obj.SetValue(CalculateAccentProperty, value);

    /// <summary>
    ///  Gets the calculated accent color for the specified <see cref="UIElement"/>.
    /// </summary>
    /// <param name="obj">The <see cref="UIElement"/> from which to retrieve the value.</param>
    /// <returns>The accent color calculated for the <see cref="UIElement"/>.</returns>
    public static Color GetAccentColor(UIElement obj) => (Color)obj.GetValue(AccentColorProperty);

    /// <summary>
    /// Sets the calculated accent color for the specified <see cref="UIElement"/>.
    /// </summary>
    /// <remarks>
    /// Can this be removed? The color is calculated automatically, so there's no need to set it manually.
    /// </remarks>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetAccentColor(UIElement obj, Color value) => obj.SetValue(AccentColorProperty, value);

    /// <summary>
    /// Updates the <see cref="AccentColorProperty"/> attached property based on the dominant color of the <see cref="UIElement"/>.
    /// </summary>
    /// <param name="sender">The <see cref="UIElement"/> to extract the accent color from.</param>
    public static async Task UpdateAccentAsync(this UIElement sender)
    {
        // TODO: Properly remove Uno support
#if !HAS_UNO
        // Rerender the UIElement to a 64x64 bitmap
        RenderTargetBitmap bitmap = new RenderTargetBitmap();
        await bitmap.RenderAsync(sender, 64, 64);

        // Create a stream from the bitmap
        var pixels = await bitmap.GetPixelsAsync();
        var stream = pixels.AsStream();

        // 
        if (stream.Length == 0)
            return;

        // Read the stream into a a color array
        int pos = 0;
        Span<Color> colors = new Color[(int)stream.Length / 4]; // This should be 4096 (64x64), but it's good to be safe.
        Span<byte> bytes = stackalloc byte[4];
        while (stream.Read(bytes) > 0)
        {
            // Safety check to avoid going out of bounds
            // This should never happen, but it's good to be safe.
            if (pos >= colors.Length)
                break;

            colors[pos] = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
            pos++;
        }

        var rand = Random.Shared.Next(colors.Length - 1);
        var accent = colors[rand];

        // Set the accent color on the UI thread
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() => SetAccentColor(sender, accent));
#endif
    }

    private static void OnCalculateAccentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // No change. Do nothing.
        if ((bool)e.NewValue == (bool)e.OldValue)
            return;

        // If true, calculate the accent color immediately.
        bool assign = (bool)e.NewValue;
        if (assign && d is UIElement uie)
        {
            _ = uie.UpdateAccentAsync();
        }
        
        // If the element is an Image or ImageBrush, register for the image opened event.
        // This ensures that the accent color is recalculated when the image source changes.
        switch (d)
        {
            case Image image:
                if (assign)
                {
                    image.ImageOpened += OnImageOpened;
                }
                else
                {
                    image.ImageOpened -= OnImageOpened;
                }
                break;
            case ImageBrush imageBrush:
                if (assign)
                {
                    imageBrush.ImageOpened += OnImageOpened;
                }
                else
                {
                    imageBrush.ImageOpened -= OnImageOpened;
                }
                break;
        }
    }

    private static async void OnImageOpened(object sender, RoutedEventArgs e) => await UpdateAccentAsync((UIElement)sender);
}

