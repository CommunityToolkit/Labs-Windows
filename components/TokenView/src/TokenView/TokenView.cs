// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
   
    private ScrollViewer? _tokenViewScroller;
    private ButtonBase? _tokenViewScrollBackButton;
    private ButtonBase? _tokenViewScrollForwardButton;
    public event EventHandler<TokenItemRemovingEventArgs>? TokenItemRemoving;
    private bool _hasLoaded;

    /// <summary>
    /// Creates a new instance of the <see cref="TokenView"/> class.
    /// </summary>
    public TokenView()
    {
        this.DefaultStyleKey = typeof(TokenView);

        // Container Generation Hooks
        RegisterPropertyChangedCallback(ItemsSourceProperty, ItemsSource_PropertyChanged);

#if !HAS_UNO
        ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
#endif
    }

    protected override DependencyObject GetContainerForItemOverride() => new TokenItem();

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is TokenItem;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

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
        OnOrientationChanged();
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
}
