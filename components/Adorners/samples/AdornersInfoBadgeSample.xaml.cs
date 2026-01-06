// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace AdornersExperiment.Samples;

[ToolkitSampleBoolOption("IsAdornerVisible", true, Title = "Is Adorner Visible")]

[ToolkitSample(id: nameof(AdornersInfoBadgeSample), "InfoBadge w/ Adorner", description: "A sample for showing how add an infobadge to a component via an Adorner.")]
public sealed partial class AdornersInfoBadgeSample : Page
{
    public AdornersInfoBadgeSample()
    {
        this.InitializeComponent();
    }
}
