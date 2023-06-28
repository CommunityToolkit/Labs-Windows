using HtmlAgilityPack;
using Markdig.Syntax;
using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyHeading : IAddChild
{
    private Paragraph _paragraph;
    private HeadingBlock _headingBlock;
    private HtmlNode _htmlNode;

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyHeading(HeadingBlock headingBlock)
    {
        _headingBlock = headingBlock;
        _paragraph = new Paragraph();

        var level = headingBlock.Level;
        _paragraph.FontSize = 24 - (level * 2);
        _paragraph.Foreground = Extensions.GetAccentColorBrush();
        _paragraph.FontWeight = level == 1 ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal;
    }

    public MyHeading(HtmlNode htmlNode)
    {
        _htmlNode = htmlNode;
        _paragraph = new Paragraph();

        var align = _htmlNode.GetAttributeValue("align", "left");
        _paragraph.TextAlignment = align switch
        {
            "left" => Windows.UI.Xaml.TextAlignment.Left,
            "right" => Windows.UI.Xaml.TextAlignment.Right,
            "center" => Windows.UI.Xaml.TextAlignment.Center,
            "justify" => Windows.UI.Xaml.TextAlignment.Justify,
            _ => Windows.UI.Xaml.TextAlignment.Left,
        };

        var level = int.Parse(htmlNode.Name.Substring(1));
        _paragraph.FontSize = 24 - (level * 2);
        _paragraph.Foreground = Extensions.GetAccentColorBrush();
        _paragraph.FontWeight = level == 1 ? Windows.UI.Text.FontWeights.Bold : Windows.UI.Text.FontWeights.Normal;
    }

    public void AddChild(IAddChild child)
    {
        if (child.TextElement is Inline inlineChild)
        {
            _paragraph.Inlines.Add(inlineChild);
        }
    }
}
