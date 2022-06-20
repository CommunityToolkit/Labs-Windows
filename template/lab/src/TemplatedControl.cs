// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.Foundation;

// TODO: remove any unneeded namespaces before creating a PR
//-:cnd:noEmit
#if !WINAPPSDK
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
#endif
//+:cnd:noEmit

namespace CommunityToolkit.Labs.WinUI.ProjectTemplate;

/// <summary>
/// An example templated control.
/// </summary>
[TemplatePart(Name = "PART_HelloWorld", Type = typeof(TextBlock))]
public class TemplatedControl : Control
{
    /// <summary>
    /// Creates a new instance of the <see cref="TemplatedControl"/> class.
    /// </summary>
    public TemplatedControl()
    {
        this.DefaultStyleKey = nameof(TemplatedControl);
        this.DataContext = this; // Allows using this control as the x:DataType in the template. Do not assign any custom classes to this, or it will break external binding.
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var element = GetTemplateChild("PART_HelloWorld") as TextBlock;
        if (element is null)
        {
            // Handle missing x:Name'd element from the template
            return;
        }

        element.Text = "Hello from code behind!";
    }

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="MyProperty"/> property.
    /// </summary>
    public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
        nameof(MyProperty),
        typeof(string),
        typeof(TemplatedControl),
        new PropertyMetadata(defaultValue: string.Empty, (d, e) => ((TemplatedControl)d).OnMyPropertyChanged((string)e.OldValue, (string)e.NewValue)));

    /// <summary>
    /// Gets or sets an example string. A basic DependencyProperty example.
    /// </summary>
    /// <remarks>
    /// Works with {x:Bind MyProperty}, {TemplateBinding MyProperty}, and {Binding MyProperty, RelativeSource={RelativeSource Mode=TemplatedParent}}
    /// </remarks>
    public string MyProperty
    {
        get => (string)GetValue(MyPropertyProperty);
        set => SetValue(MyPropertyProperty, value);
    }

    private void OnMyPropertyChanged(string oldValue, string newValue)
    {
        // Do something with the changed value.
    }
}
