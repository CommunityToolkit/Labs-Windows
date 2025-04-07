// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax.Inlines;
using Windows.UI.Text;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;

internal class MyEmphasisInline : IAddChild, ICascadeChild
{
    private Span _span;
    private EmphasisInline _markdownObject;
    private TextElement _textElementCur;

    private bool _isSuperscript;
    private bool _isSubscript;

    public TextElement TextElement
    {
        get => _span;
    }

    public MyEmphasisInline(EmphasisInline emphasisInline)
    {
        _span = new Span();
        _textElementCur = _span;
        _markdownObject = emphasisInline;
    }

    public void AddChild(IAddChild child)
    {
        try
        {
            if (child is ICascadeChild cascadeChild)
                cascadeChild.InheritProperties(this);

            var inlines = _textElementCur is Span span ? span.Inlines : ((Paragraph)_textElementCur).Inlines;
            if (child is MyInlineText inlineText)
            {
                inlines.Add((Run)inlineText.TextElement);
            }
            else if (child is MyEmphasisInline emphasisInline)
            {
                inlines.Add(emphasisInline._span);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in {nameof(MyEmphasisInline)}.{nameof(AddChild)}: {ex.Message}");
        }
    }

    public void SetBold()
    {
        #if WINUI3
        _span.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
        #elif WINUI2
        _span.FontWeight = Windows.UI.Text.FontWeights.Bold;
        #endif
    }

    public void SetItalic()
    {
        _span.FontStyle = FontStyle.Italic;
    }

    public void SetStrikeThrough()
    {
        #if WINUI3
        _span.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
        #elif WINUI2
        _span.TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough;
        #endif
    }

    public void SetSubscript()
    {
        _isSubscript = true;
        ConstructSubSuperContainer();
    }

    public void SetSuperscript()
    {
        _isSuperscript = true;
        ConstructSubSuperContainer();
    }

    public void InheritProperties(IAddChild parent)
    {
        if (!_isSuperscript && !_isSubscript)
            return;

        _textElementCur.FontFamily = parent.TextElement.FontFamily;
        _textElementCur.FontWeight = parent.TextElement.FontWeight;
        _textElementCur.FontStyle = parent.TextElement.FontStyle;
        _textElementCur.Foreground = parent.TextElement.Foreground;
    }

    private void ConstructSubSuperContainer()
    {
        // usually runs get added directly under _span.Inlines (_span -> Run)
        // for sub/superscript, we use a inline container under _span to translate the Y position of the text
        // (_span -> InlineUIContainer -> RichTextBlock -> Paragraph -> Run)

        var container = new InlineUIContainer();
        var richText = new RichTextBlock();
        var paragraph = new Paragraph
        {
            FontSize = _span.FontSize * 0.8,
        };
        richText.Blocks.Add(paragraph);
        container.Child = richText;

        double offset = _isSuperscript ? -0.4 : 0.16;
        richText.RenderTransform = new TranslateTransform
        {
            Y = _span.FontSize * offset
        };

        _span.Inlines.Add(container);
        _textElementCur = paragraph;
    }
}
