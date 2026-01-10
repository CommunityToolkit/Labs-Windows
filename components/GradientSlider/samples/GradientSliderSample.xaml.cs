// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace GradientSliderExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>

[ToolkitSample(id: nameof(GradientSliderSample), "Custom control", description: $"A sample for showing how to create and use a {nameof(GradientSlider)} custom control.")]
[ToolkitSampleBoolOption("CanAddStops", true, Title = nameof(GradientSlider.IsAddStopsEnabled))]
public sealed partial class GradientSliderSample : Page
{
    public GradientSliderSample()
    {
        this.InitializeComponent();
    }
}
