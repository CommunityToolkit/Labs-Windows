// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax;

namespace CommunityToolkit.WinUI.Controls.Renderers.ObjectRenderers;

internal class ListItemRenderer : UWPObjectRenderer<ListItemBlock>
{
    protected override void Write(WinUIRenderer renderer, ListItemBlock listItem)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (listItem == null) throw new ArgumentNullException(nameof(listItem));

        renderer.WriteChildren(listItem);
    }
}
