// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public class TokenItemRemovingEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenItemRemovingEventArgs"/> class.
    /// </summary>
    /// <param name="item">Item being removed.</param>
    /// <param name="Token"><see cref="Token"/> container being closed.</param>
    public TokenItemRemovingEventArgs(object item, TokenItem tokenITem)
    {
        Item = item;
        TokenItem = tokenITem;
    }

    /// <summary>
    /// Gets the Item being closed.
    /// </summary>
    public object Item { get; private set; }

    /// <summary>
    /// Gets the Tab being closed.
    /// </summary>
    public TokenItem TokenItem { get; private set; }
}
