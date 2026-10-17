// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Markdig.Syntax.Inlines;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyAutolinkInline : IAddChild
{
    private AutolinkInline _autoLinkInline;
    private MarkdownTextBlock _control;

    public TextElement TextElement { get; private set; }

    public event TypedEventHandler<Hyperlink, HyperlinkClickEventArgs>? ClickEvent
    {
        add
        {
            ((Hyperlink)TextElement).Click += value;
        }
        remove
        {
            ((Hyperlink)TextElement).Click -= value;
        }
    }

    public MyAutolinkInline(AutolinkInline autoLinkInline, MarkdownTextBlock control)
    {
        _autoLinkInline = autoLinkInline;
        _control = control;
        TextElement = new Hyperlink()
        {
            NavigateUri = new Uri(autoLinkInline.Url),
            Foreground = _control.LinkForeground
        };
    }

    public void AddChild(IAddChild child)
    {
        try
        {
            var text = (MyInlineText)child;
            ((Hyperlink)TextElement).Inlines.Add((Run)text.TextElement);
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding child to MyAutolinkInline", ex);
        }
    }
}
