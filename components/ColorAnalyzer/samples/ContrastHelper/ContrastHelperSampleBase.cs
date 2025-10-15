// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINUI3
using Microsoft.UI;
#endif

using Windows.UI;

namespace ColorAnalyzerExperiment.Samples;

public abstract partial class ContrastHelperSampleBase : Page
{
    public static readonly DependencyProperty DesiredBackgroundProperty =
        DependencyProperty.Register(nameof(DesiredBackground), typeof(Color), typeof(ContrastHelperSampleBase), new PropertyMetadata(Colors.Black));

    public static readonly DependencyProperty DesiredForegroundProperty =
        DependencyProperty.Register(nameof(DesiredForeground), typeof(Color), typeof(ContrastHelperSampleBase), new PropertyMetadata(Colors.White));

    public static readonly DependencyProperty MinRatioProperty =
        DependencyProperty.Register(nameof(MinRatio), typeof(double), typeof(ContrastHelperSampleBase), new PropertyMetadata(3d));

    public ContrastHelperSampleBase()
    {
    }

    public Color DesiredBackground
    {
        get => (Color)GetValue(DesiredBackgroundProperty);
        set => SetValue(DesiredBackgroundProperty, value);
    }

    public Color DesiredForeground
    {
        get => (Color)GetValue(DesiredForegroundProperty);
        set => SetValue(DesiredForegroundProperty, value);
    }

    public double MinRatio
    {
        get => (double)GetValue(MinRatioProperty);
        set => SetValue(MinRatioProperty, value);
    }
}
