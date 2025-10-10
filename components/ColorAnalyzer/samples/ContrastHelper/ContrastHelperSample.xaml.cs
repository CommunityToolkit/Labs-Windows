// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI;
using Windows.UI;

namespace ColorAnalyzerExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(ContrastHelperSample), "ContrastAnalyzer helper", description: $"A sample for showing how the contrast analyzer can be used.")]
public sealed partial class ContrastHelperSample : Page
{
    public static readonly DependencyProperty DesiredBackgroundProperty =
        DependencyProperty.Register(nameof(DesiredBackground), typeof(Color), typeof(ContrastHelperSample), new PropertyMetadata(Colors.Black));

    public static readonly DependencyProperty DesiredForegroundProperty =
        DependencyProperty.Register(nameof(DesiredForeground), typeof(Color), typeof(ContrastHelperSample), new PropertyMetadata(Colors.White));

    private static readonly DependencyProperty MinRatioProperty =
        DependencyProperty.Register(nameof(MinRatio), typeof(double), typeof(ContrastHelperSample), new PropertyMetadata(0d));

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

    public ContrastHelperSample()
    {
        this.InitializeComponent();
    }
}
