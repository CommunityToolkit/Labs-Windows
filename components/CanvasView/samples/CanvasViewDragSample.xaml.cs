// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CanvasViewExperiment.Samples;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
[ToolkitSample(id: nameof(CanvasViewDragSample), "Canvas View", description: "Draging items with a CanvasView control.")]
public sealed partial class CanvasViewDragSample : Page
{
    public List<ObservableRect> Rectangles = new()
    {
        new(100, 50, 200, 50),
        new(300, 150, 75, 75),
        new(200, 250, 100, 100),
    };

    public CanvasViewDragSample()
    {
        this.InitializeComponent();
    }
}

#pragma warning disable CA1001 // Type 'NavigateToUriAction' owns disposable field(s) '__storeBackingField' but is not disposable. From Uno - Gtk, Skia/WPF, WASM
public partial class ObservableRect : DependencyObject
#pragma warning restore CA1001 // Type 'NavigateToUriAction' owns disposable field(s) '__storeBackingField' but is not disposable. From Uno - Gtk, Skia/WPF, WASM
{
    public ObservableRect(int left, int top, int width, int height)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;
    }

    public int Left
    {
        get { return (int)GetValue(LeftProperty); }
        set { SetValue(LeftProperty, value); }
    }

    public static readonly DependencyProperty LeftProperty =
        DependencyProperty.Register(nameof(Left), typeof(int), typeof(ObservableRect), new PropertyMetadata(0));

    public int Top
    {
        get { return (int)GetValue(TopProperty); }
        set { SetValue(TopProperty, value); }
    }

    public static readonly DependencyProperty TopProperty =
        DependencyProperty.Register(nameof(Top), typeof(int), typeof(ObservableRect), new PropertyMetadata(0));

    public int Width
    {
        get { return (int)GetValue(WidthProperty); }
        set { SetValue(WidthProperty, value); }
    }

    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(nameof(Width), typeof(int), typeof(ObservableRect), new PropertyMetadata(0));

    public int Height
    {
        get { return (int)GetValue(HeightProperty); }
        set { SetValue(HeightProperty, value); }
    }

    public static readonly DependencyProperty HeightProperty =
        DependencyProperty.Register(nameof(Height), typeof(int), typeof(ObservableRect), new PropertyMetadata(0));
}
