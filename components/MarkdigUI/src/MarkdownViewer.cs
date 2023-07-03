namespace CommunityToolkit.Labs.WinUI.MarkdigUI;

public partial class MarkdownViewer : Control
{
    private const string MarkdownContainerName = "MarkdownContainerName";
    private Grid? _container;

    private static readonly DependencyProperty ConfigProperty = DependencyProperty.Register(
        nameof(Config),
        typeof(MarkdownConfig),
        typeof(MarkdownViewer),
        new PropertyMetadata(null, ConfigChanged)
    );

    private static void ConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MarkdownViewer self && e.NewValue != null)
        {
            var uiElement = MarkdownUIBuilder.Build(self.Config);
            self._container?.Children.Clear();
            self._container?.Children.Add(uiElement);
        }
    }

    public MarkdownConfig Config
    {
        get => (MarkdownConfig)GetValue(ConfigProperty);
        set => SetValue(ConfigProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _container = (Grid)GetTemplateChild(MarkdownContainerName);
    }
}
