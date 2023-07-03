using HtmlAgilityPack;
using Markdig.Syntax;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

public class MyFlowDocument : IAddChild
{
    private HtmlNode? _htmlNode;
    private RichTextBlock _richTextBlock = new RichTextBlock();
    private MarkdownObject? _markdownObject;

    // useless property
    public TextElement TextElement { get; set; } = new Run();
    //

    public RichTextBlock RichTextBlock
    {
        get => _richTextBlock;
        set => _richTextBlock = value;
    }

    public bool IsHtml => _htmlNode != null;

    public MyFlowDocument()
    {
    }

    public MyFlowDocument(MarkdownObject markdownObject)
    {
        _markdownObject = markdownObject;
    }

    public MyFlowDocument(HtmlNode node)
    {
        _htmlNode = node;
    }

    public void AddChild(IAddChild child)
    {
        TextElement element = child.TextElement;
        if (element != null)
        {
#if !WINAPPSDK
            if (element is Windows.UI.Xaml.Documents.Block block)
#else
            if (element is Microsoft.UI.Xaml.Documents.Block block)
#endif
            {
                _richTextBlock.Blocks.Add(block);
            }
#if !WINAPPSDK
            else if (element is Windows.UI.Xaml.Documents.Inline inline)
#else
            else if (element is Microsoft.UI.Xaml.Documents.Inline inline)
#endif
            {
                var paragraph = new Paragraph();
                paragraph.Inlines.Add(inline);
                _richTextBlock.Blocks.Add(paragraph);
            }
        }
    }
}
