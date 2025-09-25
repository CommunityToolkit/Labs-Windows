// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINDOWS_UWP
using Microsoft.UI.Xaml.Media.Imaging;
#elif WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace AccentAnalyzerExperiment.Samples;

[ToolkitSampleOptionsPane(nameof(AccentAnalyzerSample))]
public partial class AccentAnalyzerSampleOptionsPane : UserControl
{
    private AccentAnalyzerSample.XamlNamedPropertyRelay _sample;
    
    public AccentAnalyzerSampleOptionsPane(AccentAnalyzerSample sample)
    {
        this.InitializeComponent();

        _sample = new AccentAnalyzerSample.XamlNamedPropertyRelay(sample);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        _sample.AccentedImage.Source = new BitmapImage(new Uri(UrlTextbox.Text));
    }
}
