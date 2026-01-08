// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public sealed partial class GradientSliderThumb : Control
{
    private bool _pointerOver;
    private bool _pressed;
    private bool _isDragging;
    private Point _dragStartPosition;
    private Point _lastPosition;

    /// <summary>
    /// Fires when the <see cref="GradientSliderThumb"/> captures the pointer.
    /// </summary>
    public event DragStartedEventHandler? DragStarted;

    /// <summary>
    /// Fires as the <see cref="GradientSliderThumb"/> is moved while capturing the pointer.
    /// </summary>
    public event DragDeltaEventHandler? DragDelta;

    /// <summary>
    /// Fires when the <see cref="GradientSliderThumb"/> releases the captured pointer.
    /// </summary>
    public event DragCompletedEventHandler? DragCompleted;

    private void GradientSliderThumb_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _pointerOver = true;

        if (!_pressed)
        {
            VisualStateManager.GoToState(this, PointerOverState, true);
        }
    }

    private void GradientSliderThumb_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        _pointerOver = false;

        if (!_pressed)
        {
            VisualStateManager.GoToState(this, NormalState, true);
        }
    }

    private void GradientSliderThumb_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _pressed = true;
        _isDragging = true;

        CapturePointer(e.Pointer);

        _dragStartPosition = e.GetCurrentPoint(this).Position;
        _lastPosition = _dragStartPosition;

        DragStarted?.Invoke(this,
            new DragStartedEventArgs(_dragStartPosition.X, _dragStartPosition.Y));

        VisualStateManager.GoToState(this, PressedState, true);
    }

    private void GradientSliderThumb_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging)
            return;

        var position = e.GetCurrentPoint(this).Position;

        double deltaX = position.X - _lastPosition.X;
        double deltaY = position.Y - _lastPosition.Y;

        _lastPosition = position;

        DragDelta?.Invoke(this, new DragDeltaEventArgs(deltaX, deltaY));
    }

    private void GradientSliderThumb_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (_isDragging)
        {
            var end = e.GetCurrentPoint(this).Position;

            double totalX = end.X - _dragStartPosition.X;
            double totalY = end.Y - _dragStartPosition.Y;

            DragCompleted?.Invoke(this,
                new DragCompletedEventArgs(totalX, totalY, false));
        }

        _isDragging = false;
        _pressed = false;

        ReleasePointerCapture(e.Pointer);

        VisualStateManager.GoToState(this, _pointerOver ? PointerOverState : NormalState, true);
    }

    private void GradientSliderThumb_PointerCanceled(object sender, PointerRoutedEventArgs e)
    {
        if (_isDragging)
        {
            DragCompleted?.Invoke(this,
                new DragCompletedEventArgs(0, 0, true));
        }

        _isDragging = false;
        _pressed = false;

        ReleasePointerCapture(e.Pointer);
        VisualStateManager.GoToState(this, NormalState, true);
    }

    private void GradientSliderThumb_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        VisualStateManager.GoToState(this, IsEnabled ? NormalState : DisabledState, true);
    }
}
