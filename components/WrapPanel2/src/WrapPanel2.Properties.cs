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
    public static readonly DependencyProperty lineSpacingProperty = DependencyProperty.Register(
        nameof(LineSpacing),
        typeof(double),
        typeof(WrapPanel2),
        new PropertyMetadata(default(double), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="FixedRowLengths"/> property.
    /// </summary>
    public static readonly DependencyProperty FixedRowLengthsProperty = DependencyProperty.Register(
        nameof(FixedRowLengths),
        typeof(bool),
        typeof(WrapPanel2),
        new PropertyMetadata(default(bool), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="StretchChildren"/> property.
    /// </summary>
    public static readonly DependencyProperty StretchChildrenProperty = DependencyProperty.Register(
        nameof(StretchChildren),
        typeof(StretchChildren),
        typeof(WrapPanel2),
        new PropertyMetadata(default(StretchChildren), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="OverflowBehavior"/> property.
    /// </summary>
    public static readonly DependencyProperty OverflowBehaviorProperty = DependencyProperty.Register(
        nameof(OverflowBehavior),
        typeof(OverflowBehavior),
        typeof(WrapPanel2),
        new PropertyMetadata(default(OverflowBehavior), OnPropertyChanged));

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
        get => (double)GetValue(lineSpacingProperty);
        set => SetValue(lineSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets whether or not all rows/columns should stretch to match the length of the longest.
    /// </summary>
    public bool FixedRowLengths
    {
        get => (bool)GetValue(FixedRowLengthsProperty);
        set => SetValue(FixedRowLengthsProperty, value);
    }

    /// <summary>
    /// Gets or sets the method used to fill rows without a star-sized item.
    /// </summary>
    public StretchChildren StretchChildren
    {
        get => (StretchChildren)GetValue(StretchChildrenProperty);
        set => SetValue(StretchChildrenProperty, value);
    }

    /// <summary>
    /// Gets or sets how the panel handles content overflowing the available space.
    /// </summary>
    public OverflowBehavior OverflowBehavior
    {
        get => (OverflowBehavior)GetValue(OverflowBehaviorProperty);
        set => SetValue(OverflowBehaviorProperty, value);
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
