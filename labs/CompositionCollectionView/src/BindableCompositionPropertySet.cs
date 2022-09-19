// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#if !WINAPPSDK
#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Text;
using Windows.UI;
using Windows.UI.Composition;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;

public class BindableCompositionPropertySet : INotifyPropertyChanged, IDisposable
{
    private CompositionPropertySet _propertySet;
    private bool disposedValue;

    public event PropertyChangedEventHandler? PropertyChanged;

    public BindableCompositionPropertySet(CompositionPropertySet propertySet)
    {
        _propertySet = propertySet;
    }

    public void InsertColor(string propertyName, Color value)
    {
        _propertySet.InsertColor(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertMatrix3x2(string propertyName, Matrix3x2 value)
    {
        _propertySet.InsertMatrix3x2(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertMatrix4x4(string propertyName, Matrix4x4 value)
    {
        _propertySet.InsertMatrix4x4(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertQuaternion(string propertyName, Quaternion value)
    {
        _propertySet.InsertQuaternion(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertScalar(string propertyName, float value)
    {
        _propertySet.InsertScalar(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertVector2(string propertyName, Vector2 value)
    {
        _propertySet.InsertVector2(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertVector3(string propertyName, Vector3 value)
    {
        _propertySet.InsertVector3(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertVector4(string propertyName, Vector4 value)
    {
        _propertySet.InsertVector4(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    public void InsertBoolean(string propertyName, bool value)
    {
        _propertySet.InsertBoolean(propertyName, value);
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public CompositionGetValueStatus TryGetColor(string propertyName, out Color value) => _propertySet.TryGetColor(propertyName, out value);
    public CompositionGetValueStatus TryGetMatrix3x2(string propertyName, out Matrix3x2 value) => _propertySet.TryGetMatrix3x2(propertyName, out value);
    public CompositionGetValueStatus TryGetMatrix4x4(string propertyName, out Matrix4x4 value) => _propertySet.TryGetMatrix4x4(propertyName, out value);
    public CompositionGetValueStatus TryGetQuaternion(string propertyName, out Quaternion value) => _propertySet.TryGetQuaternion(propertyName, out value);
    public CompositionGetValueStatus TryGetScalar(string propertyName, out float value) => _propertySet.TryGetScalar(propertyName, out value);
    public CompositionGetValueStatus TryGetVector2(string propertyName, out Vector2 value) => _propertySet.TryGetVector2(propertyName, out value);
    public CompositionGetValueStatus TryGetVector3(string propertyName, out Vector3 value) => _propertySet.TryGetVector3(propertyName, out value);
    public CompositionGetValueStatus TryGetVector4(string propertyName, out Vector4 value) => _propertySet.TryGetVector4(propertyName, out value);
    public CompositionGetValueStatus TryGetBoolean(string propertyName, out bool value) => _propertySet.TryGetBoolean(propertyName, out value);

    public PropertySetReferenceNode GetReference() => _propertySet.GetReference();

    #region IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _propertySet.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BindableCompositionPropertySet()
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
    #endregion
}
#endif
