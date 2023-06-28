using Markdig.Syntax;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyParagraph : IAddChild
{
    private ParagraphBlock _paragraphBlock;
    private Paragraph _paragraph;

    public TextElement TextElement
    {
        get => _paragraph;
    }

    public MyParagraph(ParagraphBlock paragraphBlock)
    {
        _paragraphBlock = paragraphBlock;
        _paragraph = new Paragraph();
    }

    public void AddChild(IAddChild child)
    {
        if (child.TextElement is Inline inlineChild)
        {
            _paragraph.Inlines.Add(inlineChild);
        }
        else if (child.TextElement is Windows.UI.Xaml.Documents.Block blockChild)
        {
            var inlineUIContainer = new InlineUIContainer();
            var richTextBlock = new RichTextBlock();
            richTextBlock.TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap;
            richTextBlock.Blocks.Add(blockChild);
            inlineUIContainer.Child = richTextBlock;
            _paragraph.Inlines.Add(inlineUIContainer);
        }
    }
}
