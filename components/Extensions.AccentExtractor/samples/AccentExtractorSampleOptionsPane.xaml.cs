// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Media.Imaging;

namespace Extensions.AccentExtractorExperiment.Samples;

[ToolkitSampleOptionsPane(nameof(AccentExtractorSample))]
public partial class AccentExtractorSampleOptionsPane : UserControl
{
    private AccentExtractorSample.XamlNamedPropertyRelay _sample;
    
    public AccentExtractorSampleOptionsPane(AccentExtractorSample sample)
    {
        this.InitializeComponent();

        _sample = new AccentExtractorSample.XamlNamedPropertyRelay(sample);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _sample.AccentedImage.Source = new BitmapImage(new Uri(UrlTextbox.Text));
    }
}
