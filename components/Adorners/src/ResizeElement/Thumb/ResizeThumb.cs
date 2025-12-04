// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A simple Thumb control which can be manipulated in multiple directions to assist with resize scenarios.
/// </summary>
public partial class ResizeThumb : Control
{
    private Thickness? _originalMargin;
    private Point? _originalPosition;
    private Size? _originalSize;

    /// <inheritdoc/>
    public ResizeThumb()
    {
        this.DefaultStyleKey = typeof(ResizeThumb);

        Loaded += this.ResizeThumb_Loaded;
    }

    private void ResizeThumb_Loaded(object sender, RoutedEventArgs e)
    {
        if (TargetControl == null)
        {
            TargetControl = this.FindAscendant<FrameworkElement>();
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Ensure we have the proper cursor value setup
        OnDirectionPropertyChanged(this, null!);
        OnCursorPropertyChanged(this, null!);
    }

    /// <inheritdoc/>
    protected override void OnManipulationStarting(ManipulationStartingRoutedEventArgs e)
    {
        // Snap the original size and position when we start dragging.
        _originalSize = new Size(TargetControl?.ActualWidth ?? 0, TargetControl?.ActualHeight ?? 0);

        if (PositionMode == ResizePositionMode.MarginTopLeft)
        {
            _originalMargin = TargetControl?.Margin;
        }
        else
        {
            _originalPosition = new Point(Canvas.GetLeft(TargetControl ?? this), Canvas.GetTop(TargetControl ?? this));
        }
    }

    /// <inheritdoc/>
    protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
    {
        base.OnManipulationDelta(e);

        // We use Truncate here to provide 'snapping' points with the DragIncrement property
        // It works for both our negative and positive values, as otherwise we'd need to use
        // Ceiling when negative and Floor when positive to maintain the correct behavior.
        var horizontalChange =
            Math.Truncate(e.Cumulative.Translation.X / DragIncrement) * DragIncrement;
        var verticalChange =
            Math.Truncate(e.Cumulative.Translation.Y / DragIncrement) * DragIncrement;

        // Important: adjust for RTL language flow settings and invert horizontal axis
#if !HAS_UNO
        if (this.FlowDirection == FlowDirection.RightToLeft)
        {
            horizontalChange *= -1;
        }
#endif

        // Apply the changes to the target control
        if (TargetControl != null)
        {
            // Keep track if we became constrained in a direction and don't adjust position if we didn't update the size.
            bool adjustWidth = false;
            bool adjustHeight = false;

            // Calculate the new size
            var newWidth = _originalSize?.Width ?? 0 + horizontalChange;
            var newHeight = _originalSize?.Height ?? 0 + verticalChange;

            if (Direction != ResizeDirection.Top && Direction != ResizeDirection.Bottom)
            {
                if (IsValidWidth(TargetControl, newWidth, ActualWidth))
                {
                    TargetControl.Width = newWidth;
                    adjustWidth = true;
                }
            }

            if (Direction != ResizeDirection.Left && Direction != ResizeDirection.Right)
            {
                if (IsValidHeight(TargetControl, newHeight, ActualHeight))
                {
                    TargetControl.Height = newHeight;
                    adjustHeight = true;
                }
            }

            // Adjust the position based on position mode first
            if (PositionMode == ResizePositionMode.MarginTopLeft)
            {
                var newMargin = _originalMargin ?? new Thickness();

                if ((Direction == ResizeDirection.Left || Direction == ResizeDirection.TopLeft || Direction == ResizeDirection.BottomLeft)
                    && adjustWidth)
                    newMargin.Left += horizontalChange;

                if ((Direction == ResizeDirection.Top || Direction == ResizeDirection.TopLeft || Direction == ResizeDirection.TopRight)
                    && adjustHeight)
                    newMargin.Top += verticalChange;

                TargetControl.Margin = newMargin;
            }
            else
            {
                var newX = (_originalPosition?.X ?? 0) + horizontalChange;
                var newY = (_originalPosition?.Y ?? 0) + verticalChange;

                if ((Direction == ResizeDirection.Left || Direction == ResizeDirection.TopLeft || Direction == ResizeDirection.BottomLeft)
                    && adjustWidth)
                    Canvas.SetLeft(TargetControl, newX);

                if ((Direction == ResizeDirection.Top || Direction == ResizeDirection.TopLeft || Direction == ResizeDirection.TopRight)
                    && adjustHeight)
                    Canvas.SetTop(TargetControl, newY);
            }
        }
    }
}
