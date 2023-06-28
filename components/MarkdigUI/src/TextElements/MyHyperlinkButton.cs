using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyHyperlinkButton : IAddChild
{
    private HyperlinkButton _hyperLinkButton;
    private InlineUIContainer _inlineUIContainer;
    private MyFlowDocument _flowDoc;
    private string _baseUrl;
    private LinkInline _linkInline;
    private HtmlNode _htmlNode;

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement
    {
        get => _inlineUIContainer;
    }

    public MyHyperlinkButton(LinkInline linkInline, string baseUrl)
    {
        _baseUrl = baseUrl;
        var url = linkInline.GetDynamicUrl != null ? linkInline.GetDynamicUrl() ?? linkInline.Url : linkInline.Url;
        _linkInline = linkInline;
        Init(url, baseUrl);
    }

    public MyHyperlinkButton(HtmlNode htmlNode, string baseUrl)
    {
        _baseUrl = baseUrl;
        _htmlNode = htmlNode;
        var url = htmlNode.GetAttributeValue("href", "#");
        Init(url, baseUrl);
    }

    private void Init(string url, string baseUrl)
    {
        _hyperLinkButton = new HyperlinkButton()
        {
            NavigateUri = Extensions.GetUri(url, baseUrl),
        };
        _hyperLinkButton.Padding = new Windows.UI.Xaml.Thickness(0);
        _hyperLinkButton.Margin = new Windows.UI.Xaml.Thickness(0);
        _inlineUIContainer = new InlineUIContainer();
        if (IsHtml)
        {
            _flowDoc = new MyFlowDocument(_htmlNode);
        }
        else
        {
            _flowDoc = new MyFlowDocument(_linkInline);
        }
        _inlineUIContainer.Child = _hyperLinkButton;
        _hyperLinkButton.Content = _flowDoc.RichTextBlock;
    }

    public void AddChild(IAddChild child)
    {
        _flowDoc.AddChild(child);
    }
}
