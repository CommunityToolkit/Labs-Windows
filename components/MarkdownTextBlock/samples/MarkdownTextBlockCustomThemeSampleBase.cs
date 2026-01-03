// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI;
using Windows.UI;

#if !WINAPPSDK
using FontWeights = Windows.UI.Text.FontWeights;
#else
using FontWeights = Microsoft.UI.Text.FontWeights;
#endif

namespace MarkdownTextBlockExperiment.Samples;

public abstract partial class MarkdownTextBlockCustomThemeSampleBase : Page
{
    // Dependency properties for theme customization
    public static readonly DependencyProperty H1FontSizeProperty =
        DependencyProperty.Register(nameof(H1FontSize), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(28d));

    public static readonly DependencyProperty H2FontSizeProperty =
        DependencyProperty.Register(nameof(H2FontSize), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(24d));

    public static readonly DependencyProperty H3FontSizeProperty =
        DependencyProperty.Register(nameof(H3FontSize), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(20d));

    public static readonly DependencyProperty H1ColorIndexProperty =
        DependencyProperty.Register(nameof(H1ColorIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty InlineCodeFontSizeProperty =
        DependencyProperty.Register(nameof(InlineCodeFontSize), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(13d));

    public static readonly DependencyProperty InlineCodePaddingProperty =
        DependencyProperty.Register(nameof(InlineCodePadding), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(4d));

    public static readonly DependencyProperty InlineCodeCornerRadiusProperty =
        DependencyProperty.Register(nameof(InlineCodeCornerRadius), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(4d));

    public static readonly DependencyProperty InlineCodeBorderThicknessProperty =
        DependencyProperty.Register(nameof(InlineCodeBorderThickness), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(1d));

    public static readonly DependencyProperty InlineCodeColorIndexProperty =
        DependencyProperty.Register(nameof(InlineCodeColorIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty InlineCodeBackgroundColorIndexProperty =
        DependencyProperty.Register(nameof(InlineCodeBackgroundColorIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty InlineCodeBorderColorIndexProperty =
        DependencyProperty.Register(nameof(InlineCodeBorderColorIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty CodeBlockPaddingProperty =
        DependencyProperty.Register(nameof(CodeBlockPadding), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(12d));

    public static readonly DependencyProperty CodeBlockCornerRadiusProperty =
        DependencyProperty.Register(nameof(CodeBlockCornerRadius), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(8d));

    public static readonly DependencyProperty CodeBlockBorderThicknessProperty =
        DependencyProperty.Register(nameof(CodeBlockBorderThickness), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(1d));

    public static readonly DependencyProperty CodeBlockFontIndexProperty =
        DependencyProperty.Register(nameof(CodeBlockFontIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty CodeBlockBackgroundColorIndexProperty =
        DependencyProperty.Register(nameof(CodeBlockBackgroundColorIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty CodeBlockBorderColorIndexProperty =
        DependencyProperty.Register(nameof(CodeBlockBorderColorIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty QuoteBorderWidthProperty =
        DependencyProperty.Register(nameof(QuoteBorderWidth), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(4d));

    public static readonly DependencyProperty QuotePaddingProperty =
        DependencyProperty.Register(nameof(QuotePadding), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(12d));

    public static readonly DependencyProperty QuoteCornerRadiusProperty =
        DependencyProperty.Register(nameof(QuoteCornerRadius), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(8d));

    public static readonly DependencyProperty QuoteColorIndexProperty =
        DependencyProperty.Register(nameof(QuoteColorIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty TableCellPaddingProperty =
        DependencyProperty.Register(nameof(TableCellPadding), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(8d));

    public static readonly DependencyProperty TableBorderThicknessProperty =
        DependencyProperty.Register(nameof(TableBorderThickness), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(1d));

    public static readonly DependencyProperty HorizontalRuleThicknessProperty =
        DependencyProperty.Register(nameof(HorizontalRuleThickness), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(2d));

    public static readonly DependencyProperty HorizontalRuleMarginProperty =
        DependencyProperty.Register(nameof(HorizontalRuleMargin), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(16d));

    public static readonly DependencyProperty ImageMaxWidthProperty =
        DependencyProperty.Register(nameof(ImageMaxWidth), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0d));

    public static readonly DependencyProperty ImageMaxHeightProperty =
        DependencyProperty.Register(nameof(ImageMaxHeight), typeof(double), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0d));

    public static readonly DependencyProperty ImageStretchIndexProperty =
        DependencyProperty.Register(nameof(ImageStretchIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public static readonly DependencyProperty BoldFontWeightIndexProperty =
        DependencyProperty.Register(nameof(BoldFontWeightIndex), typeof(int), typeof(MarkdownTextBlockCustomThemeSampleBase), new PropertyMetadata(0));

    public MarkdownTextBlockCustomThemeSampleBase()
    {
    }

    // Properties
    public double H1FontSize
    {
        get => (double)GetValue(H1FontSizeProperty);
        set => SetValue(H1FontSizeProperty, value);
    }

    public double H2FontSize
    {
        get => (double)GetValue(H2FontSizeProperty);
        set => SetValue(H2FontSizeProperty, value);
    }

    public double H3FontSize
    {
        get => (double)GetValue(H3FontSizeProperty);
        set => SetValue(H3FontSizeProperty, value);
    }

    public int H1ColorIndex
    {
        get => (int)GetValue(H1ColorIndexProperty);
        set => SetValue(H1ColorIndexProperty, value);
    }

    public double InlineCodeFontSize
    {
        get => (double)GetValue(InlineCodeFontSizeProperty);
        set => SetValue(InlineCodeFontSizeProperty, value);
    }

    public double InlineCodePadding
    {
        get => (double)GetValue(InlineCodePaddingProperty);
        set => SetValue(InlineCodePaddingProperty, value);
    }

    public double InlineCodeCornerRadius
    {
        get => (double)GetValue(InlineCodeCornerRadiusProperty);
        set => SetValue(InlineCodeCornerRadiusProperty, value);
    }

    public double InlineCodeBorderThickness
    {
        get => (double)GetValue(InlineCodeBorderThicknessProperty);
        set => SetValue(InlineCodeBorderThicknessProperty, value);
    }

    public int InlineCodeColorIndex
    {
        get => (int)GetValue(InlineCodeColorIndexProperty);
        set => SetValue(InlineCodeColorIndexProperty, value);
    }

    public int InlineCodeBackgroundColorIndex
    {
        get => (int)GetValue(InlineCodeBackgroundColorIndexProperty);
        set => SetValue(InlineCodeBackgroundColorIndexProperty, value);
    }

    public int InlineCodeBorderColorIndex
    {
        get => (int)GetValue(InlineCodeBorderColorIndexProperty);
        set => SetValue(InlineCodeBorderColorIndexProperty, value);
    }

    public double CodeBlockPadding
    {
        get => (double)GetValue(CodeBlockPaddingProperty);
        set => SetValue(CodeBlockPaddingProperty, value);
    }

    public double CodeBlockCornerRadius
    {
        get => (double)GetValue(CodeBlockCornerRadiusProperty);
        set => SetValue(CodeBlockCornerRadiusProperty, value);
    }

    public double CodeBlockBorderThickness
    {
        get => (double)GetValue(CodeBlockBorderThicknessProperty);
        set => SetValue(CodeBlockBorderThicknessProperty, value);
    }

    public int CodeBlockFontIndex
    {
        get => (int)GetValue(CodeBlockFontIndexProperty);
        set => SetValue(CodeBlockFontIndexProperty, value);
    }

    public int CodeBlockBackgroundColorIndex
    {
        get => (int)GetValue(CodeBlockBackgroundColorIndexProperty);
        set => SetValue(CodeBlockBackgroundColorIndexProperty, value);
    }

    public int CodeBlockBorderColorIndex
    {
        get => (int)GetValue(CodeBlockBorderColorIndexProperty);
        set => SetValue(CodeBlockBorderColorIndexProperty, value);
    }

    public double QuoteBorderWidth
    {
        get => (double)GetValue(QuoteBorderWidthProperty);
        set => SetValue(QuoteBorderWidthProperty, value);
    }

    public double QuotePadding
    {
        get => (double)GetValue(QuotePaddingProperty);
        set => SetValue(QuotePaddingProperty, value);
    }

    public double QuoteCornerRadius
    {
        get => (double)GetValue(QuoteCornerRadiusProperty);
        set => SetValue(QuoteCornerRadiusProperty, value);
    }

    public int QuoteColorIndex
    {
        get => (int)GetValue(QuoteColorIndexProperty);
        set => SetValue(QuoteColorIndexProperty, value);
    }

    public double TableCellPadding
    {
        get => (double)GetValue(TableCellPaddingProperty);
        set => SetValue(TableCellPaddingProperty, value);
    }

    public double TableBorderThickness
    {
        get => (double)GetValue(TableBorderThicknessProperty);
        set => SetValue(TableBorderThicknessProperty, value);
    }

    public double HorizontalRuleThickness
    {
        get => (double)GetValue(HorizontalRuleThicknessProperty);
        set => SetValue(HorizontalRuleThicknessProperty, value);
    }

    public double HorizontalRuleMargin
    {
        get => (double)GetValue(HorizontalRuleMarginProperty);
        set => SetValue(HorizontalRuleMarginProperty, value);
    }

    public double ImageMaxWidth
    {
        get => (double)GetValue(ImageMaxWidthProperty);
        set => SetValue(ImageMaxWidthProperty, value);
    }

    public double ImageMaxHeight
    {
        get => (double)GetValue(ImageMaxHeightProperty);
        set => SetValue(ImageMaxHeightProperty, value);
    }

    public int ImageStretchIndex
    {
        get => (int)GetValue(ImageStretchIndexProperty);
        set => SetValue(ImageStretchIndexProperty, value);
    }

    public int BoldFontWeightIndex
    {
        get => (int)GetValue(BoldFontWeightIndexProperty);
        set => SetValue(BoldFontWeightIndexProperty, value);
    }

    // Static lookup arrays
    public static Brush[] HeadingColors { get; } = new Brush[]
    {
        new SolidColorBrush(Colors.DodgerBlue),
        new SolidColorBrush(Colors.Coral),
        new SolidColorBrush(Colors.MediumSeaGreen),
        new SolidColorBrush(Colors.Gold),
        new SolidColorBrush(Colors.Orchid),
        (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"]
    };

    public static Brush[] InlineCodeColors { get; } = new Brush[]
    {
        new SolidColorBrush(Colors.Orange),
        new SolidColorBrush(Colors.Coral),
        new SolidColorBrush(Colors.LimeGreen),
        new SolidColorBrush(Colors.DeepSkyBlue),
        (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"]
    };

    public static Brush[] CodeBackgroundColors { get; } = new Brush[]
    {
        new SolidColorBrush(Color.FromArgb(40, 100, 100, 255)),
        new SolidColorBrush(Color.FromArgb(30, 50, 50, 80)),
        new SolidColorBrush(Color.FromArgb(40, 0, 0, 0)),
        new SolidColorBrush(Color.FromArgb(40, 50, 150, 50)),
        (Brush)Application.Current.Resources["ExpanderHeaderBackground"]
    };

    public static Brush[] CodeBorderColors { get; } = new Brush[]
    {
        new SolidColorBrush(Colors.SlateGray),
        new SolidColorBrush(Colors.DimGray),
        new SolidColorBrush(Colors.DarkSlateGray),
        new SolidColorBrush(Colors.MediumSlateBlue),
        new SolidColorBrush(Colors.Transparent)
    };

    public static Brush[] QuoteColors { get; } = new Brush[]
    {
        new SolidColorBrush(Colors.DodgerBlue),
        new SolidColorBrush(Colors.Gray),
        new SolidColorBrush(Colors.MediumSeaGreen),
        new SolidColorBrush(Colors.Coral)
    };

    public static FontFamily[] CodeFonts { get; } = new FontFamily[]
    {
        new FontFamily("Cascadia Code"),
        new FontFamily("Consolas"),
        new FontFamily("Courier New"),
        new FontFamily("Segoe UI")
    };

    public static Stretch[] ImageStretchOptions { get; } = new Stretch[]
    {
        Stretch.Uniform,
        Stretch.None,
        Stretch.Fill,
        Stretch.UniformToFill
    };

    public static Windows.UI.Text.FontWeight[] BoldFontWeights { get; } = new Windows.UI.Text.FontWeight[]
    {
        FontWeights.Bold,
        FontWeights.SemiBold,
        FontWeights.Medium,
        FontWeights.Normal,
        FontWeights.ExtraBold
    };

    public MarkdownThemes CreateThemes()
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

            BoldFontWeight = BoldFontWeights[BoldFontWeightIndex],
        };
    }

    public void ResetToDefaults()
    {
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

        BoldFontWeightIndex = 0;
    }

    public abstract void ApplyTheme();
}
