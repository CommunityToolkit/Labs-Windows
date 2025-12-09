// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Describes the behavior for stretching childrenn in the wrap-panel.
/// </summary>
public enum StretchChildren
{
    /// <summary>
    /// Only star-sized items will ever be stretched.
    /// </summary>
    StarSizedOnly,

    /// <summary>
    /// In a row without star-sized items, the first item in the row will be stretched to fill the row.
    /// </summary>
    First,

    /// <summary>
    /// In a row without star-sized items, the last item in the row will be stretched to fill the row.
    /// </summary>
    Last,

    /// <summary>
    /// In a row without star-sized items, each item will be stretched to an equal size to fill the row.
    /// </summary>
    Equal,

    /// <summary>
    /// In a row without star-sized items, each item will be stretched proportional to their desired size to fill the row.
    /// </summary>
    Proportional,
}
