// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINUI3 && HAS_UNO
using SkiaSharp;
using Uno.WinUI.Graphics2DSK;

namespace CommunityToolkit.WinUI.Rive;

internal sealed partial class RiveCanvasElement : SKCanvasElement
{
    private readonly RivePlayer _owner;

    public RiveCanvasElement(RivePlayer owner) => _owner = owner;

    protected override void RenderOverride(SKCanvas canvas, global::Windows.Foundation.Size area) =>
        _owner.PaintNextAnimationFrame(canvas, area);
}
#endif
