// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

[TemplatePart(Name = MarkdownContainerName, Type = typeof(Grid))]
public partial class MarkdownViewer : Control
{
    private const string MarkdownContainerName = "MarkdownContainer";
    private Grid? _container;

    private static readonly DependencyProperty ConfigProperty = DependencyProperty.Register(
        nameof(Config),
        typeof(MarkdownConfig),
        typeof(MarkdownViewer),
        new PropertyMetadata(null, OnConfigChanged)
    );

    public MarkdownConfig Config
    {
        get => (MarkdownConfig)GetValue(ConfigProperty);
        set => SetValue(ConfigProperty, value);
    }

    private static void OnConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownViewer self && e.NewValue != null)
        {
            self.Build();
        }
    }

    public MarkdownViewer()
    {
        this.DefaultStyleKey = typeof(MarkdownViewer);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _container = (Grid)GetTemplateChild(MarkdownContainerName);
        Build();
    }

    private void Build()
    {
        if (_container != null)
        {
            var uiElement = MarkdownUIBuilder.Build(Config);
            _container.Children.Clear();
            _container.Children.Add(uiElement);
        }
    }

}
