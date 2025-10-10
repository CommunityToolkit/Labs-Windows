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
            var ratio = CalculateWCAGContrastRatio(@base, opponent);
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
        var luminance = CalculatePerceivedLuminance(opponent);
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
        var actualRatio = CalculateWCAGContrastRatio(color, GetOpponent(d));
        SetContrastRatio(d, actualRatio);

        // Unlock the original color updates
        _selfUpdate = false;
    }

    private static double CalculateWCAGContrastRatio(Color color1, Color color2)
    {
        // Using the formula for contrast ratio
        // Source WCAG guidelines: https://www.w3.org/TR/WCAG20/#contrast-ratiodef

        // Calculate perceived luminance for both colors
        double luminance1 = CalculatePerceivedLuminance(color1);
        double luminance2 = CalculatePerceivedLuminance(color2);

        // Determine lighter and darker luminance
        double lighter = Math.Max(luminance1, luminance2);
        double darker = Math.Min(luminance1, luminance2);

        // Calculate contrast ratio
        return (lighter + 0.05f) / (darker + 0.05f);
    }

    private static double CalculatePerceivedLuminance(Color color)
    {
        // Color theory is a massive iceberg. Here's a peek at the tippy top:

        // There's two (main) standards for calculating luminance from RGB values.

        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +
        // | Standard      | Formula                              | Ref. Section       | Ref. Link                                                                       |
        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +
        // | ITU Rec. 709  | Y = 0.2126 R + 0.7152 G + 0.0722 B   | Page 4/Item 3.2    | https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf  |
        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +
        // | ITU Rec. 601  | Y = 0.299 R + 0.587 G + 0.114 B      | Page 2/Item 2.5.1  | https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.601-7-201103-I!!PDF-E.pdf  |
        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +

        // They're based on the standard ability of the human eye to perceive brightness,
        // from different colors, as well as the average monitor's ability to produce them.
        // Both standards produce similar results, but Rec. 709 is more accurate for modern displays.

        // NOTE: If we for whatever reason we ever need to optimize this code,
        // we can make approximations using integer math instead of floating point math.
        // The precise values are not critical, as long as the relative luminance is accurate.
        // Like so: return (2 * color.R + 7 * color.G + color.B);

        // TLDR: We use ITU Rec. 709 standard formula for perceived luminance.
        return (0.2126f * color.R + 0.7152f * color.G + 0.0722 * color.B) / 255;
    }
}
