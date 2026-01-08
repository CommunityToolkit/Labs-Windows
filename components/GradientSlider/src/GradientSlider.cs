// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// An example templated control.
/// </summary>
public partial class GradientSlider : Control
{
    /// <summary>
    /// Creates a new instance of the <see cref="GradientSlider"/> class.
    /// </summary>
    public GradientSlider()
    {
        this.DefaultStyleKey = typeof(GradientSlider);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }
}
