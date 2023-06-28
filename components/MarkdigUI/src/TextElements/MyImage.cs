using Markdig.Syntax.Inlines;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using HtmlAgilityPack;
namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyImage : IAddChild
{
    private InlineUIContainer _container;
    private LinkInline _linkInline;
    private Windows.UI.Xaml.Controls.Image _image;
    private Uri _uri;
    private HtmlNode _htmlNode;
    private IImageProvider _imageProvider;
    private ISVGRenderer _svgRenderer;
    private double _precedentWidth;
    private double _precedentHeight;
    private bool _loaded;

    public TextElement TextElement
    {
        get => _container;
    }

    public MyImage(LinkInline linkInline, Uri uri, MarkdownConfig config)
    {
        _linkInline = linkInline;
        _uri = uri;
        _imageProvider = config.ImageProvider;
        _svgRenderer = config.SVGRenderer == null ? new DefaultSVGRenderer() : config.SVGRenderer;
        Init();
        var size = Extensions.GetMarkdownImageSize(linkInline);
        if (size.Width != 0)
        {
            _precedentWidth = size.Width;
        }
        if (size.Height != 0)
        {
            _precedentHeight = size.Height;
        }
    }

    public MyImage(HtmlNode htmlNode, MarkdownConfig config)
    {
        Uri.TryCreate(htmlNode.GetAttributeValue("src", "#"), UriKind.RelativeOrAbsolute, out _uri);
        _htmlNode = htmlNode;
        _imageProvider = config.ImageProvider;
        _svgRenderer = config.SVGRenderer == null ? new DefaultSVGRenderer() : config.SVGRenderer;
        Init();
        int.TryParse(
            htmlNode.GetAttributeValue("width", "0"),
            System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture,
            out var width
        );
        int.TryParse(
            htmlNode.GetAttributeValue("height", "0"),
            System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture,
            out var height
        );
        if (width > 0)
        {
            _precedentWidth = width;
        }
        if (height > 0)
        {
            _precedentHeight = height;
        }
    }

    private void Init()
    {
        _container = new InlineUIContainer();
        _image = new Windows.UI.Xaml.Controls.Image();

        _image.Loaded += LoadImage;

        _container.Child = _image;
    }

    private async void LoadImage(object sender, RoutedEventArgs e)
    {
        if (_loaded) return;
        try
        {
            if (_imageProvider != null && _imageProvider.ShouldUseThisProvider(_uri.AbsoluteUri))
            {
                _image = await _imageProvider.GetImage(_uri.AbsoluteUri);
                _container.Child = _image;
            }
            else
            {
                HttpClient client = new HttpClient();

                // Download data from URL
                HttpResponseMessage response = await client.GetAsync(_uri);


                // Get the Content-Type header
                string contentType = response.Content.Headers.ContentType.MediaType;

                if (contentType == "image/svg+xml")
                {
                    var svgString = await response.Content.ReadAsStringAsync();
                    var resImage = await _svgRenderer.SvgToImage(svgString);
                    if (resImage != null)
                    {
                        _image = resImage;
                        _container.Child = _image;
                    }
                }
                else
                {
                    byte[] data = await response.Content.ReadAsByteArrayAsync();
                    // Create a BitmapImage for other supported formats
                    BitmapImage bitmap = new BitmapImage();
                    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                    {
                        // Write the data to the stream
                        await stream.WriteAsync(data.AsBuffer());
                        stream.Seek(0);

                        // Set the source of the BitmapImage
                        await bitmap.SetSourceAsync(stream);
                    }
                    _image.Source = bitmap;
                }

                _loaded = true;
            }

            if (_precedentWidth != 0)
            {
                _image.Width = _precedentWidth;
            }
            if (_precedentHeight != 0)
            {
                _image.Height = _precedentHeight;
            }
        }
        catch (Exception) { }
    }

    public void AddChild(IAddChild child) {}
}
