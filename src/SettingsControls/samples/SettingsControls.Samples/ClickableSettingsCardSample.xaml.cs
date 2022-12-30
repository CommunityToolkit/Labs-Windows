// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace SettingsControlsExperiment.Samples;

[ToolkitSampleBoolOption("IsCardEnabled", "Is Enabled", true)]
// Single values without a colon are used for both label and value.
// To provide a different label for the value, separate with a colon surrounded by a single space on both sides ("label : value").
//[ToolkitSampleMultiChoiceOption("TextSize", title: "Text size", "Small : 12", "Normal : 16", "Big : 32")]
//[ToolkitSampleMultiChoiceOption("TextFontFamily", title: "Font family", "Segoe UI", "Arial", "Consolas")]
//[ToolkitSampleMultiChoiceOption("TextForeground", title: "Text foreground",
//    "Teal       : #0ddc8c",
//    "Sand       : #e7a676",
//    "Dull green : #5d7577")]

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
