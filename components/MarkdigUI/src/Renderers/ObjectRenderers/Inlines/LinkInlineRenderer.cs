using Markdig.Syntax.Inlines;
using CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class LinkInlineRenderer : UWPObjectRenderer<LinkInline>
{
    protected override void Write(UWPRenderer renderer, LinkInline link)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (link == null) throw new ArgumentNullException(nameof(link));

        var url = link.GetDynamicUrl != null ? link.GetDynamicUrl() ?? link.Url : link.Url;

        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
            url = "#";
        }

        if (link.IsImage)
        {
            var image = new MyImage(link, CommunityToolkit.Labs.WinUI.MarkdigUI.Extensions.GetUri(url, renderer.Config.BaseUrl), renderer.Config);
            renderer.WriteInline(image);
        }
        else
        {
            if (link.FirstChild is LinkInline linkInlineChild && linkInlineChild.IsImage)
            {
                renderer.Push(new MyHyperlinkButton(link, renderer.Config.BaseUrl));
            }
            else
            {
                var hyperlink = new MyHyperlink(link, renderer.Config.BaseUrl);

                renderer.Push(hyperlink);
            }

            renderer.WriteChildren(link);
            renderer.Pop();
        }
    }
}
