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
    private static bool _selfUpdate = false;

    private static void OnOpponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Subscribe to brush updates 
        if (GetCallback(d) is 0)
        {
            SubscribeToUpdates(d);
        }

        ApplyContrastCheck(d);
    }

    private static void OnMinRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ApplyContrastCheck(d);
    }

    private static void ApplyContrastCheck(DependencyObject d)
    {
        // Grab brush to update
        var brush = FindBrush(d, out _);
        if (brush is null)
            return;

        // Find WCAG contrast ratio
        Color @base = GetOriginal(d);
        Color opponent = GetOpponent(d);
        var ratio = CalculateWCAGContrastRatio(@base, opponent);

        // Use original color if the contrast is in the acceptable range
        if (ratio >= GetMinRatio(d))
        {
            AssignColor(d, @base);
            return;
        }

        // Current contrast is too small.
        // Select either black or white backed on the opponent luminance
        var luminance = CalculatePerceivedLuminance(opponent);
        var contrastingColor = luminance < 0.5f ? Colors.White : Colors.Black;
        AssignColor(d, contrastingColor);
    }

    private static void SubscribeToUpdates(DependencyObject d)
    {
        var brush = FindBrush(d, out var dp);
        if (brush is null)
            return;

        // Apply initial update
        SetOriginal(d, brush.Color);

        if (d is not SolidColorBrush)
        {
            // Subscribe to updates from the source Foreground and Brush
            d.RegisterPropertyChangedCallback(dp, (sender, prop) =>
            {
                OnOriginalChangedFromSource(d, sender, prop);
            });
        }

        var callback = brush.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, (sender, prop) =>
        {
            OnOriginalChangedFromSource(d, sender, prop);
        });

        SetCallback(d, callback);
    }

    private static void OnOriginalChangedFromSource(DependencyObject obj, DependencyObject sender, DependencyProperty prop)
    {
        // The contrast helper is updating the color.
        // Ignore the assignment.
        if (_selfUpdate)
            return;

        // Get brush
        var brush = FindBrush(sender, out _);
        if (brush is null)
            return;

        // Update original color
        SetOriginal(obj, brush.Color);

        // Apply contrast correction
        ApplyContrastCheck(obj);
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

    private static double CalculatePerceivedLuminance(Color color) =>
        // Using the formula for perceived luminance
        // Source WCAG guidelines: https://www.w3.org/TR/AERT/#color-contrast
        (0.299f * color.R + 0.587f * color.G + 0.114f * color.B) / 255f;
}
