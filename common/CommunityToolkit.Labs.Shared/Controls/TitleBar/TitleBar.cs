// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


// A control to show a Fluent titlebar

namespace CommunityToolkit.Labs.Shared.Controls;

[TemplateVisualState(Name = "Visible", GroupName = "BackButtonStates")]
[TemplateVisualState(Name = "Collapsed", GroupName = "BackButtonStates")]
[TemplatePart(Name = PartIconPresenter, Type = typeof(Button))]
public sealed class TitleBar : Control
{
    private const string PartIconPresenter = "PART_BackButton";
    private Button? _backButton;
    private TitleBar? _titleBar;
  
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TitleBar), new PropertyMetadata(default(string)));

    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TitleBar), new PropertyMetadata(default(ImageSource)));

    public bool IsBackButtonVisible
    {
        get => (bool)GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(TitleBar), new PropertyMetadata(default(bool), IsBackButtonVisibleChanged));

    public event EventHandler<RoutedEventArgs>? BackButtonClick;


    public TitleBar()
    {
        this.DefaultStyleKey = typeof(TitleBar);
    }

    protected override void OnApplyTemplate()
    {
        Update();
        _titleBar = (TitleBar)this;
        _backButton = (Button)_titleBar.GetTemplateChild(PartIconPresenter);
        _backButton.Click += _backButton_Click;
        base.OnApplyTemplate();
    }

    private void _backButton_Click(object sender, RoutedEventArgs e)
    {
        OnBackButtonClicked();
    }

    private void OnBackButtonClicked()
    {
        BackButtonClick?.Invoke(this, new RoutedEventArgs());
    }

    private static void IsBackButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((TitleBar)d).Update();
    }

    private void Update()
    {
        VisualStateManager.GoToState(this, IsBackButtonVisible ? "Visible" : "Collapsed", true);
    }
}
