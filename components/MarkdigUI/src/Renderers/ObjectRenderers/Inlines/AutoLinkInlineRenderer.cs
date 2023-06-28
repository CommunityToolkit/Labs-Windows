using Markdig.Syntax.Inlines;
using CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;
using System;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class AutoLinkInlineRenderer : UWPObjectRenderer<AutolinkInline>
{
    protected override void Write(UWPRenderer renderer, AutolinkInline link)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (link == null) throw new ArgumentNullException(nameof(link));

        var url = link.Url;
        if (link.IsEmail)
        {
            url = "mailto:" + url;
        }

        if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
        {
            url = "#";
        }

        var autolink = new MyAutolinkInline(link);

        renderer.Push(autolink);

        renderer.WriteText(link.Url);
        renderer.Pop();
    }
}
