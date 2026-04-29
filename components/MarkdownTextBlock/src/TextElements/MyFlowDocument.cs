// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using HtmlAgilityPack;
using Markdig.Syntax;
#if !WINAPPSDK
using Block = Windows.UI.Xaml.Documents.Block;
using Inline = Windows.UI.Xaml.Documents.Inline;
#else
using Block = Microsoft.UI.Xaml.Documents.Block;
using Inline = Microsoft.UI.Xaml.Documents.Inline;
#endif

namespace CommunityToolkit.WinUI.Controls.TextElements;

/// <summary>
/// Represents a flow document that wraps a <see cref="Microsoft.UI.Xaml.Controls.RichTextBlock"/> for rendering markdown or HTML content.
/// </summary>
public class MyFlowDocument : IAddChild
{
    private HtmlNode? _htmlNode;
    private RichTextBlock _richTextBlock = new RichTextBlock();
    private MarkdownObject? _markdownObject;

    /// <summary>Gets or sets the text element (unused, required by <see cref="IAddChild"/>).</summary>
    public TextElement TextElement { get; set; } = new Run();

    /// <summary>Gets or sets the underlying <see cref="Microsoft.UI.Xaml.Controls.RichTextBlock"/>.</summary>
    public RichTextBlock RichTextBlock
    {
        get => _richTextBlock;
        set => _richTextBlock = value;
    }

    /// <summary>Gets a value indicating whether this document was created from an HTML node.</summary>
    public bool IsHtml => _htmlNode != null;

    /// <summary>Initializes a new instance of the <see cref="MyFlowDocument"/> class.</summary>
    public MyFlowDocument()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="MyFlowDocument"/> class from a markdown object.</summary>
    public MyFlowDocument(MarkdownObject markdownObject)
    {
        _markdownObject = markdownObject;
    }

    /// <summary>Initializes a new instance of the <see cref="MyFlowDocument"/> class from an HTML node.</summary>
    public MyFlowDocument(HtmlNode node)
    {
        _htmlNode = node;
    }

    /// <summary>Adds a child element as a block or inline to the document.</summary>
    public void AddChild(IAddChild child)
    {
        TextElement element = child.TextElement;
        if (element != null)
        {
            if (element is Block block)
            {
                _richTextBlock.Blocks.Add(block);
            }
            else if (element is Inline inline)
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(inline);
                _richTextBlock.Blocks.Add(paragraph);
            }
        }
    }
}
