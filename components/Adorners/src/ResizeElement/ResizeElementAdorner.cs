// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Adorners;

/// <summary>
/// An <see cref="Adorner"/> that will allow a user to resize the adorned element.
/// </summary>
public sealed partial class ResizeElementAdorner : Adorner<FrameworkElement>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResizeElementAdorner"/> class.
    /// </summary>
    public ResizeElementAdorner()
    {
        this.DefaultStyleKey = typeof(ResizeElementAdorner);

        // Uno workaround
        DataContext = this;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        base.OnDetaching();
    }
}
