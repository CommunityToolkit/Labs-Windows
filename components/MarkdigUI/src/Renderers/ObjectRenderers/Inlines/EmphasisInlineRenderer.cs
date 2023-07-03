using Markdig.Syntax.Inlines;
using CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers.Inlines;

internal class EmphasisInlineRenderer : UWPObjectRenderer<EmphasisInline>
{
    protected override void Write(UWPRenderer renderer, EmphasisInline obj)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        MyEmphasisInline? span = null;

        switch (obj.DelimiterChar)
        {
            case '*':
            case '_':
                span = new MyEmphasisInline(obj);
                if (obj.DelimiterCount == 2) { span.SetBold(); } else { span.SetItalic(); }
                break;
            case '~':
                span = new MyEmphasisInline(obj);
                if (obj.DelimiterCount == 2) { span.SetStrikeThrough(); } else { span.SetSubscript(); }
                break;
            case '^':
                span = new MyEmphasisInline(obj);
                span.SetSuperscript();
                break;
        }

        if (span != null)
        {
            renderer.Push(span);
            renderer.WriteChildren(obj);
            renderer.Pop();
        }
        else
        {
            renderer.WriteChildren(obj);
        }
    }
}
