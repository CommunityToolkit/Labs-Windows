using Markdig.Syntax.Inlines;
using System;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class LiteralInlineRenderer : UWPObjectRenderer<LiteralInline>
{
    protected override void Write(UWPRenderer renderer, LiteralInline obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        if (obj.Content.IsEmpty)
            return;

        renderer.WriteText(ref obj.Content);
    }
}
