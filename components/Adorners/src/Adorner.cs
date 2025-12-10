// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Helpers;

namespace CommunityToolkit.WinUI;

/// <summary>
/// A class which represents a <see cref="FrameworkElement"/> that decorates a <see cref="UIElement"/>.
/// </summary>
/// <remarks>
/// An adorner is a custom element which is bound to a specific <see cref="UIElement"/> and can
/// provide additional visual cues to the user. Adorners are rendered in an
/// <see cref="AdornerLayer"/>, a special layer that is on top of the adorned element or a collection
/// of adorned elements. Rendering of an adorner is independent of the UIElement it is bound to. An
/// adorner is typically positioned relative to the element it is bound to based on the upper-left 
/// coordinate origin of the adorned element.
///
/// Note: The parent of an <see cref="Adorner"/> is always an <see cref="AdornerLayer"/> and not the element being adorned.
/// </remarks>
public partial class Adorner : ContentControl
{
    /// <summary>
    /// Gets the element being adorned by this <see cref="Adorner"/>.
    /// </summary>
    public UIElement? AdornedElement
    {
        get;
        internal set
        {
            var oldvalue = field;
            field = value;
            OnAdornedElementChanged(oldvalue, value);
        }
    }

    private void OnAdornedElementChanged(UIElement? oldvalue, UIElement? newvalue)
    {
        if (oldvalue is not null
            && oldvalue is FrameworkElement oldfe)
        {
            // TODO: Should we explicitly detach the WEL here?
        }

        if (newvalue is not null
            && newvalue is FrameworkElement newfe)
        {
            // Track changes to the AdornedElement's size
            var weakPropertyChangedListenerSize = new WeakEventListener<Adorner, object, SizeChangedEventArgs>(this)
            {
                OnEventAction = static (instance, source, eventArgs) => instance.OnSizeChanged(source, eventArgs),
                OnDetachAction = (weakEventListener) => newfe.SizeChanged -= weakEventListener.OnEvent // Use Local References Only
            };
            newfe.SizeChanged += weakPropertyChangedListenerSize.OnEvent;

            // Track changes to the AdornedElement's layout
            // Note: This is pretty spammy, thinking we don't need this?
            /*var weakPropertyChangedListenerLayout = new WeakEventListener<Adorner, object?, object>(this)
            {
                OnEventAction = static (instance, source, eventArgs) => instance.OnLayoutUpdated(source, eventArgs),
                OnDetachAction = (weakEventListener) => newfe.LayoutUpdated -= weakEventListener.OnEvent // Use Local References Only
            };
            newfe.LayoutUpdated += weakPropertyChangedListenerLayout.OnEvent;*/

            // Initial size & layout update
            OnSizeChanged(null, null!);
            OnLayoutUpdated(null, null!);

            // Track if AdornedElement is loaded
            var weakPropertyChangedListenerLoaded = new WeakEventListener<Adorner, object, RoutedEventArgs>(this)
            {
                OnEventAction = static (instance, source, eventArgs) => instance.OnAdornedElementLoaded(source, eventArgs),
                OnDetachAction = (weakEventListener) => newfe.Loaded -= weakEventListener.OnEvent // Use Local References Only
            };
            newfe.Loaded += weakPropertyChangedListenerLoaded.OnEvent;

            // Track if AdornedElement is unloaded
            var weakPropertyChangedListenerUnloaded = new WeakEventListener<Adorner, object, RoutedEventArgs>(this)
            {
                OnEventAction = static (instance, source, eventArgs) => instance.OnAdornedElementUnloaded(source, eventArgs),
                OnDetachAction = (weakEventListener) => newfe.Unloaded -= weakEventListener.OnEvent // Use Local References Only
            };
            newfe.Unloaded += weakPropertyChangedListenerUnloaded.OnEvent;

            OnAttached();
        }
    }

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (AdornedElement is null) return;

        Width = AdornedElement.ActualSize.X;
        Height = AdornedElement.ActualSize.Y;
    }

    internal void OnLayoutUpdated(object? sender, object e)
    {
        // Note: Also called by the parent AdornerLayer when its size changes
        if (AdornerLayer is not null
            && AdornedElement is not null)
        {
            var coord = AdornerLayer.CoordinatesTo(AdornedElement);

            Canvas.SetLeft(this, coord.X);
            Canvas.SetTop(this, coord.Y);

            // Also update size
            OnSizeChanged(this, null!);
        }
    }

    private void OnAdornedElementLoaded(object source, RoutedEventArgs eventArgs)
    {
        if (AdornerLayer is null) return;

        OnAttached();
    }

    private void OnAdornedElementUnloaded(object source, RoutedEventArgs eventArgs)
    {
        if (AdornerLayer is null) return;

        OnDetaching();

        // TODO: Need to evaluate lifecycle a bit more, right now AdornerLayer (via attached property) mostly constrols the lifecycle
        // We could use private WeakReference to AdornedElement to re-listen for Loaded event and still remove/re-add via those
        // We just like to have the harder reference while we're active to make binding/interaction for Adorner implementer easier in XAML...
        //// AdornerLayer.RemoveAdorner(AdornerLayer, this);        
    }

    internal AdornerLayer? AdornerLayer { get; set; }

    /// <summary>
    /// Constructs a new instance of <see cref="Adorner"/>.
    /// </summary>
    public Adorner()
	{
		this.DefaultStyleKey = typeof(Adorner);
	}

    /// <summary>
    /// Called after the <see cref="Adorner"/> is attached to the <see cref="AdornedElement"/>.
    /// </summary>
    /// <remarks>
    /// Override this method in a subclass to initiate functionality of the <see cref="Adorner"/>.
    /// </remarks>
    protected virtual void OnAttached() { }

    /// <summary>
    /// Called when the <see cref="Adorner"/> is being detached from the <see cref="AdornedElement"/>.
    /// </summary>
    /// <remarks>
    /// Override this method to unhook functionality from the <see cref="AdornedElement"/>.
    /// </remarks>
    protected virtual void OnDetaching() { }

    /// <inheritdoc/>
    public new void UpdateLayout()
    {
        OnLayoutUpdated(this, null!);

        base.UpdateLayout();
    }
}
