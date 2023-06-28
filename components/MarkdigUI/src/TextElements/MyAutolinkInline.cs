using Markdig.Syntax.Inlines;
using System;
using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyAutolinkInline : IAddChild
{
    private AutolinkInline _autoLinkInline;

    public TextElement TextElement { get; private set; }

    public MyAutolinkInline(AutolinkInline autoLinkInline)
    {
        _autoLinkInline = autoLinkInline;
        TextElement = new Hyperlink()
        {
            NavigateUri = new Uri(autoLinkInline.Url),
        };
    }


    public void AddChild(IAddChild child)
    {
        try
        {
            var text = (MyInlineText)child;
            ((Hyperlink)TextElement).Inlines.Add((Run)text.TextElement);
        }
        catch (Exception ex)
        {
            throw new Exception("Error adding child to MyAutolinkInline", ex);
        }
    }
}
