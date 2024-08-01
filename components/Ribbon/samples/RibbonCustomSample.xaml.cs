// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace RibbonExperiment.Samples;

/// <summary>
/// An example of the <see cref="Ribbon"/> control.
/// </summary>
[ToolkitSample(id: nameof(RibbonCustomSample), "Ribbon control sample", description: $"A sample for showing how to create and use a {nameof(Ribbon)} custom control.")]
public sealed partial class RibbonCustomSample : Page
{
    public RibbonCustomSample() => InitializeComponent();
}
