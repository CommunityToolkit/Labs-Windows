// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINDOWS_UWP
using Microsoft.UI.Xaml.Media.Imaging;
#elif WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
#endif

#if WINUI3
using Microsoft.UI;
#endif

using Windows.UI;

namespace ColorAnalyzerExperiment.Samples;

[ToolkitSampleOptionsPane(nameof(TextBlockContrastSample))]
[ToolkitSampleOptionsPane(nameof(SolidColorBrushContrastSample))]
public partial class ContrastOptionsPane : UserControl
{
    private ContrastHelperSampleBase _sample;
    
    public ContrastOptionsPane(ContrastHelperSampleBase sample)
    {
        _sample = sample;

        this.InitializeComponent();
    }

    public Color DesiredForeground
    {
        get => _sample.DesiredForeground;
        set => _sample.DesiredForeground = value;
    }

    public Color DesiredBackground
    {
        get => _sample.DesiredBackground;
        set => _sample.DesiredBackground = value;
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
}
