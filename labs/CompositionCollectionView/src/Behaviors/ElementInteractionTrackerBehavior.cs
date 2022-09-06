// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections.Generic;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public class ElementInteractionTrackerBehavior<TId> : LayoutBehavior<TId>
{
    Dictionary<TId, InteractionTrackerBehavior<TId>> _elementTrackers = new();

    public InteractionTrackerBehavior<TId> CreateTrackerFor(ElementReference<TId> element)
    {
        if (TryGetTrackerFor(element.Id, out var tracker) && tracker is not null)
        {
            return tracker;
        }

        tracker = new InteractionTrackerBehavior<TId>(element.Container);
        tracker.Configure(Layout);
        _elementTrackers[element.Id] = tracker;
        return tracker;
    }

    public bool TryGetTrackerFor(TId id, out InteractionTrackerBehavior<TId>? tracker)
    {
        return _elementTrackers.TryGetValue(id, out tracker);
    }

    //override public void CleanupElement(ElementReference<TId> element)
    //{
    //    _elementTrackers.Remove(element.Id);
    //}

    override public void OnActivated()
    {
        foreach (var tracker in _elementTrackers)
        {
            tracker.Value.OnActivated();
        }
    }

    override public void OnDeactivated()
    {
        foreach (var tracker in _elementTrackers)
        {
            tracker.Value.OnDeactivated();
        }
    }
}
