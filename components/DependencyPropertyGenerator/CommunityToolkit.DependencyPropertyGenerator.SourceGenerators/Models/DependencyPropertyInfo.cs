// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Models;

/// <summary>
/// A model representing a generated dependency property.
/// </summary>
/// <param name="Hierarchy">The hierarchy info for the containing type.</param>
/// <param name="PropertyName">The property name.</param>
/// <param name="PropertyModifiers">The list of additional modifiers for the property (they are <see cref="SyntaxKind"/> values).</param>
/// <param name="DeclaredAccessibility">The accessibility of the property, if available.</param>
/// <param name="GetterAccessibility">The accessibility of the <see langword="get"/> accessor, if available.</param>
/// <param name="SetterAccessibility">The accessibility of the <see langword="set"/> accessor, if available.</param>
/// <param name="TypeName">The type name for the generated property (without nullability annotations).</param>
/// <param name="TypeNameWithNullabilityAnnotations">The type name for the generated property, including nullability annotations.</param>
/// <param name="MetadataTypeName">The type name for the metadata declaration of the property, if explicitly set (otherwise, <paramref name="TypeName"/> will be used).</param>
/// <param name="DefaultValue">The default value to set the generated property to.</param>
/// <param name="IsReferenceTypeOrUnconstraindTypeParameter">Indicates whether the property is of a reference type or an unconstrained type parameter.</param>
/// <param name="IsLocalCachingEnabled">Indicates whether local caching should be used for the property value.</param>
/// <param name="IsPropertyChangedCallbackImplemented">Indicates whether the WinRT-based property changed callback is implemented.</param>
/// <param name="IsSharedPropertyChangedCallbackImplemented">Indicates whether the WinRT-based shared property changed callback is implemented.</param>
/// <param name="IsAdditionalTypesGenerationSupported">Indicates whether additional types can be generated.</param>
/// <param name="UseWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
/// <param name="StaticFieldAttributes">The attributes to emit on the generated static field, if any.</param>
internal sealed record DependencyPropertyInfo(
    HierarchyInfo Hierarchy,
    string PropertyName,
    EquatableArray<ushort> PropertyModifiers,
    Accessibility DeclaredAccessibility,
    Accessibility GetterAccessibility,
    Accessibility SetterAccessibility,
    string TypeName,
    string TypeNameWithNullabilityAnnotations,
    string? MetadataTypeName,
    DependencyPropertyDefaultValue DefaultValue,
    bool IsReferenceTypeOrUnconstraindTypeParameter,
    bool IsLocalCachingEnabled,
    bool IsPropertyChangedCallbackImplemented,
    bool IsSharedPropertyChangedCallbackImplemented,
    bool IsAdditionalTypesGenerationSupported,
    bool UseWindowsUIXaml,
    EquatableArray<AttributeInfo> StaticFieldAttributes);
