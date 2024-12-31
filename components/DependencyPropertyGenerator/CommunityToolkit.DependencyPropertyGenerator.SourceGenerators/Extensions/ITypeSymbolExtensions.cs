// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for <see cref="ITypeSymbol"/> types.
/// </summary>
internal static class ITypeSymbolExtensions
{
    /// <summary>
    /// Checks whether a given type has a default value of <see langword="null"/>.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <returns>Whether the default value of <paramref name="symbol"/> is <see langword="null"/>.</returns>
    public static bool IsDefaultValueNull(this ITypeSymbol symbol)
    {
        return symbol is { IsValueType: false } or INamedTypeSymbol { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };
    }

    /// <summary>
    /// Checks whether a given type symbol represents some <see cref="Nullable{T}"/> type.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <returns>Whether <paramref name="symbol"/> represents some <see cref="Nullable{T}"/> type.</returns>
    public static bool IsNullableValueType(this ITypeSymbol symbol)
    {
        return symbol is INamedTypeSymbol { IsValueType: true, IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };
    }

    /// <summary>
    /// Checks whether a given type symbol represents a <see cref="Nullable{T}"/> type with a specific underlying type.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="underlyingType">The underlyign type to check.</param>
    /// <returns>Whether <paramref name="symbol"/> represents a <see cref="Nullable{T}"/> type with a specific underlying type.</returns>
    public static bool IsNullableValueTypeWithUnderlyingType(this ITypeSymbol symbol, ITypeSymbol underlyingType)
    {
        if (!IsNullableValueType(symbol))
        {
            return false;
        }

        return SymbolEqualityComparer.Default.Equals(((INamedTypeSymbol)symbol).TypeArguments[0], underlyingType);  
    }

    /// <summary>
    /// Tries to get the default value of a given enum type.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="value">The resulting default value for <paramref name="symbol"/>, if it was an enum type.</param>
    /// <returns>Whether <paramref name="value"/> was retrieved successfully.</returns>
    public static bool TryGetDefaultValueForEnumType(this ITypeSymbol symbol, [NotNullWhen(true)] out object? value)
    {
        if (symbol.TypeKind is not TypeKind.Enum)
        {
            value = null;

            return false;
        }

        // The default value of the enum is the value of its first constant field
        foreach (ISymbol memberSymbol in symbol.GetMembers())
        {
            if (memberSymbol is IFieldSymbol { IsConst: true, ConstantValue: object defaultValue })
            {
                value = defaultValue;

                return true;
            }
        }

        value = null;

        return false;
    }

    /// <summary>
    /// Tries to get the name of the enum field matching a given value.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="value">The value for to try to get the field for.</param>
    /// <param name="fieldName">The name of the field with the specified value, if found.</param>
    /// <returns>Whether <paramref name="fieldName"/> was successfully retrieved.</returns>
    public static bool TryGetEnumFieldName(this ITypeSymbol symbol, object value, [NotNullWhen(true)] out string? fieldName)
    {
        if (symbol.TypeKind is not TypeKind.Enum)
        {
            fieldName = null;

            return false;
        }

        // The default value of the enum is the value of its first constant field
        foreach (ISymbol memberSymbol in symbol.GetMembers())
        {
            if (memberSymbol is not IFieldSymbol { IsConst: true, ConstantValue: object fieldValue } fieldSymbol)
            {
                continue;
            }

            if (fieldValue.Equals(value))
            {
                fieldName = fieldSymbol.Name;

                return true;
            }
        }
        fieldName = null;

        return false;
    }

    /// <summary>
    /// Checks whether or not a given type symbol has a specified fully qualified metadata name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedMetadataName(this ITypeSymbol symbol, string name)
    {
        using ImmutableArrayBuilder<char> builder = new();

        symbol.AppendFullyQualifiedMetadataName(in builder);

        return builder.WrittenSpan.SequenceEqual(name.AsSpan());
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="baseTypeSymbol">The <see cref="ITypeSymbol"/> instance to check for inheritance from.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="baseTypeSymbol"/>.</returns>
    public static bool InheritsFromType(this ITypeSymbol typeSymbol, ITypeSymbol baseTypeSymbol)
    {
        INamedTypeSymbol? currentBaseTypeSymbol = typeSymbol.BaseType;

        while (currentBaseTypeSymbol is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(currentBaseTypeSymbol, baseTypeSymbol))
            {
                return true;
            }

            currentBaseTypeSymbol = currentBaseTypeSymbol.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Checks whether or not a given <see cref="ITypeSymbol"/> inherits from a specified type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="name">The full name of the type to check for inheritance.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> inherits from <paramref name="name"/>.</returns>
    public static bool InheritsFromFullyQualifiedMetadataName(this ITypeSymbol typeSymbol, string name)
    {
        INamedTypeSymbol? baseType = typeSymbol.BaseType;

        while (baseType is not null)
        {
            if (baseType.HasFullyQualifiedMetadataName(name))
            {
                return true;
            }

            baseType = baseType.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Gets the fully qualified metadata name for a given <see cref="ITypeSymbol"/> instance.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <returns>The fully qualified metadata name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedMetadataName(this ITypeSymbol symbol)
    {
        using ImmutableArrayBuilder<char> builder = new();

        symbol.AppendFullyQualifiedMetadataName(in builder);

        return builder.ToString();
    }

    /// <summary>
    /// Appends the fully qualified metadata name for a given symbol to a target builder.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <param name="builder">The target <see cref="ImmutableArrayBuilder{T}"/> instance.</param>
    public static void AppendFullyQualifiedMetadataName(this ITypeSymbol symbol, ref readonly ImmutableArrayBuilder<char> builder)
    {
        static void BuildFrom(ISymbol? symbol, ref readonly ImmutableArrayBuilder<char> builder)
        {
            switch (symbol)
            {
                // Namespaces that are nested also append a leading '.'
                case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                    BuildFrom(symbol.ContainingNamespace, in builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Other namespaces (i.e. the one right before global) skip the leading '.'
                case INamespaceSymbol { IsGlobalNamespace: false }:
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Types with no namespace just have their metadata name directly written
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol { IsGlobalNamespace: true } }:
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Types with a containing non-global namespace also append a leading '.'
                case ITypeSymbol { ContainingSymbol: INamespaceSymbol namespaceSymbol }:
                    BuildFrom(namespaceSymbol, in builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Nested types append a leading '+'
                case ITypeSymbol { ContainingSymbol: ITypeSymbol typeSymbol }:
                    BuildFrom(typeSymbol, in builder);
                    builder.Add('+');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;
                default:
                    break;
            }
        }

        BuildFrom(symbol, in builder);
    }

    /// <summary>
    /// Checks whether a given type is contained in a namespace with a specified name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <param name="namespaceName">The namespace to check.</param>
    /// <returns>Whether <paramref name="symbol"/> is contained within <paramref name="namespaceName"/>.</returns>
    public static bool IsContainedInNamespace(this ITypeSymbol symbol, string? namespaceName)
    {
        static void BuildFrom(INamespaceSymbol? symbol, ref readonly ImmutableArrayBuilder<char> builder)
        {
            switch (symbol)
            {
                // Namespaces that are nested also append a leading '.'
                case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                    BuildFrom(symbol.ContainingNamespace, in builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Other namespaces (i.e. the one right before global) skip the leading '.'
                case INamespaceSymbol { IsGlobalNamespace: false }:
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;
                default:
                    break;
            }
        }

        // Special case for no containing namespace
        if (symbol.ContainingNamespace is not { } containingNamespace)
        {
            return namespaceName is null;
        }

        // Special case if the type is directly in the global namespace
        if (containingNamespace.IsGlobalNamespace)
        {
            return containingNamespace.MetadataName == namespaceName;
        }

        using ImmutableArrayBuilder<char> builder = new();

        BuildFrom(containingNamespace, in builder);

        return builder.WrittenSpan.SequenceEqual(namespaceName.AsSpan());
    }
}
