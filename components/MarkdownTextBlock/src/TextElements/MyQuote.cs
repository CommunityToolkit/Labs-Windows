// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Markdig.Syntax;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyQuote : IAddChild
{
    private Paragraph _paragraph;
    private MyFlowDocument _flowDocument;
    private QuoteBlock _quoteBlock;
    private MarkdownTextBlock _control;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyQuote(QuoteBlock quoteBlock, MarkdownTextBlock control)
    {
        _quoteBlock = quoteBlock;
        _control = control;
        _paragraph = new Paragraph();

        _flowDocument = new MyFlowDocument(quoteBlock);
        var inlineUIContainer = new InlineUIContainer();

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

        var bar = new Grid();
        var borderThickness = _control.QuoteBorderThickness.Left > 0 ? _control.QuoteBorderThickness.Left : 4;
        bar.Width = borderThickness;
        bar.Background = _control.QuoteBorderBrush;
        bar.SetValue(Grid.ColumnProperty, 0);
        bar.VerticalAlignment = VerticalAlignment.Stretch;
        bar.Margin = _control.QuoteBarMargin;
        grid.Children.Add(bar);

    var rightGrid = new Grid();
    rightGrid.Padding = _control.QuotePadding;
    rightGrid.Background = _control.QuoteBackground;
    rightGrid.CornerRadius = _control.QuoteCornerRadius;
        rightGrid.Children.Add(_flowDocument.RichTextBlock);
    _flowDocument.RichTextBlock.Foreground = _control.QuoteForeground;

        rightGrid.SetValue(Grid.ColumnProperty, 1);
        grid.Children.Add(rightGrid);
        grid.Margin = _control.QuoteMargin;

        inlineUIContainer.Child = grid;

        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child)
    {
        _flowDocument.AddChild(child);
    }
}
