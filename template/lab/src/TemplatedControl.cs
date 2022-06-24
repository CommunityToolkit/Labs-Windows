// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        if (element is not null)
        {
            element.HorizontalTextAlignment = TextAlignment.Left;
        }
    }

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="ItemPadding"/> property.
    /// </summary>
    public static readonly DependencyProperty ItemPaddingProperty = DependencyProperty.Register(
        nameof(ItemPadding),
        typeof(Thickness),
        typeof(TemplatedControl),
        new PropertyMetadata(defaultValue: new Thickness(0)));

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

    /// <summary>
    /// Gets or sets a padding for an item. A basic DependencyProperty example.
    /// </summary>
    /// <remarks>
    /// Works with {x:Bind MyProperty}, {TemplateBinding MyProperty}, and {Binding MyProperty, RelativeSource={RelativeSource Mode=TemplatedParent}}
    /// </remarks>
    public Thickness ItemPadding
    {
        get => (Thickness)GetValue(ItemPaddingProperty);
        set => SetValue(ItemPaddingProperty, value);
    }

    private void OnMyPropertyChanged(string oldValue, string newValue)
    {
        // Do something with the changed value.
    }
}
