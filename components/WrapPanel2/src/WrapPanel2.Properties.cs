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
        DependencyProperty.Register(
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
        new PropertyMetadata(default(bool), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="ItemsStretch"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemsStretchProperty = DependencyProperty.Register(
        nameof(ItemsStretch),
        typeof(WrapPanelItemsStretch),
        typeof(WrapPanel2),
        new PropertyMetadata(default(WrapPanelItemsStretch), OnPropertyChanged));

    /// <summary>
    /// Gets or sets the panel orientation.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <summary>
    /// Gets or sets the spacing between items.
    /// </summary>
    /// <remarks>
    /// When <see cref="ItemsJustification"/> is in a spacing mode and <see cref="ItemsStretch"/> is <see cref="WrapPanelItemsStretch.None"/>,
    /// this may instead be used as the minimum space between items, while the exact spacing is adjusted to ensure the items span from margin to margin.
    /// </remarks>
    public double ItemSpacing
    {
        get => (double)GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical spacing between items.
    /// </summary>
    public double LineSpacing
    {
        get => (double)GetValue(LineSpacingProperty);
        set => SetValue(LineSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether or not all rows/columns should stretch its children to ensure the space is filled from margin to margin.
    /// </summary>
    /// <remarks>
    /// This will not apply on lines without star-sized items unless a <see cref="ItemsStretch"/> behavior is selected.
    /// </remarks>
    public WrapPanelItemsJustification ItemsJustification
    {
        get => (WrapPanelItemsJustification)GetValue(ItemsJustificationProperty);
        set => SetValue(ItemsJustificationProperty, value);
    }

    /// <summary>
    /// Gets or sets the method used to fill rows without a star-sized item when <see cref="ItemsJustification"/> is in a spacing mode.
    /// </summary>
    public WrapPanelItemsStretch ItemsStretch
    {
        get => (WrapPanelItemsStretch)GetValue(ItemsStretchProperty);
        set => SetValue(ItemsStretchProperty, value);
    }

    /// <summary>
    /// Gets the <see cref="LayoutLengthProperty"/> of an item in the <see cref="WrapPanel2"/>.
    /// </summary>
    public static GridLength GetLayoutLength(DependencyObject obj) => (GridLength)obj.GetValue(LayoutLengthProperty);

    /// <summary>
    /// Sets the <see cref="LayoutLengthProperty"/> of an item in the <see cref="WrapPanel2"/>.
    /// </summary>
    public static void SetLayoutLength(DependencyObject obj, GridLength value) => obj.SetValue(LayoutLengthProperty, value);

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (WrapPanel2)d;
        panel.InvalidateMeasure();
    }
}
