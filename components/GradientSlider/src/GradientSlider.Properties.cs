// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class GradientSlider
{
    public static readonly DependencyProperty GradientStopsProperty =
        DependencyProperty.Register(nameof(GradientStops),
            typeof(GradientStopCollection),
            typeof(GradientSlider),
            new PropertyMetadata(new GradientStopCollection(), GradientStopsChangedCallback));

    public GradientStopCollection GradientStops
    {
        get => (GradientStopCollection)GetValue(GradientStopsProperty);
        set => SetValue(GradientStopsProperty, value);
    }

    private static void GradientStopsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GradientSlider slider)
            return;

        slider.RefreshThumbs();
        slider.SyncBackground();
    }
}
