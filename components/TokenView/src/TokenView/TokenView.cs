// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// This is an controĺ easily visualize tokens, to create filtering experiences.
/// </summary>

[TemplatePart(Name = TokenViewScrollViewerName, Type = typeof(ScrollViewer))]
[TemplatePart(Name = TokenViewScrollBackButtonName, Type = typeof(ButtonBase))]
[TemplatePart(Name = TokenViewScrollForwardButtonName, Type = typeof(ButtonBase))]
public partial class TokenView : ListViewBase
{
    private const string TokenViewScrollViewerName = "ScrollViewer";
    private const string TokenViewScrollBackButtonName = "ScrollBackButton";
    private const string TokenViewScrollForwardButtonName = "ScrollForwardButton";
    private int _internalSelectedIndex = -1;

    private ScrollViewer? _tokenViewScroller;
    private ButtonBase? _tokenViewScrollBackButton;
    private ButtonBase? _tokenViewScrollForwardButton;
    public event EventHandler<TokenItemRemovingEventArgs>? TokenItemRemoving;

    /// <summary>
    /// Creates a new instance of the <see cref="TokenView"/> class.
    /// </summary>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "The 'ItemsSource' change handler accesses the 'Remove' method of the collection in a trim-unsafe (we should revisit this later).")]
#endif
    public TokenView()
    {
        this.DefaultStyleKey = typeof(TokenView);

        // Container Generation Hooks
        RegisterPropertyChangedCallback(ItemsSourceProperty, ItemsSource_PropertyChanged);
        RegisterPropertyChangedCallback(SelectedIndexProperty, SelectedIndex_PropertyChanged);
    }

    protected override DependencyObject GetContainerForItemOverride() => new TokenItem();

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is TokenItem;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        SelectedIndex = _internalSelectedIndex;
        PreviewKeyDown -= TokenView_PreviewKeyDown;
        SizeChanged += TokenView_SizeChanged;
        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.Loaded -= ScrollViewer_Loaded;
        }

        _tokenViewScroller = GetTemplateChild(TokenViewScrollViewerName) as ScrollViewer;

        if (_tokenViewScroller != null)
        {
            _tokenViewScroller.Loaded += ScrollViewer_Loaded;
        }

        PreviewKeyDown += TokenView_PreviewKeyDown;
        OnIsWrappedChanged();
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is TokenItem tokenItem)
        {
            tokenItem.Loaded += Token_Loaded;
            tokenItem.Removing += Token_Removing;

            if (tokenItem.IsRemoveable != true && tokenItem.ReadLocalValue(TokenItem.IsRemoveableProperty) == DependencyProperty.UnsetValue)
            {
                var isRemovableBinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(CanRemoveTokens)),
                    Mode = BindingMode.OneWay,
                };
                tokenItem.SetBinding(TokenItem.IsRemoveableProperty, isRemovableBinding);
            }
        }
    }


    private bool RemoveItem()
    {
        if (GetCurrentContainerItem() is TokenItem currentContainerItem && currentContainerItem.IsRemoveable)
        {
            Items.Remove(currentContainerItem);
            return true;
        }
        else
        {
            return false;
        }
    }
   
    private void UpdateScrollButtonsVisibility()
    {
        if (_tokenViewScrollForwardButton != null && _tokenViewScroller != null)
        {
            if (_tokenViewScroller.ScrollableWidth > 0)
            {
                _tokenViewScrollForwardButton.Visibility = Visibility.Visible;
            }
            else
            {
                _tokenViewScrollForwardButton.Visibility = Visibility.Collapsed;
            }
        }
    }

    private TokenItem? GetCurrentContainerItem()
    {
        if (ControlHelpers.IsXamlRootAvailable && XamlRoot != null)
        {
            return FocusManager.GetFocusedElement(XamlRoot) as TokenItem;
        }
        else
        {
            return FocusManager.GetFocusedElement() as TokenItem;
        }
    }

    private void SelectedIndex_PropertyChanged(DependencyObject sender, DependencyProperty dp)
    {
        // This is a workaround for https://github.com/microsoft/microsoft-ui-xaml/issues/8257
        if (_internalSelectedIndex == -1 && SelectedIndex > -1)
        {
            // We catch the correct SelectedIndex and save it.
            _internalSelectedIndex = SelectedIndex;
        }
    }
}
