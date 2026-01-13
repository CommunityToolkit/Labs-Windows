// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Windows.Storage.Streams;

#if WINAPPSDK
using Microsoft.UI.Xaml.Media.Imaging;
using Colors = Microsoft.UI.Colors;
#elif WINDOWS_UWP
using Windows.UI.Xaml.Media.Imaging;
using Colors = Windows.UI.Colors;
#endif


namespace MarkdownTextBlockExperiment.Samples;

/// <summary>
/// Sample demonstrating IImageProvider constraint behavior for manual testing.
/// This helps verify the fix for: https://github.com/CommunityToolkit/Labs-Windows/pull/771
/// 
/// Bug description:
/// When using IImageProvider, if natural dimensions weren't set on the image, 
/// theme constraints would incorrectly be applied, potentially enlarging small images.
/// </summary>
[ToolkitSample(id: nameof(MarkdownTextBlockImageProviderSample), "Image Provider Constraints", 
    description: "Manual test for IImageProvider constraint behavior - verifies small images aren't enlarged")]
public sealed partial class MarkdownTextBlockImageProviderSample : Page
{
    public MarkdownTextBlockImageProviderSample()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await ApplyConfigurationAsync();
    }

    private async void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        await ApplyConfigurationAsync();
    }

    private async Task ApplyConfigurationAsync()
    {
        var providerWidth = ProviderWidthBox.Value;
        var providerHeight = ProviderHeightBox.Value;
        var themeMaxWidth = ThemeMaxWidthBox.Value;
        var themeMaxHeight = ThemeMaxHeightBox.Value;

        // Test Case 1: Small image with larger theme constraint
        // The image should NOT be enlarged
        var provider1 = new TestImageProvider(providerWidth, providerHeight);
        var config1 = new MarkdownConfig
        {
            ImageProvider = provider1,
            Themes = new MarkdownThemes
            {
                ImageMaxWidth = themeMaxWidth,
                ImageMaxHeight = themeMaxHeight
            }
        };
        TestCase1.Config = config1;
        TestCase1.Text = "";
        TestCase1.Text = "![test](provider://test1.png)";

        // Test Case 2: Large image with smaller theme constraint
        // The image SHOULD be constrained
        var provider2 = new TestImageProvider(800, 600);
        var config2 = new MarkdownConfig
        {
            ImageProvider = provider2,
            Themes = new MarkdownThemes
            {
                ImageMaxWidth = 400,
                ImageMaxHeight = 400
            }
        };
        TestCase2.Config = config2;
        TestCase2.Text = "";
        TestCase2.Text = "![test](provider://test2.png)";

        // Test Case 3: Image with no dimensions from provider
        // Theme constraints should NOT be applied
        var provider3 = new TestImageProvider(0, 0);
        var config3 = new MarkdownConfig
        {
            ImageProvider = provider3,
            Themes = new MarkdownThemes
            {
                ImageMaxWidth = themeMaxWidth,
                ImageMaxHeight = themeMaxHeight
            }
        };
        TestCase3.Config = config3;
        TestCase3.Text = "";
        TestCase3.Text = "![test](provider://test3-nodims.png)";

        // Give time for images to load, then check results

        await Task.Delay(1000);
        UpdateResults();
    }

    private void UpdateResults()
    {
        var image1 = FindImage(TestCase1);
        var image2 = FindImage(TestCase2);
        var image3 = FindImage(TestCase3);

        var providerWidth = ProviderWidthBox.Value;
        var themeMaxWidth = ThemeMaxWidthBox.Value;

        // Test Case 1: Small image should stay small
        if (image1 != null)
        {
            var pass = Math.Abs(image1.MaxWidth - providerWidth) < 1;
            TestCase1Result.Text = $"MaxWidth: {image1.MaxWidth:F0} (Expected: {providerWidth}) - {(pass ? "✅ PASS" : "❌ FAIL - Image incorrectly enlarged!")}";
            TestCase1Result.Foreground = new SolidColorBrush(pass ? Colors.Green : Colors.Red);
        }
        else
        {
            TestCase1Result.Text = "❌ Image not found";
            TestCase1Result.Foreground = new SolidColorBrush(Colors.Red);
        }

        // Test Case 2: Large image should be constrained to 400
        if (image2 != null)
        {
            var pass = Math.Abs(image2.MaxWidth - 400) < 1;
            TestCase2Result.Text = $"MaxWidth: {image2.MaxWidth:F0} (Expected: 400) - {(pass ? "✅ PASS" : "❌ FAIL")}";
            TestCase2Result.Foreground = new SolidColorBrush(pass ? Colors.Green : Colors.Red);
        }
        else
        {
            TestCase2Result.Text = "❌ Image not found";
            TestCase2Result.Foreground = new SolidColorBrush(Colors.Red);
        }

        // Test Case 3: No dimensions - theme should NOT be applied
        if (image3 != null)
        {
            var pass = double.IsInfinity(image3.MaxWidth);
            TestCase3Result.Text = $"MaxWidth: {image3.MaxWidth} (Expected: Infinity) - {(pass ? "✅ PASS" : "❌ FAIL - Theme incorrectly applied!")}";
            TestCase3Result.Foreground = new SolidColorBrush(pass ? Colors.Green : Colors.Red);
        }
        else
        {
            TestCase3Result.Text = "❌ Image not found";
            TestCase3Result.Foreground = new SolidColorBrush(Colors.Red);
        }
    }

    private Image? FindImage(MarkdownTextBlock markdown)
    {
        return FindDescendant<Image>(markdown);
    }

    private T? FindDescendant<T>(DependencyObject parent) where T : DependencyObject
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T result)
                return result;
            var descendant = FindDescendant<T>(child);
            if (descendant != null)
                return descendant;
        }
        return null;
    }

    /// <summary>
    /// Test image provider that downloads a real image but reports configurable dimensions.
    /// This simulates providers that know the image size vs those that don't.
    /// </summary>
    private class TestImageProvider : IImageProvider
    {
        private static readonly System.Net.Http.HttpClient s_httpClient = new();

        private readonly double _width;
        private readonly double _height;
        private readonly string _realImageUrl;

        // Use a small, fast-loading image
        private const string DefaultImageUrl = "https://devblogs.microsoft.com/commandline/wp-content/uploads/sites/33/2025/09/ShortcutConflict.png";

        public TestImageProvider(double width, double height, string? realImageUrl = null)
        {
            _width = width;
            _height = height;
            _realImageUrl = realImageUrl ?? DefaultImageUrl;
        }

        public bool ShouldUseThisProvider(string url) => url.Contains("provider://");

        public async Task<Image> GetImage(string url)
        {
            var image = new Image();

            // Load a real image so it's visible
            try
            {
                var data = await s_httpClient.GetByteArrayAsync(_realImageUrl);
                var bitmap = new BitmapImage();
                using (var stream = new InMemoryRandomAccessStream())
                {
                    await stream.WriteAsync(data.AsBuffer());
                    stream.Seek(0);
                    await bitmap.SetSourceAsync(stream);
                }
                image.Source = bitmap;
            }
            catch
            {
                // Fallback to empty bitmap if download fails
                image.Source = new BitmapImage();
            }

            // Only set dimensions if they're valid
            // This simulates providers that may or may not know the image size
            if (_width > 0)
            {
                image.Width = _width;
            }
            if (_height > 0)
            {
                image.Height = _height;
            }

            return image;
        }
    }
}
