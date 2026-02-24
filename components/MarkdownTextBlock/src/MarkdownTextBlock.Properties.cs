// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax;

#if !WINAPPSDK
using FontWeight = Windows.UI.Text.FontWeight;
using FontWeights = Windows.UI.Text.FontWeights;
#else
using FontWeight = Windows.UI.Text.FontWeight;
using FontWeights = Microsoft.UI.Text.FontWeights;
#endif

namespace CommunityToolkit.WinUI.Controls;

public partial class MarkdownTextBlock
{
    /// <summary>
    /// Identifies the <see cref="BaseUrl"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BaseUrlProperty = DependencyProperty.Register(
        nameof(BaseUrl),
        typeof(string),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null)
    );

    /// <summary>
    /// Identifies the <see cref="ImageProvider"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ImageProviderProperty = DependencyProperty.Register(
        nameof(ImageProvider),
        typeof(IImageProvider),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null)
    );

    /// <summary>
    /// Identifies the <see cref="SVGRenderer"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SVGRendererProperty = DependencyProperty.Register(
        nameof(SVGRenderer),
        typeof(ISVGRenderer),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null)
    );

    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null, OnTextChanged));

    /// <summary>
    /// Identifies the <see cref="UseEmphasisExtras"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty UseEmphasisExtrasProperty = DependencyProperty.Register(
        nameof(UseEmphasisExtras),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="UsePipeTables"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty UsePipeTablesProperty = DependencyProperty.Register(
        nameof(UsePipeTables),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="UseListExtras"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty UseListExtrasProperty = DependencyProperty.Register(
        nameof(UseListExtras),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="UseTaskLists"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty UseTaskListsProperty = DependencyProperty.Register(
        nameof(UseTaskLists),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="UseAutoLinks"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty UseAutoLinksProperty = DependencyProperty.Register(
        nameof(UseAutoLinks),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="DisableHtmlProperty"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty DisableHtmlProperty = DependencyProperty.Register(
        nameof(DisableHtmlProperty),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="DisableLinksProperty"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty DisableLinksProperty = DependencyProperty.Register(
        nameof(DisableLinksProperty),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="UseSoftlineBreakAsHardlineBreak"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty UseSoftlineBreakAsHardlineBreakProperty = DependencyProperty.Register(
        nameof(UseSoftlineBreakAsHardlineBreak),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));

    /// <summary>
    /// Identifies the <see cref="MarkdownDocument"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty MarkdownDocumentProperty = DependencyProperty.Register(
        nameof(MarkdownDocument),
        typeof(MarkdownDocument),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="IsTextSelectionEnabled"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty IsTextSelectionEnabledProperty = DependencyProperty.Register(
        nameof(IsTextSelectionEnabled),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false, OnIsTextSelectionEnabledChanged));

    private static void OnIsTextSelectionEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownTextBlock mtb && mtb._document != null)
        {
            mtb._document.RichTextBlock.IsTextSelectionEnabled = (bool)e.NewValue;
        }
    }

    // ── Theme DPs ───────────────────────────────────────────────────────
    // Defaults come from the XAML style. C# defaults are type-appropriate
    // fallbacks only. The shared property-changed callback batches multiple
    // simultaneous DP updates (e.g. theme switch) into a single re-render.

    private static void OnThemePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownTextBlock self && !self._themePropertyChangeQueued)
        {
            self._themePropertyChangeQueued = true;
            self.DispatcherQueue.TryEnqueue(() =>
            {
                self._themePropertyChangeQueued = false;
                self.ApplyText(true);
            });
        }
    }

    // ── Heading font sizes ──────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="H1FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H1FontSizeProperty =
        DependencyProperty.Register(nameof(H1FontSize), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H2FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H2FontSizeProperty =
        DependencyProperty.Register(nameof(H2FontSize), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H3FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H3FontSizeProperty =
        DependencyProperty.Register(nameof(H3FontSize), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H4FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H4FontSizeProperty =
        DependencyProperty.Register(nameof(H4FontSize), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H5FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H5FontSizeProperty =
        DependencyProperty.Register(nameof(H5FontSize), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H6FontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H6FontSizeProperty =
        DependencyProperty.Register(nameof(H6FontSize), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    // ── Heading foregrounds ─────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="H1Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H1ForegroundProperty =
        DependencyProperty.Register(nameof(H1Foreground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H2Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H2ForegroundProperty =
        DependencyProperty.Register(nameof(H2Foreground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H3Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H3ForegroundProperty =
        DependencyProperty.Register(nameof(H3Foreground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H4Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H4ForegroundProperty =
        DependencyProperty.Register(nameof(H4Foreground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H5Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H5ForegroundProperty =
        DependencyProperty.Register(nameof(H5Foreground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H6Foreground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H6ForegroundProperty =
        DependencyProperty.Register(nameof(H6Foreground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    // ── Heading font weights ────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="H1FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H1FontWeightProperty =
        DependencyProperty.Register(nameof(H1FontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H2FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H2FontWeightProperty =
        DependencyProperty.Register(nameof(H2FontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H3FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H3FontWeightProperty =
        DependencyProperty.Register(nameof(H3FontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H4FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H4FontWeightProperty =
        DependencyProperty.Register(nameof(H4FontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H5FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H5FontWeightProperty =
        DependencyProperty.Register(nameof(H5FontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H6FontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H6FontWeightProperty =
        DependencyProperty.Register(nameof(H6FontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    // ── Heading margins ─────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="H1Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H1MarginProperty =
        DependencyProperty.Register(nameof(H1Margin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H2Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H2MarginProperty =
        DependencyProperty.Register(nameof(H2Margin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H3Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H3MarginProperty =
        DependencyProperty.Register(nameof(H3Margin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H4Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H4MarginProperty =
        DependencyProperty.Register(nameof(H4Margin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H5Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H5MarginProperty =
        DependencyProperty.Register(nameof(H5Margin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="H6Margin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty H6MarginProperty =
        DependencyProperty.Register(nameof(H6Margin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    // ── Inline code ─────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="InlineCodeBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodeBackgroundProperty =
        DependencyProperty.Register(nameof(InlineCodeBackground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="InlineCodeForeground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodeForegroundProperty =
        DependencyProperty.Register(nameof(InlineCodeForeground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="InlineCodeBorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodeBorderBrushProperty =
        DependencyProperty.Register(nameof(InlineCodeBorderBrush), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="InlineCodeBorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodeBorderThicknessProperty =
        DependencyProperty.Register(nameof(InlineCodeBorderThickness), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="InlineCodeCornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodeCornerRadiusProperty =
        DependencyProperty.Register(nameof(InlineCodeCornerRadius), typeof(CornerRadius), typeof(MarkdownTextBlock), new PropertyMetadata(default(CornerRadius), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="InlineCodePadding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodePaddingProperty =
        DependencyProperty.Register(nameof(InlineCodePadding), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="InlineCodeFontSize"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodeFontSizeProperty =
        DependencyProperty.Register(nameof(InlineCodeFontSize), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="InlineCodeFontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty InlineCodeFontWeightProperty =
        DependencyProperty.Register(nameof(InlineCodeFontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    // ── Bold ────────────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="BoldFontWeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BoldFontWeightProperty =
        DependencyProperty.Register(nameof(BoldFontWeight), typeof(FontWeight), typeof(MarkdownTextBlock), new PropertyMetadata(default(FontWeight), OnThemePropertyChanged));

    // ── Code block ──────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="CodeBlockBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockBackgroundProperty =
        DependencyProperty.Register(nameof(CodeBlockBackground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="CodeBlockBorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockBorderBrushProperty =
        DependencyProperty.Register(nameof(CodeBlockBorderBrush), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="CodeBlockForeground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockForegroundProperty =
        DependencyProperty.Register(nameof(CodeBlockForeground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="CodeBlockBorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockBorderThicknessProperty =
        DependencyProperty.Register(nameof(CodeBlockBorderThickness), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="CodeBlockPadding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockPaddingProperty =
        DependencyProperty.Register(nameof(CodeBlockPadding), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="CodeBlockMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockMarginProperty =
        DependencyProperty.Register(nameof(CodeBlockMargin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="CodeBlockFontFamily"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockFontFamilyProperty =
        DependencyProperty.Register(nameof(CodeBlockFontFamily), typeof(FontFamily), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="CodeBlockCornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CodeBlockCornerRadiusProperty =
        DependencyProperty.Register(nameof(CodeBlockCornerRadius), typeof(CornerRadius), typeof(MarkdownTextBlock), new PropertyMetadata(default(CornerRadius), OnThemePropertyChanged));

    // ── Horizontal rule ─────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="HorizontalRuleBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalRuleBrushProperty =
        DependencyProperty.Register(nameof(HorizontalRuleBrush), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="HorizontalRuleThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalRuleThicknessProperty =
        DependencyProperty.Register(nameof(HorizontalRuleThickness), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="HorizontalRuleMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalRuleMarginProperty =
        DependencyProperty.Register(nameof(HorizontalRuleMargin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="HorizontalRuleX2"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HorizontalRuleX2Property =
        DependencyProperty.Register(nameof(HorizontalRuleX2), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    // ── Link ────────────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="LinkForeground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LinkForegroundProperty =
        DependencyProperty.Register(nameof(LinkForeground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    // ── Paragraph / list ────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="ParagraphMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ParagraphMarginProperty =
        DependencyProperty.Register(nameof(ParagraphMargin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="ParagraphLineHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ParagraphLineHeightProperty =
        DependencyProperty.Register(nameof(ParagraphLineHeight), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="ListBulletSpacing"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ListBulletSpacingProperty =
        DependencyProperty.Register(nameof(ListBulletSpacing), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="ListGutterWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ListGutterWidthProperty =
        DependencyProperty.Register(nameof(ListGutterWidth), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    // ── Quote ───────────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="QuoteBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuoteBackgroundProperty =
        DependencyProperty.Register(nameof(QuoteBackground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="QuoteBorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuoteBorderBrushProperty =
        DependencyProperty.Register(nameof(QuoteBorderBrush), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="QuoteForeground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuoteForegroundProperty =
        DependencyProperty.Register(nameof(QuoteForeground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="QuoteBorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuoteBorderThicknessProperty =
        DependencyProperty.Register(nameof(QuoteBorderThickness), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="QuoteMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuoteMarginProperty =
        DependencyProperty.Register(nameof(QuoteMargin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="QuotePadding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuotePaddingProperty =
        DependencyProperty.Register(nameof(QuotePadding), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="QuoteBarMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuoteBarMarginProperty =
        DependencyProperty.Register(nameof(QuoteBarMargin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="QuoteCornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty QuoteCornerRadiusProperty =
        DependencyProperty.Register(nameof(QuoteCornerRadius), typeof(CornerRadius), typeof(MarkdownTextBlock), new PropertyMetadata(default(CornerRadius), OnThemePropertyChanged));

    // ── Image ───────────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="ImageMaxWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ImageMaxWidthProperty =
        DependencyProperty.Register(nameof(ImageMaxWidth), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="ImageMaxHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ImageMaxHeightProperty =
        DependencyProperty.Register(nameof(ImageMaxHeight), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="ImageStretch"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ImageStretchProperty =
        DependencyProperty.Register(nameof(ImageStretch), typeof(Stretch), typeof(MarkdownTextBlock), new PropertyMetadata(default(Stretch), OnThemePropertyChanged));

    // ── Table ───────────────────────────────────────────────────────────

    /// <summary>
    /// Identifies the <see cref="TableHeadingBackground"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TableHeadingBackgroundProperty =
        DependencyProperty.Register(nameof(TableHeadingBackground), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="TableBorderBrush"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TableBorderBrushProperty =
        DependencyProperty.Register(nameof(TableBorderBrush), typeof(Brush), typeof(MarkdownTextBlock), new PropertyMetadata(null, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="TableBorderThickness"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TableBorderThicknessProperty =
        DependencyProperty.Register(nameof(TableBorderThickness), typeof(double), typeof(MarkdownTextBlock), new PropertyMetadata(0d, OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="TableCellPadding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TableCellPaddingProperty =
        DependencyProperty.Register(nameof(TableCellPadding), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="TableMargin"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TableMarginProperty =
        DependencyProperty.Register(nameof(TableMargin), typeof(Thickness), typeof(MarkdownTextBlock), new PropertyMetadata(default(Thickness), OnThemePropertyChanged));

    /// <summary>
    /// Identifies the <see cref="TableCornerRadius"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TableCornerRadiusProperty =
        DependencyProperty.Register(nameof(TableCornerRadius), typeof(CornerRadius), typeof(MarkdownTextBlock), new PropertyMetadata(default(CornerRadius), OnThemePropertyChanged));

    // ── CLR property wrappers ───────────────────────────────────────────

    /// <summary>Gets or sets the base URL for resolving relative links.</summary>
    public string? BaseUrl
    {
        get => (string?)GetValue(BaseUrlProperty);
        set => SetValue(BaseUrlProperty, value);
    }

    /// <summary>Gets or sets the custom image provider.</summary>
    public IImageProvider? ImageProvider
    {
        get => (IImageProvider?)GetValue(ImageProviderProperty);
        set => SetValue(ImageProviderProperty, value);
    }

    /// <summary>Gets or sets the SVG renderer.</summary>
    public ISVGRenderer? SVGRenderer
    {
        get => (ISVGRenderer?)GetValue(SVGRendererProperty);
        set => SetValue(SVGRendererProperty, value);
    }

    /// <summary>
    /// Gets or sets the markdown text to display.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// If true, adds support for strikethroughs, superscript, subscript, inserted, and marked text.
    /// </summary>
    public bool UseEmphasisExtras
    {
        get => (bool)GetValue(UseEmphasisExtrasProperty);
        set => SetValue(UseEmphasisExtrasProperty, value);
    }

    /// <summary>
    /// If true, adds support for GitHub-style pipe tables.
    /// </summary>
    public bool UsePipeTables
    {
        get => (bool)GetValue(UsePipeTablesProperty);
        set => SetValue(UsePipeTablesProperty, value);
    }

    /// <summary>
    /// If true, adds support for alphabetic and roman numbering in lists.
    /// </summary>
    public bool UseListExtras
    {
        get => (bool)GetValue(UseListExtrasProperty);
        set => SetValue(UseListExtrasProperty, value);
    }

    /// <summary>
    /// If true, adds support for GitHub-style task lists using the [ ] and [x] syntax.
    /// </summary>
    public bool UseTaskLists
    {
        get => (bool)GetValue(UseTaskListsProperty);
        set => SetValue(UseTaskListsProperty, value);
    }

    /// <summary>
    /// If true, parses text that looks like URIs into hyperlinks (e.g. https://...).
    /// </summary>
    public bool UseAutoLinks
    {
        get => (bool)GetValue(UseAutoLinksProperty);
        set => SetValue(UseAutoLinksProperty, value);
    }

    /// <summary>
    /// If true, Disables HTML parsing.
    /// </summary>
    public bool DisableHtml
    {
        get => (bool)GetValue(DisableHtmlProperty);
        set => SetValue(DisableHtmlProperty, value);
    }

    /// <summary>
    /// If true, Disables link parsing.
    /// </summary>
    public bool DisableLinks
    {
        get => (bool)GetValue(DisableLinksProperty);
        set => SetValue(DisableLinksProperty, value);
    }

    /// <summary>
    /// If true, considers single newlines as hardline breaks.
    /// </summary>
    public bool UseSoftlineBreakAsHardlineBreak
    {
        get => (bool)GetValue(UseSoftlineBreakAsHardlineBreakProperty);
        set => SetValue(UseSoftlineBreakAsHardlineBreakProperty, value);
    }

    /// <summary>
    /// Gets the parsed markdown document. May be null if the document has not been parsed yet.
    /// </summary>
    public MarkdownDocument? MarkdownDocument
    {
        get => (MarkdownDocument)GetValue(MarkdownDocumentProperty);
        private set => SetValue(MarkdownDocumentProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether text selection is enabled.
    /// </summary>
    public bool IsTextSelectionEnabled
    {
        get => (bool)GetValue(IsTextSelectionEnabledProperty);
        set => SetValue(IsTextSelectionEnabledProperty, value);
    }

    // ── Theme property CLR wrappers ─────────────────────────────────────

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
    public Brush H1Foreground { get => (Brush)GetValue(H1ForegroundProperty); set => SetValue(H1ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H2 headings.</summary>
    public Brush H2Foreground { get => (Brush)GetValue(H2ForegroundProperty); set => SetValue(H2ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H3 headings.</summary>
    public Brush H3Foreground { get => (Brush)GetValue(H3ForegroundProperty); set => SetValue(H3ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H4 headings.</summary>
    public Brush H4Foreground { get => (Brush)GetValue(H4ForegroundProperty); set => SetValue(H4ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H5 headings.</summary>
    public Brush H5Foreground { get => (Brush)GetValue(H5ForegroundProperty); set => SetValue(H5ForegroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for H6 headings.</summary>
    public Brush H6Foreground { get => (Brush)GetValue(H6ForegroundProperty); set => SetValue(H6ForegroundProperty, value); }

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
    public Brush InlineCodeBackground { get => (Brush)GetValue(InlineCodeBackgroundProperty); set => SetValue(InlineCodeBackgroundProperty, value); }
    /// <summary>Gets or sets the foreground brush for inline code.</summary>
    public Brush InlineCodeForeground { get => (Brush)GetValue(InlineCodeForegroundProperty); set => SetValue(InlineCodeForegroundProperty, value); }
    /// <summary>Gets or sets the border brush for inline code.</summary>
    public Brush InlineCodeBorderBrush { get => (Brush)GetValue(InlineCodeBorderBrushProperty); set => SetValue(InlineCodeBorderBrushProperty, value); }
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
    public Brush CodeBlockBackground { get => (Brush)GetValue(CodeBlockBackgroundProperty); set => SetValue(CodeBlockBackgroundProperty, value); }
    /// <summary>Gets or sets the border brush for code blocks.</summary>
    public Brush CodeBlockBorderBrush { get => (Brush)GetValue(CodeBlockBorderBrushProperty); set => SetValue(CodeBlockBorderBrushProperty, value); }
    /// <summary>Gets or sets the foreground brush for code blocks.</summary>
    public Brush CodeBlockForeground { get => (Brush)GetValue(CodeBlockForegroundProperty); set => SetValue(CodeBlockForegroundProperty, value); }
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
    public Brush HorizontalRuleBrush { get => (Brush)GetValue(HorizontalRuleBrushProperty); set => SetValue(HorizontalRuleBrushProperty, value); }
    /// <summary>Gets or sets the thickness of horizontal rules.</summary>
    public double HorizontalRuleThickness { get => (double)GetValue(HorizontalRuleThicknessProperty); set => SetValue(HorizontalRuleThicknessProperty, value); }
    /// <summary>Gets or sets the margin around horizontal rules.</summary>
    public Thickness HorizontalRuleMargin { get => (Thickness)GetValue(HorizontalRuleMarginProperty); set => SetValue(HorizontalRuleMarginProperty, value); }
    /// <summary>Gets or sets the X2 endpoint coordinate for horizontal rules.</summary>
    public double HorizontalRuleX2 { get => (double)GetValue(HorizontalRuleX2Property); set => SetValue(HorizontalRuleX2Property, value); }

    /// <summary>Gets or sets the foreground brush for hyperlinks.</summary>
    public Brush LinkForeground { get => (Brush)GetValue(LinkForegroundProperty); set => SetValue(LinkForegroundProperty, value); }

    /// <summary>Gets or sets the margin for paragraphs.</summary>
    public Thickness ParagraphMargin { get => (Thickness)GetValue(ParagraphMarginProperty); set => SetValue(ParagraphMarginProperty, value); }
    /// <summary>Gets or sets the line height for paragraphs.</summary>
    public double ParagraphLineHeight { get => (double)GetValue(ParagraphLineHeightProperty); set => SetValue(ParagraphLineHeightProperty, value); }
    /// <summary>Gets or sets the spacing after list bullets.</summary>
    public double ListBulletSpacing { get => (double)GetValue(ListBulletSpacingProperty); set => SetValue(ListBulletSpacingProperty, value); }
    /// <summary>Gets or sets the gutter width for list indentation.</summary>
    public double ListGutterWidth { get => (double)GetValue(ListGutterWidthProperty); set => SetValue(ListGutterWidthProperty, value); }

    /// <summary>Gets or sets the background brush for block quotes.</summary>
    public Brush QuoteBackground { get => (Brush)GetValue(QuoteBackgroundProperty); set => SetValue(QuoteBackgroundProperty, value); }
    /// <summary>Gets or sets the border brush for block quotes.</summary>
    public Brush QuoteBorderBrush { get => (Brush)GetValue(QuoteBorderBrushProperty); set => SetValue(QuoteBorderBrushProperty, value); }
    /// <summary>Gets or sets the foreground brush for block quotes.</summary>
    public Brush QuoteForeground { get => (Brush)GetValue(QuoteForegroundProperty); set => SetValue(QuoteForegroundProperty, value); }
    /// <summary>Gets or sets the border thickness for block quotes.</summary>
    public Thickness QuoteBorderThickness { get => (Thickness)GetValue(QuoteBorderThicknessProperty); set => SetValue(QuoteBorderThicknessProperty, value); }
    /// <summary>Gets or sets the margin for block quotes.</summary>
    public Thickness QuoteMargin { get => (Thickness)GetValue(QuoteMarginProperty); set => SetValue(QuoteMarginProperty, value); }
    /// <summary>Gets or sets the padding for block quotes.</summary>
    public Thickness QuotePadding { get => (Thickness)GetValue(QuotePaddingProperty); set => SetValue(QuotePaddingProperty, value); }
    /// <summary>Gets or sets the corner radius for block quotes.</summary>
    public CornerRadius QuoteCornerRadius { get => (CornerRadius)GetValue(QuoteCornerRadiusProperty); set => SetValue(QuoteCornerRadiusProperty, value); }
    /// <summary>Gets or sets the margin for the quote bar indicator.</summary>
    public Thickness QuoteBarMargin { get => (Thickness)GetValue(QuoteBarMarginProperty); set => SetValue(QuoteBarMarginProperty, value); }

    /// <summary>Gets or sets the maximum width for images.</summary>
    public double ImageMaxWidth { get => (double)GetValue(ImageMaxWidthProperty); set => SetValue(ImageMaxWidthProperty, value); }
    /// <summary>Gets or sets the maximum height for images.</summary>
    public double ImageMaxHeight { get => (double)GetValue(ImageMaxHeightProperty); set => SetValue(ImageMaxHeightProperty, value); }
    /// <summary>Gets or sets the stretch mode for images.</summary>
    public Stretch ImageStretch { get => (Stretch)GetValue(ImageStretchProperty); set => SetValue(ImageStretchProperty, value); }

    /// <summary>Gets or sets the background brush for table headings.</summary>
    public Brush TableHeadingBackground { get => (Brush)GetValue(TableHeadingBackgroundProperty); set => SetValue(TableHeadingBackgroundProperty, value); }
    /// <summary>Gets or sets the border brush for tables.</summary>
    public Brush TableBorderBrush { get => (Brush)GetValue(TableBorderBrushProperty); set => SetValue(TableBorderBrushProperty, value); }
    /// <summary>Gets or sets the border thickness for tables.</summary>
    public double TableBorderThickness { get => (double)GetValue(TableBorderThicknessProperty); set => SetValue(TableBorderThicknessProperty, value); }
    /// <summary>Gets or sets the cell padding for tables.</summary>
    public Thickness TableCellPadding { get => (Thickness)GetValue(TableCellPaddingProperty); set => SetValue(TableCellPaddingProperty, value); }
    /// <summary>Gets or sets the margin for tables.</summary>
    public Thickness TableMargin { get => (Thickness)GetValue(TableMarginProperty); set => SetValue(TableMarginProperty, value); }
    /// <summary>Gets or sets the corner radius for tables.</summary>
    public CornerRadius TableCornerRadius { get => (CornerRadius)GetValue(TableCornerRadiusProperty); set => SetValue(TableCornerRadiusProperty, value); }
}
