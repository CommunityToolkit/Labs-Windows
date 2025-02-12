// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace OpacityMaskViewExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(OpacityMaskViewSample), "OpacityMaskView sample", description: $"A sample for showing how to create and use a {nameof(OpacityMaskView)} control.")]
public sealed partial class OpacityMaskViewSample : Page
{
    public OpacityMaskViewSample()
    {
        this.InitializeComponent();
    }
}
