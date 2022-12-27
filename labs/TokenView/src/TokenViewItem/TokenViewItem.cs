// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = TabCloseButtonName, Type = typeof(ButtonBase))]
public partial class TokenViewItem : ListViewItem
{
    internal const string IconLeftState = "IconLeft";
    internal const string IconOnlyState = "IconOnly";
    internal const string ContentOnlyState = "ContentOnly";
    internal const string CloseButtonVisibleState = "CloseButtonVisible";
    internal const string CloseButtonNotVisibleState = "CloseButtonNotVisible";
    internal const string TabCloseButtonName = "PART_CloseButton";
    internal ButtonBase? _tabCloseButton;

    /// <summary>
    /// Fired when the Tab's close button is clicked.
    /// </summary>
    public event EventHandler<TokenViewItemClosingEventArgs>? Closing;

    public TokenViewItem()
    {
        this.DefaultStyleKey = typeof(TokenViewItem);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_tabCloseButton != null)
        {
            _tabCloseButton.Click -= TabCloseButton_Click;
        }

        _tabCloseButton = GetTemplateChild(TabCloseButtonName) as ButtonBase;

        if (_tabCloseButton != null)
        {
            _tabCloseButton.Click += TabCloseButton_Click;
        }

        IconChanged();
        ContentChanged();
        IsCloseableChanged();
    }

    private void TabCloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (IsClosable)
        {
            Closing?.Invoke(this, new TokenViewItemClosingEventArgs(Content, this));
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
            VisualStateManager.GoToState(this, IconLeftState, true);
        else
            VisualStateManager.GoToState(this, IconOnlyState, true);
    }

    protected virtual void OnIconPropertyChanged(IconElement oldValue, IconElement newValue)
    {
        IconChanged();
    }

    private void IconChanged()
    {
        if (Icon != null)
            VisualStateManager.GoToState(this, IconLeftState, true);
        else
            VisualStateManager.GoToState(this, ContentOnlyState, true);
    }

    protected virtual void OnIsCloseablePropertyChanged(bool oldValue, bool newValue)
    {
        IsCloseableChanged();
    }

    private void IsCloseableChanged()
    {
        if (IsClosable)
            VisualStateManager.GoToState(this, CloseButtonVisibleState, true);
        else
            VisualStateManager.GoToState(this, CloseButtonNotVisibleState, true);
    }
}
    public class TokenViewItemClosingEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabClosingEventArgs"/> class.
        /// </summary>
        /// <param name="item">Item being closed.</param>
        /// <param name="tab"><see cref="TabViewItem"/> container being closed.</param>
        public TokenViewItemClosingEventArgs(object item, TokenViewItem tokenViewItem)
        {
            Item = item;
            TokenViewItem = tokenViewItem;
        }

        /// <summary>
        /// Gets the Item being closed.
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// Gets the Tab being closed.
        /// </summary>
        public TokenViewItem TokenViewItem { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the notification should be closed.
        /// </summary>
        public bool Cancel { get; set; }
    }

