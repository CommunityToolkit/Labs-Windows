using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

public interface IImageProvider
{
    Task<Image> GetImage(string url);
    bool ShouldUseThisProvider(string url);
}
