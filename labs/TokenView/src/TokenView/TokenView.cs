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
    private const string? TokenViewScrollViewerName = "ScrollViewer";
    private const string? TokenViewScrollBackButtonName = "ScrollBackButton";
    private const string? TokenViewScrollForwardButtonName = "ScrollForwardButton";
   
    private ScrollViewer? _tokenViewScroller;
    private ButtonBase? _tokenViewScrollBackButton;
    private ButtonBase? _tokenViewScrollForwardButton;
    public event EventHandler<TokenRemovingEventArgs>? TokenRemoving;
    private bool _hasLoaded;

    /// <summary>
    /// Creates a new instance of the <see cref="TokenView"/> class.
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public TokenView()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        this.DefaultStyleKey = typeof(TokenView);

        // Container Generation Hooks
        RegisterPropertyChangedCallback(ItemsSourceProperty, ItemsSource_PropertyChanged);
        ItemContainerGenerator.ItemsChanged += ItemContainerGenerator_ItemsChanged;
    }

    protected override DependencyObject GetContainerForItemOverride() => new Token();

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is Token;
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

        if (element is Token Token)
        {
            Token.Loaded += Token_Loaded;
            Token.Removing += Token_Removing;

            if (Token.IsRemoveable != true && Token.ReadLocalValue(Token.IsRemoveableProperty) == DependencyProperty.UnsetValue)
            {
                var isRemovableBinding = new Binding()
                {
                    Source = this,
                    Path = new PropertyPath(nameof(CanRemoveTokens)),
                    Mode = BindingMode.OneWay,
                };
                Token.SetBinding(Token.IsRemoveableProperty, isRemovableBinding);
            }
        }
    }


    private bool RemoveItem()
    {
        var currentContainerItem = GetCurrentContainerItem();
        if (currentContainerItem != null && currentContainerItem.IsRemoveable)
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

    private Token? GetCurrentContainerItem()
    {
        if (ControlHelpers.IsXamlRootAvailable && XamlRoot != null)
        {
            return FocusManager.GetFocusedElement(XamlRoot) as Token;
        }
        else
        {
            return FocusManager.GetFocusedElement() as Token;
        }
    }
}
