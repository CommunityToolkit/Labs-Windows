using Markdig.Syntax;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers;

internal class CodeBlockRenderer : UWPObjectRenderer<CodeBlock>
{
    protected override void Write(WinUIRenderer renderer, CodeBlock obj)
    {
        var code = new MyCodeBlock(obj, renderer.Config);
        renderer.Push(code);
        renderer.Pop();
    }
}
