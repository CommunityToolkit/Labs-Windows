// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Specialized;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// An Office like Ribbon control which displays groups of commands. If there is not enough space to display all the groups,
/// some of them can be collapsed based on a priority order.
/// </summary>
[ContentProperty(Name = nameof(Items))]
[TemplatePart(Name = PanelTemplatePart, Type = typeof(Panel))]
[TemplatePart(Name = ScrollViewerTemplatePart, Type = typeof(ScrollViewer))]
[TemplatePart(Name = ScrollDecrementButtonTempatePart, Type = typeof(ButtonBase))]
[TemplatePart(Name = ScrollIncrementButtonTempatePart, Type = typeof(ButtonBase))]
[TemplateVisualState(GroupName = ScrollButtonGroupNameTemplatePart, Name = NoButtonsStateTemplatePart)]
[TemplateVisualState(GroupName = ScrollButtonGroupNameTemplatePart, Name = DecrementButtonStateTemplatePart)]
[TemplateVisualState(GroupName = ScrollButtonGroupNameTemplatePart, Name = IncrementButtonStateTemplatePart)]
[TemplateVisualState(GroupName = ScrollButtonGroupNameTemplatePart, Name = BothButtonsStateTemplatePart)]
public sealed partial class Ribbon : Control
{
    private const string PanelTemplatePart = "Panel";
    private const string ScrollViewerTemplatePart = "ScrollViewer";
    private const string ScrollDecrementButtonTempatePart = "ScrollDecrementButton";
    private const string ScrollIncrementButtonTempatePart = "ScrollIncrementButton";
    private const string ScrollButtonGroupNameTemplatePart = "ScrollButtonGroup";
    private const string NoButtonsStateTemplatePart = "NoButtons";
    private const string DecrementButtonStateTemplatePart = "DecrementButton";
    private const string IncrementButtonStateTemplatePart = "IncrementButton";
    private const string BothButtonsStateTemplatePart = "BothButtons";

    private Panel? _panel;
    private ScrollViewer? _scrollViewer;
    private ButtonBase? _decrementButton;
    private ButtonBase? _incrementButton;
    private readonly ObservableCollection<UIElement> _items;

    /// <summary>
    /// The DP to store the <see cref="ScrollStep"/> property value.
    /// </summary>
    public static readonly DependencyProperty ScrollStepProperty = DependencyProperty.Register(
        nameof(ScrollStep),
        typeof(double),
        typeof(Ribbon),
        new PropertyMetadata(20.0));

    /// <summary>
    /// The amount to add or remove from the current scroll position.
    /// </summary>
    public double ScrollStep
    {
        get => (double)GetValue(ScrollStepProperty);
        set => SetValue(ScrollStepProperty, value);
    }

    public Ribbon()
    {
        DefaultStyleKey = typeof(Ribbon);

        _items = [];
        _items.CollectionChanged += OnItemsCollectionChanged;
    }

    public ICollection<UIElement> Items => _items;

    protected override void OnApplyTemplate()
    {
        _panel = GetTemplateChild(PanelTemplatePart) as Panel;
        if (_panel is not null)
        {
            foreach (var item in _items)
            {
                _panel.Children.Add(item);
            }

            _panel.SizeChanged += OnSizeChanged;
        }

        _decrementButton = GetTemplateChild(ScrollDecrementButtonTempatePart) as ButtonBase;
        if (_decrementButton is not null)
        {
            _decrementButton.Click += OnDecrementScrollViewer;
        }

        _incrementButton = GetTemplateChild(ScrollIncrementButtonTempatePart) as ButtonBase;
        if (_incrementButton is not null)
        {
            _incrementButton.Click += OnIncrementScrollViewer;
        }

        _scrollViewer = GetTemplateChild(ScrollViewerTemplatePart) as ScrollViewer;
        if (_scrollViewer is not null)
        {
            _scrollViewer.ViewChanged += OnViewChanged;
            _scrollViewer.SizeChanged += OnSizeChanged;
            UpdateScrollButtonsState();
        }
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_panel is null)
        {
            return;
        }

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        var element = (UIElement?)e.NewItems[i] ?? throw new ArgumentException("Item must not be null");
                        _panel.Children.Insert(e.NewStartingIndex + i, element);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems is not null)
                {
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        var element = (UIElement?)e.OldItems[i] ?? throw new ArgumentException("Item must not be null");
                        _panel.Children.Insert(e.OldStartingIndex, element);
                    }
                }
                break;
            case NotifyCollectionChangedAction.Replace:
                if (e.NewItems is not null)
                {
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        var element = (UIElement?)e.NewItems[i] ?? throw new ArgumentException("Item must not be null");
                        _panel.Children[e.NewStartingIndex + i] = element;
                    }
                }
                break;
            case NotifyCollectionChangedAction.Move:
                _panel.Children.Move((uint)e.OldStartingIndex, (uint)e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                _panel.Children.Clear();
                if (e.NewItems is not null)
                {
                    foreach (var newItem in e.NewItems)
                    {
                        _panel.Children.Add((UIElement)newItem);
                    }
                }
                break;
            default:
                throw new ArgumentException("Invalid value for NotifyCollectionChangedAction");
        }
    }

    private void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e) => UpdateScrollButtonsState();

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) => UpdateScrollButtonsState();

    private void UpdateScrollButtonsState()
    {
        if (_scrollViewer is null)
        {
            return;
        }

        if (_scrollViewer.ExtentWidth <= _scrollViewer.ViewportWidth)
        {
            VisualStateManager.GoToState(this, NoButtonsStateTemplatePart, useTransitions: true);
            return;
        }

        var showDecrement = _scrollViewer.HorizontalOffset >= 1;
        var showIncrement = _scrollViewer.ExtentWidth - _scrollViewer.HorizontalOffset - _scrollViewer.ViewportWidth >= 1;
        if (showDecrement && showIncrement)
        {
            VisualStateManager.GoToState(this, BothButtonsStateTemplatePart, useTransitions: true);
        }
        else if (showDecrement)
        {
            VisualStateManager.GoToState(this, DecrementButtonStateTemplatePart, useTransitions: true);
        }
        else if (showIncrement)
        {
            VisualStateManager.GoToState(this, IncrementButtonStateTemplatePart, useTransitions: true);
        }
        else
        {
            VisualStateManager.GoToState(this, NoButtonsStateTemplatePart, useTransitions: true);
        }
    }

    private void OnDecrementScrollViewer(object sender, RoutedEventArgs e)
        => _scrollViewer?.ChangeView(_scrollViewer.HorizontalOffset - ScrollStep, verticalOffset: null, zoomFactor: null);

    private void OnIncrementScrollViewer(object sender, RoutedEventArgs e)
        => _scrollViewer?.ChangeView(_scrollViewer.HorizontalOffset + ScrollStep, verticalOffset: null, zoomFactor: null);
}
