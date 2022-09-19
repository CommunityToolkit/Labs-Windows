// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#if !WINAPPSDK
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Animation = CommunityToolkit.Labs.WinUI.CompositionCollectionView.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public abstract class GesturePreviewControl : UserControl
{
    private ScalarNode? _opacity;
    private Matrix4x4Node? _transform;

    public ScalarNode? OpacityNode
    {
        get => _opacity;
        set
        {
            _opacity = value;
            ResetVisualState();
        }
    }
    public Matrix4x4Node? TransformNode
    {
        get => _transform;
        set
        {
            _transform = value;
            ResetVisualState();
        }
    }

    public virtual void ResetVisualState()
    {
        var visual = ElementCompositionPreview.GetElementVisual(this);
        if (TransformNode is { })
        {
            visual.StartAnimation(Animation.TransformMatrix, TransformNode);
        }
        if (OpacityNode is { })
        {
            visual.StartAnimation(Animation.Opacity, OpacityNode);
        }
    }

    public abstract void SetPageSize(int width, int height);
    public abstract Task StartCommandCompletedAnimation(float offset = 0);
}
#endif
