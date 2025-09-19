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

public sealed class MarkdownThemes : DependencyObject
{
    internal static MarkdownThemes Default { get; } = new();

    public Thickness Padding { get; set; } = new(8);

    public Thickness InternalMargin { get; set; } = new(4);

    public CornerRadius CornerRadius { get; set; } = new(4);

    public double H1FontSize { get; set; } = 22;

    public double H2FontSize { get; set; } = 20;

    public double H3FontSize { get; set; } = 18;

    public double H4FontSize { get; set; } = 16;

    public double H5FontSize { get; set; } = 14;

    public double H6FontSize { get; set; } = 12;

    public Brush H1Foreground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    public Brush H2Foreground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    public Brush H3Foreground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    public Brush H4Foreground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    public Brush H5Foreground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    public Brush H6Foreground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];

    public FontWeight H1FontWeight { get; set; } = FontWeights.SemiBold;

    public FontWeight H2FontWeight { get; set; } = FontWeights.Normal;

    public FontWeight H3FontWeight { get; set; } = FontWeights.Normal;

    public FontWeight H4FontWeight { get; set;} = FontWeights.Normal;

    public FontWeight H5FontWeight { get; set; } = FontWeights.Normal;

    public FontWeight H6FontWeight { get; set; } = FontWeights.Normal;

    public Thickness H1Margin { get; set; } = new(left: 0, top: 16, right: 0, bottom: 0);
    public Thickness H2Margin { get; set; } = new(left: 0, top: 16, right: 0, bottom: 0);
    public Thickness H3Margin { get; set; } = new(left: 0, top: 16, right: 0, bottom: 0);
    public Thickness H4Margin { get; set; } = new(left: 0, top: 16, right: 0, bottom: 0);
    public Thickness H5Margin { get; set; } = new(left: 0, top: 8, right: 0, bottom: 0);
    public Thickness H6Margin { get; set; } = new(left: 0, top: 8, right: 0, bottom: 0);

    public Brush BorderBrush { get; set; } = new SolidColorBrush(Colors.Gray);

    public Brush TableHeadingBackground { get; set; } = Extensions.GetAccentColorBrush(Windows.UI.ViewManagement.UIColorType.AccentLight3);

    public Brush InlineCodeBackground { get; set; } = (Brush)Application.Current.Resources["ExpanderHeaderBackground"];
    public Brush InlineCodeForeground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];

    public Brush InlineCodeBorderBrush { get; set; } = new SolidColorBrush(Colors.Gray);

    public Thickness InlineCodeBorderThickness { get; set; } = new (1);

    public CornerRadius InlineCodeCornerRadius { get; set; } = new (2);

    public Thickness InlineCodePadding { get; set; } = new(0);

    public double InlineCodeFontSize { get; set; } = 10;

    public FontWeight InlineCodeFontWeight { get; set; } = FontWeights.Normal;

    // Legacy parity properties (new)
    // Code block styling
    public Brush CodeBlockBackground { get; set; } = (Brush)Application.Current.Resources["ExpanderHeaderBackground"];
    public Brush CodeBlockBorderBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    public Thickness CodeBlockBorderThickness { get; set; } = new Thickness(1);
    public Thickness CodeBlockPadding { get; set; } = new Thickness(8);
    public Thickness CodeBlockMargin { get; set; } = new Thickness(0, 8, 0, 8);
    public FontFamily CodeBlockFontFamily { get; set; } = new FontFamily("Consolas");
    public Brush CodeBlockForeground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    public CornerRadius CodeBlockCornerRadius { get; set; } = new CornerRadius(4);

    // Horizontal rule
    public Brush HorizontalRuleBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    public double HorizontalRuleThickness { get; set; } = 1.0;
    public Thickness HorizontalRuleMargin { get; set; } = new Thickness(0, 12, 0, 12);

    // Link styling
    public Brush LinkForeground { get; set; } = (Brush)Application.Current.Resources["AccentTextFillColorPrimaryBrush"] ?? new SolidColorBrush(Colors.DodgerBlue);

    // Paragraph / list
    public Thickness ParagraphMargin { get; set; } = new Thickness(0, 8, 0, 8);
    public double ParagraphLineHeight { get; set; } = 0; // 0 = default
    public double ListBulletSpacing { get; set; } = 4; // spaces after bullet
    public double ListGutterWidth { get; set; } = 30; // indent delta per level
    public Thickness ListMargin { get; set; } = new Thickness(0, 4, 0, 4);

    // Quote styling
    public Brush QuoteBackground { get; set; } = new SolidColorBrush(Colors.Transparent);
    public Brush QuoteBorderBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    public Thickness QuoteBorderThickness { get; set; } = new Thickness(4, 0, 0, 0);
    public Brush QuoteForeground { get; set; } = (Brush)Application.Current.Resources["TextFillColorPrimaryBrush"];
    public Thickness QuoteMargin { get; set; } = new Thickness(0, 4, 0, 4);
    public Thickness QuotePadding { get; set; } = new Thickness(4);
    public CornerRadius QuoteCornerRadius { get; set; } = new CornerRadius(4);

    // Image styling
    public double ImageMaxWidth { get; set; } = 0; // 0 = no constraint
    public double ImageMaxHeight { get; set; } = 0;
    public Stretch ImageStretch { get; set; } = Stretch.Uniform;

    // Table styling
    public Brush TableBorderBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    public double TableBorderThickness { get; set; } = 1;
    public Thickness TableCellPadding { get; set; } = new Thickness(4);
    public Thickness TableMargin { get; set; } = new Thickness(0, 10, 0, 10);

    // YAML / not currently used - placeholders for parity
    public Brush YamlBorderBrush { get; set; } = new SolidColorBrush(Colors.Gray);
    public Thickness YamlBorderThickness { get; set; } = new Thickness(1);
}
