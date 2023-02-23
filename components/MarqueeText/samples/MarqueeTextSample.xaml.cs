// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MarqueeTextExperiment.Samples;
[ToolkitSample(id: nameof(MarqueeTextSample), "MarqueeText", description: "A control for scrolling text in a marquee fashion.")]
[ToolkitSampleMultiChoiceOption("MarqueeSpeed", "Speed", "Slow : 64", "Medium : 96", "Fast : 128")]
//[ToolkitSampleMultiChoiceOption("MarqueeMode", "Mode", "Ticker", "Looping", "Bouncing")]
//[ToolkitSampleMultiChoiceOption("MarqueeDirection", "Direction", "Left", "Right", "Up", "Down")]
[ToolkitSampleMultiChoiceOption("MarqueeRepeat", "Repeat", "Forever", "1x", "2x")]
public sealed partial class MarqueeTextSample : Page
{
    public MarqueeTextSample()
    {
        this.InitializeComponent();
    }
}
