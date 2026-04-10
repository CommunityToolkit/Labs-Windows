// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Describes the behavior for stretching childrenn in the wrap-panel.
/// </summary>
/// <remarks>
/// This only effects lines without a star-sized item.
/// </remarks>
public enum WrapPanelItemsStretch
{
    /// <summary>
    /// No additional stretching is applied to non-star items. 
    /// </summary>
    /// <remarks>
    /// Items with a Star-sized <see cref="WrapPanel2.LayoutLengthProperty"/> will still expand 
    /// if the justification mode allows for stretching to fill the line.
    /// </remarks>
    None,

    /// <summary>
    /// The first item in the line is stretched to occupy all remaining space.
    /// </summary>
    First,

    /// <summary>
    /// The last item in the line is stretched to occupy all remaining space.
    /// </summary>
    Last,

    /// <summary>
    /// All items in the line are stretched to an equal size to fill the available space.
    /// </summary>
    Equal,

    /// <summary>
    /// All items in the line are stretched proportionally based on their desired size to fill the remaining space.
    /// </summary>
    Proportional,
}
