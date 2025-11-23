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

            // Track if AdornedElement is unloaded
            var weakPropertyChangedListenerUnloaded = new WeakEventListener<Adorner, object, RoutedEventArgs>(this)
            {
                OnEventAction = static (instance, source, eventArgs) => instance.OnUnloaded(source, eventArgs),
                OnDetachAction = (weakEventListener) => newfe.Unloaded -= weakEventListener.OnEvent // Use Local References Only
            };
            newfe.Unloaded += weakPropertyChangedListenerUnloaded.OnEvent;
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
        }
    }

    private void OnUnloaded(object source, RoutedEventArgs eventArgs)
    {
        if (AdornerLayer is null) return;

        AdornerLayer.RemoveAdorner(AdornerLayer, this);        
    }

    internal AdornerLayer? AdornerLayer { get; set; }

    /// <summary>
    /// Constructs a new instance of <see cref="Adorner"/>.
    /// </summary>
    public Adorner()
	{
		this.DefaultStyleKey = typeof(Adorner);
	}
}
