// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Describes the behavior of items that exceed the available space in the panel.
/// </summary>
public enum OverflowBehavior
{
    /// <summary>
    /// When an item exceeds the available space, it will be moved to a new row or column.
    /// </summary>
    Wrap,

    /// <summary>
    /// Items which do not fit within the available space will be removed from the layout.
    /// </summary>
    Drop,
}
