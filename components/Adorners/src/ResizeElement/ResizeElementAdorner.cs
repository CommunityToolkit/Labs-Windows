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

        var parts = new ResizeThumb?[]
        {
            TopThumbPart,
            BottomThumbPart,
            LeftThumbPart,
            RightThumbPart,
            TopLeftThumbPart,
            TopRightThumbPart,
            BottomLeftThumbPart,
            BottomRightThumbPart
        };

        foreach (var part in parts)
        {
            part?.TargetControl = AdornedElement;
            part?.TargetControlResized += OnTargetControlResized;
        }

        // If the adorned element moves than we need to update our layout.
        AdornedElement?.ManipulationDelta += OnTargetManipulated;
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        var parts = new ResizeThumb?[]
        {
            TopThumbPart,
            BottomThumbPart,
            LeftThumbPart,
            RightThumbPart,
            TopLeftThumbPart,
            TopRightThumbPart,
            BottomLeftThumbPart,
            BottomRightThumbPart
        };

        foreach (var part in parts)
        {
            part?.TargetControlResized -= OnTargetControlResized;
            part?.TargetControl = null;
        }

        AdornedElement?.ManipulationDelta -= OnTargetManipulated;
    }
    private void OnTargetManipulated(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        // If the underlying adorned element moves than we need to update our layout.
        this.UpdateLayout();
    }

    private void OnTargetControlResized(ResizeThumb sender, TargetControlResizedEventArgs args)
    {
        // TODO: Investigate more
        // Note: I'm not sure why the AdornedElement's SizeChanged/LayoutUpdate isn't getting triggered by our changes...
        // So for now, we'll just force a layout update here of the Adorner itself to realign to the new size of the AdornedElement.
        this.UpdateLayout();
    }
}
