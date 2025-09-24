// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Media.Imaging;

namespace CommunityToolkit.WinUI.Extensions;

public static partial class AccentExtractor
{
    /// <summary>
    /// Attached <see cref="DependencyProperty"/> for the <see cref="Image"/> that enables or disables accent color calculation.
    /// </summary>
    public static readonly DependencyProperty ImageCalculateAccentProperty =
        DependencyProperty.RegisterAttached("CalculateAccent", typeof(bool), typeof(Image), new PropertyMetadata(false, OnCalculateAccentChanged));
    
    /// <summary>
    /// Attached <see cref="DependencyProperty"/> for the <see cref="ImageBrush"/> that enables or disables accent color calculation.
    /// </summary>
    public static readonly DependencyProperty ImageBrushCalculateAccentProperty =
        DependencyProperty.RegisterAttached("CalculateAccent", typeof(bool), typeof(ImageBrush), new PropertyMetadata(false, OnCalculateAccentChanged));

    private static void OnCalculateAccentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // No change. Do nothing.
        if ((bool)e.NewValue == (bool)e.OldValue)
            return;
        
        bool assign = (bool)e.NewValue;
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

    private static void OnImageOpened(object sender, RoutedEventArgs e)
    {
        // Extract the ImageSource from the sender
        ImageSource? source = null;
        switch (sender)
        {
            case Image image:
                source = image.Source;
                break;
            case ImageBrush imageBrush:
                source = imageBrush.ImageSource;
                break;
        }

        // If the source is not a BitmapSource, we cannot process it
        if (source is not BitmapSource bitmapSource)
            return;
    }
}
