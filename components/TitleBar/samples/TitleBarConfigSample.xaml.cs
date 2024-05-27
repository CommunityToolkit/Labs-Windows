// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace TitleBarExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleTextOption("TitleText", "Contoso", Title = "Title")]
[ToolkitSampleTextOption("SubtitleText", "Preview", Title = "Subtitle")]
[ToolkitSampleBoolOption("ShowBackButtonSetting", false, Title = "ShowBackButton")]
[ToolkitSampleBoolOption("ShowPaneButtonSetting", false, Title = "ShowPaneButton")]

[ToolkitSample(id: nameof(TitleBarConfigSample), "Full titlebar sample", description: $"A sample for showing how to create and use a {nameof(TitleBar)} in a window.")]
public sealed partial class TitleBarConfigSample : Page
{
    public TitleBarConfigSample()
    {
        this.InitializeComponent();
    }

 
}
