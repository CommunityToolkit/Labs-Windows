// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
}
