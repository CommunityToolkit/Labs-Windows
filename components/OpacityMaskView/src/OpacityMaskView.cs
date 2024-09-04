// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
#if WINDOWS_WINAPPSDK
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Shapes;
#else
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;
#endif

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A control that applies an opacity mask to its content.
/// </summary>
[TemplatePart(Name = RootGridTemplateName, Type = typeof(Grid))]
[TemplatePart(Name = MaskContainerTemplateName, Type = typeof(Border))]
[TemplatePart(Name = ContentPresenterTemplateName, Type = typeof(ContentPresenter))]
public partial class OpacityMaskView : ContentControl
{
    /// <summary>
    /// Identifies the <see cref="OpacityMask"/> property.
    /// </summary>
    public static readonly DependencyProperty OpacityMaskProperty =
        DependencyProperty.Register(nameof(OpacityMask), typeof(UIElement), typeof(OpacityMaskView), new PropertyMetadata(null, OnOpacityMaskChanged));

    private const string ContentPresenterTemplateName = "PART_ContentPresenter";
    private const string MaskContainerTemplateName = "PART_MaskContainer";
    private const string RootGridTemplateName = "PART_RootGrid";

#if WINDOWS_WINAPPSDK
    private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();
#else
    private readonly Compositor _compositor = Window.Current.Compositor;
#endif
    private CompositionBrush? _mask;
    private CompositionMaskBrush? _maskBrush;

    /// <summary>
    /// Creates a new instance of the <see cref="OpacityMaskView"/> class.
    /// </summary>
    public OpacityMaskView()
    {
        DefaultStyleKey = typeof(OpacityMaskView);
    }

    /// <summary>
    /// Gets or sets a <see cref="UIElement"/> as the opacity mask that is applied to alpha-channel masking for the rendered content of the content.
    /// </summary>
    public UIElement? OpacityMask
    {
        get => (UIElement?)GetValue(OpacityMaskProperty);
        set => SetValue(OpacityMaskProperty, value);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Grid rootGrid = (Grid)GetTemplateChild(RootGridTemplateName);
        ContentPresenter contentPresenter = (ContentPresenter)GetTemplateChild(ContentPresenterTemplateName);
        Border maskContainer = (Border)GetTemplateChild(MaskContainerTemplateName);

        _maskBrush = _compositor.CreateMaskBrush();
        _maskBrush.Source = GetVisualBrush(contentPresenter);
        _mask = GetVisualBrush(maskContainer);
        _maskBrush.Mask = OpacityMask is null ? null : _mask;

        SpriteVisual redirectVisual = _compositor.CreateSpriteVisual();
        redirectVisual.RelativeSizeAdjustment = Vector2.One;
        redirectVisual.Brush = _maskBrush;
        ElementCompositionPreview.SetElementChildVisual(rootGrid, redirectVisual);
    }

    private static CompositionBrush GetVisualBrush(UIElement element)
    {
        Visual visual = ElementCompositionPreview.GetElementVisual(element);

        Compositor compositor = visual.Compositor;

        CompositionVisualSurface visualSurface = compositor.CreateVisualSurface();
        visualSurface.SourceVisual = visual;
        ExpressionAnimation sourceSizeAnimation = compositor.CreateExpressionAnimation($"{nameof(visual)}.Size");
        sourceSizeAnimation.SetReferenceParameter(nameof(visual), visual);
        visualSurface.StartAnimation(nameof(visualSurface.SourceSize), sourceSizeAnimation);

        CompositionSurfaceBrush brush = compositor.CreateSurfaceBrush(visualSurface);

        visual.Opacity = 0;

        return brush;
    }

    private static void OnOpacityMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        OpacityMaskView self = (OpacityMaskView)d;
        if (self._maskBrush is not { } maskBrush)
        {
            return;
        }

        UIElement? opacityMask = (UIElement?)e.NewValue;
        maskBrush.Mask = opacityMask is null ? null : self._mask;
    }
}
