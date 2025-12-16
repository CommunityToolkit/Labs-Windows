// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace WrapPanel2Experiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleMultiChoiceOption("LayoutOrientation", "Horizontal", "Vertical", Title = "Orientation")]
[ToolkitSampleMultiChoiceOption("LayoutHorizontalAlignment", "Left", "Center", "Right", "Stretch", Title = "Horizontal Alignment")]
[ToolkitSampleMultiChoiceOption("LayoutVerticalAlignment", "Top", "Center", "Bottom", "Stretch", Title = "Vertical Alignment")]
[ToolkitSampleNumericOption("ItemSpacing", 8, 0, 16, Title = "Item Spacing")]
[ToolkitSampleNumericOption("LineSpacing", 2, 0, 16, Title = "Line Spacing")]
[ToolkitSampleBoolOption("ItemJustification", false, Title = "Item Justification")]
[ToolkitSampleMultiChoiceOption("LayoutItemsStretch", "None", "First", "Last", "Equal", "Proportional", Title = "Items Stretch")]

[ToolkitSample(id: nameof(WrapPanel2BasicSample), $"Basic demo of the {nameof(WrapPanel2)} with auto-sized items.", description: $"A sample showing every property of the {nameof(WrapPanel2)} panel.")]
public sealed partial class WrapPanel2BasicSample : Page
{
    public WrapPanel2BasicSample()
    {
        this.InitializeComponent();
    }

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static Orientation ConvertStringToOrientation(string orientation) => orientation switch
    {
        "Vertical" => Orientation.Vertical,
        "Horizontal" => Orientation.Horizontal,
        _ => throw new System.NotImplementedException(),
    };

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static HorizontalAlignment ConvertStringToHorizontalAlignment(string alignment) => alignment switch
    {
        "Left" => HorizontalAlignment.Left,
        "Center" => HorizontalAlignment.Center,
        "Right" => HorizontalAlignment.Right,
        "Stretch" => HorizontalAlignment.Stretch,
        _ => throw new System.NotImplementedException(),
    };

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static VerticalAlignment ConvertStringToVerticalAlignment(string alignment) => alignment switch
    {
        "Top" => VerticalAlignment.Top,
        "Center" => VerticalAlignment.Center,
        "Bottom" => VerticalAlignment.Bottom,
        "Stretch" => VerticalAlignment.Stretch,
        _ => throw new System.NotImplementedException(),
    };

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static WrapPanelItemsStretch ConvertStringToItemsStretch(string stretchMethod) => stretchMethod switch
    {
        "None" => WrapPanelItemsStretch.None,
        "First" => WrapPanelItemsStretch.First,
        "Last" => WrapPanelItemsStretch.Last,
        "Equal" => WrapPanelItemsStretch.Equal,
        "Proportional" => WrapPanelItemsStretch.Proportional,
        _ => throw new System.NotImplementedException(),
    };

    private int _index;

    private string[] LoremIpsumWords => LoremIpsum.Split(' ');

    private string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nam fermentum placerat pretium. Phasellus molestie faucibus purus ut semper. Etiam felis ante, condimentum sed leo in, aliquam pharetra libero. Etiam ante ante, sagittis in semper eu, aliquam non sapien. Donec a pharetra magna. Suspendisse et nulla magna. Cras varius sem dolor, ac faucibus turpis malesuada ac. Maecenas rutrum tortor et faucibus rutrum. Vestibulum in gravida odio, non dapibus dui. Praesent leo tellus, vulputate sed sollicitudin id, fringilla quis ligula. Cras eget ex vitae purus pulvinar mattis. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Donec consectetur tellus id augue ultrices, eget congue tellus pharetra.";

    private void AddItemClick(object sender, RoutedEventArgs e)
    {
        AddItem();
    }

    private void Add5ItemsClick(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i < 5; i++)
            AddItem();
    }

    private void ClearItemsClick(object sender, RoutedEventArgs e)
    {
        WrapPanel.Children.Clear();
        _index = 0;
    }

    private void AddItem()
    {
        _index = _index % LoremIpsumWords.Length;

        var currentWord = LoremIpsumWords[_index++];
        var border = new Border()
        {
            Child = new TextBlock()
            {
                Text = currentWord,
            }
        };

        WrapPanel.Children.Add(border);
    }
}
