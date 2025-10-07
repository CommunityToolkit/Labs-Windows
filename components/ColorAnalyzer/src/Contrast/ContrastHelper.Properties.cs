// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

public partial class ContrastHelper
{
    /// <summary>
    /// An attached property that defines the color to compare against.
    /// </summary>
    public static readonly DependencyProperty OpponentProperty =
        DependencyProperty.RegisterAttached(
            "Opponent",
            typeof(Color),
            typeof(ContrastHelper),
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
            typeof(ContrastHelper),
            new PropertyMetadata(21d, OnMinRatioChanged));

    /// <summary>
    /// An attached property that records the original color before adjusting for contrast.
    /// </summary>
    public static readonly DependencyProperty OriginalColorProperty =
        DependencyProperty.RegisterAttached(
            "Original",
            typeof(Color),
            typeof(ContrastHelper),
            new PropertyMetadata(Colors.Transparent));

    // Tracks the callback on the original brush being updated.
    private static readonly DependencyProperty CallbackProperty =
        DependencyProperty.RegisterAttached(
            "Callback",
            typeof(long),
            typeof(ContrastHelper),
            new PropertyMetadata(0L));

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

    /// <summary>
    /// Gets the original color before adjustment for contrast.
    /// </summary>
    public static Color GetOriginal(DependencyObject obj) => (Color)obj.GetValue(OriginalColorProperty);

    private static void SetOriginal(DependencyObject obj, Color color) => obj.SetValue(OriginalColorProperty, color);

    private static long GetCallback(DependencyObject obj) => (long)obj.GetValue(CallbackProperty);

    private static void SetCallback(DependencyObject obj, long value) => obj.SetValue(CallbackProperty, value);
}
