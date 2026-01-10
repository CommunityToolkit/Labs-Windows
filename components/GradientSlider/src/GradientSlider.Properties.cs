// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class GradientSlider
{
    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="GradientStops"/> property.
    /// </summary>
    public static readonly DependencyProperty GradientStopsProperty =
        DependencyProperty.Register(nameof(GradientStops),
            typeof(GradientStopCollection),
            typeof(GradientSlider),
            new PropertyMetadata(null, GradientStopsChangedCallback));

    /// <summary>
    /// Gets or sets the <see cref="GradientStopCollection"/> being modified by the <see cref="GradientSlider"/>.
    /// </summary>
    public GradientStopCollection GradientStops
    {
        get => (GradientStopCollection)GetValue(GradientStopsProperty);
        set => SetValue(GradientStopsProperty, value);
    }

    private static void GradientStopsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not GradientSlider slider)
            return;

        if (slider._containerCanvas is null)
            return;

        // TODO: What happens if the gradient stop collection changes while the user is dragging a stop?

        slider.RefreshThumbs();
    }
}
