// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace AdornersExperiment.Samples.ResizeElement;

[ToolkitSample(id: nameof(ResizeElementAdornerWithDragSample), "ResizeElementAdorner combined with Drag", description: "A sample for showing how to use an Adorner for resizing an element in combination with drag functionality.")]
public sealed partial class ResizeElementAdornerWithDragSample : Page
{
    public readonly List<CanvasImage> Images = new()
    {
        new CanvasImage("ms-appx:///AdornersExperiment.Samples/Assets/davis-vargas-2vSNlKHn9h0-unsplash.jpg", 100, 50),
        new CanvasImage("ms-appx:///AdornersExperiment.Samples/Assets/sergey-zolkin-m9qMoh-scfE-unsplash.jpg", 300, 150),
    };

    public ResizeElementAdornerWithDragSample()
    {
        this.InitializeComponent();
    }

    private void Image_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        // Select the tapped image and place in front of the other.
        if (e.OriginalSource is FrameworkElement fe && fe.DataContext is CanvasImage ci)
        {
            foreach (var img in Images)
            {
                img.IsSelected = img == ci;
                img.ZIndex = img == ci ? 1 : -1;
            }
        }
    }
}

public partial class CanvasImage(string _imageSource, double _left, double _top) : ObservableObject
{
    public string ImageSource => _imageSource;

    [ObservableProperty]
    public partial double Left { get; set; } = _left;

    [ObservableProperty]
    public partial double Top { get; set; } = _top;

    [ObservableProperty]
    public partial int ZIndex { get; set; } = -1;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}
