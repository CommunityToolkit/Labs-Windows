// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarqueeTextRns;

namespace MarqueeTextExperiment.Samples;

[ToolkitSample(id: nameof(CustomStyleMarqueeTextSample), "MarqueeText", description: "A control for scrolling text in a marquee fashion.")]
[ToolkitSampleTextOption("MQText", LoremIpsum, Title = "Text")]
//[ToolkitSampleMultiChoiceOption("MarqueeRepeat", "Repeat", "Forever", "1x", "2x")]
public sealed partial class CustomStyleMarqueeTextSample : Page
{
    private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

    public CustomStyleMarqueeTextSample()
    {
        this.InitializeComponent();
    }

    private MarqueeDirection ConvertStringToMarqueeDirection(string str) => str switch
    {
        "Left" => MarqueeDirection.Left,
        "Up" => MarqueeDirection.Up,
        "Right" => MarqueeDirection.Right,
        "Down" => MarqueeDirection.Down,
        _ => throw new System.NotImplementedException(),
    };
}
