// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for <see cref="IMethodSymbol"/> types.
/// </summary>
internal static class IMethodSymbolExtensions
{
    /// <summary>
    /// Checks whether or not a given symbol has a return attribute with the specified type.
    /// </summary>
    /// <param name="symbol">The input <see cref="IMethodSymbol"/> instance to check.</param>
    /// <param name="typeSymbols">The <see cref="ITypeSymbol"/> instance for the attribute type to look for.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has a return attribute with the specified type.</returns>
    public static bool HasReturnAttributeWithAnyType(this IMethodSymbol symbol, ImmutableArray<INamedTypeSymbol> typeSymbols)
    {
        return TryGetReturnAttributeWithAnyType(symbol, typeSymbols, out _);
    }

    /// <summary>
    /// Tries to get a return attribute with any of the specified types.
    /// </summary>
    /// <param name="symbol">The input <see cref="IMethodSymbol"/> instance to check.</param>
    /// <param name="typeSymbols">The <see cref="ITypeSymbol"/> instance for the attribute type to look for.</param>
    /// <param name="attributeData">The first return attribute of a type matching any type in <paramref name="typeSymbols"/>, if found.</param>
    /// <returns>Whether or not <paramref name="symbol"/> has a return attribute with the specified type.</returns>
    public static bool TryGetReturnAttributeWithAnyType(this IMethodSymbol symbol, ImmutableArray<INamedTypeSymbol> typeSymbols, [NotNullWhen(true)] out AttributeData? attributeData)
    {
        foreach (AttributeData attribute in symbol.GetReturnTypeAttributes())
        {
            if (typeSymbols.Contains(attribute.AttributeClass!, SymbolEqualityComparer.Default))
            {
                attributeData = attribute;

                return true;
            }
        }

        attributeData = null;

        return false;
    }
}
