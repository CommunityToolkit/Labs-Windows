// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Windows.UI;

namespace WrapPanel2Experiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleMultiChoiceOption("LayoutOrientation", "Horizontal", "Vertical", Title = "Orientation")]
[ToolkitSampleMultiChoiceOption("LayoutHorizontalAlignment", "Left", "Center", "Right", "Stretch", Title = "Horizontal Alignment")]
[ToolkitSampleMultiChoiceOption("LayoutVerticalAlignment", "Top", "Center", "Bottom", "Stretch", Title = "Vertical Alignment")]
[ToolkitSampleNumericOption("ItemSpacing", 8, 0, 16, Title = "Item Spacing")]
[ToolkitSampleNumericOption("LineSpacing", 2, 0, 16, Title = "Line Spacing")]
[ToolkitSampleBoolOption("FixedRowLengths", false, Title = "Fixed Row Lengths")]
[ToolkitSampleMultiChoiceOption("LayoutStretchChildren", "StarSizedOnly", "First", "Last", "Equal", "Proportional", Title = "Forced Stretch Method")]
[ToolkitSampleMultiChoiceOption("LayoutOverflowBehavior", "Wrap", "Drop", Title = "Overflow Behavior")]

[ToolkitSample(id: nameof(WrapPanel2ProportionalSample), "Demo of proportional sizing", description: $"A sample showing every property of the {nameof(WrapPanel2)} panel.")]
public sealed partial class WrapPanel2ProportionalSample : Page
{
    public WrapPanel2ProportionalSample()
    {
        this.InitializeComponent();
    }
}
