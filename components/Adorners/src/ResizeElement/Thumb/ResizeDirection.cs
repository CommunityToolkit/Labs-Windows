// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Specifies the direction in which a <see cref="ResizeThumb"/> will resize its target element.
/// </summary>
public enum ResizeDirection
{
    /// <summary>
    /// No resize.
    /// </summary>
    None,

    /// <summary>
    /// Resize from the top. Manipulates the Y position and Height.
    /// </summary>
    Top,

    /// <summary>
    /// Resize from the bottom. Manipulates the Height.
    /// </summary>
    Bottom,

    /// <summary>
    /// Resize from the left. Manipulates the X position and Width.
    /// </summary>
    Left,

    /// <summary>
    /// Resize from the right. Manipulates the Width.
    /// </summary>
    Right,

    /// <summary>
    /// Resize from the upper-left corner. Manipulates the X position, Y position, Width, and Height.
    /// </summary>
    TopLeft,

    /// <summary>
    /// Resize from the upper-right corner. Manipulates the Y position, Width, and Height.
    /// </summary>
    TopRight,

    /// <summary>
    /// Resize from the lower-left corner. Manipulates the X position, Width, and Height.
    /// </summary>
    BottomLeft,

    /// <summary>
    /// Resize from the lower-right corner. Manipulates the Width and Height.
    /// </summary>
    BottomRight,
}
