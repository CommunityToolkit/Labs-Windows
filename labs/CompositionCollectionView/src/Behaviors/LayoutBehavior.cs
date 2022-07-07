#nullable enable

using System;

namespace CommunityToolkit.Labs.WinUI.CompositionCollectionView;
public abstract class LayoutBehavior<TId>
{
    protected Layout<TId> Layout => _layout is null ? throw new InvalidOperationException("Behavior has not been added to any layout yet") : _layout;
    private Layout<TId>? _layout = null;

    public void Configure(Layout<TId> layout)
    {
        if (_layout != layout)
        {
            _layout = layout;
            OnConfigure();
        }
    }

    virtual public void ConfigureElement(ElementReference<TId> element) { }
    virtual public void CleanupElement(ElementReference<TId> element) { }

    virtual public void OnConfigure() { }
    virtual public void OnActivated() { }
    virtual public void OnDeactivated() { }
}
