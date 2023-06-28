using Markdig.Renderers;
using Markdig.Syntax;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers;

public abstract class UWPObjectRenderer<TObject> : MarkdownObjectRenderer<UWPRenderer, TObject>
    where TObject : MarkdownObject
{
}
