// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Composition.Interactions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public record ElementReference<TId, TItem> : IDisposable
{
    private bool disposedValue;

    public TId Id { get; init; }
    public FrameworkElement Container { get; init; }
    public Visual Visual { get; init; }
    public CompositionPropertySet CompositionProperties { get; init; }
    public AnimatableCompositionNodeSet AnimatableNodes { get; init; }
    public Layout<TId, TItem> Layout { get; internal set; }
    public TItem Model { get; internal set; }

    public ElementReference(TId id, TItem model, FrameworkElement container, Layout<TId, TItem> layout)
    {
        Id = id;
        Container = container;
        Visual = ElementCompositionPreview.GetElementVisual(Container);
        CompositionProperties = Visual.Compositor.CreatePropertySet();
        AnimatableNodes = new AnimatableCompositionNodeSet(Visual.Compositor);
        Layout = layout;
        Model = model;
    }

    internal void ReasignTo(Layout<TId, TItem> newLayout)
    {
        Layout = newLayout;
    }

    public void SetZIndex(int zIndex)
    {
        //TODO: allow allow elements to process their own z index changes
        Canvas.SetZIndex(Container, zIndex);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                CompositionProperties.Dispose();
                AnimatableNodes.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~ElementReference()
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
