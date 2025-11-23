// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace AdornersExperiment.Samples;

[ToolkitSampleBoolOption("IsAdornerVisible", true, Title = "Is Adorner Visible")]

[ToolkitSample(id: nameof(AdornersTabBadgeSample), "InfoBadge w/ Adorner in TabView", description: "A sample for showing how add an InfoBadge to a TabViewItem via an Adorner.")]
public sealed partial class AdornersTabBadgeSample : Page
{
    public AdornersTabBadgeSample()
    {
        this.InitializeComponent();
    }
}
