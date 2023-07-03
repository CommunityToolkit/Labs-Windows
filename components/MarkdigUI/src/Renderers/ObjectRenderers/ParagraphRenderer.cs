using Markdig.Syntax;
using CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers;

internal class ParagraphRenderer : UWPObjectRenderer<ParagraphBlock>
{
    protected override void Write(UWPRenderer renderer, ParagraphBlock obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var paragraph = new MyParagraph(obj);
        // set style
        renderer.Push(paragraph);
        renderer.WriteLeafInline(obj);
        renderer.Pop();
    }
}
