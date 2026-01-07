// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax.Inlines;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyAutolinkInline : IAddChild
{
    private AutolinkInline _autoLinkInline;
    private MarkdownThemes _themes;

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

    public MyAutolinkInline(AutolinkInline autoLinkInline, MarkdownThemes themes)
    {
        _autoLinkInline = autoLinkInline;
        _themes = themes;
        TextElement = new Hyperlink()
        {
            NavigateUri = new Uri(autoLinkInline.Url),
            Foreground = _themes.LinkForeground
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
