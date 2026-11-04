// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using HtmlAgilityPack;
using Markdig.Syntax.Inlines;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyHyperlink : IAddChild
{
    private Hyperlink _hyperlink;
    private LinkInline? _linkInline;
    private HtmlNode? _htmlNode;
    private string? _baseUrl;
    private MarkdownTextBlock _control;

    public event TypedEventHandler<Hyperlink, HyperlinkClickEventArgs> ClickEvent
    {
        add
        {
            _hyperlink.Click += value;
        }
        remove
        {
            _hyperlink.Click -= value;
        }
    }

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement
    {
        get => _hyperlink;
    }

    public MyHyperlink(LinkInline linkInline, MarkdownTextBlock control)
    {
        _baseUrl = control.BaseUrl;
        _control = control;
        var url = linkInline.GetDynamicUrl != null ? linkInline.GetDynamicUrl() ?? linkInline.Url : linkInline.Url;
        _linkInline = linkInline;
        _hyperlink = new Hyperlink()
        {
            NavigateUri = Extensions.GetUri(url, _baseUrl),
            Foreground = _control.LinkForeground
        };
    }

    public MyHyperlink(HtmlNode htmlNode, MarkdownTextBlock control)
    {
        _baseUrl = control.BaseUrl;
        _control = control;
        var url = htmlNode.GetAttribute("href", "#");
        _htmlNode = htmlNode;
        _hyperlink = new Hyperlink()
        {
            NavigateUri = Extensions.GetUri(url, _baseUrl),
            Foreground = _control.LinkForeground
        };
    }

    public void AddChild(IAddChild child)
    {
#if !WINAPPSDK
        if (child.TextElement is Windows.UI.Xaml.Documents.Inline inlineChild)
        {
            try
            {
                _hyperlink.Inlines.Add(inlineChild);
                // TODO: Add support for click handler
            }
            catch { }
        }
#else
        if (child.TextElement is Microsoft.UI.Xaml.Documents.Inline inlineChild)
        {
            try
            {
                _hyperlink.Inlines.Add(inlineChild);
                // TODO: Add support for click handler
            }
            catch { }
        }
#endif
    }
}
