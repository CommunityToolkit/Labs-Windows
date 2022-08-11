// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class SettingsExpanderItem : SettingsCard
{
    private const string RightState = "Right";
    private const string LeftState = "Left";
    private const string VerticalState = "Vertical";


    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="Header"/> property.
    /// </summary>
    public static readonly DependencyProperty ContentAlignmentProperty = DependencyProperty.Register(
        nameof(ContentAlignment),
        typeof(ContentAlignment),
        typeof(SettingsExpanderItem),
        new PropertyMetadata(defaultValue: ContentAlignment.Right, (d, e) => ((SettingsExpanderItem)d).OnContentAlignmentPropertyChanged((ContentAlignment)e.OldValue, (ContentAlignment)e.NewValue)));

    /// <summary>


    /// Gets or sets an example string. A basic DependencyProperty example.
    /// </summary>
    public ContentAlignment ContentAlignment
    {
        get => (ContentAlignment)GetValue(ContentAlignmentProperty);
        set => SetValue(ContentAlignmentProperty, value);
    }


    /// <summary>
    /// Creates a new instance of the <see cref="SettingsExpanderItem"/> class.
    /// </summary>
    public SettingsExpanderItem()
    {
        this.DefaultStyleKey = typeof(SettingsExpanderItem);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        OnContentAlignmentChanged();
    }

    protected virtual void OnContentAlignmentPropertyChanged(ContentAlignment oldValue, ContentAlignment newValue)
    {
        OnContentAlignmentChanged();
    }

    private void OnContentAlignmentChanged()
    {
        switch (ContentAlignment)
        {
            case ContentAlignment.Right: VisualStateManager.GoToState(this, RightState, true); break;
            case ContentAlignment.Left: VisualStateManager.GoToState(this, LeftState, true); break;
            case ContentAlignment.Vertical: VisualStateManager.GoToState(this, VerticalState, true); break;
        }
    }
}

public enum ContentAlignment
{
    Right,
    Left,
    Vertical
}

