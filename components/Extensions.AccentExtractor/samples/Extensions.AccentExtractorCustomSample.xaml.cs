// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Media.Imaging;

namespace Extensions.AccentExtractorExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(AccentExtractorCustomSample), "Custom control", description: $"A sample for showing how ")]
public sealed partial class AccentExtractorCustomSample : Page
{
    public AccentExtractorCustomSample()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        AccentedImage.Source = new BitmapImage(new Uri(UrlTextbox.Text));
    }
}
