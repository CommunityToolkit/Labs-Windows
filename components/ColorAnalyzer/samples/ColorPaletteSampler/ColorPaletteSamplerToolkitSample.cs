// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ColorAnalyzerExperiment.Samples;

public abstract partial class ColorPaletteSamplerToolkitSampleBase : Page
{
    public static readonly DependencyProperty SelectedImageProperty =
        DependencyProperty.Register(nameof(SelectedImage), typeof(ImageSource), typeof(ColorPaletteSamplerToolkitSampleBase), new PropertyMetadata(null));

    public ColorPaletteSamplerToolkitSampleBase()
    {
    }

    public ImageSource SelectedImage
    {
        get => (ImageSource)GetValue(SelectedImageProperty);
        set => SetValue(SelectedImageProperty, value);
    }
}
