---
title: DependencyPropertyGenerator
author: Sergio0694
description: Generate dependency properties for WinUI 3 and UWP controls with partial properties, configurable defaults, callbacks, and optional local caching.
keywords: DependencyPropertyGenerator, GeneratedDependencyProperty, DependencyProperty, SourceGenerator, WinUI, UWP, XAML
dev_langs:
  - csharp
category: Helpers
subcategory: Developer
experimental: true
discussion-id: 449
issue-id: 621
icon: Assets/icon.png
---

# DependencyPropertyGenerator

The `DependencyPropertyGenerator` creates the registration, identifier field, and CLR property implementation for a XAML dependency property. Add `[GeneratedDependencyProperty]` to a partial property instead of writing the `DependencyProperty.Register`, `GetValue`, and `SetValue` boilerplate yourself.

Generated properties participate in the existing XAML property system, including bindings, styles, and property-changed notifications. The generator also provides optional partial methods for customizing accessors and observing changes, configurable default values, and local caching for scenarios where all writes go through the CLR property.

> [!NOTE]
> This component is an experiment in Windows Community Toolkit Labs. Its API and behavior can change before graduation to the Windows Community Toolkit.

## Getting started

Install the package that matches your application's XAML framework from the [Community Toolkit Labs NuGet feed](https://aka.ms/toolkit/wiki/previewpackages):

| Framework | Package | XAML namespace |
| --- | --- | --- |
| WinUI 3 / Windows App SDK | `CommunityToolkit.Labs.WinUI.DependencyPropertyGenerator` | `Microsoft.UI.Xaml` |
| UWP | `CommunityToolkit.Labs.Uwp.DependencyPropertyGenerator` | `Windows.UI.Xaml` |

Both packages expose the attribute in the `CommunityToolkit.WinUI` namespace. Install the package in the project that declares the generated properties, not just in an application that references that project.

Use a compiler with Roslyn 4.12 or later, such as the .NET 9 SDK and Visual Studio 2022 version 17.12 or later. Partial properties require C# 13 or later. The compiler requirement does not mean your application must target .NET 9: the generator also supports the UWP and .NET target frameworks provided by the package.

Set the language version explicitly if your project's target framework would otherwise select an older version:

```xml
<PropertyGroup>
  <LangVersion>13.0</LangVersion>
  <Nullable>enable</Nullable>
</PropertyGroup>
```

The examples on this page use WinUI 3. For UWP, replace `Microsoft.UI.Xaml` and `Microsoft.UI.Xaml.Controls` with `Windows.UI.Xaml` and `Windows.UI.Xaml.Controls`.

## Declaring a dependency property

The containing class must be `partial` and derive from `DependencyObject`, directly or through a type such as `Control`, `UserControl`, or `Page`. Declare an instance partial property with `get;` and `set;` accessors and let the generator provide its implementation:

```csharp
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;

namespace MyApp.Controls;

public partial class CounterControl : Control
{
    [GeneratedDependencyProperty]
    public partial int Count { get; set; }

    [GeneratedDependencyProperty(DefaultValue = "Ready")]
    public partial string Caption { get; set; }
}
```

For `Count`, the generator registers a dependency property named `"Count"`, with `typeof(int)` as the property type and `typeof(CounterControl)` as the owner type. It also creates the public static readonly `CountProperty` identifier and implements the CLR accessors. `Caption` similarly gets a `CaptionProperty` identifier.

Use the properties like any other dependency properties. For example, after mapping the `local` XAML namespace to `MyApp.Controls`:

```xml
<local:CounterControl Count="5" Caption="Items" />
```

You can also use `CounterControl.CountProperty` with APIs such as `GetValue`, `SetValue`, and `ClearValue`. Do not declare the identifier field or another implementation of the partial property yourself.

Without local caching, the getter reads the effective value from the XAML property system. The setter writes to that system, using optimized `XamlBindingHelper` APIs where available and appropriate. The generator handles the details, including falling back to `SetValue` for null or empty strings.

## Attribute options

| Option | Purpose | Default behavior |
| --- | --- | --- |
| `DefaultValue` | Specify a constant default value for the property's metadata. | Use the property's default value, as described below. |
| `DefaultValueCallback` | Name a static factory that supplies an instance's default value. | No factory. |
| `PropertyType` | Override the type used to register the property in metadata. | Match the declared property type. |
| `IsLocalCacheEnabled` | Store the CLR property's value in a compiler-generated backing field. | Disabled. |

`DefaultValue` and `DefaultValueCallback` are mutually exclusive, including when `DefaultValue` is explicitly set to `null`.

## Default values

When neither default-value option is specified, reference types and nullable value types default to `null`, and non-nullable value types default to their zero-initialized value. For example, `int` defaults to `0`, `bool` defaults to `false`, and `int?` defaults to `null`. For generic properties, the generator accounts for the type parameter's constraints and uses `default(T)` where needed.

The generator may omit explicit `PropertyMetadata` when the XAML property system already supplies the required default and there are no metadata callbacks. An omitted metadata object is an implementation optimization, not a request for an arbitrary default.

### Constant defaults

Use `DefaultValue` for values that can be represented in an attribute argument:

```csharp
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class DefaultValues : DependencyObject
{
    [GeneratedDependencyProperty(DefaultValue = 42)]
    public partial int Count { get; set; }

    [GeneratedDependencyProperty(DefaultValue = true)]
    public partial bool IsActive { get; set; }

    [GeneratedDependencyProperty(DefaultValue = "Ready")]
    public partial string Caption { get; set; }

    [GeneratedDependencyProperty(DefaultValue = 42)]
    public partial int? OptionalCount { get; set; }

    [GeneratedDependencyProperty(DefaultValue = Visibility.Collapsed)]
    public partial Visibility DisplayVisibility { get; set; }
}
```

Match the constant's type to the declared property type, or to the underlying type of a nullable value type. For example, use `42d` for a `double` property rather than the `int` constant `42`. Boxing a constant into the attribute's `object` parameter does not change the type of that constant.

Use a factory for defaults that cannot be expressed as attribute arguments, such as a new collection or a custom object.

### Default-value factories

`DefaultValueCallback` names a method in the same containing type. Use `nameof` so renaming the method also updates the attribute:

```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class CollectionOwner : DependencyObject
{
    [GeneratedDependencyProperty(DefaultValueCallback = nameof(CreateItems))]
    public partial ObservableCollection<string> Items { get; set; }

    private static ObservableCollection<string> CreateItems() => new();

    [GeneratedDependencyProperty(DefaultValueCallback = nameof(CreateOptionalCount))]
    public partial int? OptionalCount { get; set; }

    private static int CreateOptionalCount() => 42;
}
```

The factory must be static, parameterless, and callable without type arguments. Its return type must be the exact property type, `object`, or, for a nullable value-type property, the underlying value type. For example, an `int?` property can use an `int?`, `int`, or `object` factory. An object-returning factory is responsible for returning a value compatible with the property.

The factory is registered with `PropertyMetadata.Create`; the XAML property system requests the default for each owning instance. This makes a factory suitable for defaults that should not be shared, such as the collection in this example. The factory is not a CLR getter callback, and it has no access to the owning instance through a `this` reference or a callback parameter.

Value-type and unconstrained generic results are boxed by the generated callback adapter when necessary. Reference-type and object-returning factories can be used directly. A factory can be combined with either or both of the property-changed callbacks described below.

Keep local caching disabled for properties that use `DefaultValueCallback`: the factory initializes dependency-property storage, not the CLR property's cached field.

### The unset-value sentinel

For advanced scenarios, use `GeneratedDependencyProperty.UnsetValue` as the `DefaultValue` argument:

```csharp
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class OptionalValueOwner : DependencyObject
{
    [GeneratedDependencyProperty(DefaultValue = GeneratedDependencyProperty.UnsetValue)]
    public partial object? Value { get; set; }
}
```

The generator replaces this attribute-only placeholder with the XAML framework's actual `DependencyProperty.UnsetValue` sentinel. It is not equivalent to `DefaultValue = null`. Do not read or compare against `GeneratedDependencyProperty.UnsetValue` in application code; use `DependencyProperty.UnsetValue` there.

This option cannot be combined with local caching. For a strongly typed property, ensure that its getter can handle any value supplied by the property system before casting it, using the boxed getter hook when necessary.

## Customizing the metadata type

`PropertyType` changes the type passed to `DependencyProperty.Register`, without changing the CLR property type:

```csharp
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class SelectionOwner : DependencyObject
{
    [GeneratedDependencyProperty(PropertyType = typeof(object))]
    public partial bool? IsSelected { get; set; }
}
```

This can help with classic reflection-based XAML binding scenarios that need a nullable value-type property registered as `object`. Prefer the default registration unless a specific scenario requires an override.

The metadata type must be compatible with the declared type. The analyzer rejects incompatible types and warns when an explicit type is redundant. The option does not add a value converter: in this example, values read through the CLR getter must still be compatible with `bool?`.

## Receiving property-changed notifications

There are two different kinds of notifications: callbacks registered with the dependency-property system, and hooks in the generated CLR accessors. Choose the dependency-property callbacks when changes from bindings, styles, animations, or direct `SetValue` calls must be observed.

Implement `On<PropertyName>PropertyChanged` to observe one generated property. Implement `OnPropertyChanged` to observe all generated properties declared in the same containing type:

```csharp
using System.Diagnostics;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class ObservedValues : DependencyObject
{
    [GeneratedDependencyProperty]
    public partial int Count { get; set; }

    [GeneratedDependencyProperty(DefaultValue = "Ready")]
    public partial string Caption { get; set; }

    partial void OnCountPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine($"Count changed from {e.OldValue} to {e.NewValue}.");
    }

    partial void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine($"{e.Property} changed.");
    }
}
```

When both callbacks are implemented, the property-specific callback runs before the shared callback. `OnPropertyChanged` is not a subscription to every dependency property on the object, including unrelated or inherited properties: it is wired into the metadata for this type's generated properties.

The generator only registers the callbacks you implement. Implement the partial methods without an explicit accessibility modifier, matching the generated signature.

## Customizing CLR accessors

Accessor hooks run only when application code invokes the generated CLR property. They are not a replacement for dependency-property callbacks, because the XAML property system can bypass CLR accessors.

For an `int` property named `Count`, these hooks are available without local caching:

| Hook | When it runs |
| --- | --- |
| `OnCountGet(ref object propertyValue)` | After `GetValue`, before the result is cast to `int`. |
| `OnCountGet(ref int propertyValue)` | After the cast, before the getter returns. |
| `OnCountSet(ref int propertyValue)` | Before the setter writes the value. |
| `OnCountSet(ref object propertyValue)` | After boxing, before `SetValue`, on the boxed setter path. |
| `OnCountChanged(int newValue)` | After the CLR setter writes the value. |

Implement either, both, or none of the typed and boxed hooks. Hooks with `ref` parameters can replace the value. For example, this implementation normalizes assignments made through `Count`:

```csharp
using System;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class NormalizedCounter : DependencyObject
{
    [GeneratedDependencyProperty]
    public partial int Count { get; set; }

    partial void OnCountSet(ref int propertyValue)
    {
        propertyValue = Math.Max(0, propertyValue);
    }
}
```

For ordinary properties without local caching, implementing the boxed setter hook makes the generator use the `SetValue` path instead of a typed `XamlBindingHelper` optimization, so the hook can inspect or replace the boxed value. The boxed getter hook can normalize values before a cast that would otherwise fail. Replacements must remain compatible with the declared property and metadata types.

An object-typed property has only one getter hook and one setter hook, rather than duplicate typed and boxed signatures. Nullable annotations on hook parameters follow the generated property's signatures.

Without local caching, the setter does not compare old and new values before running `OnCountChanged(int newValue)`. That hook can therefore run on repeated assignments of the same value. In contrast, `OnCountPropertyChanged` follows the XAML property's effective-value change notifications.

Unimplemented partial hooks and their calls are removed by the C# compiler. Do not call a hook directly to change a dependency property; assign the property or use the XAML property APIs.

> [!IMPORTANT]
> Setter normalization is not system-wide dependency-property coercion. A binding, animation, or direct `SetValue` call can bypass `OnCountSet`. Keep this distinction in mind when enforcing invariants.

## Local caching

Local caching is an opt-in optimization for properties whose values are read and written exclusively through the generated CLR property. It stores the value in a compiler-generated backing field, avoids `GetValue` on reads, and skips equal-value writes.

The current generator requires C# `preview` when `IsLocalCacheEnabled` is enabled, because the generated implementation uses the `field` keyword:

```xml
<PropertyGroup>
  <LangVersion>preview</LangVersion>
</PropertyGroup>
```

```csharp
using System.Diagnostics;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class CachedCounter : DependencyObject
{
    [GeneratedDependencyProperty(IsLocalCacheEnabled = true, DefaultValue = 42)]
    public partial int Count { get; set; }

    partial void OnCountChanging(int newValue)
    {
        Debug.WriteLine($"About to assign {newValue}.");
    }

    partial void OnCountChanging(int oldValue, int newValue)
    {
        Debug.WriteLine($"Changing from {oldValue} to {newValue}.");
    }

    partial void OnCountChanged(int oldValue, int newValue)
    {
        Debug.WriteLine($"Assigned {newValue}, replacing {oldValue}.");
    }
}
```

The generated setter first invokes the typed `OnCountSet` hook, then compares the new value with the cached value using `EqualityComparer<T>.Default`. If they are equal, it returns without writing to the property system or invoking the changing/changed hooks. Otherwise, it invokes both `OnCountChanging` overloads, updates the field, writes to the dependency-property system, and invokes `OnCountChanged(newValue)` followed by `OnCountChanged(oldValue, newValue)`.

The getter returns the cached field directly, so getter hooks are not generated in this mode. Constant defaults initialize both the metadata and the cached field. For reference types, the `oldValue` parameter in a two-value hook can be nullable even when the property is non-nullable; match the generated signature.

> [!WARNING]
> Do not enable local caching when bindings, styles, animations, `SetValue`, `ClearValue`, or other code can change the value outside the CLR setter. Those changes do not synchronize the cached field, so the CLR getter can return a stale value. Registering a property-changed callback does not automatically synchronize the cache.

Keep caching disabled for default-value factories, and do not combine it with `GeneratedDependencyProperty.UnsetValue`. If you customize a cached setter, prefer the typed `On<PropertyName>Set` hook so normalization happens before the field is updated. Changing only the boxed value later in the setter can make the cached and dependency-property values differ.

Local caching does not remove the XAML property system or its threading requirements, and changed values still need to be written to that system.

## Nullable reference types

Nullability annotations describe a property's contract; they do not automatically prevent the XAML property system from storing `null`. For a non-nullable reference property, provide a non-null default, use a non-null-returning factory, mark the property `required` when appropriate, or make the getter handle null values. Otherwise, declare the property as nullable.

The analyzers understand `[MaybeNull]`, `[NotNull]`, `[AllowNull]`, and `[DisallowNull]` from `System.Diagnostics.CodeAnalysis`. When a hook guarantees a non-null value, annotate its `ref` parameter with `[NotNull]` and implement the corresponding normalization:

```csharp
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class CaptionOwner : DependencyObject
{
    [GeneratedDependencyProperty(DefaultValue = "")]
    [AllowNull]
    public partial string Caption { get; set; }

    partial void OnCaptionGet([NotNull] ref string propertyValue)
    {
        propertyValue ??= string.Empty;
    }

    partial void OnCaptionSet([NotNull] ref string propertyValue)
    {
        propertyValue ??= string.Empty;
    }
}
```

The getter hook makes this CLR getter resilient even if a null value reaches dependency-property storage without passing through the CLR setter. It changes the returned value, not the value stored by the XAML property system. Keep local caching disabled when using getter hooks.

Similarly, adding `[NotNull]` to a nullable property requires an implementation that actually guarantees a non-null result. Attributes alone do not add runtime checks. The `required` modifier enforces C# initialization rules; it does not prevent later writes through XAML property APIs.

## Attributes on the generated identifier

Normal property attributes apply to the CLR property. To forward an attribute to the generated dependency-property identifier field, use an attribute list with the `static:` target:

```csharp
using System.ComponentModel;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class AttributedValueOwner : DependencyObject
{
    [GeneratedDependencyProperty]
    [static: EditorBrowsable(EditorBrowsableState.Never)]
    public partial string? Caption { get; set; }
}
```

Here, `EditorBrowsable` is applied to `CaptionProperty`, not `Caption`. Forwarded attributes must be valid for fields, and their types and arguments must resolve correctly. The package includes a diagnostic suppressor for the compiler warning caused by this generator-specific `static:` target.

## Accessibility, modifiers, and generic types

The generator preserves the CLR property's accessibility and supported modifiers, including `required`, `new`, `virtual`, `override`, and `sealed`, subject to normal C# rules. An accessor can have a more restrictive accessibility where C# permits it, such as `public partial int Count { get; private set; }`. The generated dependency-property identifier is still a public static readonly field.

Generic containing types and nested types are supported. Every containing type must be partial. For example, a generic owner can use a factory returning its type parameter:

```csharp
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;

public partial class ItemOwner<T> : DependencyObject
    where T : class, new()
{
    [GeneratedDependencyProperty(DefaultValueCallback = nameof(CreateItem))]
    public partial T Item { get; set; }

    private static T CreateItem() => new();
}
```

Support for a generic C# owner does not imply that the XAML markup language can instantiate that generic type. Normal framework and XAML restrictions still apply.

Only instance partial property definitions with a getter and a non-init setter are supported. The generator does not generate attached properties, static properties, get-only properties, `init` accessors, or properties returning pointers, by-reference values, or byref-like types. Name a CLR property `Count`, not `CountProperty`; the generator adds the `Property` suffix to its identifier.

## Advanced build configuration

### XAML namespace selection

The package selects UWP XAML automatically for UAP targets and projects with `UseUwp` enabled; otherwise it selects WinUI 3 XAML. If a custom build setup needs an explicit selection, set `DependencyPropertyGeneratorUseWindowsUIXaml`:

| Value | Generated XAML types |
| --- | --- |
| `true` | `Windows.UI.Xaml` |
| `false` | `Microsoft.UI.Xaml` |

For example:

```xml
<PropertyGroup>
  <DependencyPropertyGeneratorUseWindowsUIXaml>true</DependencyPropertyGeneratorUseWindowsUIXaml>
</PropertyGroup>
```

This setting controls generation and analysis; it does not add references to another XAML framework. It must match the framework used by the containing `DependencyObject`.

### Embedded mode

Library authors can use the generator without taking a runtime dependency on the package's attribute assembly. Exclude the runtime library from the package reference and enable generation of internal helper types in the consuming project.

Update your existing package reference as follows, replacing `PACKAGE_VERSION` with the version you have selected. Use the UWP package name instead for a UWP project:

```xml
<ItemGroup>
  <PackageReference Include="CommunityToolkit.Labs.WinUI.DependencyPropertyGenerator" Version="PACKAGE_VERSION" PrivateAssets="all" ExcludeAssets="lib" />
</ItemGroup>

<PropertyGroup>
  <EnableGeneratedDependencyPropertyAttributeEmbeddedMode>true</EnableGeneratedDependencyPropertyAttributeEmbeddedMode>
  <EnableGeneratedDependencyPropertyEmbeddedMode>true</EnableGeneratedDependencyPropertyEmbeddedMode>
</PropertyGroup>
```

`EnableGeneratedDependencyPropertyAttributeEmbeddedMode` emits the internal `GeneratedDependencyPropertyAttribute` type. `EnableGeneratedDependencyPropertyEmbeddedMode` emits the internal `GeneratedDependencyProperty` helper for the `UnsetValue` placeholder; it is only needed if you use that helper. Both options are disabled by default.

Do not combine embedded types with a reference to the package's runtime attribute assembly. Embedded mode removes that Toolkit assembly dependency, not the dependency on your application's XAML framework.

The embedded attribute's applications are omitted from compiled metadata by default. If you specifically need to preserve them, define `GENERATED_DEPENDENCY_PROPERTY_PRIVATE_ASSETS_ALL_PRESERVE_ATTRIBUTES`:

```xml
<PropertyGroup>
  <DefineConstants>$(DefineConstants);GENERATED_DEPENDENCY_PROPERTY_PRIVATE_ASSETS_ALL_PRESERVE_ATTRIBUTES</DefineConstants>
</PropertyGroup>
```

## Migrating existing dependency properties

The package includes analyzers and code fixes for existing handwritten dependency properties. When a supported `DependencyProperty.Register` declaration and its CLR wrapper can be replaced, diagnostic `WCTDPG0017` offers the **Use a partial property** code fix. The fixer can carry supported default values, metadata types, and field attributes into the generated declaration.

Use the code fix when it is offered rather than assuming every custom registration pattern is supported. Review any existing accessor logic and metadata callbacks, and move custom behavior into the appropriate hooks. In particular, preserve the distinction between CLR setter behavior and dependency-property change notifications.

Additional code fixes offer **Declare dependency property as field** and **Declare dependency property field correctly** for handwritten identifiers that do not follow the recommended field declaration pattern. The analyzers also check registration names, owner types, property types, and default values on manual declarations.

## Troubleshooting

| Diagnostic or symptom | What to check |
| --- | --- |
| `WCTDPCFG0002`, or partial properties have no generated implementation | Use Roslyn 4.12 or later, ensure the generator package is referenced by the declaring project, and check earlier build diagnostics. |
| `WCTDPG0006` | Select C# 13 or later. |
| `WCTDPG0007` | Select `preview` when using local caching. |
| `WCTDPG0001` through `WCTDPG0005`, or `WCTDPG0012` | Use an incomplete instance partial property with `get; set;`, a supported property type, and a partial owner deriving from the correct `DependencyObject`. |
| `WCTDPG0008` | Avoid a property named `Property` with type `object` or `DependencyPropertyChangedEventArgs`, which would cause generated hook name collisions. |
| `WCTDPG0009`, `WCTDPG0024`, or `WCTDPG0025` | Make the declared nullability contract consistent with defaults and accessor implementations. |
| `WCTDPG0010` or `WCTDPG0011` | Use a compatible default-value type, including the correct numeric literal type. |
| `WCTDPG0013`, `WCTDPG0014`, or `WCTDPG0015` | Specify only one default-value option and use a static, parameterless factory in the same type with a supported return type. |
| `WCTDPG0018` or `WCTDPG0019` | Check the types and argument expressions in forwarded `static:` attributes. |
| `WCTDPG0022` or `WCTDPG0023` | Remove a redundant `PropertyType` override or replace an incompatible metadata type. |
| `WCTDPCFG0001` | Remove the runtime attribute assembly reference when enabling embedded mode. |
| Accessor hooks do not run for a binding update | Use `On<PropertyName>PropertyChanged` or `OnPropertyChanged` for property-system notifications. |
| The CLR getter returns an old value | Disable local caching if any writes can bypass the CLR setter. |

Generated files are available under the generator's entry in your IDE's analyzer/generated-source view. Inspect the generated partial declaration when matching hook signatures, especially for nullable or generic properties.

## See also

- [UWP custom dependency properties](/windows/uwp/xaml-platform/custom-dependency-properties)
- [DependencyPropertyGenerator experiment](https://github.com/CommunityToolkit/Labs-Windows/issues/621)
- [DependencyPropertyGenerator design discussion](https://github.com/CommunityToolkit/Labs-Windows/discussions/449)
