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
    /// Get the opponent color to compare against.
    /// </summary>
    /// <returns>The opponent color.</returns>
    public static Color GetOpponent(DependencyObject obj)
    {
        return (Color)obj.GetValue(OpponentProperty);
    }

    /// <summary>
    /// Set the opponent color to compare against.
    /// </summary>
    public static void SetOpponent(DependencyObject obj, Color value)
    {
        obj.SetValue(OpponentProperty, value);
    }

    private static void OnOpponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SolidColorBrush? brush = d switch
        {
            SolidColorBrush b => b,
            TextBlock t => t.Foreground as SolidColorBrush,
            Control c => c.Foreground as SolidColorBrush,
            _ => null,
        };

        // Could not find a brush to modify.
        if (brush is null)
            return;

        Color @base = brush.Color;
        Color opponent = (Color)e.NewValue;

        // TODO: Cache original color
        // TODO: Adjust only if contrast is insufficient

        // In the meantime, adjust by percieved luminance regardless
        var luminance = CalculatePerceivedLuminance(opponent);
        var contrastingColor = luminance < 0.5f ? Colors.White : Colors.Black;

        // Assign back
        switch (d)
        {
            case SolidColorBrush b:
                b.Color = contrastingColor;
                break;
            case TextBlock t:
                t.Foreground = new SolidColorBrush(contrastingColor);
                break;
            case Control c:
                c.Foreground = new SolidColorBrush(contrastingColor);
                break;
        }
    }

    private static float CalculatePerceivedLuminance(Color color)
    {
        // Using the formula for perceived luminance
        // Source WCAG guidelines: https://www.w3.org/TR/AERT/#color-contrast
        return (0.299f * color.R + 0.587f * color.G + 0.114f * color.B) / 255f;
    }
}
