// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINAPPSDK
using FontWeight = Windows.UI.Text.FontWeight;
using FontWeights = Windows.UI.Text.FontWeights;
#else
using FontWeight = Windows.UI.Text.FontWeight;
using FontWeights = Microsoft.UI.Text.FontWeights;
#endif

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Configuration object containing all theme and styling properties for <see cref="MarkdownTextBlock"/>.
/// </summary>
public partial class MarkdownConfig : DependencyObject
{
    /// <summary>
    /// Raised when any configuration property changes.
    /// </summary>
    internal event EventHandler? ConfigPropertyChanged;

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MarkdownConfig)d).ConfigPropertyChanged?.Invoke(d, EventArgs.Empty);
    }

    // ── Heading font sizes ──────────────────────────────────────────

    /// <summary>Identifies the <see cref="H1FontSize"/> dependency property.</summary>
    public static readonly DependencyProperty H1FontSizeProperty =
        DependencyProperty.Register(nameof(H1FontSize), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(22d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H2FontSize"/> dependency property.</summary>
    public static readonly DependencyProperty H2FontSizeProperty =
        DependencyProperty.Register(nameof(H2FontSize), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(20d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H3FontSize"/> dependency property.</summary>
    public static readonly DependencyProperty H3FontSizeProperty =
        DependencyProperty.Register(nameof(H3FontSize), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(18d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H4FontSize"/> dependency property.</summary>
    public static readonly DependencyProperty H4FontSizeProperty =
        DependencyProperty.Register(nameof(H4FontSize), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(16d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H5FontSize"/> dependency property.</summary>
    public static readonly DependencyProperty H5FontSizeProperty =
        DependencyProperty.Register(nameof(H5FontSize), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(14d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H6FontSize"/> dependency property.</summary>
    public static readonly DependencyProperty H6FontSizeProperty =
        DependencyProperty.Register(nameof(H6FontSize), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(12d, OnPropertyChanged));

    // ── Heading foregrounds ─────────────────────────────────────────

    /// <summary>Identifies the <see cref="H1Foreground"/> dependency property.</summary>
    public static readonly DependencyProperty H1ForegroundProperty =
        DependencyProperty.Register(nameof(H1Foreground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H2Foreground"/> dependency property.</summary>
    public static readonly DependencyProperty H2ForegroundProperty =
        DependencyProperty.Register(nameof(H2Foreground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H3Foreground"/> dependency property.</summary>
    public static readonly DependencyProperty H3ForegroundProperty =
        DependencyProperty.Register(nameof(H3Foreground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H4Foreground"/> dependency property.</summary>
    public static readonly DependencyProperty H4ForegroundProperty =
        DependencyProperty.Register(nameof(H4Foreground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H5Foreground"/> dependency property.</summary>
    public static readonly DependencyProperty H5ForegroundProperty =
        DependencyProperty.Register(nameof(H5Foreground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H6Foreground"/> dependency property.</summary>
    public static readonly DependencyProperty H6ForegroundProperty =
        DependencyProperty.Register(nameof(H6Foreground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    // ── Heading font weights ────────────────────────────────────────

    /// <summary>Identifies the <see cref="H1FontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty H1FontWeightProperty =
        DependencyProperty.Register(nameof(H1FontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.SemiBold, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H2FontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty H2FontWeightProperty =
        DependencyProperty.Register(nameof(H2FontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.SemiBold, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H3FontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty H3FontWeightProperty =
        DependencyProperty.Register(nameof(H3FontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.SemiBold, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H4FontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty H4FontWeightProperty =
        DependencyProperty.Register(nameof(H4FontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.SemiBold, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H5FontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty H5FontWeightProperty =
        DependencyProperty.Register(nameof(H5FontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.SemiBold, OnPropertyChanged));

    /// <summary>Identifies the <see cref="H6FontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty H6FontWeightProperty =
        DependencyProperty.Register(nameof(H6FontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.SemiBold, OnPropertyChanged));

    // ── Heading margins ─────────────────────────────────────────────

    /// <summary>Identifies the <see cref="H1Margin"/> dependency property.</summary>
    public static readonly DependencyProperty H1MarginProperty =
        DependencyProperty.Register(nameof(H1Margin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 16, 0, 0), OnPropertyChanged));

    /// <summary>Identifies the <see cref="H2Margin"/> dependency property.</summary>
    public static readonly DependencyProperty H2MarginProperty =
        DependencyProperty.Register(nameof(H2Margin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 16, 0, 0), OnPropertyChanged));

    /// <summary>Identifies the <see cref="H3Margin"/> dependency property.</summary>
    public static readonly DependencyProperty H3MarginProperty =
        DependencyProperty.Register(nameof(H3Margin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 16, 0, 0), OnPropertyChanged));

    /// <summary>Identifies the <see cref="H4Margin"/> dependency property.</summary>
    public static readonly DependencyProperty H4MarginProperty =
        DependencyProperty.Register(nameof(H4Margin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 16, 0, 0), OnPropertyChanged));

    /// <summary>Identifies the <see cref="H5Margin"/> dependency property.</summary>
    public static readonly DependencyProperty H5MarginProperty =
        DependencyProperty.Register(nameof(H5Margin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 8, 0, 0), OnPropertyChanged));

    /// <summary>Identifies the <see cref="H6Margin"/> dependency property.</summary>
    public static readonly DependencyProperty H6MarginProperty =
        DependencyProperty.Register(nameof(H6Margin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 8, 0, 0), OnPropertyChanged));

    // ── Inline code ─────────────────────────────────────────────────

    /// <summary>Identifies the <see cref="InlineCodeBackground"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodeBackgroundProperty =
        DependencyProperty.Register(nameof(InlineCodeBackground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="InlineCodeForeground"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodeForegroundProperty =
        DependencyProperty.Register(nameof(InlineCodeForeground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="InlineCodeBorderBrush"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodeBorderBrushProperty =
        DependencyProperty.Register(nameof(InlineCodeBorderBrush), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="InlineCodeBorderThickness"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodeBorderThicknessProperty =
        DependencyProperty.Register(nameof(InlineCodeBorderThickness), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(1), OnPropertyChanged));

    /// <summary>Identifies the <see cref="InlineCodeCornerRadius"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodeCornerRadiusProperty =
        DependencyProperty.Register(nameof(InlineCodeCornerRadius), typeof(CornerRadius), typeof(MarkdownConfig), new PropertyMetadata(new CornerRadius(2), OnPropertyChanged));

    /// <summary>Identifies the <see cref="InlineCodePadding"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodePaddingProperty =
        DependencyProperty.Register(nameof(InlineCodePadding), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(default(Thickness), OnPropertyChanged));

    /// <summary>Identifies the <see cref="InlineCodeFontSize"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodeFontSizeProperty =
        DependencyProperty.Register(nameof(InlineCodeFontSize), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(10d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="InlineCodeFontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty InlineCodeFontWeightProperty =
        DependencyProperty.Register(nameof(InlineCodeFontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.Normal, OnPropertyChanged));

    // ── Bold ────────────────────────────────────────────────────────

    /// <summary>Identifies the <see cref="BoldFontWeight"/> dependency property.</summary>
    public static readonly DependencyProperty BoldFontWeightProperty =
        DependencyProperty.Register(nameof(BoldFontWeight), typeof(FontWeight), typeof(MarkdownConfig), new PropertyMetadata(FontWeights.SemiBold, OnPropertyChanged));

    // ── Code block ──────────────────────────────────────────────────

    /// <summary>Identifies the <see cref="CodeBlockBackground"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockBackgroundProperty =
        DependencyProperty.Register(nameof(CodeBlockBackground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="CodeBlockBorderBrush"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockBorderBrushProperty =
        DependencyProperty.Register(nameof(CodeBlockBorderBrush), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="CodeBlockForeground"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockForegroundProperty =
        DependencyProperty.Register(nameof(CodeBlockForeground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="CodeBlockBorderThickness"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockBorderThicknessProperty =
        DependencyProperty.Register(nameof(CodeBlockBorderThickness), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(1), OnPropertyChanged));

    /// <summary>Identifies the <see cref="CodeBlockPadding"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockPaddingProperty =
        DependencyProperty.Register(nameof(CodeBlockPadding), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(8), OnPropertyChanged));

    /// <summary>Identifies the <see cref="CodeBlockMargin"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockMarginProperty =
        DependencyProperty.Register(nameof(CodeBlockMargin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 8, 0, 8), OnPropertyChanged));

    /// <summary>Identifies the <see cref="CodeBlockFontFamily"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockFontFamilyProperty =
        DependencyProperty.Register(nameof(CodeBlockFontFamily), typeof(FontFamily), typeof(MarkdownConfig), new PropertyMetadata(new FontFamily("Consolas"), OnPropertyChanged));

    /// <summary>Identifies the <see cref="CodeBlockCornerRadius"/> dependency property.</summary>
    public static readonly DependencyProperty CodeBlockCornerRadiusProperty =
        DependencyProperty.Register(nameof(CodeBlockCornerRadius), typeof(CornerRadius), typeof(MarkdownConfig), new PropertyMetadata(new CornerRadius(4), OnPropertyChanged));

    // ── Horizontal rule ─────────────────────────────────────────────

    /// <summary>Identifies the <see cref="HorizontalRuleBrush"/> dependency property.</summary>
    public static readonly DependencyProperty HorizontalRuleBrushProperty =
        DependencyProperty.Register(nameof(HorizontalRuleBrush), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="HorizontalRuleThickness"/> dependency property.</summary>
    public static readonly DependencyProperty HorizontalRuleThicknessProperty =
        DependencyProperty.Register(nameof(HorizontalRuleThickness), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(1d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="HorizontalRuleMargin"/> dependency property.</summary>
    public static readonly DependencyProperty HorizontalRuleMarginProperty =
        DependencyProperty.Register(nameof(HorizontalRuleMargin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 12, 0, 12), OnPropertyChanged));

    // ── Link ────────────────────────────────────────────────────────

    /// <summary>Identifies the <see cref="LinkForeground"/> dependency property.</summary>
    public static readonly DependencyProperty LinkForegroundProperty =
        DependencyProperty.Register(nameof(LinkForeground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    // ── Paragraph / list ────────────────────────────────────────────

    /// <summary>Identifies the <see cref="ParagraphMargin"/> dependency property.</summary>
    public static readonly DependencyProperty ParagraphMarginProperty =
        DependencyProperty.Register(nameof(ParagraphMargin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 8, 0, 8), OnPropertyChanged));

    /// <summary>Identifies the <see cref="ParagraphLineHeight"/> dependency property.</summary>
    public static readonly DependencyProperty ParagraphLineHeightProperty =
        DependencyProperty.Register(nameof(ParagraphLineHeight), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(0d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="ListBulletSpacing"/> dependency property.</summary>
    public static readonly DependencyProperty ListBulletSpacingProperty =
        DependencyProperty.Register(nameof(ListBulletSpacing), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(4d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="ListGutterWidth"/> dependency property.</summary>
    public static readonly DependencyProperty ListGutterWidthProperty =
        DependencyProperty.Register(nameof(ListGutterWidth), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(32d, OnPropertyChanged));

    // ── Quote ───────────────────────────────────────────────────────

    /// <summary>Identifies the <see cref="QuoteBackground"/> dependency property.</summary>
    public static readonly DependencyProperty QuoteBackgroundProperty =
        DependencyProperty.Register(nameof(QuoteBackground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="QuoteBorderBrush"/> dependency property.</summary>
    public static readonly DependencyProperty QuoteBorderBrushProperty =
        DependencyProperty.Register(nameof(QuoteBorderBrush), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="QuoteForeground"/> dependency property.</summary>
    public static readonly DependencyProperty QuoteForegroundProperty =
        DependencyProperty.Register(nameof(QuoteForeground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="QuoteBorderThickness"/> dependency property.</summary>
    public static readonly DependencyProperty QuoteBorderThicknessProperty =
        DependencyProperty.Register(nameof(QuoteBorderThickness), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(4, 0, 0, 0), OnPropertyChanged));

    /// <summary>Identifies the <see cref="QuoteMargin"/> dependency property.</summary>
    public static readonly DependencyProperty QuoteMarginProperty =
        DependencyProperty.Register(nameof(QuoteMargin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 4, 0, 4), OnPropertyChanged));

    /// <summary>Identifies the <see cref="QuotePadding"/> dependency property.</summary>
    public static readonly DependencyProperty QuotePaddingProperty =
        DependencyProperty.Register(nameof(QuotePadding), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(4), OnPropertyChanged));

    /// <summary>Identifies the <see cref="QuoteBarMargin"/> dependency property.</summary>
    public static readonly DependencyProperty QuoteBarMarginProperty =
        DependencyProperty.Register(nameof(QuoteBarMargin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 0, 4, 0), OnPropertyChanged));

    /// <summary>Identifies the <see cref="QuoteCornerRadius"/> dependency property.</summary>
    public static readonly DependencyProperty QuoteCornerRadiusProperty =
        DependencyProperty.Register(nameof(QuoteCornerRadius), typeof(CornerRadius), typeof(MarkdownConfig), new PropertyMetadata(new CornerRadius(4), OnPropertyChanged));

    // ── Image ───────────────────────────────────────────────────────

    /// <summary>Identifies the <see cref="ImageMaxWidth"/> dependency property.</summary>
    public static readonly DependencyProperty ImageMaxWidthProperty =
        DependencyProperty.Register(nameof(ImageMaxWidth), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(0d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="ImageMaxHeight"/> dependency property.</summary>
    public static readonly DependencyProperty ImageMaxHeightProperty =
        DependencyProperty.Register(nameof(ImageMaxHeight), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(0d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="ImageStretch"/> dependency property.</summary>
    public static readonly DependencyProperty ImageStretchProperty =
        DependencyProperty.Register(nameof(ImageStretch), typeof(Stretch), typeof(MarkdownConfig), new PropertyMetadata(Stretch.Uniform, OnPropertyChanged));

    // ── Table ───────────────────────────────────────────────────────

    /// <summary>Identifies the <see cref="TableHeadingBackground"/> dependency property.</summary>
    public static readonly DependencyProperty TableHeadingBackgroundProperty =
        DependencyProperty.Register(nameof(TableHeadingBackground), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="TableBorderBrush"/> dependency property.</summary>
    public static readonly DependencyProperty TableBorderBrushProperty =
        DependencyProperty.Register(nameof(TableBorderBrush), typeof(Brush), typeof(MarkdownConfig), new PropertyMetadata(null, OnPropertyChanged));

    /// <summary>Identifies the <see cref="TableBorderThickness"/> dependency property.</summary>
    public static readonly DependencyProperty TableBorderThicknessProperty =
        DependencyProperty.Register(nameof(TableBorderThickness), typeof(double), typeof(MarkdownConfig), new PropertyMetadata(1d, OnPropertyChanged));

    /// <summary>Identifies the <see cref="TableCellPadding"/> dependency property.</summary>
    public static readonly DependencyProperty TableCellPaddingProperty =
        DependencyProperty.Register(nameof(TableCellPadding), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(4), OnPropertyChanged));

    /// <summary>Identifies the <see cref="TableMargin"/> dependency property.</summary>
    public static readonly DependencyProperty TableMarginProperty =
        DependencyProperty.Register(nameof(TableMargin), typeof(Thickness), typeof(MarkdownConfig), new PropertyMetadata(new Thickness(0, 8, 0, 8), OnPropertyChanged));

    /// <summary>Identifies the <see cref="TableCornerRadius"/> dependency property.</summary>
    public static readonly DependencyProperty TableCornerRadiusProperty =
        DependencyProperty.Register(nameof(TableCornerRadius), typeof(CornerRadius), typeof(MarkdownConfig), new PropertyMetadata(new CornerRadius(4), OnPropertyChanged));

    // ── CLR property wrappers ────────────────────────────────────────

    /// <summary>Gets or sets the font size for H1 headings.</summary>
    public double H1FontSize { get => (double)GetValue(H1FontSizeProperty); set => SetValue(H1FontSizeProperty, value); }
    /// <summary>Gets or sets the font size for H2 headings.</summary>
    public double H2FontSize { get => (double)GetValue(H2FontSizeProperty); set => SetValue(H2FontSizeProperty, value); }
    /// <summary>Gets or sets the font size for H3 headings.</summary>
    public double H3FontSize { get => (double)GetValue(H3FontSizeProperty); set => SetValue(H3FontSizeProperty, value); }
    /// <summary>Gets or sets the font size for H4 headings.</summary>
    public double H4FontSize { get => (double)GetValue(H4FontSizeProperty); set => SetValue(H4FontSizeProperty, value); }
    /// <summary>Gets or sets the font size for H5 headings.</summary>
    public double H5FontSize { get => (double)GetValue(H5FontSizeProperty); set => SetValue(H5FontSizeProperty, value); }
    /// <summary>Gets or sets the font size for H6 headings.</summary>
    public double H6FontSize { get => (double)GetValue(H6FontSizeProperty); set => SetValue(H6FontSizeProperty, value); }
    /// <summary>Gets or sets the foreground brush for H1 headings.</summary>
    public Brush? H1Foreground { get => (Brush?)GetValue(H1ForegroundProperty); set => SetValue(H1ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H2 headings.</summary>
    public Brush? H2Foreground { get => (Brush?)GetValue(H2ForegroundProperty); set => SetValue(H2ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H3 headings.</summary>
    public Brush? H3Foreground { get => (Brush?)GetValue(H3ForegroundProperty); set => SetValue(H3ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H4 headings.</summary>
    public Brush? H4Foreground { get => (Brush?)GetValue(H4ForegroundProperty); set => SetValue(H4ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H5 headings.</summary>
    public Brush? H5Foreground { get => (Brush?)GetValue(H5ForegroundProperty); set => SetValue(H5ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H6 headings.</summary>
    public Brush? H6Foreground { get => (Brush?)GetValue(H6ForegroundProperty); set => SetValue(H6ForegroundProperty, value); }
    /// <summary>Gets or sets the font weight for H1 headings.</summary>
    public FontWeight H1FontWeight { get => (FontWeight)GetValue(H1FontWeightProperty); set => SetValue(H1FontWeightProperty, value); }
    /// <summary>Gets or sets the font weight for H2 headings.</summary>
    public FontWeight H2FontWeight { get => (FontWeight)GetValue(H2FontWeightProperty); set => SetValue(H2FontWeightProperty, value); }
    /// <summary>Gets or sets the font weight for H3 headings.</summary>
    public FontWeight H3FontWeight { get => (FontWeight)GetValue(H3FontWeightProperty); set => SetValue(H3FontWeightProperty, value); }
    /// <summary>Gets or sets the font weight for H4 headings.</summary>
    public FontWeight H4FontWeight { get => (FontWeight)GetValue(H4FontWeightProperty); set => SetValue(H4FontWeightProperty, value); }
    /// <summary>Gets or sets the font weight for H5 headings.</summary>
    public FontWeight H5FontWeight { get => (FontWeight)GetValue(H5FontWeightProperty); set => SetValue(H5FontWeightProperty, value); }
    /// <summary>Gets or sets the font weight for H6 headings.</summary>
    public FontWeight H6FontWeight { get => (FontWeight)GetValue(H6FontWeightProperty); set => SetValue(H6FontWeightProperty, value); }
    /// <summary>Gets or sets the margin for H1 headings.</summary>
    public Thickness H1Margin { get => (Thickness)GetValue(H1MarginProperty); set => SetValue(H1MarginProperty, value); }
    /// <summary>Gets or sets the margin for H2 headings.</summary>
    public Thickness H2Margin { get => (Thickness)GetValue(H2MarginProperty); set => SetValue(H2MarginProperty, value); }
    /// <summary>Gets or sets the margin for H3 headings.</summary>
    public Thickness H3Margin { get => (Thickness)GetValue(H3MarginProperty); set => SetValue(H3MarginProperty, value); }
    /// <summary>Gets or sets the margin for H4 headings.</summary>
    public Thickness H4Margin { get => (Thickness)GetValue(H4MarginProperty); set => SetValue(H4MarginProperty, value); }
    /// <summary>Gets or sets the margin for H5 headings.</summary>
    public Thickness H5Margin { get => (Thickness)GetValue(H5MarginProperty); set => SetValue(H5MarginProperty, value); }
    /// <summary>Gets or sets the margin for H6 headings.</summary>
    public Thickness H6Margin { get => (Thickness)GetValue(H6MarginProperty); set => SetValue(H6MarginProperty, value); }
    /// <summary>Gets or sets the background brush for inline code.</summary>
    public Brush? InlineCodeBackground { get => (Brush?)GetValue(InlineCodeBackgroundProperty); set => SetValue(InlineCodeBackgroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for inline code.</summary>
    public Brush? InlineCodeForeground { get => (Brush?)GetValue(InlineCodeForegroundProperty); set => SetValue(InlineCodeForegroundProperty, value); }
    /// <summary>Gets or sets the border brush for inline code.</summary>
    public Brush? InlineCodeBorderBrush { get => (Brush?)GetValue(InlineCodeBorderBrushProperty); set => SetValue(InlineCodeBorderBrushProperty, value); }
    /// <summary>Gets or sets the border thickness for inline code.</summary>
    public Thickness InlineCodeBorderThickness { get => (Thickness)GetValue(InlineCodeBorderThicknessProperty); set => SetValue(InlineCodeBorderThicknessProperty, value); }
    /// <summary>Gets or sets the corner radius for inline code.</summary>
    public CornerRadius InlineCodeCornerRadius { get => (CornerRadius)GetValue(InlineCodeCornerRadiusProperty); set => SetValue(InlineCodeCornerRadiusProperty, value); }
    /// <summary>Gets or sets the padding for inline code.</summary>
    public Thickness InlineCodePadding { get => (Thickness)GetValue(InlineCodePaddingProperty); set => SetValue(InlineCodePaddingProperty, value); }
    /// <summary>Gets or sets the font size for inline code.</summary>
    public double InlineCodeFontSize { get => (double)GetValue(InlineCodeFontSizeProperty); set => SetValue(InlineCodeFontSizeProperty, value); }
    /// <summary>Gets or sets the font weight for inline code.</summary>
    public FontWeight InlineCodeFontWeight { get => (FontWeight)GetValue(InlineCodeFontWeightProperty); set => SetValue(InlineCodeFontWeightProperty, value); }
    /// <summary>Gets or sets the font weight used for bold text.</summary>
    public FontWeight BoldFontWeight { get => (FontWeight)GetValue(BoldFontWeightProperty); set => SetValue(BoldFontWeightProperty, value); }
    /// <summary>Gets or sets the background brush for code blocks.</summary>
    public Brush? CodeBlockBackground { get => (Brush?)GetValue(CodeBlockBackgroundProperty); set => SetValue(CodeBlockBackgroundProperty, value); }
    /// <summary>Gets or sets the border brush for code blocks.</summary>
    public Brush? CodeBlockBorderBrush { get => (Brush?)GetValue(CodeBlockBorderBrushProperty); set => SetValue(CodeBlockBorderBrushProperty, value); }
    /// <summary>Gets or sets the foreground brush for code blocks.</summary>
    public Brush? CodeBlockForeground { get => (Brush?)GetValue(CodeBlockForegroundProperty); set => SetValue(CodeBlockForegroundProperty, value); }
    /// <summary>Gets or sets the border thickness for code blocks.</summary>
    public Thickness CodeBlockBorderThickness { get => (Thickness)GetValue(CodeBlockBorderThicknessProperty); set => SetValue(CodeBlockBorderThicknessProperty, value); }
    /// <summary>Gets or sets the padding for code blocks.</summary>
    public Thickness CodeBlockPadding { get => (Thickness)GetValue(CodeBlockPaddingProperty); set => SetValue(CodeBlockPaddingProperty, value); }
    /// <summary>Gets or sets the margin for code blocks.</summary>
    public Thickness CodeBlockMargin { get => (Thickness)GetValue(CodeBlockMarginProperty); set => SetValue(CodeBlockMarginProperty, value); }
    /// <summary>Gets or sets the font family for code blocks.</summary>
    public FontFamily CodeBlockFontFamily { get => (FontFamily)GetValue(CodeBlockFontFamilyProperty); set => SetValue(CodeBlockFontFamilyProperty, value); }
    /// <summary>Gets or sets the corner radius for code blocks.</summary>
    public CornerRadius CodeBlockCornerRadius { get => (CornerRadius)GetValue(CodeBlockCornerRadiusProperty); set => SetValue(CodeBlockCornerRadiusProperty, value); }
    /// <summary>Gets or sets the brush for horizontal rules.</summary>
    public Brush? HorizontalRuleBrush { get => (Brush?)GetValue(HorizontalRuleBrushProperty); set => SetValue(HorizontalRuleBrushProperty, value); }
    /// <summary>Gets or sets the thickness of horizontal rules.</summary>
    public double HorizontalRuleThickness { get => (double)GetValue(HorizontalRuleThicknessProperty); set => SetValue(HorizontalRuleThicknessProperty, value); }
    /// <summary>Gets or sets the margin around horizontal rules.</summary>
    public Thickness HorizontalRuleMargin { get => (Thickness)GetValue(HorizontalRuleMarginProperty); set => SetValue(HorizontalRuleMarginProperty, value); }
    /// <summary>Gets or sets the foreground brush for hyperlinks.</summary>
    public Brush? LinkForeground { get => (Brush?)GetValue(LinkForegroundProperty); set => SetValue(LinkForegroundProperty, value); }
    /// <summary>Gets or sets the margin for paragraphs.</summary>
    public Thickness ParagraphMargin { get => (Thickness)GetValue(ParagraphMarginProperty); set => SetValue(ParagraphMarginProperty, value); }
    /// <summary>Gets or sets the line height for paragraphs.</summary>
    public double ParagraphLineHeight { get => (double)GetValue(ParagraphLineHeightProperty); set => SetValue(ParagraphLineHeightProperty, value); }
    /// <summary>Gets or sets the spacing after list bullets.</summary>
    public double ListBulletSpacing { get => (double)GetValue(ListBulletSpacingProperty); set => SetValue(ListBulletSpacingProperty, value); }
    /// <summary>Gets or sets the gutter width for list indentation.</summary>
    public double ListGutterWidth { get => (double)GetValue(ListGutterWidthProperty); set => SetValue(ListGutterWidthProperty, value); }
    /// <summary>Gets or sets the background brush for block quotes.</summary>
    public Brush? QuoteBackground { get => (Brush?)GetValue(QuoteBackgroundProperty); set => SetValue(QuoteBackgroundProperty, value); }
    /// <summary>Gets or sets the border brush for block quotes.</summary>
    public Brush? QuoteBorderBrush { get => (Brush?)GetValue(QuoteBorderBrushProperty); set => SetValue(QuoteBorderBrushProperty, value); }
    /// <summary>Gets or sets the foreground brush for block quotes.</summary>
    public Brush? QuoteForeground { get => (Brush?)GetValue(QuoteForegroundProperty); set => SetValue(QuoteForegroundProperty, value); }
    /// <summary>Gets or sets the border thickness for block quotes.</summary>
    public Thickness QuoteBorderThickness { get => (Thickness)GetValue(QuoteBorderThicknessProperty); set => SetValue(QuoteBorderThicknessProperty, value); }
    /// <summary>Gets or sets the margin for block quotes.</summary>
    public Thickness QuoteMargin { get => (Thickness)GetValue(QuoteMarginProperty); set => SetValue(QuoteMarginProperty, value); }
    /// <summary>Gets or sets the padding for block quotes.</summary>
    public Thickness QuotePadding { get => (Thickness)GetValue(QuotePaddingProperty); set => SetValue(QuotePaddingProperty, value); }
    /// <summary>Gets or sets the margin for the quote bar indicator.</summary>
    public Thickness QuoteBarMargin { get => (Thickness)GetValue(QuoteBarMarginProperty); set => SetValue(QuoteBarMarginProperty, value); }
    /// <summary>Gets or sets the corner radius for block quotes.</summary>
    public CornerRadius QuoteCornerRadius { get => (CornerRadius)GetValue(QuoteCornerRadiusProperty); set => SetValue(QuoteCornerRadiusProperty, value); }
    /// <summary>Gets or sets the maximum width for images.</summary>
    public double ImageMaxWidth { get => (double)GetValue(ImageMaxWidthProperty); set => SetValue(ImageMaxWidthProperty, value); }
    /// <summary>Gets or sets the maximum height for images.</summary>
    public double ImageMaxHeight { get => (double)GetValue(ImageMaxHeightProperty); set => SetValue(ImageMaxHeightProperty, value); }
    /// <summary>Gets or sets the stretch mode for images.</summary>
    public Stretch ImageStretch { get => (Stretch)GetValue(ImageStretchProperty); set => SetValue(ImageStretchProperty, value); }
    /// <summary>Gets or sets the background brush for table headings.</summary>
    public Brush? TableHeadingBackground { get => (Brush?)GetValue(TableHeadingBackgroundProperty); set => SetValue(TableHeadingBackgroundProperty, value); }
    /// <summary>Gets or sets the border brush for tables.</summary>
    public Brush? TableBorderBrush { get => (Brush?)GetValue(TableBorderBrushProperty); set => SetValue(TableBorderBrushProperty, value); }
    /// <summary>Gets or sets the border thickness for tables.</summary>
    public double TableBorderThickness { get => (double)GetValue(TableBorderThicknessProperty); set => SetValue(TableBorderThicknessProperty, value); }
    /// <summary>Gets or sets the cell padding for tables.</summary>
    public Thickness TableCellPadding { get => (Thickness)GetValue(TableCellPaddingProperty); set => SetValue(TableCellPaddingProperty, value); }
    /// <summary>Gets or sets the margin for tables.</summary>
    public Thickness TableMargin { get => (Thickness)GetValue(TableMarginProperty); set => SetValue(TableMarginProperty, value); }
    /// <summary>Gets or sets the corner radius for tables.</summary>
    public CornerRadius TableCornerRadius { get => (CornerRadius)GetValue(TableCornerRadiusProperty); set => SetValue(TableCornerRadiusProperty, value); }
}
