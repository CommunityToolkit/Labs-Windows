// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI;

namespace TokenViewExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleBoolOption("CanRemoveTokens", false, Title = "Can tokens be removed")]

[ToolkitSample(id: nameof(TokenViewRemoveSample), "Remove sample", description: $"A sample for showing how to create and use a {nameof(TokenView)} control.")]
public sealed partial class TokenViewRemoveSample : Page
{
   
    public TokenViewRemoveSample()
    {
        this.InitializeComponent();
    }
}
