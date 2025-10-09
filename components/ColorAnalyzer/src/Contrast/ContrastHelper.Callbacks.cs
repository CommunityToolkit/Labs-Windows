// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINUI2
using Windows.UI;
#endif

namespace CommunityToolkit.WinUI.Helpers;

public partial class ContrastHelper
{
    /// <summary>
    /// Entry point upon the <see cref="OpponentProperty"/> updating.
    /// </summary>
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
    
    /// <summary>
    /// Entry point upon the <see cref="MinRatioProperty"/> updating.
    /// </summary>
    private static void OnMinRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // No opponent has been set, nothing to do
        if (GetCallback(d) is 0)
            return;
        
        // Update the actual color to ensure contrast
        ApplyContrastCheck(d);
    }

    /// <summary>
    /// Entry point upon a subscribed <see cref="TextBlock.ForegroundProperty"/>, <see cref="Control.ForegroundProperty"/>,
    /// or <see cref="SolidColorBrush.ColorProperty"/> updating.
    /// </summary>
    /// <param name="obj">The <see cref="DependencyObject"/> the <see cref="OpponentProperty"/> is attached to.</param>
    /// <param name="sender">The <see cref="DependencyObject"/> the <paramref name="prop"/> belongs to.</param>
    /// <param name="prop">The property that updated.</param>
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
        SetOriginalColor(obj, color);

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
    
    private static void SubscribeToUpdates(DependencyObject d)
    {
        // Get the original color from the brush and the property to monitor.
        // Use Transparent as a sentinel value if the brush is not a SolidColorBrush
        var solidColorBrush = FindBrush(d, out var dp);
        var color = solidColorBrush?.Color ?? Colors.Transparent;

        // Record the original color
        SetOriginalColor(d, color);

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
}
