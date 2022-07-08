// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Expander.Sample;

[ToolkitSampleMultiChoiceOption("ExpandDirection", title: "Expand Direction", "Down", "Up")]
[ToolkitSample(id: nameof(ExpanderFirstSamplePage), "Simple Expander", description: "A sample page for showing the Expander control.")]
public sealed partial class ExpanderFirstSamplePage : Page
{
    public ExpanderFirstSamplePage()
    {
        this.InitializeComponent();
    }

    public static MUXC.ExpandDirection ConvertStringToDirection(string direction) => direction switch
    {
        "Down" => MUXC.ExpandDirection.Down,
        "Up" => MUXC.ExpandDirection.Up,
        _ => throw new System.NotImplementedException(),
    };
}
