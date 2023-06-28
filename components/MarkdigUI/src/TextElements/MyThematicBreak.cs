using Markdig.Syntax;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyThematicBreak : IAddChild
{
    private ThematicBreakBlock _thematicBreakBlock;
    private Paragraph _paragraph;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyThematicBreak(ThematicBreakBlock thematicBreakBlock)
    {
        _thematicBreakBlock = thematicBreakBlock;
        _paragraph = new Paragraph();

        var inlineUIContainer = new InlineUIContainer();
        var border = new Border();
        border.Width = 500;
        border.BorderThickness = new Thickness(1);
        border.Margin = new Thickness(0, 4, 0, 4);
        border.BorderBrush = new SolidColorBrush(Colors.White);
        border.Height = 1;
        border.HorizontalAlignment = HorizontalAlignment.Stretch;
        inlineUIContainer.Child = border;
        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child) {}
}
