#nullable enable
#if !WINAPPSDK
using System.Numerics;
using Windows.UI.Composition;
using static CommunityToolkit.Labs.WinUI.CompositionCollectionView.AnimationConstants;


namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;

public class AnimatableScalarCompositionNode : IDisposable
{
    private Visual _underlyingVisual;
    private bool disposedValue;

    public float Value { get => _underlyingVisual.Offset.X; set => _underlyingVisual.Offset = new Vector3(value, 0, 0); }


    public AnimatableScalarCompositionNode(Compositor compositor)
    {
        _underlyingVisual = compositor.CreateShapeVisual();
    }

    public void Animate(CompositionAnimation animation)
    {
        _underlyingVisual.StartAnimation(Offset.X, animation);
    }

    public void Animate(ExpressionNode animation)
    {
        _underlyingVisual.StartAnimation(Offset.X, animation);
    }

    public ScalarNode Reference { get => _underlyingVisual.GetReference().Offset.X; }

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
