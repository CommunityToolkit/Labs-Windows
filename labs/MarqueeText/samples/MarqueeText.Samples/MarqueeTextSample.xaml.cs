// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MarqueeTextExperiment.Samples;
[ToolkitSample(id: nameof(MarqueeTextSample), "Templated control", description: "A sample for showing how to create and use a templated control.")]
[ToolkitSampleMultiChoiceOption("MarqueeSpeed", "Speed", "64", "96", "128")]
//[ToolkitSampleMultiChoiceOption("MarqueeMode", "Mode", "Ticker : 0", "Looping : 1", "Bouncing : 2")]
public sealed partial class MarqueeTextSample : Page
{
    public MarqueeTextSample()
    {
        this.InitializeComponent();
    }
}
