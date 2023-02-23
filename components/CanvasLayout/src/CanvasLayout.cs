// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public class CanvasLayout : MUXC.VirtualizingLayout
{
#region Setup / teardown
    protected override void InitializeForContextCore(MUXC.VirtualizingLayoutContext context)
    {
        base.InitializeForContextCore(context);

        if (context.LayoutState is not CanvasLayoutState)
        {
            // Store any state we might need since (in theory) the layout could be in use by multiple
            // elements simultaneously
            context.LayoutState = new CanvasLayoutState();
        }
    }

    protected override void UninitializeForContextCore(MUXC.VirtualizingLayoutContext context)
    {
        base.UninitializeForContextCore(context);

        // clear any state
        context.LayoutState = null;
    }

#endregion

#region Layout

    protected override Size MeasureOverride(MUXC.VirtualizingLayoutContext context, Size availableSize)
    {
        int maxWidth = 0;
        int maxHeight = 0;

        // Get underlying data about positioning of items and determine if in viewport.
        for (int i = 0; i < context.ItemCount; i++)
        {
            if (context.GetItemAt(i) is CanvasLayoutItem item)
            {
                // See if this item pushes our maximum boundary
                maxWidth = Math.Max(item.Left + item.Width, maxWidth);
                maxHeight = Math.Max(item.Top + item.Height, maxHeight);

                // Calculate if this item is in our current viewport
                Rect rect = new(item.Left, item.Top, item.Width, item.Height);
                rect.Intersect(context.RealizationRect);

                // Check if we're in view now so we can compare to if we were last time.
                bool nowInView = rect.Width > 0 || rect.Height > 0;

                // If it wasn't visible and now is, realize the container
                if (nowInView && !item.IsInView)
                {
                    var element = context.GetOrCreateElementAt(i);
                    element.Measure(new Size(item.Width, item.Height));
                }
                // If it was visible, but now isn't recycle the container
                else if (!nowInView && item.IsInView)
                {
                    var element = context.GetOrCreateElementAt(i);
                    context.RecycleElement(element);
                }

                // Update our current visibility
                item.IsInView = rect.Width > 0 || rect.Height > 0;
            }
        }

        return new Size(maxWidth, maxHeight);
    }

    protected override Size ArrangeOverride(MUXC.VirtualizingLayoutContext context, Size finalSize)
    {
        for (int i = 0; i < context.ItemCount; i++)
        {
            if (context.GetItemAt(i) is CanvasLayoutItem item && item.IsInView)
            {
                var container = context.GetOrCreateElementAt(i);
                // Is it better to have cached this from above?
                container.Arrange(new Rect(item.Left, item.Top, item.Width, item.Height));
            }
        }

        return finalSize;
    }

#endregion
}

internal class CanvasLayoutState
{
    public int FirstRealizedIndex { get; set; }

    /// <summary>
    /// List of layout bounds for items starting with the
    /// FirstRealizedIndex.
    /// </summary>
    public List<Rect> LayoutRects { get; } = new List<Rect>();
}

// TODO: Make DP? Can we do this with property mapping instead?
public class CanvasLayoutItem
{
    public int Left { get; set; }

    public int Top { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public bool IsInView { get; internal set; }
}
