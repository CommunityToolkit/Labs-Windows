// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINDOWS_UWP
using Microsoft.UI.Xaml.Media.Imaging;
#elif WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace ColorAnalyzerExperiment.Samples;

[ToolkitSampleOptionsPane(nameof(AccentAnalyzerSample))]
public partial class ImageOptionsPane : UserControl
{
    private AccentAnalyzerSample.XamlNamedPropertyRelay _sample;
    
    public ImageOptionsPane(AccentAnalyzerSample sample)
    {
        this.InitializeComponent();

        _sample = new AccentAnalyzerSample.XamlNamedPropertyRelay(sample);

        string[] images = ["Bloom.jpg", "Headphones.jpg", "Paint.jpg"];
        StockImages = images.Select(x => $"ms-appx:///Assets/StockImages/{x}").ToList();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        SetImage(new Uri(UrlTextbox.Text));
    }

    public IList<string> StockImages { get; }

    private void GridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        SetImage(new Uri((string)e.ClickedItem));
    }

    private void SetImage(Uri uri)
    {
        _sample.AccentedImage.Source = new BitmapImage(uri);
    }
}
