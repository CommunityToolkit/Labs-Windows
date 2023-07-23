using Markdig.Renderers;
using Markdig.Syntax;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers;

public abstract class UWPObjectRenderer<TObject> : MarkdownObjectRenderer<UWPRenderer, TObject>
    where TObject : MarkdownObject
{
}
