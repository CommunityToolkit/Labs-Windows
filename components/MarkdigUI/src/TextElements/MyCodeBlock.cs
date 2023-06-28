using ColorCode;
using Markdig.Helpers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyCodeBlock : IAddChild
{
    private CodeBlock _codeBlock;
    private Paragraph _paragraph;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyCodeBlock(CodeBlock codeBlock)
    {
        _codeBlock = codeBlock;
        _paragraph = new Paragraph();
        var container = new InlineUIContainer();
        var border = new Border();
        border.Background = (Brush)Application.Current.Resources["ExpanderHeaderBackground"];
        border.Padding = new Windows.UI.Xaml.Thickness(8);
        border.Margin = new Windows.UI.Xaml.Thickness(4);
        border.CornerRadius = new Windows.UI.Xaml.CornerRadius(4);
        var richTextBlock = new RichTextBlock();

        if (codeBlock is FencedCodeBlock fencedCodeBlock)
        {
            var formatter = new RichTextBlockFormatter(Extensions.GetOneDarkProStyle());

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

            formatter.FormatRichTextBlock(stringBuilder.ToString(), fencedCodeBlock.ToLanguage(), richTextBlock);
        }
        else
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
        }
        border.Child = richTextBlock;
        container.Child = border;
        _paragraph.Inlines.Add(container);
    }

    public void AddChild(IAddChild child) {}
}
