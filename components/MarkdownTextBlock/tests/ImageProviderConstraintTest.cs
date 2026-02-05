// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.TestGen;
using CommunityToolkit.Tests;
using CommunityToolkit.WinUI.Controls;

namespace MarkdownTextBlockTests;

/// <summary>
/// Test class to verify image constraint behavior when using IImageProvider
/// Regression test for: https://github.com/CommunityToolkit/Labs-Windows/pull/771
/// </summary>
[TestClass]
public partial class ImageProviderConstraintTest : VisualUITestBase
{
    /// <summary>
    /// Mock image provider that returns an image with specific dimensions
    /// </summary>
    private class TestImageProvider : IImageProvider
    {
        private readonly double _width;
        private readonly double _height;

        public TestImageProvider(double width = 100, double height = 100)
        {
            _width = width;
            _height = height;
        }

        public bool ShouldUseThisProvider(string url) => url.StartsWith("test://");

        public Task<Image> GetImage(string url)
        {
            var image = new Image
            {
                // Simulate a 100x100 image with natural dimensions
                Width = _width,
                Height = _height,
                Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage()
            };
            return Task.FromResult(image);
        }
    }

    [UIThreadTestMethod]
    public async Task ImageProvider_WithThemeConstraints_ShouldNotApplyConstraintsToSmallerImages()
    {
        // Arrange: Image provider returns a 100x100 image
        var provider = new TestImageProvider(width: 100, height: 100);
        
        var config = new MarkdownConfig
        {
            ImageProvider = provider,
            Themes = new MarkdownThemes
            {
                // Theme allows images up to 500px wide
                ImageMaxWidth = 500,
                ImageMaxHeight = 500
            }
        };

        var markdown = new MarkdownTextBlock
        {
            Config = config,
            // Image URL with no precedent dimensions specified
            Text = "![Test Image](test://example.png)"
        };

        // Act
        await LoadTestContentAsync(markdown);
        
        // Give the async image loading time to complete
        await Task.Delay(500);
        await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

        // Find the image element
        var image = markdown.FindDescendant<Image>();
        Assert.IsNotNull(image, "Image element should be rendered");

        // Assert: The image should maintain its natural size (100x100)
        // and NOT be constrained to the theme's larger MaxWidth (500).
        //
        // This is a regression test for a previous bug where, when IImageProvider was used,
        // natural dimensions weren't set on the image, so MaxWidth defaulted to Infinity.
        // In that case the theme constraint (500 < Infinity) incorrectly applied, forcing MaxWidth=500.
        //
        // The purpose of this test is to ensure that the image's MaxWidth is set to its natural size (100)
        // before theme constraints are evaluated, so the theme constraint is not incorrectly applied.
        
        Assert.AreEqual(100.0, image.MaxWidth, 0.1, 
            "Image MaxWidth should be its natural size (100), not the theme constraint (500). " +
            "When using IImageProvider, natural dimensions must be set before applying theme constraints.");
    }

    [UIThreadTestMethod]
    public async Task ImageProvider_WithPrecedentDimensions_ShouldUsePrecedent()
    {
        // Arrange: Image provider returns a 100x100 image
        var provider = new TestImageProvider(width: 100, height: 100);
        
        var config = new MarkdownConfig
        {
            ImageProvider = provider,
            Themes = new MarkdownThemes
            {
                ImageMaxWidth = 500,
                ImageMaxHeight = 500
            }
        };

        var markdown = new MarkdownTextBlock
        {
            Config = config,
            // Image with explicit width specified in markdown
            Text = "<img src=\"test://example.png\" width=\"200\">"
        };

        // Act
        await LoadTestContentAsync(markdown);
        await Task.Delay(500);
        await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

        var image = markdown.FindDescendant<Image>();
        Assert.IsNotNull(image, "Image element should be rendered");

        // Assert: Precedent dimensions should be respected
        Assert.AreEqual(200.0, image.MaxWidth, 0.1, 
            "Image MaxWidth should respect precedent dimension (200) from markdown");
    }

    [UIThreadTestMethod]
    public async Task ImageProvider_WithSmallerThemeConstraint_ShouldApplyThemeConstraint()
    {
        // Arrange: Image provider returns a 800x600 image
        var provider = new TestImageProvider(width: 800, height: 600);
        
        var config = new MarkdownConfig
        {
            ImageProvider = provider,
            Themes = new MarkdownThemes
            {
                // Theme constrains images to max 400px
                ImageMaxWidth = 400,
                ImageMaxHeight = 400
            }
        };

        var markdown = new MarkdownTextBlock
        {
            Config = config,
            Text = "![Test Image](test://large.png)"
        };

        // Act
        await LoadTestContentAsync(markdown);
        await Task.Delay(500);
        await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

        var image = markdown.FindDescendant<Image>();
        Assert.IsNotNull(image, "Image element should be rendered");

        // Assert: When natural size (800) is larger than theme constraint (400),
        // the theme constraint should apply
        Assert.AreEqual(400.0, image.MaxWidth, 0.1, 
            "Image MaxWidth should be constrained to theme max (400) when natural size (800) is larger");
    }
}
