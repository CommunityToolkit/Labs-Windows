// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable

namespace CommunityToolkit.Labs.WinUI;
public class ElementInteractionTrackerBehavior<TId, TItem> : CompositionCollectionLayoutBehavior<TId, TItem> where TId : notnull
{
    Dictionary<TId, InteractionTrackerBehavior<TId, TItem>> _elementTrackers = new();

    public override void Configure(CompositionCollectionLayout<TId, TItem> layout)
    {
        base.Configure(layout);
        foreach (var tracker in _elementTrackers)
        {
            tracker.Value.Configure(layout);
        }
    }

    public InteractionTrackerBehavior<TId, TItem> CreateTrackerFor(ElementReference<TId, TItem> element)
    {
        if (TryGetTrackerFor(element.Id, out var tracker) && tracker is not null)
        {
            return tracker;
        }

        tracker = new InteractionTrackerBehavior<TId, TItem>(element.Container);
        tracker.Configure(Layout);
        _elementTrackers[element.Id] = tracker;
        return tracker;
    }

    public bool TryGetTrackerFor(TId id, out InteractionTrackerBehavior<TId, TItem>? tracker)
    {
        return _elementTrackers.TryGetValue(id, out tracker);
    }

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
