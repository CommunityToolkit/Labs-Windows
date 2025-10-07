// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A class that provides attached properties for contrast analysis and adjustment.
/// </summary>
public partial class ContrastAnalyzer
{
    /// <summary>
    /// An attached property that defines the color to compare against.
    /// </summary>
    public static readonly DependencyProperty OpponentProperty =
        DependencyProperty.RegisterAttached(
            "Opponent",
            typeof(Color),
            typeof(ContrastAnalyzer),
            new PropertyMetadata(Colors.Transparent, OnOpponentChanged));

    /// <summary>
    /// An attached property that defines the minimum acceptable contrast ratio against the opponent color.
    /// </summary>
    /// <remarks>
    /// Range: 1 to 21 (inclusive). Default is 21 (maximum contrast).
    /// </remarks>
    public static readonly DependencyProperty MinRatioProperty =
        DependencyProperty.RegisterAttached(
            "MinRatio",
            typeof(double),
            typeof(ContrastAnalyzer),
            new PropertyMetadata(21d));

    /// <summary>
    /// Get the opponent color to compare against.
    /// </summary>
    /// <returns>The opponent color.</returns>
    public static Color GetOpponent(DependencyObject obj) => (Color)obj.GetValue(OpponentProperty);

    /// <summary>
    /// Set the opponent color to compare against.
    /// </summary>
    public static void SetOpponent(DependencyObject obj, Color value) => obj.SetValue(OpponentProperty, value);

    /// <summary>
    /// Get the minimum acceptable contrast ratio against the opponent color.
    /// </summary>
    public static double GetMinRatio(DependencyObject obj) => (double)obj.GetValue(MinRatioProperty);

    /// <summary>
    /// Set the minimum acceptable contrast ratio against the opponent color.
    /// </summary>
    public static void SetMinRatio(DependencyObject obj, double value) => obj.SetValue(MinRatioProperty, value);

    private static void OnOpponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var brush = FindBrush(d, out var dp);

        // Could not find a brush to modify.
        if (brush is null)
            return;

        Color @base = brush.Color;
        Color opponent = (Color)e.NewValue;

        // If the colors already meet the minimum ratio, no adjustment is needed
        if (CalculateWCAGContrastRatio(@base, opponent) >= GetMinRatio(d))
            return;

        // In the meantime, adjust by percieved luminance regardless
        var luminance = CalculatePerceivedLuminance(opponent);
        var contrastingColor = luminance < 0.5f ? Colors.White : Colors.Black;

        // Assign color
        AssignColor(d, contrastingColor);
    }

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

    private static void AssignColor(DependencyObject d, Color color)
    {
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

    private static double CalculatePerceivedLuminance(Color color) =>
        // Using the formula for perceived luminance
        // Source WCAG guidelines: https://www.w3.org/TR/AERT/#color-contrast
        (0.299f * color.R + 0.587f * color.G + 0.114f * color.B) / 255f;
}
