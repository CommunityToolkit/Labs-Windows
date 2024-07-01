// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A group which can be collapsed if its content does not fit in the <see cref="Ribbon"/>'s available space.
/// If the content does not fit, the group will display a single button. Clicking on this button will open
/// a flyout containing the group's content.
/// </summary>
[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = VisibleContentContainerTemplatePart, Type = typeof(ContentPresenter))]
[TemplatePart(Name = CollapsedButtonTemplatePart, Type = typeof(Button))]
[TemplatePart(Name = CollapsedFlyoutTemplatePart, Type = typeof(Flyout))]
[TemplatePart(Name = CollapsedContentPresenterTemplatePart, Type = typeof(ContentPresenter))]
public partial class RibbonCollapsibleGroup : RibbonGroup
{
    private const string VisibleContentContainerTemplatePart = "VisibleContentContainer";
    private const string CollapsedButtonTemplatePart = "CollapsedButton";
    private const string CollapsedFlyoutTemplatePart = "CollapsedFlyout";
    private const string CollapsedContentPresenterTemplatePart = "CollapsedContentPresenter";

    /// <summary>
    /// The DP to store the <see cref="IconSource"/> property value.
    /// </summary>
    public static readonly DependencyProperty IconSourceProperty = DependencyProperty.Register(
        nameof(IconSource),
        typeof(IconSource),
        typeof(RibbonCollapsibleGroup),
        new PropertyMetadata(null));

    /// <summary>
    /// The group icon. It will only be displayed when the group is in the collapsed state.
    /// </summary>
    public IconSource IconSource
    {
        get => (IconSource)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    /// <summary>
    /// The DP to store the <see cref="State"/> property value.
    /// </summary>
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(
        nameof(State),
        typeof(Visibility),
        typeof(RibbonCollapsibleGroup),
        new PropertyMetadata(Visibility.Visible, OnStatePropertyChanged));

    /// <summary>
    /// The state of the group.
    /// </summary>
    public Visibility State
    {
        get => (Visibility)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    /// <summary>
    /// The DP to store the <see cref="AutoCloseFlyout"/> property value.
    /// </summary>
    public static readonly DependencyProperty AutoCloseFlyoutProperty = DependencyProperty.Register(
        nameof(AutoCloseFlyout),
        typeof(bool),
        typeof(RibbonCollapsibleGroup),
        new PropertyMetadata(true));

    /// <summary>
    /// True to automatically close the overflow flyout if one interactive element is clicked.
    /// Note that the logic to detect the click is very basic. It will request the flyout to close
    /// for all the handled pointer released events. It assumes that if the pointer has been handled
    /// something reacted to it. It works well for buttons or check boxes but does not work for text
    /// or combo boxes.
    /// </summary>
    public bool AutoCloseFlyout
    {
        get => (bool)GetValue(AutoCloseFlyoutProperty);
        set => SetValue(AutoCloseFlyoutProperty, value);
    }

    /// <summary>
    /// The DP to store the <see cref="Priority"/> property value.
    /// </summary>
    public static readonly DependencyProperty PriorityProperty = DependencyProperty.Register(
        nameof(Priority),
        typeof(int),
        typeof(RibbonCollapsibleGroup),
        new PropertyMetadata(0));

    /// <summary>
    /// The priority of the group.
    /// The group with the lower priority will be the first one to be collapsed.
    /// </summary>
    public int Priority
    {
        get => (int)GetValue(PriorityProperty);
        set => SetValue(PriorityProperty, value);
    }


    /// <summary>
    /// The DP to store the <see cref="CollapsedAccessKey"/> property value.
    /// </summary>
    public static readonly DependencyProperty CollapsedAccessKeyProperty = DependencyProperty.Register(
        nameof(CollapsedAccessKey),
        typeof(string),
        typeof(RibbonCollapsibleGroup),
        new PropertyMetadata(string.Empty));

    /// <summary>
    /// The access key to access the collapsed button and open the flyout.
    /// </summary>
    public string CollapsedAccessKey
    {
        get => (string)GetValue(CollapsedAccessKeyProperty);
        set => SetValue(CollapsedAccessKeyProperty, value);
    }


    /// <summary>
    /// The DP to store the <see cref="RequestedWidths"/> property value.
    /// </summary>
    public static readonly DependencyProperty RequestedWidthsProperty = DependencyProperty.Register(
        nameof(RequestedWidths),
        typeof(DoubleList),
        typeof(RibbonCollapsibleGroup),
        new PropertyMetadata(null, OnRequestedWidthChanged));

    private static void OnRequestedWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != null)
        {
            ((DoubleList)e.NewValue).Sort();
        }
    }

    /// <summary>
    /// The list of requested widths for the group.
    /// If null or empty, the group will automatically use the size of its content.
    /// If set, the group will use the smallest provided width fitting in the ribbon.
    /// This is useful if the group contains a variable size control which can adjust
    /// its width (like a GridView with several items).
    /// </summary>
    public DoubleList RequestedWidths
    {
        get => (DoubleList)GetValue(RequestedWidthsProperty);
        set => SetValue(RequestedWidthsProperty, value);
    }

    private ContentControl? _visibleContentContainer;
    private ContentControl? _collapsedContentContainer;
    private Button? _collapsedButton;
    private Flyout? _collapsedFlyout;

    public RibbonCollapsibleGroup()
        => DefaultStyleKey = typeof(RibbonCollapsibleGroup);

    protected override void OnApplyTemplate()
    {
        if (_collapsedFlyout is not null)
        {
            _collapsedFlyout.Opened -= OnFlyoutOpened;
        }

        if (_collapsedContentContainer is not null)
        {
            _collapsedContentContainer.RemoveHandler(PointerReleasedEvent, new PointerEventHandler(OnFlyoutPointerReleased));
            _collapsedContentContainer.RemoveHandler(KeyUpEvent, new KeyEventHandler(OnFlyoutKeyUp));
        }

        _visibleContentContainer = GetOrThrow<ContentControl>(VisibleContentContainerTemplatePart);
        _collapsedContentContainer = GetOrThrow<ContentControl>(CollapsedContentPresenterTemplatePart);
        _collapsedButton = GetOrThrow<Button>(CollapsedButtonTemplatePart);
        _collapsedFlyout = GetOrThrow<Flyout>(CollapsedFlyoutTemplatePart);

        _collapsedFlyout.Opened += OnFlyoutOpened;
        _collapsedContentContainer.AddHandler(PointerReleasedEvent, new PointerEventHandler(OnFlyoutPointerReleased), handledEventsToo: true);
        _collapsedContentContainer.AddHandler(KeyUpEvent, new KeyEventHandler(OnFlyoutKeyUp), handledEventsToo: true);

        UpdateState();
    }

    private T GetOrThrow<T>(string templatePart) where T : class
        => GetTemplateChild(templatePart) is T element
            ? element
            : throw new ArgumentException($"{templatePart} missing or not of the expected type: {typeof(T).Name}");

    private void OnFlyoutOpened(object? sender, object e)
        => _collapsedContentContainer?.Focus(FocusState.Programmatic);

    private void OnFlyoutPointerReleased(object sender, PointerRoutedEventArgs e)
        => AutoCollapseFlyout(e.Handled, e.OriginalSource);

    private void OnFlyoutKeyUp(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key != VirtualKey.Enter && e.Key != VirtualKey.Space)
        {
            return;
        }

        AutoCollapseFlyout(e.Handled, e.OriginalSource);
    }

    private void AutoCollapseFlyout(bool eventHasBeenHandled, object originalSource)
    {
        // We only consider events which have been processed since it usually means
        // that a control has processed the event and that the click is not in an
        // empty/non-interactive area.
        if (eventHasBeenHandled && AutoCloseFlyout && _collapsedFlyout!.IsOpen && !DoesRoutedEventOriginateFromAFlyoutHost(originalSource as UIElement))
        {
            _collapsedFlyout.Hide();
        }
    }

    private bool DoesRoutedEventOriginateFromAFlyoutHost(UIElement? source)
    {
        while (source != null && source != _collapsedContentContainer)
        {
            // TODO: handle MUX variants in UWP
            if (source is DropDownButton ||
                source is ComboBox ||
                source is ComboBoxItem ||
                (source is Button buttonSource && buttonSource.Flyout != null) ||
                (source is FrameworkElement frameworkSource && FlyoutBase.GetAttachedFlyout(frameworkSource) != null))
            {
                return true;
            }

            source = VisualTreeHelper.GetParent(source) as UIElement;
        }

        return false;
    }

    private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var group = (RibbonCollapsibleGroup)d;
        group.UpdateState();
    }

    private void UpdateState()
    {
        if (_visibleContentContainer is null)
        {
            // Template is not ready yet.
            return;
        }

        switch (State)
        {
            case Visibility.Visible:
                _collapsedFlyout!.Hide();
                _collapsedContentContainer!.Content = null;
                _visibleContentContainer.Content = Content;

                _collapsedButton!.Visibility = Visibility.Collapsed;
                _visibleContentContainer.Visibility = Visibility.Visible;
                break;
            case Visibility.Collapsed:
                _visibleContentContainer.Content = null;
                _collapsedContentContainer!.Content = Content;

                _visibleContentContainer.Visibility = Visibility.Collapsed;
                _collapsedButton!.Visibility = Visibility.Visible;
                break;
            default:
                throw new ArgumentException("Invalid state");
        }
    }
}
