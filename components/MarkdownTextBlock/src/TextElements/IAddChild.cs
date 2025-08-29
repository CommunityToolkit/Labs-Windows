// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls.TextElements;

public interface IAddChild
{
    TextElement TextElement { get; }
    void AddChild(IAddChild child);
}
