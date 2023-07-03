using Markdig.Syntax.Inlines;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class HtmlEntityInlineRenderer : UWPObjectRenderer<HtmlEntityInline>
{
    protected override void Write(UWPRenderer renderer, HtmlEntityInline obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var transcoded = obj.Transcoded;
        renderer.WriteText(ref transcoded);
        // todo: wtf is this?
    }
}
