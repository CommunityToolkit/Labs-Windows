// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Numerics;
using Windows.UI.Composition;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public class AnimatableCompositionNodeSet : IDisposable
{
    private Dictionary<string, object> _nodes = new();
    private Compositor _compositor;

    private bool disposedValue;

    public AnimatableCompositionNodeSet(Compositor compositor)
    {
        _compositor = compositor;
    }

    public AnimatableScalarCompositionNode GetOrCreateScalarNode(string id, float defaultValue)
    {
        if (_nodes.ContainsKey(id))
        {
            if (_nodes[id] is AnimatableScalarCompositionNode node)
            {
                return node;
            }
            else
            {
                throw new InvalidCastException("Node {id} is not a scalar node");
            }
        }
        else
        {
            var newNode = new AnimatableScalarCompositionNode(_compositor);
            newNode.Value = defaultValue;
            _nodes[id] = newNode;
            return newNode;
        }
    }

    public AnimatableVector3CompositionNode GetOrCreateVector3Node(string id, Vector3 defaultValue)
    {
        if (_nodes.ContainsKey(id))
        {
            if (_nodes[id] is AnimatableVector3CompositionNode node)
            {
                return node;
            }
            else
            {
                throw new InvalidCastException("Node {id} is not a vector3 node");
            }
        }
        else
        {
            var newNode = new AnimatableVector3CompositionNode(_compositor);
            newNode.Value = defaultValue;
            _nodes[id] = newNode;
            return newNode;
        }
    }


    public AnimatableQuaternionCompositionNode GetOrCreateQuaternionNode(string id, Quaternion defaultValue)
    {
        if (_nodes.ContainsKey(id))
        {
            if (_nodes[id] is AnimatableQuaternionCompositionNode node)
            {
                return node;
            }
            else
            {
                throw new InvalidCastException("Node {id} is not a vector3 node");
            }
        }
        else
        {
            var newNode = new AnimatableQuaternionCompositionNode(_compositor);
            newNode.Value = defaultValue;
            _nodes[id] = newNode;
            return newNode;
        }
    }

    public AnimatableMatrix4x4CompositionNode GetOrCreateMatrix4x4Node(string id, Matrix4x4 defaultValue)
    {
        if (_nodes.ContainsKey(id))
        {
            if (_nodes[id] is AnimatableMatrix4x4CompositionNode node)
            {
                return node;
            }
            else
            {
                throw new InvalidCastException("Node {id} is not a matrix4x4 node");
            }
        }
        else
        {
            var newNode = new AnimatableMatrix4x4CompositionNode(_compositor);
            newNode.Value = defaultValue;
            _nodes[id] = newNode;
            return newNode;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
                foreach (var node in _nodes)
                {
                    if (node.Value is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
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
