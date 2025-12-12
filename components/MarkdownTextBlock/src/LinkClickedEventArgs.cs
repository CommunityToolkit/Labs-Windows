// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public class LinkClickedEventArgs : EventArgs
{
    public Uri Uri { get; }
    /// <summary>
    /// Set to true in your handler to indicate the link click was handled and default navigation should be suppressed.
    /// </summary>
    public bool Handled { get; set; }

    public LinkClickedEventArgs(Uri uri)
    {
        this.Uri = uri;
    }
}
