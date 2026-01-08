// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Shapes;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A slider for gradient color input.
/// </summary>
[TemplatePart(Name = "ContainerCanvas", Type = typeof(Canvas))]
[TemplatePart(Name = "BackgroundRectangle", Type = typeof(Rectangle))]
public partial class GradientSlider : Control
{
    private readonly Dictionary<GradientStop, GradientSliderThumb> _stopThumbs = [];
    private readonly Dictionary<GradientStop, long> _stopCallbacks = [];

    private Canvas? _containerCanvas;
    private Rectangle? _backgroundRectangle;

    /// <summary>
    /// Creates a new instance of the <see cref="GradientSlider"/> class.
    /// </summary>
    public GradientSlider()
    {
        this.DefaultStyleKey = typeof(GradientSlider);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _containerCanvas = (Canvas)GetTemplateChild("ContainerCanvas");
        _backgroundRectangle = (Rectangle?)GetTemplateChild("BackgroundRectangle");

        _containerCanvas.SizeChanged += this.ContainerCanvas_SizeChanged;

        RefreshThumbs();
    }

    private void AddStop(GradientStop stop)
    {
        if (_containerCanvas is null)
            return;

        // Prepare a thumb for the gradient stop
        var thumb = new GradientSliderThumb()
        {
            GradientStop = stop,
        };

        // Register callbacks
        var callback = stop.RegisterPropertyChangedCallback(GradientStop.OffsetProperty, OnGradientStopOffsetChanged);
        _stopCallbacks.Add(stop, callback);
        thumb.Loaded += this.Thumb_Loaded;

        _stopThumbs.Add(stop, thumb);
        _containerCanvas.Children.Add(thumb);
    }

    private void RemoveStop(GradientStop stop)
    {
        if (_containerCanvas is null)
            return;

        // Should this be an exception?
        if (!_stopThumbs.TryGetValue(stop, out var thumb))
            return;

        stop.UnregisterPropertyChangedCallback(GradientStop.OffsetProperty, _stopCallbacks[stop]);
        _stopCallbacks.Remove(stop);

        _containerCanvas.Children.Remove(thumb);
        _stopThumbs.Remove(stop);
    }

    private void RefreshThumbs()
    {
        ClearThumbs();
        foreach (var stop in GradientStops)
            AddStop(stop);

        SyncBackground();
    }

    private void SyncThumbs()
    {
        foreach (var thumb in _stopThumbs.Values)
            UpdateThumbPosition(thumb);
    }

    private void ClearThumbs()
    {
        foreach (var (stop, thumb) in _stopThumbs)
            RemoveStop(stop);
    }

    private void SyncBackground()
    {
        if (_containerCanvas is null || _backgroundRectangle is null)
            return;

        _backgroundRectangle.Fill = new LinearGradientBrush(GradientStops, 0);
    }

    private void Thumb_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not GradientSliderThumb thumb)
            return;

        thumb.Loaded -= Thumb_Loaded;
        UpdateThumbPosition(thumb);
    }

    private void UpdateThumbPosition(GradientSliderThumb thumb)
    {
        if (_containerCanvas is null)
            return;

        var dragWidth = _containerCanvas.ActualWidth - thumb.ActualWidth;
        Canvas.SetLeft(thumb, thumb.GradientStop.Offset * dragWidth);
    }
}
