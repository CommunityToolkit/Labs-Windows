// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CanvasLayout.Sample.SampleOne;

[ToolkitSampleBoolOption("IsTextVisible", "IsVisible", true)]
[ToolkitSampleMultiChoiceOption("TextSize", title: "Text size", "Small : 12", "Normal : 16", "Big : 32")]
[ToolkitSampleMultiChoiceOption("TextFontFamily", title: "Font family", "Segoe UI", "Arial", "Consolas")]
[ToolkitSampleMultiChoiceOption("TextForeground", title: "Text foreground",
    "Teal       : #0ddc8c",
    "Sand       : #e7a676",
    "Dull green : #5d7577")]

[ToolkitSample(id: nameof(SamplePage), "Simple Options", description: "A sample page for showing how to do simple options.")]
public sealed partial class SamplePage : Page
{
    public SamplePage()
    {
        this.InitializeComponent();
    }
}
