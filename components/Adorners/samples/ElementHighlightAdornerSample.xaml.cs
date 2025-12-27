// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace AdornersExperiment.Samples;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ToolkitSampleBoolOption("IsAdornerVisible", false, Title = "Is Adorner Visible")]

[ToolkitSample(id: nameof(ElementHighlightAdornerSample), "Highlighting an Element w/ Adorner", description: "A sample for showing how to highlight an element's bounds with an Adorner.")]
public sealed partial class ElementHighlightAdornerSample : Page
{
    public ElementHighlightAdornerSample()
    {
        this.InitializeComponent();
    }
}
