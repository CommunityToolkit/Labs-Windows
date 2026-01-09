// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MarkdownTextBlockExperiment.Samples;

[ToolkitSampleOptionsPane(nameof(MarkdownTextBlockCustomThemeSample))]
public partial class ThemeOptionsPane : UserControl
{
    private readonly MarkdownTextBlockCustomThemeSampleBase _sample;
    private bool _isInitializing = true;

    public ThemeOptionsPane(MarkdownTextBlockCustomThemeSampleBase sample)
    {
        _sample = sample;

        this.InitializeComponent();

        // Initialize control values from sample
        LoadValuesFromSample();

        _isInitializing = false;
    }

    private void LoadValuesFromSample()
    {
        // Headings
        H1FontSizeBox.Value = _sample.H1FontSize;
        H2FontSizeBox.Value = _sample.H2FontSize;
        H3FontSizeBox.Value = _sample.H3FontSize;
        H1ColorComboBox.SelectedIndex = _sample.H1ColorIndex;

        // Text Styling
        BoldFontWeightComboBox.SelectedIndex = _sample.BoldFontWeightIndex;

        // Inline Code
        InlineCodeFontSizeBox.Value = _sample.InlineCodeFontSize;
        InlineCodePaddingBox.Value = _sample.InlineCodePadding;
        InlineCodeCornerRadiusBox.Value = _sample.InlineCodeCornerRadius;
        InlineCodeBorderThicknessBox.Value = _sample.InlineCodeBorderThickness;
        InlineCodeColorComboBox.SelectedIndex = _sample.InlineCodeColorIndex;
        InlineCodeBackgroundColorComboBox.SelectedIndex = _sample.InlineCodeBackgroundColorIndex;
        InlineCodeBorderColorComboBox.SelectedIndex = _sample.InlineCodeBorderColorIndex;

        // Code Block
        CodeBlockPaddingBox.Value = _sample.CodeBlockPadding;
        CodeBlockCornerRadiusBox.Value = _sample.CodeBlockCornerRadius;
        CodeBlockBorderThicknessBox.Value = _sample.CodeBlockBorderThickness;
        CodeBlockFontComboBox.SelectedIndex = _sample.CodeBlockFontIndex;
        CodeBlockBackgroundColorComboBox.SelectedIndex = _sample.CodeBlockBackgroundColorIndex;
        CodeBlockBorderColorComboBox.SelectedIndex = _sample.CodeBlockBorderColorIndex;

        // Quote
        QuoteBorderWidthBox.Value = _sample.QuoteBorderWidth;
        QuotePaddingBox.Value = _sample.QuotePadding;
        QuoteCornerRadiusBox.Value = _sample.QuoteCornerRadius;
        QuoteColorComboBox.SelectedIndex = _sample.QuoteColorIndex;

        // Table
        TableCellPaddingBox.Value = _sample.TableCellPadding;
        TableBorderThicknessBox.Value = _sample.TableBorderThickness;

        // Lists
        ListBulletSpacingBox.Value = _sample.ListBulletSpacing;
        ListGutterWidthBox.Value = _sample.ListGutterWidth;

        // Horizontal Rule
        HorizontalRuleThicknessBox.Value = _sample.HorizontalRuleThickness;
        HorizontalRuleMarginBox.Value = _sample.HorizontalRuleMargin;

        // Images
        ImageMaxWidthBox.Value = _sample.ImageMaxWidth;
        ImageMaxHeightBox.Value = _sample.ImageMaxHeight;
        ImageCornerRadiusBox.Value = _sample.ImageCornerRadius;
        ImageStretchComboBox.SelectedIndex = _sample.ImageStretchIndex;
    }

    private void SyncValuesToSample()
    {
        // Headings
        _sample.H1FontSize = H1FontSizeBox.Value;
        _sample.H2FontSize = H2FontSizeBox.Value;
        _sample.H3FontSize = H3FontSizeBox.Value;
        _sample.H1ColorIndex = H1ColorComboBox.SelectedIndex;

        // Text Styling
        _sample.BoldFontWeightIndex = BoldFontWeightComboBox.SelectedIndex;

        // Inline Code
        _sample.InlineCodeFontSize = InlineCodeFontSizeBox.Value;
        _sample.InlineCodePadding = InlineCodePaddingBox.Value;
        _sample.InlineCodeCornerRadius = InlineCodeCornerRadiusBox.Value;
        _sample.InlineCodeBorderThickness = InlineCodeBorderThicknessBox.Value;
        _sample.InlineCodeColorIndex = InlineCodeColorComboBox.SelectedIndex;
        _sample.InlineCodeBackgroundColorIndex = InlineCodeBackgroundColorComboBox.SelectedIndex;
        _sample.InlineCodeBorderColorIndex = InlineCodeBorderColorComboBox.SelectedIndex;

        // Code Block
        _sample.CodeBlockPadding = CodeBlockPaddingBox.Value;
        _sample.CodeBlockCornerRadius = CodeBlockCornerRadiusBox.Value;
        _sample.CodeBlockBorderThickness = CodeBlockBorderThicknessBox.Value;
        _sample.CodeBlockFontIndex = CodeBlockFontComboBox.SelectedIndex;
        _sample.CodeBlockBackgroundColorIndex = CodeBlockBackgroundColorComboBox.SelectedIndex;
        _sample.CodeBlockBorderColorIndex = CodeBlockBorderColorComboBox.SelectedIndex;

        // Quote
        _sample.QuoteBorderWidth = QuoteBorderWidthBox.Value;
        _sample.QuotePadding = QuotePaddingBox.Value;
        _sample.QuoteCornerRadius = QuoteCornerRadiusBox.Value;
        _sample.QuoteColorIndex = QuoteColorComboBox.SelectedIndex;

        // Table
        _sample.TableCellPadding = TableCellPaddingBox.Value;
        _sample.TableBorderThickness = TableBorderThicknessBox.Value;

        // Lists
        _sample.ListBulletSpacing = ListBulletSpacingBox.Value;
        _sample.ListGutterWidth = ListGutterWidthBox.Value;

        // Horizontal Rule
        _sample.HorizontalRuleThickness = HorizontalRuleThicknessBox.Value;
        _sample.HorizontalRuleMargin = HorizontalRuleMarginBox.Value;

        // Images
        _sample.ImageMaxWidth = ImageMaxWidthBox.Value;
        _sample.ImageMaxHeight = ImageMaxHeightBox.Value;
        _sample.ImageCornerRadius = ImageCornerRadiusBox.Value;
        _sample.ImageStretchIndex = ImageStretchComboBox.SelectedIndex;
    }

    private void OnValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
    {
        if (_isInitializing) return;

        SyncValuesToSample();
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isInitializing) return;

        SyncValuesToSample();
    }

    private void ApplyTheme_Click(object sender, RoutedEventArgs e)
    {
        SyncValuesToSample();
        _sample.ApplyTheme();
    }

    private void ResetTheme_Click(object sender, RoutedEventArgs e)
    {
        _sample.ResetToDefaults();

        _isInitializing = true;
        LoadValuesFromSample();
        _isInitializing = false;

        _sample.ApplyTheme();
    }
}
