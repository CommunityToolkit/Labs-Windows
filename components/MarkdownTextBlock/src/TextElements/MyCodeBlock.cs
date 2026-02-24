// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Markdig.Syntax;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyCodeBlock : IAddChild
{
    private CodeBlock _codeBlock;
    private Paragraph _paragraph;
    private MarkdownTextBlock _control;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyCodeBlock(CodeBlock codeBlock, MarkdownTextBlock control)
    {
        _codeBlock = codeBlock;
        _control = control;
        _paragraph = new Paragraph();
        var container = new InlineUIContainer();
        var border = new Border();
    border.Background = _control.CodeBlockBackground;
    border.BorderBrush = _control.CodeBlockBorderBrush;
    border.BorderThickness = _control.CodeBlockBorderThickness;
    border.Padding = _control.CodeBlockPadding;
    border.Margin = _control.CodeBlockMargin;
    border.CornerRadius = _control.CodeBlockCornerRadius;
        var richTextBlock = new RichTextBlock();
    richTextBlock.FontFamily = _control.CodeBlockFontFamily;
    richTextBlock.Foreground = _control.CodeBlockForeground;

#if false
        if (codeBlock is FencedCodeBlock fencedCodeBlock)
        {
//#if !WINAPPSDK
//            var formatter = new ColorCode.RichTextBlockFormatter(Extensions.GetOneDarkProStyle());
//#else
//            var formatter = new ColorCode.RichTextBlockFormatter(Extensions.GetOneDarkProStyle());
//#endif
            var stringBuilder = new StringBuilder();

            // go through all the lines backwards and only add the lines to a stack if we have encountered the first non-empty line
            var lines = fencedCodeBlock.Lines.Lines;
            var stack = new Stack<string>();
            var encounteredFirstNonEmptyLine = false;
            if (lines != null)
            {
                for (var i = lines.Length - 1; i >= 0; i--)
                {
                    var line = lines[i];
                    if (String.IsNullOrWhiteSpace(line.ToString()) && !encounteredFirstNonEmptyLine)
                    {
                        continue;
                    }

                    encounteredFirstNonEmptyLine = true;
                    stack.Push(line.ToString());
                }

                // append all the lines in the stack to the string builder
                while (stack.Count > 0)
                {
                    stringBuilder.AppendLine(stack.Pop());
                }
            }

            //formatter.FormatRichTextBlock(stringBuilder.ToString(), fencedCodeBlock.ToLanguage(), richTextBlock);
        }
        else
        {
#endif

        if (codeBlock.Lines.Lines != null)
        {
            foreach (var line in codeBlock.Lines.Lines)
            {
                var paragraph = new Paragraph();
                var lineString = line.ToString();
                if (!String.IsNullOrWhiteSpace(lineString))
                {
                    paragraph.Inlines.Add(new Run() { Text = lineString });
                }
                richTextBlock.Blocks.Add(paragraph);
            }

            border.Child = richTextBlock;
            container.Child = border;
            _paragraph.Inlines.Add(container);
        }
    }

    public void AddChild(IAddChild child) {}
}
