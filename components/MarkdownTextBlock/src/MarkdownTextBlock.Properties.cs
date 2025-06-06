// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Markdig.Syntax;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock;

public partial class MarkdownTextBlock
{
    /// <summary>
    /// Identifies the <see cref="Config"/> dependency property.
    /// </summary>
    private static readonly DependencyProperty ConfigProperty = DependencyProperty.Register(
        nameof(Config),
        typeof(MarkdownConfig),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null, OnConfigChanged)
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

    public MarkdownConfig Config
    {
        get => (MarkdownConfig)GetValue(ConfigProperty);
        set => SetValue(ConfigProperty, value);
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
}
