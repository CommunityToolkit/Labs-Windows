using Markdig.Extensions.TaskLists;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Media3D;

namespace CommunityToolkit.Labs.WinUI.MarkdigUI.TextElements;

internal class MyTaskListCheckBox : IAddChild
{
    private TaskList _taskList;
    public TextElement TextElement { get; private set; }

    public MyTaskListCheckBox(TaskList taskList)
    {
        _taskList = taskList;
        var grid = new Grid();
        CompositeTransform3D transform = new CompositeTransform3D();
        transform.TranslateY = 2;
        grid.Transform3D = transform;
        grid.Width = 16;
        grid.Height = 16;
        grid.Margin = new Thickness(2, 0, 2, 0);
        grid.BorderThickness = new Thickness(1);
        grid.BorderBrush = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Gray);
        FontIcon icon = new FontIcon();
        icon.FontSize = 16;
        icon.HorizontalAlignment = HorizontalAlignment.Center;
        icon.VerticalAlignment = VerticalAlignment.Center;
        icon.Glyph = "\uE73E";
        grid.Children.Add(taskList.Checked ? icon : new TextBlock());
        grid.Padding = new Thickness(0);
        grid.CornerRadius = new CornerRadius(2);
        var inlineUIContainer = new InlineUIContainer();
        inlineUIContainer.Child = grid;
        TextElement = inlineUIContainer;
    }

    public void AddChild(IAddChild child)
    {
    }
}
