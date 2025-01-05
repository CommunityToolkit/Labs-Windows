// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
#if WINDOWS_UWP || HAS_UNO
using DependencyObject = Windows.UI.Xaml.DependencyObject;
using DependencyProperty = Windows.UI.Xaml.DependencyProperty;
using PropertyMetadata = Windows.UI.Xaml.PropertyMetadata;
#else
using DependencyObject = Microsoft.UI.Xaml.DependencyObject;
using DependencyProperty = Microsoft.UI.Xaml.DependencyProperty;
using PropertyMetadata = Microsoft.UI.Xaml.PropertyMetadata;
#endif

namespace CommunityToolkit.WinUI;

/// <summary>
/// An attribute that indicates that a given partial property should generate a backing <see cref="DependencyProperty"/>.
/// In order to use this attribute, the containing type has to inherit from <see cref="DependencyObject"/>.
/// <para>
/// This attribute can be used as follows:
/// <code>
/// partial class MyClass : DependencyObject
/// {
///     [GeneratedDependencyProperty]
///     public partial string? Name { get; set; }
/// }
/// </code>
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// In order to use this attribute on partial properties, the .NET 9 SDK is required, and C# 13 (or 'preview') must be used.
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class GeneratedDependencyPropertyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets a value indicating the default value to set for the property.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If not set, the default value will be <see langword="null"/>, for all property types. If there is no callback
    /// registered for the generated property, <see cref="PropertyMetadata"/> will not be set at all.
    /// </para>
    /// <para>
    /// To set the default value to <see cref="DependencyProperty.UnsetValue"/>, use <see cref="GeneratedDependencyProperty.UnsetValue"/>.
    /// </para>
    /// <para>
    /// Using this property is mutually exclusive with <see cref="DefaultValueCallback"/>.
    /// </para>
    /// </remarks>
    public object? DefaultValue { get; init; } = null;

    /// <summary>
    /// Gets or sets the name of the method that will be invoked to produce the default value of the
    /// property, for each instance of the containing type. The referenced method needs to return either
    /// an <see cref="object"/>, or a value of exactly the property type, and it needs to be parameterless.
    /// </summary>
    /// <remarks>
    /// Using this property is mutually exclusive with <see cref="DefaultValue"/>.
    /// </remarks>
#if NET8_0_OR_GREATER
    [DisallowNull]
#endif
    public string? DefaultValueCallback { get; init; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether or not property values should be cached locally, to improve performance.
    /// This allows completely skipping boxing (for value types) and all WinRT marshalling when setting properties.
    /// </summary>
    /// <remarks>
    /// Local caching is disabled by default. It should be disabled in scenarios where the values of the dependency
    /// properties might also be set outside of the partial property implementation, meaning caching would be invalid.
    /// </remarks>
    public bool IsLocalCacheEnabled { get; init; } = false;

    /// <summary>
    /// Gets or sets the type to use to register the property in metadata. The default value will exactly match the property type.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property allows customizing the property type in metadata, in advanced scenarios. For instance, it can be used to define
    /// properties of a <see cref="Nullable{T}"/> type (e.g. <see langword="bool?"/>) as just using <see cref="object"/> in metadata.
    /// This allows working around some issues primarily around classic (reflection-based) binding in XAML.
    /// </para>
    /// <para>
    /// This property should only be set when actually required (e.g. to ensure a specific scenario can work). The default behavior
    /// (i.e. the property type in metadata matching the declared property type) should work correctly in the vast majority of cases.
    /// </para>
    /// </remarks>
#if NET8_0_OR_GREATER
    [DisallowNull]
#endif
    public Type? PropertyType { get; init; } = null!;
}
