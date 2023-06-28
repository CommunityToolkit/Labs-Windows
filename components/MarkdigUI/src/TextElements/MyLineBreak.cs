using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyLineBreak : IAddChild
{
    private LineBreak _lineBreak;

    public TextElement TextElement
    {
        get => _lineBreak;
    }

    public MyLineBreak()
    {
        _lineBreak = new LineBreak();
    }

    public void AddChild(IAddChild child) {}
}
