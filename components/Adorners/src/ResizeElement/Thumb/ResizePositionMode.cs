// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Specifies how the <see cref="ResizeThumb"/> will adjust the position of its target element.
/// </summary>
public enum ResizePositionMode
{
    /// <summary>
    /// Resize using Canvas.Left and Canvas.Top properties.
    /// </summary>
    Canvas,

    /// <summary>
    /// Resize using <see cref="FrameworkElement.Margin"/>'s Top and Left values.
    /// </summary>
    MarginTopLeft,

    // TODO: MarginBottomRight could be added in the future. Not sure of alternate anchor points are useful? e.g. TopRight, BottomLeft
    // Alternate anchor points would require more complex calculations during resize to determine when to change the Width/Height of the element.
}
