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
    /// Items will be arranged according to the control's alignment over the main axis.
    /// </summary>
    Automatic,

    /// <summary>
    /// Items will be arranged starting aligned with starting margin of the line.
    /// </summary>
    Start,

    /// <summary>
    /// Items will be arranged in the center of the line.
    /// </summary>
    Center,

    /// <summary>
    /// Items will be arranged ending aligned with the ending margin of the line.
    /// </summary>
    End,

    /// <summary>
    /// Items will be arranged with padding on both sides of each item, and half-sized padding against the margin.
    /// </summary>
    SpaceAround,

    /// <summary>
    /// Items will be arranged with equal padding between all items, and no padding against the margins.
    /// </summary>
    SpaceBetween,

    /// <summary>
    /// Items will be arranged with padding on both sides of each item, and full-sized padding against the margin.
    /// </summary>
    SpaceEvenly,
}
