// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public class TokenRemovingEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRemovingEventArgs"/> class.
    /// </summary>
    /// <param name="item">Item being removed.</param>
    /// <param name="Token"><see cref="Token"/> container being closed.</param>
    public TokenRemovingEventArgs(object item, Token token)
    {
        Item = item;
        Token = token;
    }

    /// <summary>
    /// Gets the Item being closed.
    /// </summary>
    public object Item { get; private set; }

    /// <summary>
    /// Gets the Tab being closed.
    /// </summary>
    public Token Token { get; private set; }
}
