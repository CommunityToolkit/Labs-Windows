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

    private static void OnOpponentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Subscribe to brush updates if not already
        if (GetCallbackObject(d) is null)
        {
            SubscribeToUpdates(d);
        }

        // Update the actual color to ensure contrast
        ApplyContrastCheck(d);
    }

    private static void OnMinRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // No opponent has been set, nothing to do
        if (GetCallback(d) is 0)
            return;
        
        // Update the actual color to ensure contrast
        ApplyContrastCheck(d);
    }

    private static void ApplyContrastCheck(DependencyObject d)
    {
        // Grab brush to update
        var brush = FindBrush(d, out _);
        if (brush is null)
            return;

        // Retrieve colors to compare
        Color @base = GetOriginal(d);
        Color opponent = GetOpponent(d);

        // Transparent is a sentinel value to say contrast ensurance should applied
        // regardless of contrast ratio
        if (@base != Colors.Transparent)
        {
            // Calculate the WCAG contrast ratio
            var ratio = CalculateWCAGContrastRatio(@base, opponent);

            // Use original color if the contrast is in the acceptable range
            if (ratio >= GetMinRatio(d))
            {
                AssignColor(d, @base);
                return;
            }
        }

        // Current contrast is too small.
        // Select either black or white backed on the opponent luminance
        var luminance = CalculatePerceivedLuminance(opponent);
        var contrastingColor = luminance < 0.5f ? Colors.White : Colors.Black;
        AssignColor(d, contrastingColor);
    }

    private static void SubscribeToUpdates(DependencyObject d)
    {
        // Get the original color from the brush and the property to monitor.
        // Use Transparent as a sentinel value if the brush is not a SolidColorBrush
        var solidColorBrush = FindBrush(d, out var dp);
        var color = solidColorBrush?.Color ?? Colors.Transparent;

        // Record the original color
        SetOriginal(d, color);

        // Rhetortical Question: Why don't we return if the solidColorBrush is null?
        // Just because the brush is not a SolidColorBrush doesn't mean we can't monitor the
        // Foreground property. We just can't monitor the brush's Color property

        // If the original is not a SolidColorBrush, we need to monitor the Foreground property
        if (d is not SolidColorBrush)
        {
            // Subscribe to updates from the source Foreground
            _ = d.RegisterPropertyChangedCallback(dp, (sender, prop) =>
            {
                OnOriginalChangedFromSource(d, sender, prop);
            });
        }

        // Subscribe to updates from the source SolidColorBrush
        // If solidColorBrush is null, this is a no-op
        SubscribeToBrushUpdates(d, solidColorBrush);
    }

    private static void SubscribeToBrushUpdates(DependencyObject d, SolidColorBrush? brush)
    {
        // No brush, nothing to do
        if (brush is null)
            return;

        // Unsubscribe from previous brush if any
        var oldBrush = GetCallbackObject(d);
        var oldCallback = GetCallback(d);
        oldBrush?.UnregisterPropertyChangedCallback(SolidColorBrush.ColorProperty, oldCallback);

        // Subscribe to updates from the source SolidColorBrush
        var callback = brush.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, (sender, prop) =>
        {
            OnOriginalChangedFromSource(d, sender, prop);
        });

        // Track the callback so we don't double subscribe and can unsubscribe if needed
        SetCallbackObject(d, brush);
        SetCallback(d, callback);
    }

    private static void OnOriginalChangedFromSource(DependencyObject obj, DependencyObject sender, DependencyProperty prop)
    {
        // The contrast helper is updating the color
        // Ignore the assignment.
        if (_selfUpdate)
            return;

        // Get the original color from the brush.
        // We use the sender, not the obj, because the sender is the object that changed.
        // Use Transparent as a sentinel value if the brush is not a SolidColorBrush
        var brush = FindBrush(sender, out _);
        var color = brush?.Color ?? Colors.Transparent;

        // Update original color
        SetOriginal(obj, color);

        // The sender is the Foreground property, not the brush itself.
        // This means the brush changed and our callback on the brush is dead.
        // We need to subscribe to the new brush if it's a SolidColorBrush.
        if (sender is not SolidColorBrush)
        {
            // Subscribe to the new brush
            // Notice we're finding the brush on the object, not the sender this time.
            // We may not find a SolidColorBrush, and that's ok.
            var solidColorBrush = FindBrush(obj, out _);
            SubscribeToBrushUpdates(obj, solidColorBrush);
        }

        // Apply contrast correction
        ApplyContrastCheck(obj);
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
        // ITU Rec. 709: Y = 0.2126 R + 0.7152 G + 0.0722 B
        // ITU Rec. 601: Y = 0.299 R + 0.587 G + 0.114 B

        // They're based on the standard ability of the human eye to perceive brightness,
        // from different colors, as well as the average monitor's ability to produce them.
        // Both standards produce similar results, but Rec. 709 is more accurate for modern displays.

        // NOTE: If we for whatrever reason we ever need to optimize this code,
        // we can make approximations using integer math instead of floating point math.
        // The precise values are not critical, as long as the relative luminance is accurate.
        // Like so: return (2 * color.R + 7 * color.G + color.B);

        // TLDR: We use ITU Rec. 709 standard formula for perceived luminance.
        return (0.2126f * color.R + 0.7152f * color.G + 0.0722 * color.B) / 255;
    }
}
