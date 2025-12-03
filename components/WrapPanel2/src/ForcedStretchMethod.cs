// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Describes the behavior of items in rows without a star-sized item.
/// </summary>
public enum ForcedStretchMethod
{
    /// <summary>
    /// Items will never be streched beyond their desired size.
    /// </summary>
    None,

    /// <summary>
    /// The first item in the row will be stretched to fill the row.
    /// </summary>
    First,

    /// <summary>
    /// The last item in the row will be stretched to fill the row.
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
