using HtmlAgilityPack;
using Markdig.Syntax.Inlines;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class HtmlInlineRenderer : UWPObjectRenderer<HtmlInline>
{
    protected override void Write(UWPRenderer renderer, HtmlInline obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var html = obj.Tag;
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        HtmlWriter.WriteHtml(renderer, doc.DocumentNode.ChildNodes);
    }
}
