// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI;

/// <summary>
/// A base class for <see cref="Adorner"/>s allowing for explicit types.
/// </summary>
/// <typeparam name="T">The object type to attach to</typeparam>
public abstract partial class Adorner<T> : Adorner where T : UIElement
{
    /// <inheritdoc/>
    public new T? AdornedElement
    {
        get { return base.AdornedElement as T; }
    }

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();

        if (this.AdornedElement is null)
        {
            throw new InvalidOperationException($"AdornedElement {base.AdornedElement?.GetType().FullName} is not of type {typeof(T).FullName}");
        }
    }
}
