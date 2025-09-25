// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Markdig.Extensions.Tables;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyTable : IAddChild
{
    private Table _table;
    private Paragraph _paragraph;
    private MyTableUIElement _tableElement;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyTable(Table table, MarkdownThemes themes)
    {
        _table = table;
        _paragraph = new Paragraph();
        var row = table.FirstOrDefault() as TableRow;
        var column = row == null ? 0 : row.Count;

        _tableElement = new MyTableUIElement
        (
            column,
            table.Count,
            borderThickness: themes.TableBorderThickness,
            themes.TableBorderBrush ?? themes.BorderBrush,
            themes.TableHeadingBackground,
            themes.CornerRadius,
            themes.TableMargin
        );

        var inlineUIContainer = new InlineUIContainer();
        inlineUIContainer.Child = _tableElement;
        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child)
    {
        if (child is MyTableCell cellChild)
        {
            var cell = cellChild.Container;

            Grid.SetColumn(cell, cellChild.ColumnIndex);
            Grid.SetRow(cell, cellChild.RowIndex);
            Grid.SetColumnSpan(cell, cellChild.ColumnSpan);
            Grid.SetRowSpan(cell, cellChild.RowSpan);

            _tableElement.Children.Add(cell);
        }
    }
}
