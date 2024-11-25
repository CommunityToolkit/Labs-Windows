// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;
using Markdig;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock;

[TemplatePart(Name = MarkdownContainerName, Type = typeof(Grid))]
public partial class MarkdownTextBlock : Control
{
    private const string MarkdownContainerName = "MarkdownContainer";
    private Grid? _container;
    private MarkdownPipeline _pipeline;
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
        _pipeline = new MarkdownPipelineBuilder()
            .UseEmphasisExtras()
            .UseAutoLinks()
            .UseTaskLists()
            .UsePipeTables()
            .Build();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
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
                _renderer = new WinUIRenderer(_document, Config, this);
            }
            _pipeline.Setup(_renderer);
            ApplyText(false);
        }
    }
}
