// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SegmentedControlExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleMultiChoiceOption("SelectionMode", title: "Selection", "Single", "Multiple")]

[ToolkitSample(id: nameof(SegmentedControlCustomSample), "Basics", description: $"A sample for showing how to create and use a {nameof(Segmented)} custom control.")]
public sealed partial class SegmentedControlCustomSample : Page
{
    public SegmentedControlCustomSample()
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
}
