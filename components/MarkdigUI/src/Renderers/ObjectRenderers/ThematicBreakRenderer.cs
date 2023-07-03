using Markdig.Syntax;
using CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers;

internal class ThematicBreakRenderer : UWPObjectRenderer<ThematicBreakBlock>
{
    protected override void Write(UWPRenderer renderer, ThematicBreakBlock obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var thematicBreak = new MyThematicBreak(obj);

        renderer.WriteBlock(thematicBreak);
    }
}
