// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI;
using System.Runtime.CompilerServices;
using Windows.UI;


#if !WINAPPSDK
using FontWeights = Windows.UI.Text.FontWeights;
#else
using FontWeights = Microsoft.UI.Text.FontWeights;
#endif

namespace MarkdownTextBlockExperiment.Samples;

/// <summary>
/// A sample demonstrating custom theming options for the MarkdownTextBlock control with live editing.
/// </summary>
[ToolkitSample(id: nameof(MarkdownTextBlockCustomThemeSample), "Custom Theme", description: "A sample showcasing custom theming options with live editing for headings, code blocks, quotes, tables, and more.")]
public sealed partial class MarkdownTextBlockCustomThemeSample : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MarkdownConfig MarkdownConfig { get; private set; }
    public MarkdownThemes Themes => MarkdownConfig.Themes;

    private string _markdownText = "";
    public string MarkdownText
    {
        get => _markdownText;
        set { _markdownText = value; OnPropertyChanged(); }
    }

    // === Heading Properties ===
    private double _h1FontSize = 28;
    public double H1FontSize
    {
        get => _h1FontSize;
        set { _h1FontSize = value; OnPropertyChanged(); }
    }

    private double _h2FontSize = 24;
    public double H2FontSize
    {
        get => _h2FontSize;
        set { _h2FontSize = value; OnPropertyChanged(); }
    }

    private double _h3FontSize = 20;
    public double H3FontSize
    {
        get => _h3FontSize;
        set { _h3FontSize = value; OnPropertyChanged(); }
    }

    private int _h1ColorIndex = 0;
    public int H1ColorIndex
    {
        get => _h1ColorIndex;
        set { _h1ColorIndex = value; OnPropertyChanged(); }
    }

    // === Inline Code Properties ===
    private double _inlineCodeFontSize = 13;
    public double InlineCodeFontSize
    {
        get => _inlineCodeFontSize;
        set { _inlineCodeFontSize = value; OnPropertyChanged(); }
    }

    private double _inlineCodePadding = 4;
    public double InlineCodePadding
    {
        get => _inlineCodePadding;
        set { _inlineCodePadding = value; OnPropertyChanged(); }
    }

    private double _inlineCodeCornerRadius = 4;
    public double InlineCodeCornerRadius
    {
        get => _inlineCodeCornerRadius;
        set { _inlineCodeCornerRadius = value; OnPropertyChanged(); }
    }

    private double _inlineCodeBorderThickness = 1;
    public double InlineCodeBorderThickness
    {
        get => _inlineCodeBorderThickness;
        set { _inlineCodeBorderThickness = value; OnPropertyChanged(); }
    }

    private int _inlineCodeColorIndex = 0;
    public int InlineCodeColorIndex
    {
        get => _inlineCodeColorIndex;
        set { _inlineCodeColorIndex = value; OnPropertyChanged(); }
    }

    private int _inlineCodeBackgroundColorIndex = 0;
    public int InlineCodeBackgroundColorIndex
    {
        get => _inlineCodeBackgroundColorIndex;
        set { _inlineCodeBackgroundColorIndex = value; OnPropertyChanged(); }
    }

    private int _inlineCodeBorderColorIndex = 0;
    public int InlineCodeBorderColorIndex
    {
        get => _inlineCodeBorderColorIndex;
        set { _inlineCodeBorderColorIndex = value; OnPropertyChanged(); }
    }

    // === Code Block Properties ===
    private double _codeBlockPadding = 12;
    public double CodeBlockPadding
    {
        get => _codeBlockPadding;
        set { _codeBlockPadding = value; OnPropertyChanged(); }
    }

    private double _codeBlockCornerRadius = 8;
    public double CodeBlockCornerRadius
    {
        get => _codeBlockCornerRadius;
        set { _codeBlockCornerRadius = value; OnPropertyChanged(); }
    }

    private double _codeBlockBorderThickness = 1;
    public double CodeBlockBorderThickness
    {
        get => _codeBlockBorderThickness;
        set { _codeBlockBorderThickness = value; OnPropertyChanged(); }
    }

    private int _codeBlockFontIndex = 0;
    public int CodeBlockFontIndex
    {
        get => _codeBlockFontIndex;
        set { _codeBlockFontIndex = value; OnPropertyChanged(); }
    }

    private int _codeBlockBackgroundColorIndex = 0;
    public int CodeBlockBackgroundColorIndex
    {
        get => _codeBlockBackgroundColorIndex;
        set { _codeBlockBackgroundColorIndex = value; OnPropertyChanged(); }
    }

    private int _codeBlockBorderColorIndex = 0;
    public int CodeBlockBorderColorIndex
    {
        get => _codeBlockBorderColorIndex;
        set { _codeBlockBorderColorIndex = value; OnPropertyChanged(); }
    }

    // === Quote Properties ===
    private double _quoteBorderWidth = 4;
    public double QuoteBorderWidth
    {
        get => _quoteBorderWidth;
        set { _quoteBorderWidth = value; OnPropertyChanged(); }
    }

    private double _quotePadding = 12;
    public double QuotePadding
    {
        get => _quotePadding;
        set { _quotePadding = value; OnPropertyChanged(); }
    }

    private double _quoteCornerRadius = 8;
    public double QuoteCornerRadius
    {
        get => _quoteCornerRadius;
        set { _quoteCornerRadius = value; OnPropertyChanged(); }
    }

    private int _quoteColorIndex = 0;
    public int QuoteColorIndex
    {
        get => _quoteColorIndex;
        set { _quoteColorIndex = value; OnPropertyChanged(); }
    }

    // === Table Properties ===
    private double _tableCellPadding = 8;
    public double TableCellPadding
    {
        get => _tableCellPadding;
        set { _tableCellPadding = value; OnPropertyChanged(); }
    }

    private double _tableBorderThickness = 1;
    public double TableBorderThickness
    {
        get => _tableBorderThickness;
        set { _tableBorderThickness = value; OnPropertyChanged(); }
    }

    // === Horizontal Rule Properties ===
    private double _horizontalRuleThickness = 2;
    public double HorizontalRuleThickness
    {
        get => _horizontalRuleThickness;
        set { _horizontalRuleThickness = value; OnPropertyChanged(); }
    }

    private double _horizontalRuleMargin = 16;
    public double HorizontalRuleMargin
    {
        get => _horizontalRuleMargin;
        set { _horizontalRuleMargin = value; OnPropertyChanged(); }
    }

    // === Image Properties ===
    private double _imageMaxWidth = 0;
    public double ImageMaxWidth
    {
        get => _imageMaxWidth;
        set { _imageMaxWidth = value; OnPropertyChanged(); }
    }

    private double _imageMaxHeight = 0;
    public double ImageMaxHeight
    {
        get => _imageMaxHeight;
        set { _imageMaxHeight = value; OnPropertyChanged(); }
    }

    private int _imageStretchIndex = 0;
    public int ImageStretchIndex
    {
        get => _imageStretchIndex;
        set { _imageStretchIndex = value; OnPropertyChanged(); }
    }

    // Color lookup helpers
    private static readonly Brush[] HeadingColors = new Brush[]
    {
        new SolidColorBrush(Colors.DodgerBlue),
        new SolidColorBrush(Colors.Coral),
        new SolidColorBrush(Colors.MediumSeaGreen),
        new SolidColorBrush(Colors.Gold),
        new SolidColorBrush(Colors.Orchid),
        (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"]
    };

    private static readonly Brush[] InlineCodeColors = new Brush[]
    {
        new SolidColorBrush(Colors.Orange),
        new SolidColorBrush(Colors.Coral),
        new SolidColorBrush(Colors.LimeGreen),
        new SolidColorBrush(Colors.DeepSkyBlue),
        (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"]
    };

    private static readonly Brush[] CodeBackgroundColors = new Brush[]
    {
        new SolidColorBrush(Color.FromArgb(40, 100, 100, 255)),
        new SolidColorBrush(Color.FromArgb(30, 50, 50, 80)),
        new SolidColorBrush(Color.FromArgb(40, 0, 0, 0)),
        new SolidColorBrush(Color.FromArgb(40, 50, 150, 50)),
        (Brush)Application.Current.Resources["ExpanderHeaderBackground"]
    };

    private static readonly Brush[] CodeBorderColors = new Brush[]
    {
        new SolidColorBrush(Colors.SlateGray),
        new SolidColorBrush(Colors.DimGray),
        new SolidColorBrush(Colors.DarkSlateGray),
        new SolidColorBrush(Colors.MediumSlateBlue),
        new SolidColorBrush(Colors.Transparent)
    };

    private static readonly Brush[] QuoteColors = new Brush[]
    {
        new SolidColorBrush(Colors.DodgerBlue),
        new SolidColorBrush(Colors.Gray),
        new SolidColorBrush(Colors.MediumSeaGreen),
        new SolidColorBrush(Colors.Coral)
    };

    private static readonly FontFamily[] CodeFonts = new FontFamily[]
    {
        new FontFamily("Cascadia Code"),
        new FontFamily("Consolas"),
        new FontFamily("Courier New"),
        new FontFamily("Segoe UI")
    };

    private static readonly Stretch[] ImageStretchOptions = new Stretch[]
    {
        Stretch.Uniform,
        Stretch.None,
        Stretch.Fill,
        Stretch.UniformToFill
    };

    private const string SampleMarkdown = @"
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

- Adjust the theme settings on the right panel
- Click **Apply Changes** to see updates
- Use **Reset to Defaults** to start over
";

    public MarkdownTextBlockCustomThemeSample()
    {
        MarkdownConfig = new MarkdownConfig { Themes = CreateThemes() };
        _markdownText = SampleMarkdown;
        this.InitializeComponent();
    }

    private MarkdownThemes CreateThemes()
    {
        return new MarkdownThemes
        {
            H1FontSize = H1FontSize,
            H1FontWeight = FontWeights.Bold,
            H1Foreground = HeadingColors[H1ColorIndex],
            H1Margin = new Thickness(0, 20, 0, 10),

            H2FontSize = H2FontSize,
            H2FontWeight = FontWeights.SemiBold,
            H2Foreground = new SolidColorBrush(Colors.MediumSlateBlue),

            H3FontSize = H3FontSize,
            H3FontWeight = FontWeights.SemiBold,
            H3Foreground = new SolidColorBrush(Colors.MediumPurple),

            InlineCodeFontSize = InlineCodeFontSize,
            InlineCodePadding = new Thickness(InlineCodePadding, InlineCodePadding / 2, InlineCodePadding, InlineCodePadding / 2),
            InlineCodeCornerRadius = new CornerRadius(InlineCodeCornerRadius),
            InlineCodeBorderThickness = new Thickness(InlineCodeBorderThickness),
            InlineCodeForeground = InlineCodeColors[InlineCodeColorIndex],
            InlineCodeBackground = CodeBackgroundColors[InlineCodeBackgroundColorIndex],
            InlineCodeBorderBrush = CodeBorderColors[InlineCodeBorderColorIndex],

            CodeBlockPadding = new Thickness(CodeBlockPadding),
            CodeBlockCornerRadius = new CornerRadius(CodeBlockCornerRadius),
            CodeBlockBorderThickness = new Thickness(CodeBlockBorderThickness),
            CodeBlockFontFamily = CodeFonts[CodeBlockFontIndex],
            CodeBlockBackground = CodeBackgroundColors[CodeBlockBackgroundColorIndex],
            CodeBlockForeground = new SolidColorBrush(Colors.LightGreen),
            CodeBlockBorderBrush = CodeBorderColors[CodeBlockBorderColorIndex],

            QuoteBorderThickness = new Thickness(QuoteBorderWidth, 0, 0, 0),
            QuotePadding = new Thickness(QuotePadding, QuotePadding / 2, QuotePadding, QuotePadding / 2),
            QuoteCornerRadius = new CornerRadius(0, QuoteCornerRadius, QuoteCornerRadius, 0),
            QuoteBorderBrush = QuoteColors[QuoteColorIndex],
            QuoteBackground = new SolidColorBrush(Color.FromArgb(20, 100, 149, 237)),
            QuoteForeground = new SolidColorBrush(Colors.CornflowerBlue),

            TableCellPadding = new Thickness(TableCellPadding, TableCellPadding / 2, TableCellPadding, TableCellPadding / 2),
            TableBorderThickness = TableBorderThickness,
            TableBorderBrush = new SolidColorBrush(Colors.SlateGray),
            TableHeadingBackground = new SolidColorBrush(Color.FromArgb(40, 100, 149, 237)),

            HorizontalRuleThickness = HorizontalRuleThickness,
            HorizontalRuleMargin = new Thickness(0, HorizontalRuleMargin, 0, HorizontalRuleMargin),
            HorizontalRuleBrush = new SolidColorBrush(Colors.MediumSlateBlue),

            LinkForeground = new SolidColorBrush(Colors.DeepSkyBlue),

            ImageMaxWidth = ImageMaxWidth,
            ImageMaxHeight = ImageMaxHeight,
            ImageStretch = ImageStretchOptions[ImageStretchIndex],
        };
    }

    private void ApplyTheme_Click(object sender, RoutedEventArgs e)
    {
        // Create new config with updated themes and re-render
        MarkdownConfig = new MarkdownConfig { Themes = CreateThemes() };
        MarkdownTextBlock.Config = MarkdownConfig;
        
        // Force re-render by toggling text
        var text = MarkdownText;
        MarkdownText = "";
        MarkdownText = text;
    }

    private void ResetTheme_Click(object sender, RoutedEventArgs e)
    {
        // Reset all values to defaults
        H1FontSize = 28;
        H2FontSize = 24;
        H3FontSize = 20;
        H1ColorIndex = 0;

        InlineCodeFontSize = 13;
        InlineCodePadding = 4;
        InlineCodeCornerRadius = 4;
        InlineCodeBorderThickness = 1;
        InlineCodeColorIndex = 0;
        InlineCodeBackgroundColorIndex = 0;
        InlineCodeBorderColorIndex = 0;

        CodeBlockPadding = 12;
        CodeBlockCornerRadius = 8;
        CodeBlockBorderThickness = 1;
        CodeBlockFontIndex = 0;
        CodeBlockBackgroundColorIndex = 1;
        CodeBlockBorderColorIndex = 1;

        QuoteBorderWidth = 4;
        QuotePadding = 12;
        QuoteCornerRadius = 8;
        QuoteColorIndex = 0;

        TableCellPadding = 8;
        TableBorderThickness = 1;

        HorizontalRuleThickness = 2;
        HorizontalRuleMargin = 16;

        ImageMaxWidth = 0;
        ImageMaxHeight = 0;
        ImageStretchIndex = 0;

        ApplyTheme_Click(sender, e);
    }
}
