// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="ISymbol"/> type.
/// </summary>
internal static class ISymbolExtensions
{
    /// <summary>
    /// Gets the fully qualified name for a given symbol.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The fully qualified name for <paramref name="symbol"/>.</returns>
    public static string GetFullyQualifiedName(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    /// <summary>
    /// Tries to get the app service name from a given symbol for the <c>AppServices</c> attribute.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="appServiceName">The app service name from the retrieved attribute, if found.</param>
    /// <returns>Whether the attribute was found.</returns>
    public static bool TryGetAppServicesNameFromAttribute(this ISymbol symbol, [NotNullWhen(true)] out string? appServiceName)
    {
        foreach (AttributeData attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is { Name: "AppServiceAttribute", ContainingNamespace: { Name: "AppServices", ContainingNamespace: { Name: "CommunityToolkit", ContainingNamespace.IsGlobalNamespace: true } } })
            {
                appServiceName = (string)attribute.ConstructorArguments[0].Value!;

                return true;
            }
        }

        appServiceName = null;

        return false;
    }

    /// <summary>
    /// Tries to get the serializer type from a given symbol for the <c>ValueSetSerializer</c> attribute.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="serializerType">The serializer type from the retrieved attribute, if found.</param>
    /// <returns>Whether the attribute was found.</returns>
    public static bool TryGetValueSetSerializerTypeFromAttribute(this ISymbol symbol, [NotNullWhen(true)] out INamedTypeSymbol? serializerType)
    {
        // Get either the attributes from the correct location based on symbol type
        ImmutableArray<AttributeData> attributes = symbol switch
        {
            IMethodSymbol methodSymbol => methodSymbol.GetReturnTypeAttributes(),
            IParameterSymbol parameterSymbol => parameterSymbol.GetAttributes(),
            _ => throw new ArgumentException("Invalid symbol type.")
        };

        foreach (AttributeData attribute in attributes)
        {
            if (attribute.AttributeClass is { Name: "ValueSetSerializerAttribute", ContainingNamespace: { Name: "AppServices", ContainingNamespace: { Name: "CommunityToolkit", ContainingNamespace.IsGlobalNamespace: true } } })
            {
                serializerType = (INamedTypeSymbol)attribute.ConstructorArguments[0].Value!;

                return true;
            }
        }

        serializerType = null;

        return false;
    }

    /// <summary>
    /// Checks whether a given member is an ignored member for [AppServices].
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <returns>Whether <paramref name="symbol"/> is an ignored [AppServices] member.</returns>
    /// <remarks>Interface member that are not abstract nor virtual can be ignored (eg. DIMs or static, non-abstract, non-virtual interface members).</remarks>
    public static bool IsIgnoredAppServicesMember(this ISymbol symbol)
    {
        return symbol is { IsAbstract: false, IsVirtual: false };
    }
}
