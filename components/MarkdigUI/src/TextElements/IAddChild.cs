using Windows.UI.Xaml.Documents;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

public interface IAddChild
{
    TextElement TextElement { get; }
    void AddChild(IAddChild child);
}
