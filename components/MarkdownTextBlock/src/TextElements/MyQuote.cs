// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyQuote : IAddChild
{
    private Paragraph _paragraph;
    private MyFlowDocument _flowDocument;
    private QuoteBlock _quoteBlock;
    private MarkdownThemes _themes;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyQuote(QuoteBlock quoteBlock, MarkdownThemes themes)
    {
        _quoteBlock = quoteBlock;
        _themes = themes;
        _paragraph = new Paragraph();

        _flowDocument = new MyFlowDocument(quoteBlock);
        var inlineUIContainer = new InlineUIContainer();

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

        var bar = new Grid();
        var borderThickness = _themes.QuoteBorderThickness.Left > 0 ? _themes.QuoteBorderThickness.Left : 4;
        bar.Width = borderThickness;
        bar.Background = _themes.QuoteBorderBrush ?? new SolidColorBrush(Colors.Gray);
        bar.SetValue(Grid.ColumnProperty, 0);
        bar.VerticalAlignment = VerticalAlignment.Stretch;
        bar.Margin = new Thickness(0, 0, 4, 0);
        grid.Children.Add(bar);

    var rightGrid = new Grid();
    rightGrid.Padding = _themes.QuotePadding;
    rightGrid.Background = _themes.QuoteBackground;
    rightGrid.CornerRadius = _themes.QuoteCornerRadius;
        rightGrid.Children.Add(_flowDocument.RichTextBlock);
    _flowDocument.RichTextBlock.Foreground = _themes.QuoteForeground;

        rightGrid.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(rightGrid);
        grid.Margin = _themes.QuoteMargin;

        inlineUIContainer.Child = grid;

        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child)
    {
        _flowDocument.AddChild(child);
    }
}
