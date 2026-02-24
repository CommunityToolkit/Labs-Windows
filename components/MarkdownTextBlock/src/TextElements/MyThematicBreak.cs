// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Markdig.Syntax;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyThematicBreak : IAddChild
{
    private ThematicBreakBlock _thematicBreakBlock;
    private Paragraph _paragraph;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyThematicBreak(ThematicBreakBlock thematicBreakBlock, MarkdownTextBlock control)
    {
        _thematicBreakBlock = thematicBreakBlock;
        _paragraph = new Paragraph();

        var inlineUIContainer = new InlineUIContainer();
        Line line = new Line
        {
            Stretch = Stretch.Fill,
            Stroke = control.HorizontalRuleBrush,
            X2 = control.HorizontalRuleX2,
            StrokeThickness = control.HorizontalRuleThickness,
            Margin = control.HorizontalRuleMargin
        };
        inlineUIContainer.Child = line;
        _paragraph.Inlines.Add(inlineUIContainer);
    }

    public void AddChild(IAddChild child) {}
}
