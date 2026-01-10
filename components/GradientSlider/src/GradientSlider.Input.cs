// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI;

namespace CommunityToolkit.WinUI.Controls;

public partial class GradientSlider
{
    private void Thumb_DragStarted(object sender, DragStartedEventArgs e)
    {
        if (sender is not GradientSliderThumb thumb)
            return;

        _isDragging = true;
        _dragStartPosition = Canvas.GetLeft(thumb);

        OnThumbDragStarted(e);
    }

    private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
    {
        if (_containerCanvas is null)
            return;

        if (sender is not GradientSliderThumb thumb)
            return;

        _dragStartPosition += e.HorizontalChange;
        var newPos = _dragStartPosition / (_containerCanvas.ActualWidth - thumb.ActualWidth);
        thumb.GradientStop.Offset = Math.Clamp(newPos, 0, 1);
        UpdateThumbPosition(thumb);
    }

    private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        _isDragging = false;

        OnThumbDragCompleted(e);
        OnValueChanged();
    }

    private void ContainerCanvas_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        if (_placeholderThumb is null)
            return;

        _placeholderThumb.Visibility = Visibility.Visible;
        _placeholderThumb.IsEnabled = true;
        VisualStateManager.GoToState(this, PointerOverState, false);
    }

    private void ContainerCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_containerCanvas is null || _placeholderThumb is null)
            return;

        var position = e.GetCurrentPoint(_containerCanvas).Position.X;

        // NOTE: This check could be made O(log(n)) by tracking the thumbs positions in a sorted list and running a binary search
        _placeholderThumb.IsEnabled = !IsPointerOverThumb(position);

        var thumbPosition = position - _placeholderThumb.ActualWidth / 2;
        thumbPosition = Math.Clamp(thumbPosition, 0, _containerCanvas.ActualWidth - _placeholderThumb.ActualWidth);
        Canvas.SetLeft(_placeholderThumb, thumbPosition);
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
        if (_containerCanvas is null)
            return;

        var position = e.GetCurrentPoint(_containerCanvas).Position.X;
        if (IsPointerOverThumb(position))
            return;

        var stop = new GradientStop()
        {
            Offset = position / _containerCanvas.ActualWidth,
            Color = Colors.Black,
        };

        GradientStops.Add(stop);
        AddStop(stop);
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
}
