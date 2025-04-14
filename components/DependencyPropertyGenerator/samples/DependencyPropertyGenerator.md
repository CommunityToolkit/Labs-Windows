---
title: DependencyPropertyGenerator
author: Sergio0694
description: A source generator for DependencyProperty registration.
keywords: DependencyPropertyGenerator
dev_langs:
  - csharp
category: Xaml
subcategory: Developer
experimental: true
discussion-id: 449
issue-id: 621
icon: Assets/DependencyPropertyGenerator.png
---

The `DependencyPropertyGenerator` is a source generator that simplifies the registration of `DependencyProperty` fields in your XAML projects.
It automatically generates the boilerplate code needed for property registration, making it easier to work with dependency properties.

Dependency properties are a key feature in WinUI, allowing for property value inheritance, change notification, and data binding.

## Features

- Automatically generates dependency property boilerplate code.
- Supports customization through attributes.
- Ensures consistency and reduces manual errors.
- Improves readability and maintainability of code.

Like other Windows Community Toolkit components, the DependencyPropertyGenerator supports UWP, Uno Platform and the Windows App SDK.

## Using `GeneratedDependencyPropertyAttribute`

To use the `DependencyPropertyGenerator`, you need to add the `GeneratedDependencyPropertyAttribute` to your properties.
This attribute specifies the name of the dependency property and its default value.

For example, if you were to consume the `DependencyPropertyGenerator` from UWP:

```cs
namespace MyNamespace;

public partial class MyControl : DependencyObject
{
    // The GeneratedDependencyPropertyAttribute is used to specify which property to generate the DependencyProperty for.
    [GeneratedDependencyProperty(IsLocalCacheEnabled = true)]
    // It is required that you mark the property as partial, so that the generator can add additional code to its get() method.
    public partial int Number { get; set; }
}
```

The source generator would generate the following partial methods:

- `OnNumberSet`: "Executes the logic for when the `set` accessor `Number` is invoked"
- `OnNumberChanging`: "Executes the logic for when `Number` is changing."
- `OnNumberChanged`: "Executes the logic for when `Number` has just changed."
- `OnNumberPropertyChanged`: "Executes the logic for when `Number` has just changed."
- `OnPropertyChanged`: "Executes the logic for when any dependency property has just changed."

Alongside the following changes to the `Number` property:

```cs
namespace MyNamespace;

partial class MyControl
{
    /// <summary>
    /// The backing <see cref="DependencyProperty"/> instance for <see cref="Number"/>.
    /// </summary>
    public static readonly DependencyProperty NumberProperty = DependencyProperty.Register(
        name: "Number",
        propertyType: typeof(int),
        ownerType: typeof(MyControl),
        typeMetadata: null);

    public partial int Number
    {
        get => field;
        set
        {
            OnNumberSet(ref value);

            if (EqualityComparer<int>.Default.Equals(field, value))
            {
                return;
            }

            int __oldValue = field;

            OnNumberChanging(value);
            OnNumberChanging(__oldValue, value);

            field = value;

            object? __boxedValue = value;

            OnNumberSet(ref __boxedValue);

            SetValue(NumberProperty, __boxedValue);

            OnNumberChanged(value);
            OnNumberChanged(__oldValue, value);
        }
    }
}
```

> Other DP generators might prefer using an attribute on the class instead, however the Community Toolkit's generator requires you to put the attribute on individual properties.

> [!Sample DependencyPropertyGeneratorCustomSample]
