// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class WrapPanel2
{
    /// <summary>
    /// An attached property for identifying the requested layout of a child within the panel.
    /// </summary>
    public static readonly DependencyProperty LayoutLengthProperty =
        DependencyProperty.RegisterAttached(
            "LayoutLength",
            typeof(GridLength),
            typeof(WrapPanel2),
            new PropertyMetadata(GridLength.Auto));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(WrapPanel2),
        new PropertyMetadata(Orientation.Horizontal, OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="ItemSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemSpacingProperty = DependencyProperty.Register(
        nameof(ItemSpacing),
        typeof(double),
        typeof(WrapPanel2),
        new PropertyMetadata(default(double), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="LineSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty LineSpacingProperty = DependencyProperty.Register(
        nameof(LineSpacing),
        typeof(double),
        typeof(WrapPanel2),
        new PropertyMetadata(default(double), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="ItemsJustification"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsJustificationProperty = DependencyProperty.Register(
        nameof(ItemsJustification),
        typeof(WrapPanelItemsJustification),
        typeof(WrapPanel2),
        new PropertyMetadata(default(WrapPanelItemsJustification), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="ItemsStretch"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsStretchProperty = DependencyProperty.Register(
        nameof(ItemsStretch),
        typeof(WrapPanelItemsStretch),
        typeof(WrapPanel2),
        new PropertyMetadata(default(WrapPanelItemsStretch), OnPropertyChanged));

    /// <summary>
    /// Gets or sets the direction in which child elements are arranged.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the distance between items in the same row or column.
    /// </summary>
    /// <remarks>
    /// When <see cref="ItemsJustification"/> is set to a spacing mode (e.g., SpaceBetween) and <see cref="ItemsStretch"/> is <see cref="WrapPanelItemsStretch.None"/>,
    /// this value acts as the minimum spacing between items.
    /// </remarks>
    public double ItemSpacing
    {
        get => (double)GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the distance between consecutive rows or columns.
    /// </summary>
    public double LineSpacing
    {
        get => (double)GetValue(LineSpacingProperty);
        set => SetValue(LineSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets how items are aligned and distributed within a single line.
    /// </summary>
    /// <remarks>
    /// If a non-stretching justification is used, items with a Star-Sized <see cref="WrapPanel2.LayoutLengthProperty"/> 
    /// will collapse to their minimum size while maintaining their relative proportions.
    /// </remarks>
    public WrapPanelItemsJustification ItemsJustification
    {
        get => (WrapPanelItemsJustification)GetValue(ItemsJustificationProperty);
        set => SetValue(ItemsJustificationProperty, value);
    }

    /// <summary>
    /// Gets or sets the stretching behavior for items on lines that do not contain Star-sized elements.
    /// </summary>
    public WrapPanelItemsStretch ItemsStretch
    {
        get => (WrapPanelItemsStretch)GetValue(ItemsStretchProperty);
        set => SetValue(ItemsStretchProperty, value);
    }

    /// <summary>
    /// Gets the <see cref="LayoutLengthProperty"/> attached property for a given element.
    /// </summary>
    /// <param name="obj">The element from which to read the property value.</param>
    /// <returns>The <see cref="GridLength"/> defining the item's sizing logic.</returns>
    public static GridLength GetLayoutLength(DependencyObject obj) => (GridLength)obj.GetValue(LayoutLengthProperty);

    /// <summary>
    /// Sets the <see cref="LayoutLengthProperty"/> attached property for a given element.
    /// </summary>
    /// <param name="obj">The element on which to set the property value.</param>
    /// <param name="value">The <see cref="GridLength"/> to apply.</param>
    public static void SetLayoutLength(DependencyObject obj, GridLength value) => obj.SetValue(LayoutLengthProperty, value);

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (WrapPanel2)d;
        panel.InvalidateMeasure();
    }

    private static void OnAlignmentPropertyChanged(DependencyObject obj, DependencyProperty prop)
    {
        var panel = (WrapPanel2)obj;
        panel.InvalidateMeasure();
    } 
}
