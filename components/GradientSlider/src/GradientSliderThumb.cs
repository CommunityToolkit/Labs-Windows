// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A thumb for adjusting a <see cref="Microsoft.UI.Xaml.Media.GradientStop"/> in a <see cref="GradientSlider"/>.
/// </summary>
public sealed partial class GradientSliderThumb : Control
{
    internal const string CommonStates = "CommonStates";
    internal const string NormalState = "Normal";
    internal const string PointerOverState = "PointerOver";
    internal const string PressedState = "Pressed";
    internal const string DisabledState = "Disabled";

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

        PointerEntered += this.GradientSliderThumb_PointerEntered;
        PointerExited += this.GradientSliderThumb_PointerExited;
        PointerPressed += this.GradientSliderThumb_PointerPressed;
        PointerMoved += this.GradientSliderThumb_PointerMoved;
        PointerReleased += this.GradientSliderThumb_PointerReleased;
        PointerCanceled += this.GradientSliderThumb_PointerCanceled;
        IsEnabledChanged += this.GradientSliderThumb_IsEnabledChanged;
    }
}
