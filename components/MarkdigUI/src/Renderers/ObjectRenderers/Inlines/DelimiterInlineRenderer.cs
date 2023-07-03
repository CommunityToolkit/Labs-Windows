using Markdig.Syntax.Inlines;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class DelimiterInlineRenderer : UWPObjectRenderer<DelimiterInline>
{
    protected override void Write(UWPRenderer renderer, DelimiterInline obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        // delimiter's children are emphasized text, we don't need to explicitly render them
        // Just need to render the children of the delimiter, I think..
        //renderer.WriteText(obj.ToLiteral());
        renderer.WriteChildren(obj);
    }
}
