// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINAPPSDK
using CursorEnum = Windows.UI.Core.CoreCursorType;
#else
using Microsoft.UI.Input;
using CursorEnum = Microsoft.UI.Input.InputSystemCursorShape;
#endif

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A simple Thumb control which can be manipulated in multiple directions to assist with resize scenarios.
/// </summary>
public partial class ResizeThumb : Control
{
    /// <summary>
    /// Gets or sets how the <see cref="ResizeThumb"/> should behave and manipulate it target element. Will automatically set the required <see cref="Cursor"/> and <see cref="UIElement.ManipulationMode"/> values.
    /// </summary>
    public ResizeDirection Direction
    {
        get { return (ResizeDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ResizeDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(ResizeDirection), typeof(ResizeThumb), new PropertyMetadata(ResizeDirection.None, OnDirectionPropertyChanged));

    private static void OnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ResizeThumb resizeThumb)
        {
            resizeThumb.ManipulationMode = resizeThumb.Direction switch
            {
                ResizeDirection.Top => ManipulationModes.TranslateY,
                ResizeDirection.Bottom => ManipulationModes.TranslateY,
                ResizeDirection.Left => ManipulationModes.TranslateX,
                ResizeDirection.Right => ManipulationModes.TranslateX,
                ResizeDirection.TopLeft => ManipulationModes.TranslateX | ManipulationModes.TranslateY,
                ResizeDirection.TopRight => ManipulationModes.TranslateX | ManipulationModes.TranslateY,
                ResizeDirection.BottomLeft => ManipulationModes.TranslateX | ManipulationModes.TranslateY,
                ResizeDirection.BottomRight => ManipulationModes.TranslateX | ManipulationModes.TranslateY,
                _ => ManipulationModes.None,
            };
        }
    }

    /// <summary>
    /// Gets or sets how the <see cref="ResizeThumb"/> should resize its target element.
    /// </summary>
    public ResizePositionMode PositionMode
    {
        get { return (ResizePositionMode)GetValue(PositionModeProperty); }
        set { SetValue(PositionModeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PositionMode"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PositionModeProperty =
        DependencyProperty.Register(nameof(PositionMode), typeof(ResizePositionMode), typeof(ResizeThumb), new PropertyMetadata(ResizePositionMode.Canvas));

    /// <summary>
    /// Gets or sets the cursor which should be displayed when the mouse is over this thumb. If unset, will automatically be set based on 
    /// </summary>
    public CursorEnum Cursor
    {
        get { return (CursorEnum)GetValue(CursorProperty); }
        set { SetValue(CursorProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Cursor"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CursorProperty =
        DependencyProperty.Register(nameof(Cursor), typeof(CursorEnum), typeof(ResizeThumb), new PropertyMetadata(null, OnCursorPropertyChanged));

    private static void OnCursorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ResizeThumb resizeThumb)
        {
            // Set cursor based on resize direction, if not explicitly set.
            var cursor = resizeThumb.ReadLocalValue(CursorProperty);
            if (cursor == DependencyProperty.UnsetValue || cursor == null)
            {
                cursor = resizeThumb.Direction switch {
                    ResizeDirection.Top => CursorEnum.SizeNorthSouth,
                    ResizeDirection.Bottom => CursorEnum.SizeNorthSouth,
                    ResizeDirection.Left => CursorEnum.SizeWestEast,
                    ResizeDirection.Right => CursorEnum.SizeWestEast,
                    ResizeDirection.TopLeft => CursorEnum.SizeNorthwestSoutheast,
                    ResizeDirection.TopRight => CursorEnum.SizeNortheastSouthwest,
                    ResizeDirection.BottomLeft => CursorEnum.SizeNortheastSouthwest,
                    ResizeDirection.BottomRight => CursorEnum.SizeNorthwestSoutheast,
                    _ => CursorEnum.UniversalNo,
                };
            }

#if !WINAPPSDK
            // On UWP, we use our XAML extension to control this behavior,
            // so we'll update it here (and maintain any cursor override).
            if (cursor is CursorEnum cursorValue)
            {
                FrameworkElementExtensions.SetCursor(resizeThumb, cursorValue);
            }

            return;
#endif

#if WINUI3
            // On WinUI 3, we can set the ProtectedCursor directly.
            if (cursor is CursorEnum cursorValue &&
                (resizeThumb.ProtectedCursor == null ||
                    (resizeThumb.ProtectedCursor is InputSystemCursor current &&
                     current.CursorShape != cursorValue)))
            {
                resizeThumb.ProtectedCursor = InputSystemCursor.Create(cursorValue);
            }
#endif
        }
    }

    /// <summary>
    /// Gets or sets the control that the <see cref="ResizeThumb"/> is resizing. Be default, this will be the visual ancestor of the <see cref="ResizeThumb"/>.
    /// </summary>
    public FrameworkElement? TargetControl
    {
        get { return (FrameworkElement?)GetValue(TargetControlProperty); }
        set { SetValue(TargetControlProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="TargetControl"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TargetControlProperty =
        DependencyProperty.Register(nameof(TargetControl), typeof(FrameworkElement), typeof(ResizeThumb), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the incremental amount of change for dragging with the mouse or touch of a sizer control. Effectively a snapping increment for changes. The default is 1.
    /// </summary>
    /// <example>
    /// For instance, if the DragIncrement is set to 16. Then when a component is resized with the sizer, it will only increase or decrease in size in that increment. I.e. -16, 0, 16, 32, 48, etc...
    /// </example>
    /// <remarks>
    /// TODO: (Need to figure out how keyboard input works and if handled here or by adorner, or both) This value is independent of the KeyboardIncrement property. If you need to provide consistent snapping when moving regardless of input device, set these properties to the same value.
    /// </remarks>
    public double DragIncrement
    {
        get { return (double)GetValue(DragIncrementProperty); }
        set { SetValue(DragIncrementProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DragIncrement"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DragIncrementProperty =
        DependencyProperty.Register(nameof(DragIncrement), typeof(double), typeof(ResizeThumb), new PropertyMetadata(1d));
}
