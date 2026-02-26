// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using HtmlAgilityPack;
using Markdig.Syntax;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyHeading : IAddChild
{
    private Paragraph _paragraph;
    private HeadingBlock? _headingBlock;
    private HtmlNode? _htmlNode;
    private MarkdownTextBlock _control;

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyHeading(HeadingBlock headingBlock, MarkdownTextBlock control)
    {
        _headingBlock = headingBlock;
        _paragraph = new Paragraph();
        _control = control;

        SetHProperties(headingBlock.Level);
    }

    public MyHeading(HtmlNode htmlNode, MarkdownTextBlock control)
    {
        _htmlNode = htmlNode;
        _paragraph = new Paragraph();
        _control = control;

        var align = _htmlNode.GetAttribute("align", "left");
        _paragraph.TextAlignment = align switch
        {
            "left" => TextAlignment.Left,
            "right" => TextAlignment.Right,
            "center" => TextAlignment.Center,
            "justify" => TextAlignment.Justify,
            _ => TextAlignment.Left,
        };

        SetHProperties(int.Parse(htmlNode.Name.Substring(1)));
    }

    private void SetHProperties(int level)
    {
        _paragraph.FontSize = level switch
        {
            1 => _control.Config.H1FontSize,
            2 => _control.Config.H2FontSize,
            3 => _control.Config.H3FontSize,
            4 => _control.Config.H4FontSize,
            5 => _control.Config.H5FontSize,
            _ => _control.Config.H6FontSize,
        };
        _paragraph.Foreground = level switch
        {
            1 => _control.Config.H1Foreground,
            2 => _control.Config.H2Foreground,
            3 => _control.Config.H3Foreground,
            4 => _control.Config.H4Foreground,
            5 => _control.Config.H5Foreground,
            _ => _control.Config.H6Foreground,
        };
        _paragraph.FontWeight = level switch
        {
            1 => _control.Config.H1FontWeight,
            2 => _control.Config.H2FontWeight,
            3 => _control.Config.H3FontWeight,
            4 => _control.Config.H4FontWeight,
            5 => _control.Config.H5FontWeight,
            _ => _control.Config.H6FontWeight,
        };
        _paragraph.Margin = level switch
        {
            1 => _control.Config.H1Margin,
            2 => _control.Config.H2Margin,
            3 => _control.Config.H3Margin,
            4 => _control.Config.H4Margin,
            5 => _control.Config.H5Margin,
            _ => _control.Config.H6Margin,
        };
    }

    public void AddChild(IAddChild child)
    {
        if (child.TextElement is Inline inlineChild)
        {
            _paragraph.Inlines.Add(inlineChild);
        }
    }
}
