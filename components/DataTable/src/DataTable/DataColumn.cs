// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Represents a <see cref="DataTable"/> column.
/// </summary>
[TemplatePart(Name = nameof(PART_ColumnSizer), Type = typeof(ContentSizer))]
public partial class DataColumn : ContentControl
{
    private ContentSizer? PART_ColumnSizer;

    private WeakReference<DataTable>? _parent;

    // Pass-thru convenience helper to get reference from WeakReference; computed each time, do not store result!
    private DataTable? DataTable => _parent?.TryGetTarget(out DataTable? parent) == true ? parent : null;

    /// <summary>
    /// Gets or sets the internal calculated or manually set width of this column.
    /// NaN means that the column size is no yet calculated.
    /// </summary>
    internal double CurrentWidth { get; set; } = double.NaN;

    /// <summary>
    /// Gets the internal calculated or manually set width of this column, as a positive value.
    /// </summary>
    internal double ActualCurrentWidth => double.IsNaN(CurrentWidth) ? 0 : CurrentWidth;

    internal bool IsAbsolute => DesiredWidth.IsAbsolute;

    internal bool IsAuto => DesiredWidth.IsAuto;

    internal bool IsStar => DesiredWidth.IsStar;

    /// <summary>
    /// Returns <see langword="true"/> if the column width is fixed with the manual adjustment.
    /// </summary>
    internal bool IsFixed { get; set; }

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
        DependencyProperty.Register(nameof(DesiredWidth), typeof(GridLength), typeof(DataColumn), new PropertyMetadata(GridLength.Auto, DesiredWidth_PropertyChanged));

    private static void DesiredWidth_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // If the developer updates the size of the column, update our internal value.
        if (d is DataColumn column)
        {
            if (column.DesiredWidth is { GridUnitType: GridUnitType.Pixel, Value: var value })
            {
                column.IsFixed = true;
                column.CurrentWidth = value;
            }
            else if (column.DesiredWidth is { GridUnitType: GridUnitType.Star, Value: 0 })
            {
                // Handle DesiredWidth="0*" as fixed zero width column.
                column.IsFixed = true;
                column.CurrentWidth = 0;
            }
            else
            {
                // Reset the manual adjusted width.
                column.IsFixed = false;
                column.CurrentWidth = double.NaN;
            }

            // Request to measure for the IsAutoFit or IsStarProportion columns.
            column.DataTable?.InvalidateMeasure();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataColumn"/> class.
    /// </summary>
    public DataColumn()
    {
        this.DefaultStyleKey = typeof(DataColumn);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        if (PART_ColumnSizer != null)
        {
            PART_ColumnSizer.TargetControl = null;
            PART_ColumnSizer.ManipulationDelta -= this.PART_ColumnSizer_ManipulationDelta;
            PART_ColumnSizer.ManipulationCompleted -= this.PART_ColumnSizer_ManipulationCompleted;
        }

        PART_ColumnSizer = GetTemplateChild(nameof(PART_ColumnSizer)) as ContentSizer;

        if (PART_ColumnSizer != null)
        {
            PART_ColumnSizer.TargetControl = this;
            PART_ColumnSizer.ManipulationDelta += this.PART_ColumnSizer_ManipulationDelta;
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

    private void PART_ColumnSizer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        ColumnResizedByUserSizer();
    }

    private void PART_ColumnSizer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        ColumnResizedByUserSizer();
    }

    private void ColumnResizedByUserSizer()
    {
        // Update our internal representation to be our size now as a fixed value.
        if (CurrentWidth != this.ActualWidth)
        {
            CurrentWidth = this.ActualWidth;

            // Notify the rest of the table to update
            DataTable?.ColumnResized();
        }
    }
}
