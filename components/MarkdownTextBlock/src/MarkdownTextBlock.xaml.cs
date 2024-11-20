// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers.Extensions;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers.Inlines;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;
using Markdig;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock;

[TemplatePart(Name = MarkdownContainerName, Type = typeof(Grid))]
public partial class MarkdownTextBlock : Control
{
    private const string MarkdownContainerName = "MarkdownContainer";
    private Grid? _container;
    private MarkdownPipeline _pipeline = null!;
    private MyFlowDocument _document;
    private WinUIRenderer? _renderer;

    private static readonly DependencyProperty ConfigProperty = DependencyProperty.Register(
        nameof(Config),
        typeof(MarkdownConfig),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null, OnConfigChanged)
    );

    private static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(null, OnTextChanged));

    public MarkdownConfig Config
    {
        get => (MarkdownConfig)GetValue(ConfigProperty);
        set => SetValue(ConfigProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

#region Built in Extensions

    private static readonly DependencyProperty UseEmphasisExtrasProperty = DependencyProperty.Register(
        nameof(UseEmphasisExtras),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));
    public bool UseEmphasisExtras
    {
        get => (bool)GetValue(UseEmphasisExtrasProperty);
        set => SetValue(UseEmphasisExtrasProperty, value);
    }

    private static readonly DependencyProperty UsePipeTablesProperty = DependencyProperty.Register(
        nameof(UsePipeTables),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));
    public bool UsePipeTables
    {
        get => (bool)GetValue(UsePipeTablesProperty);
        set => SetValue(UsePipeTablesProperty, value);
    }

    private static readonly DependencyProperty UseListExtrasProperty = DependencyProperty.Register(
        nameof(UseListExtras),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));
    public bool UseListExtras
    {
        get => (bool)GetValue(UseListExtrasProperty);
        set => SetValue(UseListExtrasProperty, value);
    }

    private static readonly DependencyProperty UseTaskListsProperty = DependencyProperty.Register(
        nameof(UseTaskLists),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));
    public bool UseTaskLists
    {
        get => (bool)GetValue(UseTaskListsProperty);
        set => SetValue(UseTaskListsProperty, value);
    }

    private static readonly DependencyProperty UseAutoLinksProperty = DependencyProperty.Register(
        nameof(UseAutoLinks),
        typeof(bool),
        typeof(MarkdownTextBlock),
        new PropertyMetadata(false));
    public bool UseAutoLinks
    {
        get => (bool)GetValue(UseAutoLinksProperty);
        set => SetValue(UseAutoLinksProperty, value);
    }

    #endregion

    private static void OnConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownTextBlock self && e.NewValue != null)
        {
            self.ApplyConfig(self.Config);
        }
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownTextBlock self && e.NewValue != null)
        {
            self.ApplyText(true);
        }
    }

    public MarkdownTextBlock()
    {
        this.DefaultStyleKey = typeof(MarkdownTextBlock);
        _document = new MyFlowDocument();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var pipelineBuilder = new MarkdownPipelineBuilder();

        // NOTE: Order matters here
        if (UseEmphasisExtras) pipelineBuilder = pipelineBuilder.UseEmphasisExtras();
        if (UsePipeTables) pipelineBuilder = pipelineBuilder.UsePipeTables();
        if (UseListExtras) pipelineBuilder = pipelineBuilder.UseListExtras();
        if (UseTaskLists) pipelineBuilder = pipelineBuilder.UseTaskLists();
        if (UseAutoLinks) pipelineBuilder = pipelineBuilder.UseAutoLinks();

        _pipeline = pipelineBuilder.Build();

        _container = (Grid)GetTemplateChild(MarkdownContainerName);
        _container.Children.Clear();
        _container.Children.Add(_document.RichTextBlock);
        Build();
    }

    private void ApplyConfig(MarkdownConfig config)
    {
        if (_renderer != null)
        {
            _renderer.Config = config;
        }
    }

    private void ApplyText(bool rerender)
    {
        if (_renderer != null)
        {
            if (rerender)
            {
                _renderer.ReloadDocument();
            }

            if (!string.IsNullOrEmpty(Text))
            {
                var markdown = Markdown.Parse(Text, _pipeline);
                _renderer.Render(markdown);
            }
        }
    }

    private void Build()
    {
        if (Config != null)
        {
            if (_renderer == null)
            {
                _renderer = new WinUIRenderer(_document, Config);

                // Default block renderers
                _renderer.ObjectRenderers.Add(new CodeBlockRenderer());
                _renderer.ObjectRenderers.Add(new ListRenderer());
                _renderer.ObjectRenderers.Add(new HeadingRenderer());
                _renderer.ObjectRenderers.Add(new ParagraphRenderer());
                _renderer.ObjectRenderers.Add(new QuoteBlockRenderer());
                _renderer.ObjectRenderers.Add(new ThematicBreakRenderer());
                _renderer.ObjectRenderers.Add(new HtmlBlockRenderer());

                // Default inline renderers
                if (UseAutoLinks) _renderer.ObjectRenderers.Add(new AutoLinkInlineRenderer());
                _renderer.ObjectRenderers.Add(new CodeInlineRenderer());
                _renderer.ObjectRenderers.Add(new DelimiterInlineRenderer());
                _renderer.ObjectRenderers.Add(new EmphasisInlineRenderer());
                _renderer.ObjectRenderers.Add(new HtmlEntityInlineRenderer());
                _renderer.ObjectRenderers.Add(new LineBreakInlineRenderer());
                _renderer.ObjectRenderers.Add(new LinkInlineRenderer());
                _renderer.ObjectRenderers.Add(new LiteralInlineRenderer());
                _renderer.ObjectRenderers.Add(new ContainerInlineRenderer());

                // Extension renderers
                if (UsePipeTables) _renderer.ObjectRenderers.Add(new TableRenderer());
                if (UseTaskLists) _renderer.ObjectRenderers.Add(new TaskListRenderer());
                _renderer.ObjectRenderers.Add(new HtmlInlineRenderer());
            }
            _pipeline.Setup(_renderer);
            ApplyText(false);
        }
    }
}
