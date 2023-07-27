using Markdig.Syntax.Inlines;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers.Inlines;

internal class ContainerInlineRenderer : UWPObjectRenderer<ContainerInline>
{
    protected override void Write(WinUIRenderer renderer, ContainerInline obj)
    {
        foreach (var inline in obj)
        {
            renderer.Write(inline);
        }
    }
}
