// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI;

/// <summary>
/// Helper class to hold content with an <see cref="AdornerLayer"/>. Use this to wrap another <see cref="UIElement"/> and direct where the <see cref="AdornerLayer"/> should sit. This class is helpful to constrain the <see cref="AdornerLayer"/> or in cases where an appropriate location for the layer can't be automatically determined.
/// </summary>
[TemplatePart(Name = PartAdornerLayer, Type = typeof(AdornerLayer))]
[ContentProperty(Name = nameof(Child))]
public sealed partial class AdornerDecorator : Control
{
    private const string PartAdornerLayer = "AdornerLayer";

    /// <summary>
    /// Gets or sets the single child element of the <see cref="AdornerDecorator"/>.
    /// </summary>
    public UIElement Child
    {
        get { return (UIElement)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Child"/> dependency property. 
    /// </summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Child), typeof(UIElement), typeof(AdornerDecorator), new PropertyMetadata(null));

    /// <summary>
    /// Gets the <see cref="AdornerLayer"/> contained within this <see cref="AdornerDecorator"/>.
    /// </summary>
    internal AdornerLayer? AdornerLayer { get; private set; }

    /// <summary>
    /// Constructs a new instance of <see cref="AdornerDecorator"/>.
    /// </summary>
    public AdornerDecorator()
    {
        this.DefaultStyleKey = typeof(AdornerDecorator);
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        AdornerLayer = GetTemplateChild(PartAdornerLayer) as AdornerLayer;
    }
}
