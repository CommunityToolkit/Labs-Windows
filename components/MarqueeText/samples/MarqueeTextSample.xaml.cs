// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarqueeTextRns;

namespace MarqueeTextExperiment.Samples;
[ToolkitSample(id: nameof(MarqueeTextSample), "MarqueeText", description: "A control for scrolling text in a marquee fashion.")]
[ToolkitSampleNumericOption("MQSpeed", initial: 96, min: 48, max: 196, step: 1, Title = "Speed")]
[ToolkitSampleMultiChoiceOption("MQDirection", "Left", "Right", "Up", "Down", Title = "Marquee Direction")]
//[ToolkitSampleMultiChoiceOption("MarqueeRepeat", "Repeat", "Forever", "1x", "2x")]
#if !HAS_UNO
[ToolkitSampleMultiChoiceOption("MQBehavior", "Ticker", "Looping", "Bouncing", Title = "Marquee Behavior")]
#else
[ToolkitSampleMultiChoiceOption("MQBehavior", "Ticker", "Looping", Title = "Marquee Behavior")]
#endif
public sealed partial class MarqueeTextSample : Page
{
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
        _ => throw new System.NotImplementedException(),
    };

    private MarqueeDirection ConvertStringToMarqueeDirection(string str) => str switch
    {
        "Left" => MarqueeDirection.Left,
        "Up" => MarqueeDirection.Up,
        "Right" => MarqueeDirection.Right,
        "Down" => MarqueeDirection.Down,
        _ => throw new System.NotImplementedException(),
    };
}
