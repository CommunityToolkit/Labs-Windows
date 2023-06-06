// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI;

namespace CanvasLayoutExperiment.Samples;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ToolkitSample(id: nameof(CanvasLayoutSample), "Canvas Layout", description: "A canvas-like VirtualizingLayout for use in an ItemsRepeater")]
public sealed partial class CanvasLayoutSample : Page
{
    public ObservableCollection<CanvasItem> Items = new()
    {
        new() { Left = 100, Top = 50, Width = 100, Height = 100, Text = "Item 1" },
        new() { Left = 400, Top = 250, Width = 200, Height = 200, Text = "Item 2" },
        new() { Left = 200, Top = 500, Width = 100, Height = 100, Text = "Item 3" },
        new() { Left = 1200, Top = 2500, Width = 100, Height = 100, Text = "Item 4" },
        new() { Left = 2200, Top = 1500, Width = 100, Height = 100, Text = "Item 5" },
        new() { Left = 1200, Top = 3500, Width = 100, Height = 100, Text = "Item 6" },
    };

    public CanvasLayoutSample()
    {
        this.InitializeComponent();
    }
}

public class CanvasItem : CanvasLayoutItem
{
    public string Text { get; set; } = string.Empty;
}
