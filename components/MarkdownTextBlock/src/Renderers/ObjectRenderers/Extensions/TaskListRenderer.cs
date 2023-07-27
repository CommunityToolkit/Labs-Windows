using Markdig.Extensions.TaskLists;
using CommunityToolkit.Labs.WinUI.MarkdownTextBlock.TextElements;

namespace CommunityToolkit.Labs.WinUI.MarkdownTextBlock.Renderers.ObjectRenderers.Extensions;

internal class TaskListRenderer : UWPObjectRenderer<TaskList>
{
    protected override void Write(WinUIRenderer renderer, TaskList taskList)
    {
        if (renderer == null) throw new ArgumentNullException(nameof(renderer));
        if (taskList == null) throw new ArgumentNullException(nameof(taskList));

        var checkBox = new MyTaskListCheckBox(taskList);
        renderer.WriteInline(checkBox);
    }
}
