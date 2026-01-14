// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINAPPSDK
using Windows.UI;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls.Primitives;
#endif
using Windows.System;

namespace CommunityToolkit.WinUI.Controls;

public partial class GradientSlider
{
    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        if (sender is not GradientSliderThumb thumb)
            return;

        _draggingThumb = thumb;
        var xStart = Canvas.GetLeft(thumb);
        var yStart = e.VerticalOffset;
        _dragPosition = new Point(xStart, yStart);

        OnThumbDragStarted(e);
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (_containerCanvas is null)
            return;

        if (sender is not GradientSliderThumb thumb)
            return;

        _dragPosition.X += e.HorizontalChange;
        _dragPosition.Y += e.VerticalChange;

        HandleThumbDragging(thumb, _dragPosition);
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _draggingThumb = null;

        OnThumbDragCompleted(e);
        OnValueChanged();
    }

    private void Thumb_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (sender is not GradientSliderThumb thumb)
            return;

        var change = e.Key switch
        {
#if !HAS_UNO
            VirtualKey.Left when FlowDirection is FlowDirection.RightToLeft => 0.05,
            VirtualKey.Right when FlowDirection is FlowDirection.RightToLeft => -0.05,
#endif

            VirtualKey.Left => -0.01,
            VirtualKey.Right => 0.01,

            _ => 0,
        };

        if (change is not 0)
        {
            thumb.GradientStop.Offset = Math.Clamp(change + thumb.GradientStop.Offset, 0, 1);
            UpdateThumbPosition(thumb);
            e.Handled = true;
        }
    }

    private void ContainerCanvas_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (_placeholderThumb is null)
            return;

        if (IsAddStopsEnabled)
        {
            _placeholderThumb.Visibility = Visibility.Visible;
        }

        VisualStateManager.GoToState(this, PointerOverState, false);
    }

    private void ContainerCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_containerCanvas is null || _placeholderThumb is null)
            return;

        var position = e.GetCurrentPoint(_containerCanvas).Position;
        var posX = position.X;

        if (_draggingThumb is null)
        {
            // NOTE: This check could be made O(log(n)) by tracking the thumbs positions in a sorted list and running a binary search
            _placeholderThumb.IsEnabled = !IsPointerOverThumb(posX);

            var thumbPosition = posX - _placeholderThumb.ActualWidth / 2;
            thumbPosition = Math.Clamp(thumbPosition, 0, _containerCanvas.ActualWidth - _placeholderThumb.ActualWidth);
            Canvas.SetLeft(_placeholderThumb, thumbPosition);
        }
        else if (_draggingThumb.PointerCaptures?.Count is null or 0)
        {
            HandleThumbDragging(_draggingThumb, position);
        }
    }

    private void ContainerCanvas_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (_placeholderThumb is null)
            return;

        _placeholderThumb.Visibility = Visibility.Collapsed;
        _placeholderThumb.IsEnabled = false;

        VisualStateManager.GoToState(this, NormalState, false);
    }

    private void ContainerCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (_containerCanvas is null || _placeholderThumb is null)
            return;

        if (!IsAddStopsEnabled)
            return;

        var position = e.GetCurrentPoint(_containerCanvas).Position.X;
        if (IsPointerOverThumb(position))
            return;

        _containerCanvas.CapturePointer(e.Pointer);

        _placeholderThumb.IsEnabled = false;

        var stop = new GradientStop()
        {
            Offset = position / _containerCanvas.ActualWidth,
            Color = Colors.Black,
        };

        GradientStops.Add(stop);
        _draggingThumb = AddStop(stop);
    }

    private void ContainerCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_containerCanvas is null)
            return;

        _draggingThumb = null;
        _containerCanvas.ReleasePointerCapture(e.Pointer);

        OnValueChanged();
    }

    private bool IsPointerOverThumb(double position)
    {
        if (_containerCanvas is null)
            return false;

        foreach (var child in _containerCanvas.Children)
        {
            if (child is not GradientSliderThumb thumb)
                continue;

            var thumbPos = Canvas.GetLeft(thumb);
            if (position > thumbPos - thumb.ActualWidth && position < thumbPos + (thumb.ActualWidth * 2))
                return true;
        }

        return false;
    }

    private void HandleThumbDragging(GradientSliderThumb thumb, Point position)
    {
        if (_containerCanvas is null)
            return;

        var newPos = position.X / (_containerCanvas.ActualWidth - thumb.ActualWidth);
        thumb.GradientStop.Offset = Math.Clamp(newPos, 0, 1);
        UpdateThumbPosition(thumb);
    }
}
