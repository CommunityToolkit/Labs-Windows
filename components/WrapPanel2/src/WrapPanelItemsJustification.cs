// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Describes how items are arranged between the margins of the main axis in the <see cref="WrapPanel2"/>.
/// </summary>
public enum WrapPanelItemsJustification
{
    /// <summary>
    /// Items are arranged according to the control's alignment.
    /// </summary>
    Automatic,

    /// <summary>
    /// Items are aligned toward the start of the line (Left for Horizontal, Top for Vertical).
    /// </summary>
    Start,

    /// <summary>
    /// Items are centered within the line. 
    /// </summary>
    Center,

    /// <summary>
    /// Items are aligned toward the end of the line (Right for Horizontal, Bottom for Vertical).
    /// </summary>
    End,

    /// <summary>
    /// Items are distributed with equal internal spacing and half-sized spacing at the margins.
    /// </summary>
    SpaceAround,

    /// <summary>
    /// Items are distributed with equal spacing between them; no spacing is applied at the margins.
    /// </summary>
    SpaceBetween,

    /// <summary>
    /// Items are distributed so that the spacing between any two items and the spacing to the margins is equal.
    /// </summary>
    SpaceEvenly,
}
