// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class GradientSlider
{
    /// <summary>
    /// Event raised when a thumb starts being dragged.
    /// </summary>
    public event DragStartedEventHandler? ThumbDragStarted;

    /// <summary>
    /// Event raised when a thumb ends being dragged.
    /// </summary>
    public event DragCompletedEventHandler? ThumbDragCompleted;

    /// <summary>
    /// Event raised when the gradient's value changes.
    /// </summary>
    public event EventHandler? ValueChanged;

    /// <summary>
    /// Called before the <see cref="ThumbDragStarted"/> event occurs.
    /// </summary>
    /// <param name="e">Event data for the event.</param>
    protected virtual void OnThumbDragStarted(DragStartedEventArgs e)
    {
        ThumbDragStarted?.Invoke(this, e);
    }

    /// <summary>
    /// Called before the <see cref="ThumbDragCompleted"/> event occurs.
    /// </summary>
    /// <param name="e">Event data for the event.</param>
    protected virtual void OnThumbDragCompleted(DragCompletedEventArgs e)
    {
        ThumbDragCompleted?.Invoke(this, e);
    }

    /// <summary>
    /// Called before the <see cref="ValueChanged"/> event occurs.
    /// </summary>
    protected virtual void OnValueChanged()
    {
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }
}
