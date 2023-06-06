// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Labs.WinUI;

namespace SegmentedControlExperiment.Samples;

/// <summary>
/// An example sample page of a Segmented control.
/// </summary>
[ToolkitSampleMultiChoiceOption("SelectionMode", "Single", "Multiple", Title = "Selection mode")]
[ToolkitSampleMultiChoiceOption("Alignment", "Left", "Center", "Right", "Stretch", Title = "Horizontal alignment")]

[ToolkitSample(id: nameof(SegmentedControlBasicSample), "Basics", description: $"A sample for showing how to create and use a {nameof(Segmented)} custom control.")]
public sealed partial class SegmentedControlBasicSample : Page
{
    public SegmentedControlBasicSample()
    {
        this.InitializeComponent();
    }

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static ListViewSelectionMode ConvertStringToSelectionMode(string selectionMode) => selectionMode switch
    {
        "Single" => ListViewSelectionMode.Single,
        "Multiple" => ListViewSelectionMode.Multiple,
        _ => throw new System.NotImplementedException(),
    };

    public static HorizontalAlignment ConvertStringToHorizontalAlignment(string alignment) => alignment switch
    {
        "Left" => HorizontalAlignment.Left,
        "Center" => HorizontalAlignment.Center,
        "Right" => HorizontalAlignment.Right,
        "Stretch" => HorizontalAlignment.Stretch,
        _ => throw new System.NotImplementedException(),
    };
}

