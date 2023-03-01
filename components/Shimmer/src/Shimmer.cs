
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;


using Windows.UI;
using System;
#if WINAPPSDK
using Microsoft.UI;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Animations.Expressions;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Shapes;
#else
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Shapes;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations.Expressions;
#endif


namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// A generic shimmer control that can be used to construct a beautiful loading effect.
/// </summary>
[TemplatePart(Name = PART_Shape, Type = typeof(Rectangle))]
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public partial class Shimmer : Control
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private const float InitialStartPointX = -7.92f;
    private const string PART_Shape = "Shape";

    public static readonly DependencyProperty AnimationDurationInMillisecondsProperty = DependencyProperty.Register(
        nameof(AnimationDurationInMilliseconds),
        typeof(double),
        typeof(Shimmer),
        new PropertyMetadata(
            1600d,
            (s, e) =>
            {
                var self = (Shimmer)s;
                if (self.IsAnimating)
                {
                    self.TryStartAnimation();
                }
            }));

    public static readonly DependencyProperty IsAnimatingProperty = DependencyProperty.Register(
        nameof(IsAnimating),
        typeof(bool),
        typeof(Shimmer),
        new PropertyMetadata(
            true,
            (s, e) =>
            {
                var self = (Shimmer)s;
                var isAnimating = (bool)e.NewValue;

                if (isAnimating)
                {
                    self.TryStartAnimation();
                }
                else
                {
                    self.StopAnimation();
                }
            }));

    private CompositionColorGradientStop _gradientStop1;
    private CompositionColorGradientStop _gradientStop2;
    private CompositionColorGradientStop _gradientStop3;
    private CompositionColorGradientStop _gradientStop4;
    private CompositionRoundedRectangleGeometry _rectangleGeometry;
    private ShapeVisual _shapeVisual;
    private CompositionLinearGradientBrush _shimmerMaskGradient;
    private Border _shape;

    private bool _initialized;
    private bool _animationStarted;
    private CompositeDisposable _disposableVisualResources;
    private CompositeDisposable _disposableAnimationResources;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Shimmer()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        DefaultStyleKey = typeof(Shimmer);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public double AnimationDurationInMilliseconds
    {
        get => (double)GetValue(AnimationDurationInMillisecondsProperty);
        set => SetValue(AnimationDurationInMillisecondsProperty, value);
    }

    public bool IsAnimating
    {
        get => (bool)GetValue(IsAnimatingProperty);
        set => SetValue(IsAnimatingProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

#pragma warning disable CS8601 // Possible null reference assignment.
        _shape = GetTemplateChild(PART_Shape) as Border;
#pragma warning restore CS8601 // Possible null reference assignment.
        if (_initialized is false && TryInitializationResource() && IsAnimating)
        {
            TryStartAnimation();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_initialized is false && TryInitializationResource() && IsAnimating)
        {
            TryStartAnimation();
        }

        ActualThemeChanged += OnActualThemeChanged;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ActualThemeChanged -= OnActualThemeChanged;
        StopAnimation();

        if (_initialized)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            ElementCompositionPreview.SetElementChildVisual(_shape, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            _disposableVisualResources?.Dispose();
            _initialized = false;
        }
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (_initialized is false)
        {
            return;
        }

        SetGradientStopColorsByTheme();
    }

    private bool TryInitializationResource()
    {
        if (_initialized)
        {
            return true;
        }

        if (_shape is null || IsLoaded is false)
        {
            return false;
        }

        _disposableVisualResources = new CompositeDisposable();
        var compositor = _shape.GetVisual().Compositor;

        _rectangleGeometry = compositor.CreateRoundedRectangleGeometry();
        _shapeVisual = compositor.CreateShapeVisual();
        _shimmerMaskGradient = compositor.CreateLinearGradientBrush();
        _gradientStop1 = compositor.CreateColorGradientStop();
        _gradientStop2 = compositor.CreateColorGradientStop();
        _gradientStop3 = compositor.CreateColorGradientStop();
        _gradientStop4 = compositor.CreateColorGradientStop();
        SetGradientAndStops();
        SetGradientStopColorsByTheme();
        _rectangleGeometry.CornerRadius = new Vector2((float)CornerRadius.TopLeft);
        var spriteShape = compositor.CreateSpriteShape(_rectangleGeometry);
        spriteShape.FillBrush = _shimmerMaskGradient;
        _shapeVisual.Shapes.Add(spriteShape);
        ElementCompositionPreview.SetElementChildVisual(_shape, _shapeVisual);

        _disposableVisualResources
            .Include(_rectangleGeometry)
            .Include(_shapeVisual)
            .Include(_shimmerMaskGradient)
            .Include(_gradientStop1)
            .Include(_gradientStop2)
            .Include(_gradientStop3)
            .Include(_gradientStop4);

        _initialized = true;
        return true;
    }

    private void SetGradientAndStops()
    {
        _shimmerMaskGradient.StartPoint = new Vector2(InitialStartPointX, 0.0f);
        _shimmerMaskGradient.EndPoint = new Vector2(0.0f, 1.0f); //Vector2.One

        _gradientStop1.Offset = 0.273f;
        _gradientStop2.Offset = 0.436f;
        _gradientStop3.Offset = 0.482f;
        _gradientStop4.Offset = 0.643f;

        _shimmerMaskGradient.ColorStops.Add(_gradientStop1);
        _shimmerMaskGradient.ColorStops.Add(_gradientStop2);
        _shimmerMaskGradient.ColorStops.Add(_gradientStop3);
        _shimmerMaskGradient.ColorStops.Add(_gradientStop4);
    }

    private void SetGradientStopColorsByTheme()
    {
        switch (ActualTheme)
        {
            case ElementTheme.Default:
            case ElementTheme.Dark:
                _gradientStop1.Color = Color.FromArgb((byte)(255 * 3.26 / 100), 255, 255, 255);
                _gradientStop2.Color = Color.FromArgb((byte)(255 * 6.05 / 100), 255, 255, 255);
                _gradientStop3.Color = Color.FromArgb((byte)(255 * 6.05 / 100), 255, 255, 255);
                _gradientStop4.Color = Color.FromArgb((byte)(255 * 3.26 / 100), 255, 255, 255);
                break;
            case ElementTheme.Light:
                _gradientStop1.Color = Color.FromArgb((byte)(255 * 5.37 / 100), 0, 0, 0);
                _gradientStop2.Color = Color.FromArgb((byte)(255 * 2.89 / 100), 0, 0, 0);
                _gradientStop3.Color = Color.FromArgb((byte)(255 * 2.89 / 100), 0, 0, 0);
                _gradientStop4.Color = Color.FromArgb((byte)(255 * 5.37 / 100), 0, 0, 0);
                break;
        }
    }

    private void TryStartAnimation()
    {
        if (_animationStarted || _initialized is false || _shape is null)
        {
            return;
        }

        var rootVisual = _shape.GetVisual();
        _disposableAnimationResources = new CompositeDisposable();
        var sizeAnimation = rootVisual.GetReference().Size;
        _shapeVisual.StartAnimation(nameof(ShapeVisual.Size), sizeAnimation);
        _rectangleGeometry.StartAnimation(nameof(CompositionRoundedRectangleGeometry.Size), sizeAnimation);

        var gradientStartPointAnimation = rootVisual.Compositor.CreateVector2KeyFrameAnimation();
        gradientStartPointAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDurationInMilliseconds);
        gradientStartPointAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
        gradientStartPointAnimation.InsertKeyFrame(0.0f, new Vector2(InitialStartPointX, 0.0f));
        gradientStartPointAnimation.InsertKeyFrame(1.0f, Vector2.Zero);
        _shimmerMaskGradient.StartAnimation(nameof(CompositionLinearGradientBrush.StartPoint), gradientStartPointAnimation);

        var gradientEndPointAnimation = rootVisual.Compositor.CreateVector2KeyFrameAnimation();
        gradientEndPointAnimation.Duration = TimeSpan.FromMilliseconds(AnimationDurationInMilliseconds);
        gradientEndPointAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
        gradientEndPointAnimation.InsertKeyFrame(0.0f, new Vector2(1.0f, 0.0f)); //Vector2.One
        gradientEndPointAnimation.InsertKeyFrame(1.0f, new Vector2(-InitialStartPointX, 1.0f));
        _shimmerMaskGradient.StartAnimation(nameof(CompositionLinearGradientBrush.EndPoint), gradientEndPointAnimation);

        _disposableAnimationResources
            .Include(sizeAnimation)
            .Include(gradientStartPointAnimation)
            .Include(gradientEndPointAnimation);
        _animationStarted = true;
    }

    private void StopAnimation()
    {
        if (_animationStarted is false)
        {
            return;
        }

        _shapeVisual.StopAnimation(nameof(ShapeVisual.Size));
        _rectangleGeometry.StopAnimation(nameof(CompositionRoundedRectangleGeometry.Size));
        _shimmerMaskGradient.StopAnimation(nameof(CompositionLinearGradientBrush.StartPoint));
        _shimmerMaskGradient.StopAnimation(nameof(CompositionLinearGradientBrush.EndPoint));

        _disposableAnimationResources?.Dispose();
        _animationStarted = false;
    }
}
