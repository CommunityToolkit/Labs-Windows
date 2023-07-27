using Markdig.Syntax.Inlines;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers.Inlines;

internal class CodeInlineRenderer : UWPObjectRenderer<CodeInline>
{
    protected override void Write(WinUIRenderer renderer, CodeInline obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        renderer.WriteInline(new MyInlineCode(obj));
    }
}
