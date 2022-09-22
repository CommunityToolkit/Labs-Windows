// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static CommunityToolkit.Labs.WinUI.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI;
public interface ILayout
{
    Action<ILayout, ILayout>? LayoutReplaced { get; set; }

    void Activate(Panel panel);
}

public abstract partial class CompositionCollectionLayout<TId, TItem> : ILayout, IDisposable where TId : notnull
{
    public CompositionCollectionLayout(Func<TId, FrameworkElement> elementFactory)
    {
        ElementFactory = elementFactory;

        var compositor = Window.Current.Compositor;
        Properties = new BindableCompositionPropertySet(compositor.CreatePropertySet());
        AnimatableNodes = new AnimatableCompositionNodeSet(compositor);
    }

    public CompositionCollectionLayout(CompositionCollectionLayout<TId, TItem> sourceLayout)
    {
        ParentLayout = sourceLayout;

        ElementFactory = sourceLayout.ElementFactory;

        _uiRoot = sourceLayout._uiRoot;
        Properties = sourceLayout.Properties;
        AnimatableNodes = sourceLayout.AnimatableNodes;
    }

    private Dictionary<TId, ElementReference<TId, TItem>> _elements { get; } = new();
    public IEnumerable<TId> Source => _elements.Keys;
    public IEnumerable<ElementReference<TId, TItem>> Elements => _elements.Values;

    public Action<ILayout, ILayout>? LayoutReplaced { get; set; }
    public Panel RootPanel => GetVisualProperties().RootPanel;
    public Visual RootPanelVisual => GetVisualProperties().RootPanelVisual;
    public Compositor Compositor => GetVisualProperties().RootPanelVisual.Compositor;
    public BindableCompositionPropertySet Properties { private init; get; }
    public AnimatableCompositionNodeSet AnimatableNodes { private init; get; }

    public CompositionCollectionLayout<TId, TItem>? ParentLayout { get; private set; }


    public bool IsActive { get; private set; } = false;

    private bool disposedValue;

    public ElementReference<TId, TItem>? GetElement(TId obId) =>
        _elements.TryGetValue(obId, out var element) ? element : null;


    // Protected properties provided for convenience when implementing layouts
    protected ScalarNode ViewportWidthNode => RootPanelVisual.GetReference().Size.X;
    protected ScalarNode ViewportHeightNode => RootPanelVisual.GetReference().Size.Y;


    // Fields that we need to preserve across layouts
    protected Func<TId, FrameworkElement> ElementFactory { get; init; }

    private record UIRoot(Panel RootPanel, Visual RootPanelVisual);
    private UIRoot? _uiRoot;

    private UIRoot GetVisualProperties()
    {
        if (_uiRoot is null)
        {
            throw new InvalidOperationException("Tried to use this layout when it hasn't been activated yet.");
        }
        return _uiRoot;
    }

    public void ConfigureElementAnimation(ElementReference<TId, TItem> element, Visual? proxyVisual = null)
    {
        var visual = proxyVisual ?? element.Visual;

        visual.StartAnimation(Offset, GetElementPositionNode(element));

        var scale = GetElementScaleNode(element);
        visual.StartAnimation(Scale, ExpressionFunctions.Vector3(scale, scale, scale));

        visual.StartAnimation(AnimationConstants.Orientation, GetElementOrientationNode(element));

        visual.StartAnimation(Opacity, GetElementOpacityNode(element));
    }

    public void StopElementsAnimation()
    {
        foreach (var (_, element) in _elements)
        {
            StopElementAnimation(element);
        }
    }

    public void StopElementAnimation(ElementReference<TId, TItem> element)
    {
        element.Visual.StopAnimation(Offset);
        element.Visual.StopAnimation(Scale);
        element.Visual.StopAnimation(Opacity);
        element.Visual.StopAnimation(AnimationConstants.Orientation);
    }

    public record ElementTransition(uint Length, CompositionEasingFunction EasingFunction);

    public Vector3 GetElementPositionValue(ElementReference<TId, TItem> element, Vector3? trackerPosition = null) => GetElementPositionNode(element).Evaluate();
    public float GetElementScaleValue(ElementReference<TId, TItem> element) => GetElementScaleNode(element).Evaluate();
    public Quaternion GetElementOrientationValue(ElementReference<TId, TItem> element) => GetElementOrientationNode(element).Evaluate();
    public float GetElementOpacityValue(ElementReference<TId, TItem> element) => GetElementOpacityNode(element).Evaluate();

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (IsActive)
                {
                    //We only want to dipose these fields if the layout is still active
                    //If it isn't, that means other layout owns them now
                    Properties.Dispose();
                    //InteractionSource.Dispose();
                    //Tracker.Dispose();
                    AnimatableNodes.Dispose();
                }
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Layout()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
