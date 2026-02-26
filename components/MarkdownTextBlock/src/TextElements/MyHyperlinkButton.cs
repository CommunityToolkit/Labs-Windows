// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using HtmlAgilityPack;
using Markdig.Syntax.Inlines;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyHyperlinkButton : IAddChild
{
    private HyperlinkButton _hyperLinkButton;
    private InlineUIContainer _inlineUIContainer = new InlineUIContainer();
    private MyFlowDocument _flowDoc;
    private string? _baseUrl;
    private LinkInline? _linkInline;
    private HtmlNode? _htmlNode;
    private MarkdownTextBlock _control;
    
    public event RoutedEventHandler? ClickEvent
    {
        add
        {
            _hyperLinkButton.Click += value;
        }
        remove
        {
            _hyperLinkButton.Click -= value;
        }
    }

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement
    {
        get => _inlineUIContainer;
    }

    public MyHyperlinkButton(LinkInline linkInline, MarkdownTextBlock control)
        : this(linkInline.GetDynamicUrl != null ? linkInline.GetDynamicUrl() ?? linkInline.Url : linkInline.Url, control.BaseUrl, null, linkInline, control)
    {
    }

    public MyHyperlinkButton(HtmlNode htmlNode, MarkdownTextBlock control)
        : this(htmlNode.GetAttribute("href", "#"), control.BaseUrl, htmlNode, null, control)
    {
    }

    private MyHyperlinkButton(string? url, string? baseUrl, HtmlNode? htmlNode, LinkInline? linkInline, MarkdownTextBlock control)
    {
        _baseUrl = baseUrl;
        _htmlNode = htmlNode;
        _linkInline = linkInline;
        _control = control;
        _hyperLinkButton = new HyperlinkButton
        {
            NavigateUri = Extensions.GetUri(url, baseUrl),
        };
        _hyperLinkButton.Padding = new Thickness(0);
        _hyperLinkButton.Margin = new Thickness(0);
        if (_htmlNode != null)
        {
            _flowDoc = new MyFlowDocument(_htmlNode);
        }
        else
        {
            _flowDoc = new MyFlowDocument(_linkInline!);
        }
        _inlineUIContainer.Child = _hyperLinkButton;
        _flowDoc.RichTextBlock.Foreground = _control.Config.LinkForeground;
        _hyperLinkButton.Content = _flowDoc.RichTextBlock;
    }

    public void AddChild(IAddChild child)
    {
        _flowDoc.AddChild(child);
    }
}
