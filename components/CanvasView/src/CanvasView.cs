// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using CommunityToolkit.WinUI.Helpers;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// <see cref="CanvasView"/> is an <see cref="ItemsControl"/> which uses a <see cref="Canvas"/> for the layout of its items.
/// It which provides built-in support for presenting a collection of items bound to specific coordinates 
/// and <see cref="UIElement.ManipulationMode"/> support of those items.
/// </summary>
public partial class CanvasView : ItemsControl
{
    /// <summary>
    /// Gets the set of properties that will be automatically bound to the root element within the <see cref="ContentPresenter"/> template to the containing <see cref="ContentPresenter"/> itself within the <see cref="CanvasView"/>. This allows for data binding these properties to a templated object for tracking position based properties.
    /// </summary>
    private (DependencyProperty, string)[] LiftedProperties = new (DependencyProperty, string)[] {
        (Canvas.LeftProperty, "(Canvas.Left)"),
        (Canvas.TopProperty, "(Canvas.Top)"),
        (Canvas.ZIndexProperty, "(Canvas.ZIndex)"),
        (ManipulationModeProperty, "ManipulationMode")
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CanvasView"/> class.
    /// </summary>
    public CanvasView()
    {
        // TODO: Need to use XamlReader because of https://github.com/microsoft/microsoft-ui-xaml/issues/2898
        ItemsPanel = XamlReader.Load(
            """
            <ItemsPanelTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
                <Canvas/>
            </ItemsPanelTemplate>
            """) as ItemsPanelTemplate;
    }

    /// <inheritdoc/>
    protected override DependencyObject GetContainerForItemOverride() => new ContentPresenter();

    /// <inheritdoc/>
    protected override bool IsItemItsOwnContainerOverride(object item) => item is ContentPresenter;

    /// <inheritdoc/>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        // ContentPresenter is the default container for Canvas.
        if (element is ContentPresenter cp)
        {
            _ = CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() =>
            {
                SetupChildBinding(cp);
            });

            // Loaded is not firing when dynamically loading an element to the collection. Relay on CompositionTargetHelper above.
            // Seems like a bug in Loaded event?
            cp.Loaded += ContentPresenter_Loaded;
            cp.ManipulationDelta += ContentPresenter_ManipulationDelta;
        }

        // TODO: Do we want to support something else in a custom template?? else if (item is FrameworkElement fe && fe.FindDescendant/GetContentControl?)
    }

    /// <inheritdoc/>
    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        base.ClearContainerForItemOverride(element, item);

        if (element is ContentPresenter cp)
        {
            cp.Loaded -= ContentPresenter_Loaded;
            cp.ManipulationDelta -= ContentPresenter_ManipulationDelta;
        }
    }

#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "These use of 'SetBindingExpressionValue' might be fine (we should revisit this later)")]
#endif
    private void ContentPresenter_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        // Move the rectangle.
        if (sender is ContentPresenter cp)
        {
            // TODO: Seeing some drift, not sure if due to DPI or just general drift
            // or probably we need to do the start/from delta approach we did with SizerBase to resolve.

            // We know that most likely these values have been bound to a data model object of some sort
            // Therefore, we need to use this helper to update the underlying model value of our bound property.
            cp.SetBindingExpressionValue(Canvas.LeftProperty, Canvas.GetLeft(cp) + e.Delta.Translation.X);
            cp.SetBindingExpressionValue(Canvas.TopProperty, Canvas.GetTop(cp) + e.Delta.Translation.Y);
        }
    }

    private void ContentPresenter_Loaded(object sender, RoutedEventArgs args)
    {
        if (sender is ContentPresenter cp)
        {
            cp.Loaded -= ContentPresenter_Loaded;

            SetupChildBinding(cp);
        }
    }

    private void SetupChildBinding(ContentPresenter cp)
    {
        // Get direct visual descendant for ContentPresenter to look for Canvas properties within Template.
        var child = VisualTreeHelper.GetChild(cp, 0);

        if (child != null)
        {
            // TODO: Should we avoid doing this twice?

            // Hook up any properties we care about from the templated children to it's parent ContentPresenter.
            foreach ((var prop, var path) in LiftedProperties)
            {
                var binding = new Binding();
                binding.Source = child;
                ////binding.Mode = BindingMode.TwoWay; // TODO: Should this be exposed as a general property?
                binding.Path = new PropertyPath(path);

                cp.SetBinding(prop, binding);
            }
        }
    }
}
