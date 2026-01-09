// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace MarkdownTextBlockExperiment.Samples;

/// <summary>
/// A sample demonstrating custom theming options for the MarkdownTextBlock control with live editing.
/// </summary>
/// 
[ToolkitSample(id: nameof(MarkdownTextBlockCustomThemeSample), "Custom Theme", description: "A sample showcasing custom theming options with live editing for headings, code blocks, quotes, tables, and more.")]
public sealed partial class MarkdownTextBlockCustomThemeSample : MarkdownTextBlockCustomThemeSampleBase
{
    public MarkdownConfig MarkdownConfig { get; private set; }

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

Images can be styled with max width, max height, corner radius, and stretch options. Notice how the text flows naturally without any gaps above or below the image:

![Windows Terminal](https://devblogs.microsoft.com/commandline/wp-content/uploads/sites/33/2025/11/0.96-Social-media-image-V2-1024x536.webp)

The image above automatically scales to respect the max width setting while maintaining its aspect ratio. Text continues to flow seamlessly below it.

![Shortcut Conflict](https://devblogs.microsoft.com/commandline/wp-content/uploads/sites/33/2025/09/ShortcutConflict.png)

Here's another image showing how corner radius can be applied to give images rounded corners.

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
- Click **Apply Changes** to see updates
- Use **Reset to Defaults** to start over

Numbered lists work too:

1. First item
2. Second item
   1. Nested numbered item
   2. Another nested item
      1. Deep nesting works
3. Third item
";

    public MarkdownTextBlockCustomThemeSample()
    {
        MarkdownConfig = new MarkdownConfig { Themes = CreateThemes() };
        this.InitializeComponent();
    }

    public override void ApplyTheme()
    {
        MarkdownConfig = new MarkdownConfig { Themes = CreateThemes() };

        // Force re-render by toggling text
        MarkdownTextBlock.Config = MarkdownConfig;
        var text = MarkdownTextBlock.Text;
        MarkdownTextBlock.Text = "";
        MarkdownTextBlock.Text = text;
    }
}
