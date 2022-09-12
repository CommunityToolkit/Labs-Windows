// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
#if !WINAPPSDK
using System;
using System.Numerics;
using Windows.UI.Composition;
using static CommunityToolkit.Labs.WinUI.CompositionCollectionView.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public class AnimatableMatrix4x4CompositionNode : IDisposable
{
    private Visual _underlyingVisual;
    private bool disposedValue;

    public Matrix4x4 Value { get => _underlyingVisual.TransformMatrix; set => _underlyingVisual.TransformMatrix = value; }


    public AnimatableMatrix4x4CompositionNode(Compositor compositor)
    {
        _underlyingVisual = compositor.CreateShapeVisual();
    }

    public void Animate(CompositionAnimation animation)
    {
        _underlyingVisual.StartAnimation(TransformMatrix, animation);
    }

    public void Animate(ExpressionNode animation)
    {
        _underlyingVisual.StartAnimation(TransformMatrix, animation);
    }

    public Matrix4x4Node Reference { get => _underlyingVisual.GetReference().TransformMatrix; }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _underlyingVisual.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~AnimatableScalarCompositionNode()
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
#endif