using Markdig.Syntax;
using CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.Renderers.ObjectRenderers;

internal class ListRenderer : UWPObjectRenderer<ListBlock>
{
    protected override void Write(UWPRenderer renderer, ListBlock listBlock)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (listBlock == null) throw new ArgumentNullException(nameof(listBlock));

        var list = new MyList(listBlock);

        renderer.Push(list);

        foreach (var item in listBlock)
        {
            var listItemBlock = (ListItemBlock)item;
            var listItem = new MyBlockContainer(listItemBlock);
            renderer.Push(listItem);
            renderer.WriteChildren(listItemBlock);
            renderer.Pop();
        }

        renderer.Pop();
    }
}
