// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers.Extensions;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers.Inlines;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;
using Markdig;
using Markdig.Syntax;

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

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public MarkdownDocument? MarkdownDocument
    {
        get => (MarkdownDocument)GetValue(MarkdownDocumentProperty);
        private set => SetValue(MarkdownDocumentProperty, value);
    }
    
    public event EventHandler<LinkClickedEventArgs>? OnLinkClicked;

    internal void RaiseLinkClickedEvent(Uri uri) => OnLinkClicked?.Invoke(this, new LinkClickedEventArgs(uri));

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
        if (UseSoftlineBreakAsHardlineBreak) pipelineBuilder = pipelineBuilder.UseSoftlineBreakAsHardlineBreak();

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
                this.MarkdownDocument = Markdown.Parse(Text, _pipeline);
                OnMarkdownParsed?.Invoke(this, new MarkdownParsedEventArgs(this.MarkdownDocument));
                _renderer.Render(this.MarkdownDocument);
            }
        }
    }

    private void Build()
    {
        if (Config != null)
        {
            if (_renderer == null)
            {
                _renderer = new WinUIRenderer(_document, Config, this);

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
