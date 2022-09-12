// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public abstract class LayoutBehavior<TId, TItem>
{
    protected Layout<TId, TItem> Layout => _layout is null ? throw new InvalidOperationException("Behavior has not been added to any layout yet") : _layout;
    private Layout<TId, TItem>? _layout = null;

    public void Configure(Layout<TId, TItem> layout)
    {
        if (_layout != layout)
        {
            _layout = layout;
            OnConfigure();
        }
    }

    virtual public void ConfigureElement(ElementReference<TId, TItem> element) { }
    virtual public void CleanupElement(ElementReference<TId, TItem> element) { }

    virtual public void OnConfigure() { }
    virtual public void OnActivated() { }
    virtual public void OnDeactivated() { }
}
