// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls.Renderers;
using Markdig.Syntax;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyParagraph : IAddChild
{
    private readonly WinUIRenderer _renderer;
    private ParagraphBlock _paragraphBlock;
    private Paragraph _paragraph;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyParagraph(ParagraphBlock paragraphBlock, WinUIRenderer renderer)
    {
        _paragraphBlock = paragraphBlock;
        _paragraph = new Paragraph();
        _renderer = renderer;

        // Lists are plain Paragraph_s, one per item.
        // This is so that you can select across list items.
        var themes = renderer.Config.Themes;
        Thickness margin = themes.ParagraphMargin;
        int bulletCount = renderer.GetListBulletCount();
        margin.Left += themes.ListGutterWidth * bulletCount;
        _paragraph.Margin = margin;
        if (themes.ParagraphLineHeight > 0)
        {
            _paragraph.LineHeight = themes.ParagraphLineHeight;
        }

        if (bulletCount != 0)
        {
            string bullet = renderer.PeekListBullet();
            Run bulletRun = new Run { Text = bullet + "\t" };

            _paragraph.Inlines.Add(bulletRun);
            _paragraph.TextIndent = -themes.ListGutterWidth;
        }
    }

    public void AddChild(IAddChild child)
    {
        if (child.TextElement is Inline inlineChild)
        {
            _paragraph.Inlines.Add(inlineChild);
        }
#if !WINAPPSDK
        else if (child.TextElement is Windows.UI.Xaml.Documents.Block blockChild)
#else
        else if (child.TextElement is Microsoft.UI.Xaml.Documents.Block blockChild)
#endif
        {
            var inlineUIContainer = new InlineUIContainer();
            var richTextBlock = new RichTextBlock();
            richTextBlock.TextWrapping = TextWrapping.Wrap;
            richTextBlock.Blocks.Add(blockChild);
            inlineUIContainer.Child = richTextBlock;
            _paragraph.Inlines.Add(inlineUIContainer);
        }
    }
}
