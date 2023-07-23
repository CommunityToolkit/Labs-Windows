using Markdig.Syntax;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers;

internal class CodeBlockRenderer : UWPObjectRenderer<CodeBlock>
{
    protected override void Write(UWPRenderer renderer, CodeBlock obj)
    {
        var code = new MyCodeBlock(obj);
        renderer.Push(code);
        renderer.Pop();
    }
}
