// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Security.Authentication.OnlineId;

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// This is an example control based off of the BoxPanel sample here: https://docs.microsoft.com/windows/apps/design/layout/boxpanel-example-custom-panel. If you need this similar sort of layout component for an application, see UniformGrid in the Toolkit.
/// It is provided as an example of how to inherit from another control like <see cref="Panel"/>.
/// You can choose to start here or from the <see cref="ProjectTemplate_ClassicBinding"/> or <see cref="ProjectTemplate_xBind"/> example components. Remove unused components and rename as appropriate.
/// </summary>
public class ProjectTemplate : Panel
{
    /// <summary>
    /// Identifies the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ProjectTemplate), new PropertyMetadata(null, OnOrientationChanged));

    /// <summary>
    /// Gets the preference of the rows/columns when there are a non-square number of children. Defaults to Vertical.
    /// </summary>
    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    // Invalidate our layout when the property changes.
    private static void OnOrientationChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is ProjectTemplate panel)
        {
            panel.InvalidateMeasure();
        }
    }

    // Since we expect contents of panel to be dynamic, allocate memory for calculations here as fields.
    int maxrc, rowcount, colcount;
    double cellwidth, cellheight, maxcellheight, aspectratio;

    protected override Size MeasureOverride(Size availableSize)
    {
        // Determine the square that can contain this number of items.
        maxrc = (int)Math.Ceiling(Math.Sqrt(Children.Count));
        // Get an aspect ratio from availableSize, decides whether to trim row or column.
        aspectratio = availableSize.Width / availableSize.Height;
        if (Orientation == Orientation.Vertical) { aspectratio = 1 / aspectratio; }

        // Now trim this square down to a rect, many times an entire row or column can be omitted.
        if (aspectratio > 1)
        {
            rowcount = maxrc;
            colcount = (maxrc > 2 && Children.Count <= maxrc * (maxrc - 1)) ? maxrc - 1 : maxrc;
        }
        else
        {
            rowcount = (maxrc > 2 && Children.Count <= maxrc * (maxrc - 1)) ? maxrc - 1 : maxrc;
            colcount = maxrc;
        }

        // Now that we have a column count, divide available horizontal, that's our cell width.
        cellwidth = (int)Math.Floor(availableSize.Width / colcount);
        // Next get a cell height, same logic of dividing available vertical by rowcount.
        cellheight = Double.IsInfinity(availableSize.Height) ? Double.PositiveInfinity : availableSize.Height / rowcount;

        foreach (UIElement child in Children)
        {
            child.Measure(new Size(cellwidth, cellheight));
            maxcellheight = (child.DesiredSize.Height > maxcellheight) ? child.DesiredSize.Height : maxcellheight;
        }
        return LimitUnboundedSize(availableSize);
    }

    // This method limits the panel height when no limit is imposed by the panel's parent.
    // That can happen to height if the panel is close to the root of main app window.
    // In this case, base the height of a cell on the max height from desired size
    // and base the height of the panel on that number times the #rows.
    Size LimitUnboundedSize(Size input)
    { 
        if (Double.IsInfinity(input.Height))
        {
            input.Height = maxcellheight* colcount;
            cellheight = maxcellheight;
        }
        return input;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int count = 1;
        double x, y;
        foreach (UIElement child in Children)
        {
            x = (count - 1) % colcount * cellwidth;
            y = ((int)(count - 1) / colcount) * cellheight;
            Point anchorPoint = new Point(x, y);
            child.Arrange(new Rect(anchorPoint, child.DesiredSize));
            count++;
        }
        return finalSize;
    }
}
