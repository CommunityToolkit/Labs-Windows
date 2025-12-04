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
    /// Gets or sets the cursor which should be displayed when the mouse is over this thumb.
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
        DependencyProperty.Register(nameof(Cursor), typeof(CursorEnum), typeof(ResizeThumb), new PropertyMetadata(null, OnCursorChanged));

    private static void OnCursorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ResizeThumb resizeThumb)
        {
#if !WINAPPSDK
            // On UWP, we use our XAML extension to control this behavior,
            // so we'll update it here (and maintain any cursor override).
            if (cursor is CursorEnum cursorValue)
            {
                FrameworkElementExtensions.SetCursor(gripper, cursorValue);
            }

            return;
#endif

#if WINUI3
            // On WinUI 3, we can set the ProtectedCursor directly.
            if (e.NewValue is CursorEnum cursorValue &&
                (resizeThumb.ProtectedCursor == null ||
                    (resizeThumb.ProtectedCursor is InputSystemCursor current &&
                     current.CursorShape != cursorValue)))
            {
                resizeThumb.ProtectedCursor = InputSystemCursor.Create(cursorValue);
            }
#endif
        }
    }

    /// <inheritdoc/>
    public ResizeThumb()
    {
        this.DefaultStyleKey = typeof(ResizeThumb);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }
}
