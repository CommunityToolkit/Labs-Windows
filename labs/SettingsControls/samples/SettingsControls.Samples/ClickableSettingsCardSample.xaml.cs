// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace SettingsControlsExperiment.Samples;

[ToolkitSampleTextOption("TextText", "Placeholder text", Title = "Input the text")]
[ToolkitSampleBoolOption("IsCardEnabled", true, Title = "Is Enabled")]
[ToolkitSampleSliderOption("TextSize", 50, 1, 200, 10, Title = "FontSize")]
[ToolkitSampleMultiChoiceOption("FontSelection", "Segoe UI", "Arial", "Consolas", Title = "Select font")]

[ToolkitSample(id: nameof(ClickableSettingsCardSample), "ClickableSettingsCardSample", description: "A sample for showing how SettingsCard can be static or clickable.")]
public sealed partial class ClickableSettingsCardSample : Page
{
    public ClickableSettingsCardSample()
    {
        this.InitializeComponent();
    }

    private async void OnCardClicked(object sender, RoutedEventArgs e)
    {
        await Windows.System.Launcher.LaunchUriAsync(new Uri("https://www.microsoft.com"));
    }
}
