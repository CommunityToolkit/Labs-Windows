// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace WrapPanel2Experiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(WrapPanel2ProportionalSample), "Demo of proportional sizing", description: $"A sample showing every property of the {nameof(WrapPanel2)} panel.")]
public sealed partial class WrapPanel2ProportionalSample : Page
{
    public WrapPanel2ProportionalSample()
    {
        this.InitializeComponent();
    }
}
