using HtmlAgilityPack;
using Markdig.Syntax.Inlines;
using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyHyperlink : IAddChild
{
    private Hyperlink _hyperlink;
    private LinkInline _linkInline;
    private HtmlNode _htmlNode;
    private string _baseUrl;

    public bool IsHtml => _htmlNode != null;

    public TextElement TextElement
    {
        get => _hyperlink;
    }

    public MyHyperlink(LinkInline linkInline, string baseUrl)
    {
        _baseUrl = baseUrl;
        var url = linkInline.GetDynamicUrl != null ? linkInline.GetDynamicUrl() ?? linkInline.Url : linkInline.Url;
        _linkInline = linkInline;
        _hyperlink = new Hyperlink()
        {
            NavigateUri = Extensions.GetUri(url, baseUrl),
        };
    }

    public MyHyperlink(HtmlNode htmlNode, string baseUrl)
    {
        _baseUrl = baseUrl;
        var url = htmlNode.GetAttributeValue("href", "#");
        _htmlNode = htmlNode;
        _hyperlink = new Hyperlink()
        {
            NavigateUri = Extensions.GetUri(url, baseUrl),
        };
    }

    public void AddChild(IAddChild child)
    {
        if (child.TextElement is Windows.UI.Xaml.Documents.Inline inlineChild)
        {
            try
            {
                _hyperlink.Inlines.Add(inlineChild);
                // TODO: Add support for click handler
            }
            catch { }
        }
    }
}
