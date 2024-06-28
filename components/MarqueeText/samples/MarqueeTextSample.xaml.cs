// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarqueeTextRns;

namespace MarqueeTextExperiment.Samples;

[ToolkitSample(id: nameof(MarqueeTextSample), "MarqueeText", description: "A control for scrolling text in a marquee fashion.")]
[ToolkitSampleTextOption("MQText", LoremIpsum, Title = "Text")]
[ToolkitSampleNumericOption("MQSpeed", initial: 96, min: 48, max: 196, step: 1, Title = "Speed")]
[ToolkitSampleMultiChoiceOption("MQDirection", "Left", "Right", "Up", "Down", Title = "Marquee Direction")]
//[ToolkitSampleMultiChoiceOption("MarqueeRepeat", "Repeat", "Forever", "1x", "2x")]
#if !HAS_UNO
[ToolkitSampleMultiChoiceOption("MQBehavior", "Ticker", "Looping", "Bouncing", "Cycle", Title = "Marquee Behavior")]
#else
[ToolkitSampleMultiChoiceOption("MQBehavior", "Ticker", "Looping", "Cycle", Title = "Marquee Behavior")]
#endif
public sealed partial class MarqueeTextSample : Page
{
    private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";

    public MarqueeTextSample()
    {
        this.InitializeComponent();
    }

    private MarqueeBehavior ConvertStringToMarqueeBehavior(string str) => str switch
    {
        "Looping" => MarqueeBehavior.Looping,
        "Ticker" => MarqueeBehavior.Ticker,
#if !HAS_UNO
        "Bouncing" => MarqueeBehavior.Bouncing,
#endif
        "Cycle" => MarqueeBehavior.Cycle,
        _ => throw new NotImplementedException(),
    };

    private MarqueeDirection ConvertStringToMarqueeDirection(string str) => str switch
    {
        "Left" => MarqueeDirection.Left,
        "Up" => MarqueeDirection.Up,
        "Right" => MarqueeDirection.Right,
        "Down" => MarqueeDirection.Down,
        _ => throw new System.NotImplementedException(),
    };

    private void StartMarquee_Click(object sender, RoutedEventArgs e)
    {
        Marquee?.StartMarquee();
    }
}
