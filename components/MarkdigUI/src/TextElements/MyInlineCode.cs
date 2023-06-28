using Markdig.Syntax.Inlines;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Media3D;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyInlineCode : IAddChild
{
    private CodeInline _codeInline;
    private InlineUIContainer _inlineContainer;

    public TextElement TextElement
    {
        get => _inlineContainer;
    }

    public MyInlineCode(CodeInline codeInline)
    {
        _codeInline = codeInline;
        _inlineContainer = new InlineUIContainer();
        var border = new Border();
        border.VerticalAlignment = VerticalAlignment.Bottom;
        border.Background = (Brush)Application.Current.Resources["ExpanderHeaderBackground"];
        border.BorderBrush = new SolidColorBrush(Colors.Gray);
        border.BorderThickness = new Thickness(1);
        border.CornerRadius = new CornerRadius(2);
        border.Padding = new Thickness(0);
        CompositeTransform3D transform = new CompositeTransform3D();
        transform.TranslateY = 4;
        border.Transform3D = transform;
        var textBlock = new TextBlock();
        textBlock.FontSize = 10;
        textBlock.Text = codeInline.Content.ToString();
        border.Child = textBlock;
        _inlineContainer.Child = border;
    }


    public void AddChild(IAddChild child) {}
}
