// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Windows.UI;

namespace MarqueeExperiment.Samples;

[ToolkitSample(id: nameof(MarqueeSample), "Marquee", description: "A control for scrolling content in a marquee fashion.")]
[ToolkitSampleNumericOption("MQSpeed", initial: 96, min: 48, max: 196, step: 1, Title = "Speed")]
[ToolkitSampleMultiChoiceOption("MQDirection", "Left", "Right", "Up", "Down", Title = "Marquee Direction")]
//[ToolkitSampleMultiChoiceOption("MarqueeRepeat", "Repeat", "Forever", "1x", "2x")]
#if !HAS_UNO
[ToolkitSampleMultiChoiceOption("MQBehavior", "Ticker", "Looping", "Bouncing", Title = "Marquee Behavior")]
#else
[ToolkitSampleMultiChoiceOption("MQBehavior", "Ticker", "Looping", Title = "Marquee Behavior")]
#endif
public sealed partial class MarqueeSample : Page
{
    public MarqueeSample()
    {
        this.InitializeComponent();
        
        for (int i = 0; i < 15; i++)
        {
            AddItem_Click(this, null);
        }
    }

    private MarqueeBehavior ConvertStringToMarqueeBehavior(string str) => str switch
    {
        "Looping" => MarqueeBehavior.Looping,
        "Ticker" => MarqueeBehavior.Ticker,
#if !HAS_UNO
        "Bouncing" => MarqueeBehavior.Bouncing,
#endif
        _ => throw new NotImplementedException(),
    };

    public MarqueeSampleItems Data = new();

    private MarqueeDirection ConvertStringToMarqueeDirection(string str) => str switch
    {
        "Left" => MarqueeDirection.Left,
        "Up" => MarqueeDirection.Up,
        "Right" => MarqueeDirection.Right,
        "Down" => MarqueeDirection.Down,
        _ => throw new NotImplementedException(),
    };

    private void AddItem_Click(object sender, RoutedEventArgs? e)
    {
        var rand = new Random();
        Data.Items.Add(new MarqueeSampleItem()
        {
            Name = $"Item {Data.Items.Count + 1}",
            Brush = new SolidColorBrush(Color.FromArgb(255, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))),
        });
    }
}

public class MarqueeSampleItems
{
    public ObservableCollection<MarqueeSampleItem> Items { get; } = new();
}

public record MarqueeSampleItem
{
    public string? Name { get; set; }
    
    public Brush? Brush { get; set; }
}
