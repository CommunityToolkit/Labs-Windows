namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

public interface ISVGRenderer
{
    Task<Image> SvgToImage(string svgString);
}
