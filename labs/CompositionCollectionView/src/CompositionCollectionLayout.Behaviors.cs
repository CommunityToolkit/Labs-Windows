// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using static CommunityToolkit.Labs.WinUI.AnimationConstants;

namespace CommunityToolkit.Labs.WinUI;
public abstract partial class CompositionCollectionLayout<TId, TItem> : ILayout, IDisposable where TId :notnull
{
    private List<CompositionCollectionLayoutBehavior<TId, TItem>> _behaviors = new();

    protected void AddBehavior(CompositionCollectionLayoutBehavior<TId, TItem> behavior)
    {
        if (IsActive)
        {
            behavior.Configure(this);
        }
        _behaviors.Add(behavior);
    }

    public T GetBehavior<T>() where T : CompositionCollectionLayoutBehavior<TId, TItem> =>
        _behaviors.OfType<T>().First();

    public T? TryGetBehavior<T>() where T : CompositionCollectionLayoutBehavior<TId, TItem> =>
        _behaviors.OfType<T>().FirstOrDefault();

    protected void RemoveBehavior(CompositionCollectionLayoutBehavior<TId, TItem> behavior)
    {
        _behaviors.Remove(behavior);
    }

    private void ConfigureElementBehaviors(ElementReference<TId, TItem> elementReference)
    {
        foreach (var behavior in _behaviors)
        {
            behavior.ConfigureElement(elementReference);
        }
    }

    private void CleanupElementBehaviors(ElementReference<TId, TItem> elementReference)
    {
        foreach (var behavior in _behaviors)
        {
            behavior.CleanupElement(elementReference);
        }
    }
}
