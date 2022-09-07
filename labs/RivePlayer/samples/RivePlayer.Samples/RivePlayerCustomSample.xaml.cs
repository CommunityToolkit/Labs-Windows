// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace RivePlayerExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(RivePlayerCustomSample), "Custom control", description: $"A sample for showing how to create and use a {nameof(RivePlayer)} custom control.")]
public sealed partial class RivePlayerCustomSample : Page
{
    public RivePlayerCustomSample()
    {
        this.InitializeComponent();
    }
}
