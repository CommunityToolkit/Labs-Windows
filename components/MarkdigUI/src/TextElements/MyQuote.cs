using Markdig.Syntax;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyQuote : IAddChild
{
    private Paragraph _paragraph;
    private MyFlowDocument _flowDocument;
    private QuoteBlock _quoteBlock;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyQuote(QuoteBlock quoteBlock)
    {
        _quoteBlock = quoteBlock;
        _paragraph = new Paragraph();

        _flowDocument = new MyFlowDocument(quoteBlock);
        var inlineUIContainer = new InlineUIContainer();

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Auto) });

        var bar = new Grid();
        bar.Width = 4;
        bar.Background = new SolidColorBrush(Colors.Gray);
        bar.SetValue(Grid.ColumnProperty, 0);
        bar.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
        bar.Margin = new Windows.UI.Xaml.Thickness(0, 0, 4, 0);
        grid.Children.Add(bar);

        var rightGrid = new Grid();
        rightGrid.Padding = new Windows.UI.Xaml.Thickness(4);
        rightGrid.Children.Add(_flowDocument.RichTextBlock);

        rightGrid.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(rightGrid);
        grid.Margin = new Windows.UI.Xaml.Thickness(0, 2, 0, 2);

        inlineUIContainer.Child = grid;

        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child)
    {
        _flowDocument.AddChild(child);
    }
}
