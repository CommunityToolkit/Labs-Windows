using Markdig.Syntax.Inlines;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class ContainerInlineRenderer : UWPObjectRenderer<ContainerInline>
{
    protected override void Write(UWPRenderer renderer, ContainerInline obj)
    {
        foreach (var inline in obj)
        {
            renderer.Write(inline);
        }
    }
}
