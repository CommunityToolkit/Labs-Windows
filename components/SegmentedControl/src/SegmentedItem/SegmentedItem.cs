// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

[ContentProperty(Name = nameof(Content))]
public partial class SegmentedItem : ListViewItem
{
    internal const string IconLeftState = "IconLeft";
    internal const string IconOnlyState = "IconOnly";
    internal const string ContentOnlyState = "ContentOnly";

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Icon"/> property.
    /// </summary>
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(IconElement),
        typeof(SegmentedItem),
        new PropertyMetadata(defaultValue: null, (d, e) => ((SegmentedItem)d).OnIconPropertyChanged((IconElement)e.OldValue, (IconElement)e.NewValue)));

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    public IconElement Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public SegmentedItem()
    {
        this.DefaultStyleKey = typeof(SegmentedItem);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        OnIconChanged();
        ContentChanged();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        ContentChanged();
    }

    private void ContentChanged()
    {
        if (Content != null)
        {
            VisualStateManager.GoToState(this, IconLeftState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, IconOnlyState, true);
        }
    }

    protected virtual void OnIconPropertyChanged(IconElement oldValue, IconElement newValue)
    {
        OnIconChanged();
    }

    private void OnIconChanged()
    {
        if (Icon != null)
        {
            VisualStateManager.GoToState(this, IconLeftState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, ContentOnlyState, true);
        }
    }
}
