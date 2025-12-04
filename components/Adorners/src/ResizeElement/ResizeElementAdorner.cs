// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Controls;

namespace CommunityToolkit.WinUI.Adorners;

/// <summary>
/// An <see cref="Adorner"/> that will allow a user to resize the adorned element.
/// </summary>
[TemplatePart(Name = nameof(TopThumbPart), Type = typeof(ResizeThumb))]
[TemplatePart(Name = nameof(BottomThumbPart), Type = typeof(ResizeThumb))]
[TemplatePart(Name = nameof(LeftThumbPart), Type = typeof(ResizeThumb))]
[TemplatePart(Name = nameof(RightThumbPart), Type = typeof(ResizeThumb))]
[TemplatePart(Name = nameof(TopLeftThumbPart), Type = typeof(ResizeThumb))]
[TemplatePart(Name = nameof(TopRightThumbPart), Type = typeof(ResizeThumb))]
[TemplatePart(Name = nameof(BottomLeftThumbPart), Type = typeof(ResizeThumb))]
[TemplatePart(Name = nameof(BottomRightThumbPart), Type = typeof(ResizeThumb))]
public sealed partial class ResizeElementAdorner : Adorner<FrameworkElement>
{
    private ResizeThumb? TopThumbPart;
    private ResizeThumb? BottomThumbPart;
    private ResizeThumb? LeftThumbPart;
    private ResizeThumb? RightThumbPart;
    private ResizeThumb? TopLeftThumbPart;
    private ResizeThumb? TopRightThumbPart;
    private ResizeThumb? BottomLeftThumbPart;
    private ResizeThumb? BottomRightThumbPart;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResizeElementAdorner"/> class.
    /// </summary>
    public ResizeElementAdorner()
    {
        this.DefaultStyleKey = typeof(ResizeElementAdorner);

        // Uno workaround
        DataContext = this;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        OnDetaching();

        base.OnApplyTemplate();

        TopThumbPart = GetTemplateChild(nameof(TopThumbPart)) as ResizeThumb;
        BottomThumbPart = GetTemplateChild(nameof(BottomThumbPart)) as ResizeThumb;
        LeftThumbPart = GetTemplateChild(nameof(LeftThumbPart)) as ResizeThumb;
        RightThumbPart = GetTemplateChild(nameof(RightThumbPart)) as ResizeThumb;
        TopLeftThumbPart = GetTemplateChild(nameof(TopLeftThumbPart)) as ResizeThumb;
        TopRightThumbPart = GetTemplateChild(nameof(TopRightThumbPart)) as ResizeThumb;
        BottomLeftThumbPart = GetTemplateChild(nameof(BottomLeftThumbPart)) as ResizeThumb;
        BottomRightThumbPart = GetTemplateChild(nameof(BottomRightThumbPart)) as ResizeThumb;

        // OnApplyTemplate can be called after OnAttached, especially if the Adorner isn't initially visible, so we need to re-apply the TargetControl here.
        if (AdornedElement is not null)
        {
            // Guard this incase we're getting removed from the visual tree...
            // Not sure if this is a bug in the Adorner lifecycle or not or specific to how we've set this up here.
            OnAttached();
        }
    }

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();

        TopThumbPart?.TargetControl = AdornedElement;
        BottomThumbPart?.TargetControl = AdornedElement;
        LeftThumbPart?.TargetControl = AdornedElement;
        RightThumbPart?.TargetControl = AdornedElement;
        TopLeftThumbPart?.TargetControl = AdornedElement;
        TopRightThumbPart?.TargetControl = AdornedElement;
        BottomLeftThumbPart?.TargetControl = AdornedElement;
        BottomRightThumbPart?.TargetControl = AdornedElement;
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        TopThumbPart?.TargetControl = null;
        BottomThumbPart?.TargetControl = null;
        LeftThumbPart?.TargetControl = null;
        RightThumbPart?.TargetControl = null;
        TopLeftThumbPart?.TargetControl = null;
        TopRightThumbPart?.TargetControl = null;
        BottomLeftThumbPart?.TargetControl = null;
        BottomRightThumbPart?.TargetControl = null;
    }
}
