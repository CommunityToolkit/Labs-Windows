// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class StretchPanel
{
    /// <summary>
    /// An attached property for identifying the requested layout of a child within the panel.
    /// </summary>
    public static readonly DependencyProperty LayoutLengthProperty =
        DependencyProperty.Register(
            "LayoutLength",
            typeof(GridLength),
            typeof(StretchPanel),
            new PropertyMetadata(GridLength.Auto));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="Orientation"/> property.
    /// </summary>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(StretchPanel),
        new PropertyMetadata(default(Orientation), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="HorizontalSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty HorizontalSpacingProperty = DependencyProperty.Register(
        nameof(HorizontalSpacing),
        typeof(double),
        typeof(StretchPanel),
        new PropertyMetadata(default(double), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="VerticalSpacing"/> property.
    /// </summary>
    public static readonly DependencyProperty VerticalSpacingProperty = DependencyProperty.Register(
        nameof(VerticalSpacing),
        typeof(double),
        typeof(StretchPanel),
        new PropertyMetadata(default(double), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="FixedRowLengths"/> property.
    /// </summary>
    public static readonly DependencyProperty FixedRowLengthsProperty = DependencyProperty.Register(
        nameof(FixedRowLengths),
        typeof(bool),
        typeof(StretchPanel),
        new PropertyMetadata(default(bool), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="ForcedStretchMethod"/> property.
    /// </summary>
    public static readonly DependencyProperty ForcedStretchMethodProperty = DependencyProperty.Register(
        nameof(ForcedStretchMethod),
        typeof(ForcedStretchMethod),
        typeof(StretchPanel),
        new PropertyMetadata(default(ForcedStretchMethod), OnPropertyChanged));

    /// <summary>
    /// Backing <see cref="DependencyProperty"/> for the <see cref="OverflowBehavior"/> property.
    /// </summary>
    public static readonly DependencyProperty OverflowBehaviorProperty = DependencyProperty.Register(
        nameof(OverflowBehavior),
        typeof(OverflowBehavior),
        typeof(StretchPanel),
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
    /// Gets or sets the horizontal spacing between items.
    /// </summary>
    public double HorizontalSpacing
    {
        get => (double)GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical spacing between items.
    /// </summary>
    public double VerticalSpacing
    {
        get => (double)GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, value);
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
    public ForcedStretchMethod ForcedStretchMethod
    {
        get => (ForcedStretchMethod)GetValue(ForcedStretchMethodProperty);
        set => SetValue(ForcedStretchMethodProperty, value);
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
    /// Gets the <see cref="LayoutLengthProperty"/> of an item in the <see cref="StretchPanel"/>.
    /// </summary>
    public static GridLength GetLayoutLength(DependencyObject obj) => (GridLength)obj.GetValue(LayoutLengthProperty);

    /// <summary>
    /// Sets the <see cref="LayoutLengthProperty"/> of an item in the <see cref="StretchPanel"/>.
    /// </summary>
    public static void SetLayoutLength(DependencyObject obj, GridLength value) => obj.SetValue(LayoutLengthProperty, value);

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (StretchPanel)d;
        panel.InvalidateMeasure();
    }
}
