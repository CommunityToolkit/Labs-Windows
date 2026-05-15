// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ColorPicker = Microsoft.UI.Xaml.Controls.ColorPicker;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A thumb for adjusting a <see cref="Microsoft.UI.Xaml.Media.GradientStop"/> in a <see cref="GradientSlider"/>.
/// </summary>
[TemplatePart(Name = "PART_ColorPicker", Type = typeof(ColorPicker))]
[TemplatePart(Name = "PART_Border", Type = typeof(Border))]
public sealed partial class GradientSliderThumb : Control
{
    internal const string CommonStates = "CommonStates";
    internal const string NormalState = "Normal";
    internal const string PointerOverState = "PointerOver";
    internal const string PressedState = "Pressed";
    internal const string DisabledState = "Disabled";

    private Border? _border;
    private ColorPicker? _colorPicker;

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="GradientStop"/> property.
    /// </summary>
    public static readonly DependencyProperty GradientStopProperty =
        DependencyProperty.Register(nameof(GradientStop),
            typeof(GradientStop),
            typeof(GradientSliderThumb),
            new PropertyMetadata(null));

    /// <summary>
    /// Initializes a new instance of the <see cref="GradientSliderThumb"/> class.
    /// </summary>
    public GradientSliderThumb()
    {
        DefaultStyleKey = typeof(GradientSliderThumb);
    }

    /// <summary>
    /// Gets or sets the <see cref="GradientStop"/> the thumb controls.
    /// </summary>
    public GradientStop GradientStop
    {
        get => (GradientStop)GetValue(GradientStopProperty);
        set => SetValue(GradientStopProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _border = (Border)GetTemplateChild("PART_Border");
        _colorPicker = (ColorPicker)GetTemplateChild("PART_ColorPicker");

        PointerEntered += GradientSliderThumb_PointerEntered;
        PointerExited += GradientSliderThumb_PointerExited;
        PointerPressed += GradientSliderThumb_PointerPressed;
        PointerMoved += GradientSliderThumb_PointerMoved;
        PointerReleased += GradientSliderThumb_PointerReleased;
        PointerCanceled += GradientSliderThumb_PointerCanceled;
        IsEnabledChanged += GradientSliderThumb_IsEnabledChanged;

        _colorPicker.Color = GradientStop.Color;
        _colorPicker.ColorChanged += ColorPicker_ColorChanged;

        Tapped += GradientSliderThumb_Tapped;
    }

    private void ColorPicker_ColorChanged(ColorPicker sender, MUXC.ColorChangedEventArgs args)
    {
        GradientStop.Color = args.NewColor;
    }

    private void GradientSliderThumb_Tapped(object sender, TappedRoutedEventArgs e)
    {
        FlyoutBase.ShowAttachedFlyout(_border);
    }
}
