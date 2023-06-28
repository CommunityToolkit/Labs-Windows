using HtmlAgilityPack;
using Microsoft.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements.Html;

// block
internal class MyDetails : IAddChild
{
    private HtmlNode _htmlNode;
    private InlineUIContainer _inlineUIContainer;
    private Expander _expander;
    private MyFlowDocument _flowDocument;
    private Paragraph _paragraph;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyDetails(HtmlNode details)
    {
        _htmlNode = details;

        var header = _htmlNode.ChildNodes
            .FirstOrDefault(
                x => x.Name == "summary" ||
                x.Name == "header");

        _inlineUIContainer = new InlineUIContainer();
        _expander = new Expander();
        _expander.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
        _flowDocument = new MyFlowDocument(details);
        _flowDocument.RichTextBlock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
        _expander.Content = _flowDocument.RichTextBlock;
        var headerBlock = new TextBlock()
        {
            Text = header?.InnerText
        };
        headerBlock.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
        _expander.Header = headerBlock;
        _inlineUIContainer.Child = _expander;
        _paragraph = new Paragraph();
        _paragraph.Inlines.Add(_inlineUIContainer);
    }

    public void AddChild(IAddChild child)
    {
        _flowDocument.AddChild(child);
    }
}
