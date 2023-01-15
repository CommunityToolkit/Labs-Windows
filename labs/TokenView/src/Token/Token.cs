// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = TokenRemoveButtonName, Type = typeof(ButtonBase))]
public partial class Token : ListViewItem
{
    internal const string IconLeftState = "IconLeft";
    internal const string IconOnlyState = "IconOnly";
    internal const string ContentOnlyState = "ContentOnly";
    internal const string RemoveButtonVisibleState = "RemoveButtonVisible";
    internal const string RemoveButtonNotVisibleState = "RemoveButtonNotVisible";
    internal const string TokenRemoveButtonName = "PART_RemoveButton";
    internal ButtonBase? _TokenRemoveButton;

    /// <summary>
    /// Fired when the Tab's close button is clicked.
    /// </summary>
    public event EventHandler<TokenRemovingEventArgs>? Removing;

    public Token()
    {
        this.DefaultStyleKey = typeof(Token);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_TokenRemoveButton != null)
        {
            _TokenRemoveButton.Click -= TabCloseButton_Click;
        }

        _TokenRemoveButton = GetTemplateChild(TokenRemoveButtonName) as ButtonBase;

        if (_TokenRemoveButton != null)
        {
            _TokenRemoveButton.Click += TabCloseButton_Click;
        }

        IconChanged();
        ContentChanged();
        IsRemoveableChanged();
    }

    private void TabCloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsRemoveable)
        {
            Removing?.Invoke(this, new TokenRemovingEventArgs(Content, this));
        }
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
        IconChanged();
    }

    private void IconChanged()
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

    protected virtual void OnIsRemoveablePropertyChanged(bool oldValue, bool newValue)
    {
        IsRemoveableChanged();
    }

    private void IsRemoveableChanged()
    {
        if (IsRemoveable)
        {
            VisualStateManager.GoToState(this, RemoveButtonVisibleState, true);
        }
        else
        {
            VisualStateManager.GoToState(this, RemoveButtonNotVisibleState, true);
        }
    }
}
