// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System.Numerics;

namespace CommunityToolkit.Labs.WinUI;
public abstract partial class CompositionCollectionLayout<TId, TItem> : ILayout, IDisposable where TId : notnull
{
    /// <summary>
    /// Invoked only once, when a CompositionCollectionView transitions to this layout
    /// </summary>
    protected virtual void OnActivated() { }
    /// <summary>
    /// Invoked only once, when a CompositionCollectionView transitions away from this layout
    /// </summary>
    protected virtual void OnDeactivated() { }
    /// <summary>
    /// Invoked every time the list of instantiated element is updated, when the layout is activated and in any successive source updates
    /// </summary>
    protected virtual void OnElementsUpdated() { }

    /// <summary>
    /// Invoked only once per element and layout, after the element is registered with the layout (might happen on activation or when created on a source update)
    /// </summary>
    /// <param name="element"></param>
    protected virtual void ConfigureElement(ElementReference<TId, TItem> element) { }
    /// <summary>
    /// Invoked only once per element and layout, after the element is unregistered from the layout (might happen on deactivation or when destroyed on a source update)
    /// </summary>
    protected virtual void CleanupElement(ElementReference<TId, TItem> element) { }



    /// <summary>
    /// Invoked per-element as part of a source update, before its animation has been updated.
    /// This is where any composition property set/animation nodes can be updated
    /// </summary>
    /// <param name="element"></param>
    public virtual void UpdateElementData(ElementReference<TId, TItem> element) { }
    /// <summary>
    /// Invoked per-element as part of a source update, after its animation has been updated
    /// </summary>
    /// <param name="element"></param>
    public virtual void UpdateElement(ElementReference<TId, TItem> element) { }


    public virtual Vector3Node GetElementPositionNode(ElementReference<TId, TItem> element) => Vector3.Zero;
    public virtual ScalarNode GetElementScaleNode(ElementReference<TId, TItem> element) => 1;
    public virtual ScalarNode GetElementOpacityNode(ElementReference<TId, TItem> element) => 1;
    public virtual QuaternionNode GetElementOrientationNode(ElementReference<TId, TItem> element) => Quaternion.Identity;
    protected virtual ElementTransition? GetElementTransitionEasingFunction(ElementReference<TId, TItem> element) => null;

    // These methods have a default implementation evaluates the value of the node returned by the layout and should
    // be good enough for more cases. It should only be overriden when evaluating the nodes is not always enough to determine the latest value,
    // e.g. if the node depends on a reference to another node which is also animated through composition and returns a stale value when evaluated
    public virtual Vector3 GetElementPositionValue(ElementReference<TId, TItem> element) => GetElementPositionNode(element).Evaluate();
    public virtual float GetElementScaleValue(ElementReference<TId, TItem> element) => GetElementScaleNode(element).Evaluate();
    public virtual Quaternion GetElementOrientationValue(ElementReference<TId, TItem> element) => GetElementOrientationNode(element).Evaluate();
    public virtual float GetElementOpacityValue(ElementReference<TId, TItem> element) => GetElementOpacityNode(element).Evaluate();

}
