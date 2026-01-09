// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax.Inlines;
using HtmlAgilityPack;
using System.Globalization;
using Windows.Storage.Streams;

namespace CommunityToolkit.WinUI.Controls.TextElements;

internal class MyImage : IAddChild
{
    private InlineUIContainer _container = new InlineUIContainer();
    private Border _border = new Border();
    private LinkInline? _linkInline;
    private Image _image = new Image();
    private Uri _uri;
    private HtmlNode? _htmlNode;
    private IImageProvider? _imageProvider;
    private ISVGRenderer _svgRenderer;
    private MarkdownThemes _themes;
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
        _themes = config.Themes;
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MyImage(HtmlNode htmlNode, MarkdownConfig? config)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
#pragma warning disable CS8601 // Possible null reference assignment.
        Uri.TryCreate(htmlNode.GetAttribute("src", "#"), UriKind.RelativeOrAbsolute, out _uri);
#pragma warning restore CS8601 // Possible null reference assignment.
        _htmlNode = htmlNode;
        _imageProvider = config?.ImageProvider;
        _svgRenderer = config?.SVGRenderer == null ? new DefaultSVGRenderer() : config.SVGRenderer;
        _themes = config?.Themes ?? MarkdownThemes.Default;
        Init();
        int.TryParse(
            htmlNode.GetAttribute("width", "0"),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var width
        );
        int.TryParse(
            htmlNode.GetAttribute("height", "0"),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
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
        _image.Loaded += LoadImage;
        _border.Child = _image;
        _container.Child = _border;
    }

    private async void LoadImage(object sender, RoutedEventArgs e)
    {
        if (_loaded) return;
        try
        {
            if (_imageProvider != null && _imageProvider.ShouldUseThisProvider(_uri.AbsoluteUri))
            {
                _image = await _imageProvider.GetImage(_uri.AbsoluteUri);
                _border.Child = _image;
            }
            else
            {
                HttpClient client = new HttpClient();

                // Download data from URL
                HttpResponseMessage response = await client.GetAsync(_uri);


                // Get the Content-Type header
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string contentType = response.Content.Headers.ContentType.MediaType;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                if (contentType == "image/svg+xml")
                {
                    var svgString = await response.Content.ReadAsStringAsync();
                    var resImage = await _svgRenderer.SvgToImage(svgString);
                    if (resImage != null)
                    {
                        _image = resImage;
                        _border.Child = _image;
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
                    _image.Width = bitmap.PixelWidth == 0 ? bitmap.DecodePixelWidth : bitmap.PixelWidth;
                    _image.Height = bitmap.PixelHeight == 0 ? bitmap.DecodePixelHeight : bitmap.PixelHeight;

                }

                _loaded = true;
            }

            // Determine the actual image dimensions
            double actualWidth = _precedentWidth != 0 ? _precedentWidth : _image.Width;
            double actualHeight = _precedentHeight != 0 ? _precedentHeight : _image.Height;

            // Apply max constraints and calculate the final size
            // When using Uniform stretch with max constraints, we need to calculate
            // the actual rendered size to avoid gaps
            double finalWidth = actualWidth;
            double finalHeight = actualHeight;

            bool hasMaxWidth = _themes.ImageMaxWidth > 0;
            bool hasMaxHeight = _themes.ImageMaxHeight > 0;

            if (hasMaxWidth || hasMaxHeight)
            {
                double scaleX = hasMaxWidth && actualWidth > _themes.ImageMaxWidth 
                    ? _themes.ImageMaxWidth / actualWidth 
                    : 1.0;
                double scaleY = hasMaxHeight && actualHeight > _themes.ImageMaxHeight 
                    ? _themes.ImageMaxHeight / actualHeight 
                    : 1.0;

                // For Uniform stretch, use the smaller scale to maintain aspect ratio
                if (_themes.ImageStretch == Stretch.Uniform || _themes.ImageStretch == Stretch.UniformToFill)
                {
                    double uniformScale = Math.Min(scaleX, scaleY);
                    finalWidth = actualWidth * uniformScale;
                    finalHeight = actualHeight * uniformScale;
                }
                else
                {
                    // For other stretch modes, apply constraints independently
                    finalWidth = actualWidth * scaleX;
                    finalHeight = actualHeight * scaleY;
                }
            }

            _image.Width = finalWidth;
            _image.Height = finalHeight;
            _image.Stretch = _themes.ImageStretch;

            // Apply corner radius via the border
            _border.CornerRadius = _themes.ImageCornerRadius;
        }
        catch (Exception) { }
    }

    public void AddChild(IAddChild child) {}
}
