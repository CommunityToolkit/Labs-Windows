namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyInlineText : IAddChild
{
    private Run _run;

    public TextElement TextElement
    {
        get => _run;
    }

    public MyInlineText(string text)
    {
        _run = new Run()
        {
            Text = text
        };
    }

    public void AddChild(IAddChild child) {}
}
