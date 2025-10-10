// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINDOWS_UWP
using Microsoft.UI.Xaml.Media.Imaging;
#elif WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#endif

namespace ColorAnalyzerExperiment.Samples;

[ToolkitSampleOptionsPane(nameof(ContrastHelperSample))]
public partial class ContrastOptionsPane : UserControl
{
    private ContrastHelperSample _sample;
    private ContrastHelperSample.XamlNamedPropertyRelay _sampleXamlRelay;
    
    public ContrastOptionsPane(ContrastHelperSample sample)
    {
        _sample = sample;
        _sampleXamlRelay = new ContrastHelperSample.XamlNamedPropertyRelay(sample);
        
        this.InitializeComponent();
    }

    private void Foreground_ColorChanged(MUXC.ColorPicker sender, MUXC.ColorChangedEventArgs args)
    {
        // TODO: Disect the colorpicker
        if (args.NewColor.A != 255)
            return;

        _sample.DesiredForeground = args.NewColor;
    }

    private void Background_ColorChanged(MUXC.ColorPicker sender, MUXC.ColorChangedEventArgs args)
    {
        // TODO: Disect the colorpicker
        if (args.NewColor.A != 255)
            return;

        _sample.DesiredBackground = args.NewColor;
    }

    private void Ratio_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        _sample.MinRatio = (double)e.NewValue;
    }

    private void FontSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        _sampleXamlRelay.TextSample.FontSize = (double)e.NewValue;
    }

    private void Thickness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        _sampleXamlRelay.ShapeSample.StrokeThickness = (double)e.NewValue;
    }
}
