// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Numerics;
using static CommunityToolkit.Labs.WinUI.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI;
public class AnimatableVector3CompositionNode : IDisposable
{
    private Visual _underlyingVisual;
    private bool disposedValue;
    private Vector3Node? _currentAnimationNode = null;

    public Vector3 Value
    {
        get
        {
            if (_currentAnimationNode is not null)
            {
                // When the node value is being driven by a ongoing scalarnode animation, reading the property might return a stale value,
                // so we instead evaluate the original expression to get the most accurate value
                return _currentAnimationNode.Evaluate();
            }
            else
            {
                return _underlyingVisual.Offset;
            }
        }
        set
        {
            _underlyingVisual.Offset = value;
            _currentAnimationNode = null;
        }
    }

    public AnimatableVector3CompositionNode(Compositor compositor)
    {
        _underlyingVisual = compositor.CreateShapeVisual();
    }

    public void Animate(CompositionAnimation animation)
    {
        _currentAnimationNode = null;
        _underlyingVisual.StartAnimation(Offset, animation);
    }

    public void Animate(Vector3Node animation)
    {
        _currentAnimationNode = animation;
        _underlyingVisual.StartAnimation(Offset, animation);
    }

    public Vector3Node Reference { get => _underlyingVisual.GetReference().Offset; }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                _underlyingVisual.Dispose();
                _currentAnimationNode?.Dispose();
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
