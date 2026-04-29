// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MarkdownTextBlockExperiment.Samples;

/// <summary>
/// A sample demonstrating custom theming options for the MarkdownTextBlock control with live editing.
/// </summary>
[ToolkitSample(id: nameof(MarkdownTextBlockCustomThemeSample), "Custom Theme", description: "A sample showcasing custom theming options with live editing for headings, code blocks, quotes, tables, and more.")]
public sealed partial class MarkdownTextBlockCustomThemeSample : MarkdownTextBlockCustomThemeSampleBase
{
    public string MarkdownText { get; } = @"
# Custom Theme Demo

This sample demonstrates the **custom theming** capabilities of the `MarkdownTextBlock` control.

## Heading Styles

Each heading level has customizable foreground color, font size, weight, and margin.

### Heading 3
#### Heading 4

## Inline Code

Here is some `inline code` with custom styling. Try adjusting the padding, corner radius, and colors in the editor panel!

Another example: `config.Themes.InlineCodePadding`

## Images

Images can be styled with max width, max height, and stretch options. Notice how the text flows naturally without any gaps above or below the image:

![Windows Terminal](https://devblogs.microsoft.com/commandline/wp-content/uploads/sites/33/2025/11/0.96-Social-media-image-V2-1024x536.webp)

The image above automatically scales to respect the max width setting while maintaining its aspect ratio. Text continues to flow seamlessly below it.

![Shortcut Conflict](https://devblogs.microsoft.com/commandline/wp-content/uploads/sites/33/2025/09/ShortcutConflict.png)

## Code Blocks

```csharp
// Code blocks have custom styling too!
public class CustomTheme
{
    public string Name { get; set; }
    public Color PrimaryColor { get; set; }
    
    public void ApplyTheme()
    {
        Console.WriteLine($""Applying theme: {Name}"");
    }
}
```

## Block Quotes

> This is a styled block quote with custom background, 
> border color, padding, and corner radius.
> 
> Try changing the border width and corner radius!

## Links

Check out [this link](https://github.com/CommunityToolkit) - links have custom foreground colors.

## Tables

| Feature | Status | Notes |
|---------|--------|-------|
| Headings | ✅ | Per-level colors |
| Inline Code | ✅ | Full styling |
| Code Blocks | ✅ | Background, border, font |
| Quotes | ✅ | Border, background, radius |

---

## Horizontal Rule

The line above is a horizontal rule with customizable thickness and margin.

## Lists

Try adjusting the **Bullet Spacing** and **Gutter Width** settings to see how list formatting changes!

- Top level list item
  - Nested item level 1
    - Nested item level 2
      - Nested item level 3
- Another top level item
- Adjust the theme settings in the options panel
  - The gutter width controls how much each level is indented
  - The bullet spacing controls space after the bullet character
- Changes apply live via bindings
- Use **Reset to Defaults** to start over

Numbered lists work too:

1. First item
2. Second item
   1. Nested numbered item
   2. Another nested item
      1. Deep nesting works
3. Third item
";

    // Converter methods for x:Bind function bindings (must be on the x:Class type, not the base)
    public Brush GetHeadingBrush(int index) => HeadingColors[Math.Clamp(index, 0, HeadingColors.Length - 1)];
    public Brush GetInlineCodeBrush(int index) => InlineCodeColors[Math.Clamp(index, 0, InlineCodeColors.Length - 1)];
    public Brush GetCodeBackgroundBrush(int index) => CodeBackgroundColors[Math.Clamp(index, 0, CodeBackgroundColors.Length - 1)];
    public Brush GetCodeBorderBrush(int index) => CodeBorderColors[Math.Clamp(index, 0, CodeBorderColors.Length - 1)];
    public Brush GetQuoteBrush(int index) => QuoteColors[Math.Clamp(index, 0, QuoteColors.Length - 1)];
    public FontFamily GetCodeFont(int index) => CodeFonts[Math.Clamp(index, 0, CodeFonts.Length - 1)];
    public Stretch GetImageStretch(int index) => ImageStretchOptions[Math.Clamp(index, 0, ImageStretchOptions.Length - 1)];
    public Windows.UI.Text.FontWeight GetBoldFontWeight(int index) => BoldFontWeights[Math.Clamp(index, 0, BoldFontWeights.Length - 1)];
    public Thickness GetUniformThickness(double value) => new Thickness(value);
    public CornerRadius GetUniformCornerRadius(double value) => new CornerRadius(value);
    public Thickness GetHorizontalPadding(double value) => new Thickness(value, value / 2, value, value / 2);
    public Thickness GetLeftBorderThickness(double value) => new Thickness(value, 0, 0, 0);
    public CornerRadius GetRightCornerRadius(double value) => new CornerRadius(0, value, value, 0);
    public Thickness GetVerticalMargin(double value) => new Thickness(0, value, 0, value);

    public MarkdownTextBlockCustomThemeSample()
    {
        this.InitializeComponent();
    }
}
