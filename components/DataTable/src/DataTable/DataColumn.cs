// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

[TemplatePart(Name = nameof(PART_ColumnSizer), Type = typeof(ContentSizer))]
public partial class DataColumn : ContentControl
{
    private static GridLength StarLength = new GridLength(1, GridUnitType.Star);

    private ContentSizer? PART_ColumnSizer;

    private WeakReference<DataTable>? _parent;

    /// <summary>
    /// Gets or sets the width of the largest child contained within the visible <see cref="DataRow"/>s of the <see cref="DataTable"/>.
    /// </summary>
    internal double MaxChildDesiredWidth { get; set; }

    /// <summary>
    /// Gets or sets whether the column can be resized by the user.
    /// </summary>
    public bool CanResize
    {
        get { return (bool)GetValue(CanResizeProperty); }
        set { SetValue(CanResizeProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="CanResize"/> property.
    /// </summary>
    public static readonly DependencyProperty CanResizeProperty =
        DependencyProperty.Register("CanResize", typeof(bool), typeof(DataColumn), new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets the desired width of the column upon initialization. Defaults to a <see cref="GridLength"/> of 1 <see cref="GridUnitType.Star"/>.
    /// </summary>
    public GridLength DesiredWidth
    {
        get { return (GridLength)GetValue(DesiredWidthProperty); }
        set { SetValue(DesiredWidthProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DesiredWidth"/> property.
    /// </summary>
    public static readonly DependencyProperty DesiredWidthProperty =
        DependencyProperty.Register(nameof(DesiredWidth), typeof(GridLength), typeof(DataColumn), new PropertyMetadata(GridLength.Auto));

    public DataColumn()
    {
        this.DefaultStyleKey = typeof(DataColumn);
    }

    protected override void OnApplyTemplate()
    {
        if (PART_ColumnSizer != null)
        {
            PART_ColumnSizer.TargetControl = null;
            PART_ColumnSizer.ManipulationCompleted -= this.PART_ColumnSizer_ManipulationCompleted;
        }

        PART_ColumnSizer = GetTemplateChild(nameof(PART_ColumnSizer)) as ContentSizer;

        if (PART_ColumnSizer != null)
        {
            PART_ColumnSizer.TargetControl = this;
            PART_ColumnSizer.ManipulationCompleted += this.PART_ColumnSizer_ManipulationCompleted;
        }

        // Get DataTable parent weak reference for when we manipulate columns.
        var parent = this.FindAscendant<DataTable>();
        if (parent != null)
        {
            _parent = new(parent);
        }

        base.OnApplyTemplate();
    }

    private void PART_ColumnSizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (_parent?.TryGetTarget(out DataTable? parent) == true
            && parent != null)
        {
            parent.ColumnResized();
        }
    }
}
