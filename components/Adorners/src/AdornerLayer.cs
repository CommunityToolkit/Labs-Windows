// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Future;

namespace CommunityToolkit.WinUI;

/// <summary>
/// An adornment layer which can hold content to show on top of other components.
/// If none is specified, one will be injected into your app content for you.
/// If a suitable location can't be automatically found, you can also use an
/// <see cref="AdornerDecorator"/> to specify where the <see cref="AdornerLayer"/> should be placed.
/// </summary>
public partial class AdornerLayer : Canvas
{
    /// <summary>
    /// Gets the <see cref="XamlProperty"/> of a <see cref="FrameworkElement"/>. Use this to retrieve any attached <see cref="UIElement"/> adorner from another <see cref="FrameworkElement"/>.
    /// </summary>
    /// <param name="obj">The <see cref="FrameworkElement"/> to retrieve the adorner from.</param>
    /// <returns>The <see cref="UIElement"/> attached as an adorner.</returns>
    public static UIElement GetXaml(FrameworkElement obj)
    {
        return (UIElement)obj.GetValue(XamlProperty);
    }

    /// <summary>
    /// Sets the <see cref="XamlProperty"/> of a <see cref="FrameworkElement"/>. Use this to attach any <see cref="UIElement"/> as an adorner to another <see cref="FrameworkElement"/>. Requires that an <see cref="AdornerLayer"/> is available in the visual tree above the adorned element.
    /// </summary>
    /// <param name="obj">The <see cref="FrameworkElement"/> to adorn.</param>
    /// <param name="value">The <see cref="UIElement"/> to attach as an adorner.</param>
    public static void SetXaml(FrameworkElement obj, UIElement value)
    {
        obj.SetValue(XamlProperty, value);
    }

    /// <summary>
    /// Identifies the Xaml Attached Property.
    /// </summary>
    public static readonly DependencyProperty XamlProperty =
        DependencyProperty.RegisterAttached("Xaml", typeof(UIElement), typeof(AdornerLayer), new PropertyMetadata(null, OnXamlPropertyChanged));

    /// <summary>
    /// Constructs a new instance of <see cref="AdornerLayer"/>.
    /// </summary>
    public AdornerLayer()
    {
        SizeChanged += AdornerLayer_SizeChanged;
    }

    private void AdornerLayer_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        foreach (var adornerXaml in Children)
        {
            if (adornerXaml is Adorner adorner)
            {
                // Notify each adorner that our general layout has updated.
                adorner.OnLayoutUpdated(null, EventArgs.Empty);
            }
        }
    }

    private static async void OnXamlPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is FrameworkElement fe)
        {
            if (!fe.IsLoaded || fe.Parent is null)
            {
                fe.Loaded += XamlPropertyFrameworkElement_Loaded;
            }
            else if (args.NewValue is UIElement adorner)
            {
                var layer = await GetAdornerLayerAsync(fe);

                if (layer is not null)
                {
                    AttachAdorner(layer, fe, adorner);
                }
            }
            else if (args.NewValue == null && args.OldValue is UIElement oldAdorner)
            {
                var layer = await GetAdornerLayerAsync(fe);

                if (layer is not null)
                {
                    RemoveAdorner(layer, oldAdorner);
                }
            }
        }
    }

    private static async void XamlPropertyFrameworkElement_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe)
        {
            fe.Loaded -= XamlPropertyFrameworkElement_Loaded;

            var layer = await GetAdornerLayerAsync(fe);

            if (layer is not null)
            {
                var adorner = GetXaml(fe);

                if (adorner == null) return;

                AttachAdorner(layer, fe, adorner);
            }
        }
    }

    /// <summary>
    /// Retrieves the closest (or creates an) <see cref="AdornerLayer"/> for the given element. If awaited, the retrieved adorner layer is guaranteed to be loaded. This is to assist adorners with being able to be positioned in relation to the loaded element.
    /// There may be multiple <see cref="AdornerLayer"/>s within an application, as each <see cref="ScrollViewer"/> should have one to enable relational scrolling along content that may be outside of the viewport.
    /// </summary>
    /// <param name="adornedElement">Element to adorn.</param>
    /// <returns>Loaded <see cref="AdornerLayer"/> responsible for that element.</returns>
    public static async Task<AdornerLayer?> GetAdornerLayerAsync(FrameworkElement adornedElement)
    {
        // 1. Find Adorner Layer for element or top-most element
        FrameworkElement? lastElement = null;

        var adornerLayerOrTopMostElement = adornedElement.FindAscendant<FrameworkElement>((element) =>
        {
            lastElement = element; // TODO: should this be after our if, does it matter?

            if (element is AdornerDecorator)
            {
                return true;
            }
            else if (element is AdornerLayer)
            {
                return true;
            }
            else if (element is ScrollViewer)
            {
                return true;
            }
            // TODO: Need to figure out porting new DO toolkit helpers to Uno, only needed for custom adorner layer placement...
            /*else
            {
                // TODO: Use BreadthFirst Search w/ Depth Limited?
                var child = element.FindFirstLevelDescendants<AdornerLayer>();

                if (child != null)
                {
                    lastElement = child;
                    return true;
                }
            }*/

            return false;
        }) ?? lastElement;

        // Check cases where we may have found a child that we want to use instead of the element returned by search.
        if (lastElement is AdornerLayer || lastElement is AdornerDecorator)
        {
            adornerLayerOrTopMostElement = lastElement;
        }

        if (adornerLayerOrTopMostElement is AdornerDecorator decorator)
        {
            await decorator.WaitUntilLoadedAsync();

            return decorator.AdornerLayer;
        }
        else if (adornerLayerOrTopMostElement is AdornerLayer layer)
        {
            await layer.WaitUntilLoadedAsync();

            // If we just have an adorner layer now, we're done!
            return layer;
        }
        else
        {
            // TODO: Windows.UI.Xaml.Internal.RootScrollViewer is a maybe different and what was causing issues before I looked for ScrollViewers along the way?
            // It's an internal unexposed type, so maybe it inherits from ScrollViewer? Not sure yet, but might need to detect and
            // do something different here?

            // ScrollViewers need AdornerLayers so they can provide adorners that scroll with the adorned elements (as it worked in WPF).
            // Note: ScrollViewers and the Window were the main AdornerLayer integration points in WPF.
            if (adornerLayerOrTopMostElement is ScrollViewer scroller)
            {
                var content = scroller.Content as FrameworkElement;

                // Extra code for RootScrollViewer TODO: Can we detect this better?
                if (scroller.Parent == null)
                {
                    //// XamlMarkupHelper.UnloadObject doesn't work here (throws an invalid value exception) does content need a name?
                    // TODO: Figure out this scenario?
                    throw new NotImplementedException("RootScrollViewer attachment isn't supported, add a AdornerDecorator or ScrollViewer manually to the top-level of your application.");
                }

                scroller.Content = null;

                var layerContainer = new AdornerDecorator()
                {
                    Child = content!,
                };

                scroller.Content = layerContainer;

                await layerContainer.WaitUntilLoadedAsync();

                return layerContainer.AdornerLayer;
            }
            // Grid seems like the easiest place for us to inject AdornerLayers automatically at the top-level (if needed) - not sure how common this will be?
            else if (adornerLayerOrTopMostElement is Grid grid)
            {
                // TODO: Not sure how we want to handle AdornerDecorator in this scenario...
                var adornerLayer = new AdornerLayer();

                // TODO: Handle if grid row/columns change.
                Grid.SetRowSpan(adornerLayer, grid.RowDefinitions.Count);
                Grid.SetColumnSpan(adornerLayer, grid.ColumnDefinitions.Count);
                grid.Children.Add(adornerLayer);

                await adornerLayer.WaitUntilLoadedAsync();

                return adornerLayer;
            }
        }

        return null;
    }

    // TODO: Temp helper? Build into 'Adorner' base class?
    private static void AttachAdorner(AdornerLayer layer, FrameworkElement adornedElement, UIElement adornerXaml)
    {
        if (adornerXaml is Adorner adorner)
        {
            // We already have an adorner type, use it directly.
        }
        else
        {
            adorner = new Adorner()
            {
                Content = adornerXaml,
            };
        }

        // Add adorner XAML content to the Adorner Layer
        adorner.AdornerLayer = layer;
        adorner.AdornedElement = adornedElement;

        layer.Children.Add(adorner);
    }

    internal static void RemoveAdorner(AdornerLayer layer, UIElement adornerXaml)
    {
        var adorner = adornerXaml.FindAscendantOrSelf<Adorner>();

        if (adorner != null)
        {
            adorner.AdornedElement = null;
            adorner.AdornerLayer = null;

            layer.Children.Remove(adorner);

#if !HAS_UNO
            VisualTreeHelper.DisconnectChildrenRecursive(adorner);
#endif
        }
    }
}
