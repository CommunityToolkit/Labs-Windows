using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

public interface ISVGRenderer
{
    Task<Image> SvgToImage(string svgString);
}
