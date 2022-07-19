// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SettingsControlsExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSampleBoolOption("IsCardEnabled", "IsEnabled", true)]

[ToolkitSample(id: nameof(SettingsCardButtonSample), "A clickable SettingsCard", description: "A sample for showing how a SettingsCard can be used as a button.")]
public sealed partial class SettingsCardButtonSample : Page
{
    public SettingsCardButtonSample()
    {
        this.InitializeComponent();
    }
}
