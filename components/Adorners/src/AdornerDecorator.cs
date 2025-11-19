// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI;

/// <summary>
/// Helper class to hold content with an <see cref="AdornerLayer"/>. Use this to wrap another <see cref="UIElement"/> and direct where the <see cref="AdornerLayer"/> should sit. This class is helpful to constrain the <see cref="AdornerLayer"/> or in cases where an appropriate location for the layer can't be automatically determined.
/// </summary>
[TemplatePart(Name = PartAdornerLayer, Type = typeof(AdornerLayer))]
[ContentProperty(Name = nameof(Child))]
public sealed class AdornerDecorator : Control
{
    private const string PartAdornerLayer = "AdornerLayer";

    public UIElement Child
    {
        get { return (UIElement)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Child), typeof(UIElement), typeof(AdornerDecorator), new PropertyMetadata(null));

    public AdornerLayer? AdornerLayer { get; private set; }

    public AdornerDecorator()
    {
        this.DefaultStyleKey = typeof(AdornerDecorator);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        AdornerLayer = GetTemplateChild(PartAdornerLayer) as AdornerLayer;
    }
}
