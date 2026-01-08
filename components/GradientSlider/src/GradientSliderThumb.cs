// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public sealed partial class GradientSliderThumb : Control
{
    public static readonly DependencyProperty GradientStopProperty =
        DependencyProperty.Register(nameof(GradientStop),
            typeof(GradientStop),
            typeof(GradientSliderThumb),
            new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="GradientSliderThumb"/> class.
    /// </summary>
    public GradientSliderThumb()
    {
        DefaultStyleKey = typeof(GradientSliderThumb);
    }

    public GradientStop GradientStop
    {
        get => (GradientStop)GetValue(GradientStopProperty);
        set => SetValue(GradientStopProperty, value);
    }
}
