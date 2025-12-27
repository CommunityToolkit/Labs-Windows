// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace AdornersExperiment.Samples.ResizeElement;

[ToolkitSampleBoolOption("IsAdornerVisible", false, Title = "Is Adorner Visible")]
[ToolkitSample(id: nameof(ResizeElementAdornerCanvasSample), "Resize Element Adorner on Canvas", description: "A sample for showing how to use an Adorner for resizing an element within a Canvas.")]
public sealed partial class ResizeElementAdornerCanvasSample : Page
{
    public ResizeElementAdornerCanvasSample()
    {
        this.InitializeComponent();
    }
}
