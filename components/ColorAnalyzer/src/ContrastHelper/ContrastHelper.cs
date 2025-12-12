// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A class that provides attached properties for contrast analysis and adjustment.
/// </summary>
public partial class ContrastHelper
{
    // When the helper is updating the color, this flag is set to avoid feedback loops
    // It has no threading issues since all updates are on the UI thread
    private static bool _selfUpdate = false;

    private static void ApplyContrastCheck(DependencyObject d)
    {
        // Grab brush to update
        var brush = FindBrush(d, out _);
        if (brush is null)
            return;

        // Retrieve colors to compare
        Color @base = GetOriginalColor(d);
        Color opponent = GetOpponent(d);

        // Transparent is a sentinel value to say contrast ensurance should applied
        // regardless of contrast ratio
        if (@base != Colors.Transparent)
        {
            // Calculate the WCAG contrast ratio
            var ratio = @base.ContrastRatio(opponent);
            SetOriginalContrastRatio(d, ratio);

            // Use original color if the contrast is in the acceptable range
            if (ratio >= GetMinRatio(d))
            {
                UpdateContrastedProperties(d, @base);
                return;
            }
        }

        // Current contrast is too small.
        // Select either black or white backed on the opponent luminance
        var luminance = opponent.RelativeLuminance();
        var contrastingColor = luminance < 0.5f ? Colors.White : Colors.Black;
        UpdateContrastedProperties(d, contrastingColor);
    }

    /// <summary>
    /// Finds the <see cref="SolidColorBrush"/> and its associated <see cref="DependencyProperty"/>
    /// from <paramref name="d"/>..
    /// </summary>
    /// <param name="d">The attached <see cref="DependencyObject"/>.</param>
    /// <param name="dp">
    /// The <see cref="DependencyProperty"/> associated with the <see cref="SolidColorBrush"/>
    /// belonging to <paramref name="d"/>.
    /// </param>
    /// <returns>The <see cref="SolidColorBrush"/> for <paramref name="d"/>.</returns>
    private static SolidColorBrush? FindBrush(DependencyObject d, out DependencyProperty? dp)
    {
        (SolidColorBrush? brush, dp) = d switch
        {
            SolidColorBrush b => (b, SolidColorBrush.ColorProperty),
            TextBlock t => (t.Foreground as SolidColorBrush, TextBlock.ForegroundProperty),
            Control c => (c.Foreground as SolidColorBrush, Control.ForegroundProperty),
            _ => (null, null),
        };

        return brush;
    }

    private static void UpdateContrastedProperties(DependencyObject d, Color color)
    {
        // Block the original color from updating
        _selfUpdate = true;

        switch (d)
        {
            case SolidColorBrush b:
                b.Color = color;
                break;
            case TextBlock t:
                t.Foreground = new SolidColorBrush(color);
                break;
            case Control c:
                c.Foreground = new SolidColorBrush(color);
                break;
        }

        // Calculate the actual ratio, between the opponent and the actual color
        var opponent = GetOpponent(d);
        var actualRatio = color.ContrastRatio(opponent);
        SetContrastRatio(d, actualRatio);

        // Unlock the original color updates
        _selfUpdate = false;
    }

}
