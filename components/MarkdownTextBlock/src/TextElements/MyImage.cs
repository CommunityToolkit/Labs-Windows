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
        _container.Child = _image;
    }

    private async void LoadImage(object sender, RoutedEventArgs e)
    {
        if (_loaded) return;
        try
        {
            // Track whether we have valid natural dimensions to constrain against
            bool hasNaturalWidth = false;
            bool hasNaturalHeight = false;

            if (_imageProvider != null && _imageProvider.ShouldUseThisProvider(_uri.AbsoluteUri))
            {
                _image = await _imageProvider.GetImage(_uri.AbsoluteUri);
                _container.Child = _image;
                
                // Capture natural dimensions as max constraints from the provider image
                // Then clear fixed Width/Height so images can shrink responsively
                if (_image.Width > 0 && !double.IsNaN(_image.Width) && !double.IsInfinity(_image.Width))
                {
                    _image.MaxWidth = _image.Width;
                    _image.Width = double.NaN; // Clear fixed width to allow shrinking
                    hasNaturalWidth = true;
                }
                if (_image.Height > 0 && !double.IsNaN(_image.Height) && !double.IsInfinity(_image.Height))
                {
                    _image.MaxHeight = _image.Height;
                    _image.Height = double.NaN; // Clear fixed height to allow shrinking
                    hasNaturalHeight = true;
                }
                
                _loaded = true;
            }
            else if (_uri.Scheme == "data")
            {
                // Handle data URLs (e.g., data:image/png;base64,...)
                await LoadDataUrlImageAsync();
            }
            else if (_uri.Scheme == "file")
            {
                // Handle file URLs (e.g., file:///C:/path/to/image.png)
                await LoadFileUrlImageAsync();
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
                    // Don't set fixed Width/Height - let layout system handle it
                    // Store natural dimensions for MaxWidth/MaxHeight constraints
                    double naturalWidth = bitmap.PixelWidth == 0 ? bitmap.DecodePixelWidth : bitmap.PixelWidth;
                    double naturalHeight = bitmap.PixelHeight == 0 ? bitmap.DecodePixelHeight : bitmap.PixelHeight;

                    // Use natural size as max constraint so image doesn't upscale
                    if (naturalWidth > 0)
                    {
                        _image.MaxWidth = naturalWidth;
                        hasNaturalWidth = true;
                    }
                    if (naturalHeight > 0)
                    {
                        _image.MaxHeight = naturalHeight;
                        hasNaturalHeight = true;
                    }
                }

                _loaded = true;
            }

            // Apply precedent (markdown-specified) dimensions if provided
            // Precedent always takes priority and sets a known dimension
            if (_precedentWidth != 0)
            {
                _image.MaxWidth = _precedentWidth;
                hasNaturalWidth = true;
            }
            if (_precedentHeight != 0)
            {
                _image.MaxHeight = _precedentHeight;
                hasNaturalHeight = true;
            }

            // Apply theme constraints - only if we have a known dimension to constrain
            // This prevents theme constraints from enlarging images with unknown natural size
            if (_themes.ImageMaxWidth > 0 && hasNaturalWidth && _themes.ImageMaxWidth < _image.MaxWidth)
            {
                _image.MaxWidth = _themes.ImageMaxWidth;
            }
            if (_themes.ImageMaxHeight > 0 && hasNaturalHeight && _themes.ImageMaxHeight < _image.MaxHeight)
            {
                _image.MaxHeight = _themes.ImageMaxHeight;
            }
            _image.Stretch = _themes.ImageStretch;
        }
        catch (Exception) { }
    }

    private async Task LoadDataUrlImageAsync()
    {
        try
        {
            var uriString = _uri.AbsoluteUri;

            // Parse data URL format: data:[mediatype][;base64],data
            if (!uriString.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return;

            var commaIndex = uriString.IndexOf(',');
            if (commaIndex == -1)
                return;

            var header = uriString.Substring(5, commaIndex - 5); // Skip "data:"
            var data = uriString.Substring(commaIndex + 1);

            // Check if it's base64 encoded
            bool isBase64 = header.Contains("base64", StringComparison.OrdinalIgnoreCase);

            byte[] imageBytes;
            if (isBase64)
            {
                imageBytes = Convert.FromBase64String(data);
            }
            else
            {
                // Handle URL-encoded data (less common)
                var decodedData = Uri.UnescapeDataString(data);
                imageBytes = System.Text.Encoding.UTF8.GetBytes(decodedData);
            }

            // Create BitmapImage from bytes
            BitmapImage bitmap = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(imageBytes.AsBuffer());
                stream.Seek(0);
                await bitmap.SetSourceAsync(stream);
            }

            _image.Source = bitmap;
            _image.Width = bitmap.PixelWidth == 0 ? bitmap.DecodePixelWidth : bitmap.PixelWidth;
            _image.Height = bitmap.PixelHeight == 0 ? bitmap.DecodePixelHeight : bitmap.PixelHeight;

            _loaded = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load data URL image: {ex.Message}");
        }
    }

    private async Task LoadFileUrlImageAsync()
    {
        try
        {
            var filePath = _uri.LocalPath;

            // Check if file exists
            if (!File.Exists(filePath))
            {
                System.Diagnostics.Debug.WriteLine($"File not found: {filePath}");
                return;
            }

            // Read file as bytes
            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);

            // Create BitmapImage from bytes
            BitmapImage bitmap = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(imageBytes.AsBuffer());
                stream.Seek(0);
                await bitmap.SetSourceAsync(stream);
            }

            _image.Source = bitmap;
            _image.Width = bitmap.PixelWidth == 0 ? bitmap.DecodePixelWidth : bitmap.PixelWidth;
            _image.Height = bitmap.PixelHeight == 0 ? bitmap.DecodePixelHeight : bitmap.PixelHeight;

            _loaded = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load file URL image: {ex.Message}");
        }
    }

    public void AddChild(IAddChild child) { }
}
