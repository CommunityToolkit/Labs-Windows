// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System;
using System.Numerics;
using Windows.UI.Composition;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public static class AnimationConstants
{
    // Strings for all the animatable properties of a composition visual
    // as listed in https://docs.microsoft.com/en-us/uwp/api/windows.ui.composition.compositionobject.startanimation?view=winrt-19041
    public static Vector2PropertyName AnchorPoint { get; } = new Vector2PropertyName(nameof(AnchorPoint));
    public static Vector3PropertyName CenterPoint { get; } = new Vector3PropertyName(nameof(CenterPoint));
    public static Vector3PropertyName Offset { get; } = new Vector3PropertyName(nameof(Offset));
    public static Vector3PropertyName Translation { get; } = new Vector3PropertyName(nameof(Translation));
    public static Vector3PropertyName Scale { get; } = new Vector3PropertyName(nameof(Scale));
    public static string Opacity { get; } = nameof(Opacity);
    public static Vector4PropertyName Orientation { get; } = new Vector4PropertyName(nameof(Orientation));
    public static string RotationAngle { get; } = nameof(RotationAngle);
    public static Vector3PropertyName RotationAxis { get; } = new Vector3PropertyName(nameof(RotationAxis));
    public static Vector2PropertyName Size { get; } = new Vector2PropertyName(nameof(Size));
    public static string TransformMatrix { get; } = nameof(TransformMatrix);

    public class PropertyName
    {
        private readonly string _value;
        public PropertyName(string value)
        {
            this._value = value;
        }
        public static implicit operator string(PropertyName PropertyName) => PropertyName._value;
        public override string ToString() => _value;
    }

    public class Vector2PropertyName : PropertyName
    {
        public Vector2PropertyName(string value) : base(value)
        {
        }
        public string X => this + ".X";
        public string Y => this + ".Y";
    }

    public class Vector3PropertyName : Vector2PropertyName
    {
        public Vector3PropertyName(string value) : base(value)
        {
        }
        public string Z => this + ".Z";
    }
    public class Vector4PropertyName : Vector3PropertyName
    {
        public Vector4PropertyName(string value) : base(value)
        {
        }
        public string W => this + ".W";
    }
}
