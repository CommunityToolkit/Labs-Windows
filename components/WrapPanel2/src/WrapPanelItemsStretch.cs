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
    /// Items will not be stretched to help justify items.
    /// </summary>
    /// <remarks>
    /// Items with a Star-Sized <see cref="WrapPanel2.LayoutLengthProperty"/> will still stretch if the main-axis alignment
    /// is set to stretch or if <see cref="WrapPanel2.ItemJustification"/> is enabled.
    /// </remarks>
    None,

    /// <summary>
    /// The first item in the row will be stretched to fill the row.
    /// </summary>
    First,

    /// <summary>
    /// Ehe last item in the row will be stretched to fill the row.
    /// </summary>
    Last,

    /// <summary>
    /// Each item will be stretched to an equal size to fill the row.
    /// </summary>
    Equal,

    /// <summary>
    /// Each item will be stretched proportional to their desired size to fill the row.
    /// </summary>
    Proportional,
}
