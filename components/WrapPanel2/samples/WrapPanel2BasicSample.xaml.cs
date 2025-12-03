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
[ToolkitSampleBoolOption("FixedRowLengths", false, Title = "Fixed Row Lengths")]
[ToolkitSampleMultiChoiceOption("LayoutForcedStretchMethod", "None", "First", "Last", "Equal", "Proportional", Title = "Forced Stretch Method")]
[ToolkitSampleMultiChoiceOption("LayoutOverflowBehavior", "Wrap", "Drop", Title = "Overflow Behavior")]

[ToolkitSample(id: nameof(WrapPanel2BasicSample), "WrapPanel2 Basic Sample", description: $"A sample for showing how to use a {nameof(WrapPanel2)} panel.")]
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
    public static ForcedStretchMethod ConvertStringToForcedStretchMethod(string stretchMethod) => stretchMethod switch
    {
        "None" => ForcedStretchMethod.None,
        "First" => ForcedStretchMethod.First,
        "Last" => ForcedStretchMethod.Last,
        "Equal" => ForcedStretchMethod.Equal,
        "Proportional" => ForcedStretchMethod.Proportional,
        _ => throw new System.NotImplementedException(),
    };

    // TODO: See https://github.com/CommunityToolkit/Labs-Windows/issues/149
    public static OverflowBehavior ConvertStringToOverflowBehavior(string overflowBehavior) => overflowBehavior switch
    {
        "Wrap" => OverflowBehavior.Wrap,
        "Drop" => OverflowBehavior.Drop,
        _ => throw new System.NotImplementedException(),
    };
}
