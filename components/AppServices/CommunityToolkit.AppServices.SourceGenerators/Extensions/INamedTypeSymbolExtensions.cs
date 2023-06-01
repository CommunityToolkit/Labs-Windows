// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using CommunityToolkit.AppServices.SourceGenerators.Models;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="INamedTypeSymbol"/> type.
/// </summary>
internal static class INamedTypeSymbolExtensions
{
    /// <summary>
    /// Gets all member symbols from a given <see cref="INamedTypeSymbol"/> instance, including inherited ones.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <returns>A sequence of all member symbols for <paramref name="typeSymbol"/>.</returns>
    public static IEnumerable<ISymbol> GetAllMembers(this INamedTypeSymbol typeSymbol)
    {
        for (INamedTypeSymbol? currentSymbol = typeSymbol; currentSymbol is { SpecialType: not SpecialType.System_Object }; currentSymbol = currentSymbol.BaseType)
        {
            foreach (ISymbol memberSymbol in currentSymbol.GetMembers())
            {
                yield return memberSymbol;
            }
        }
    }

    /// <summary>
    /// Checks whether or not a given symbol has an attribute of a specified type.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <param name="attributeSymbol">The attribute type to look for.</param>
    /// <returns>Whether or not <paramref name="typeSymbol"/> has an attribute of type <paramref name="attributeSymbol"/>.</returns>
    public static bool HasOrInheritsAttribute(this INamedTypeSymbol typeSymbol, INamedTypeSymbol attributeSymbol)
    {
        for (INamedTypeSymbol? currentSymbol = typeSymbol; currentSymbol is { SpecialType: not SpecialType.System_Object }; currentSymbol = currentSymbol.BaseType)
        {
            foreach (AttributeData attributeData in typeSymbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, attributeSymbol))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether an input <see cref="INamedTypeSymbol"/> object represents a valid serializer type for a return type.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <param name="methodSymbol">The <see cref="IMethodSymbol"/> whose return type should be validated.</param>
    /// <returns>Whether <paramref name="typeSymbol"/> is a valid serializer type for <paramref name="methodSymbol"/>'s return type.</returns>
    public static bool IsValidValueSetSerializerTypeForReturnType(this INamedTypeSymbol typeSymbol, IMethodSymbol methodSymbol)
    {
        // Only if the return is a Task<T>, the input serializer could potentially be valid
        if (methodSymbol.ReturnType is INamedTypeSymbol
            {
                Name: "Task",
                ContainingNamespace: { Name: "Tasks", ContainingNamespace: { Name: "Threading", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } },
                IsGenericType: true,
                TypeArguments.Length: 1
            } genericType &&
            genericType.TypeArguments[0] is INamedTypeSymbol typeArgumentSymbol)
        {
            return typeSymbol.IsValidValueSetSerializerTypeForType(typeArgumentSymbol);
        }

        return false;
    }

    /// <summary>
    /// Checks whether an input <see cref="INamedTypeSymbol"/> object represents a valid serializer type for a parameter type.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <param name="parameterSymbol">The <see cref="IMethodSymbol"/> whose return type should be validated.</param>
    /// <param name="parameterOrReturnType">The <see cref="ParameterOrReturnType"/> for <paramref name="parameterSymbol"/>, if valid.</param>
    /// <returns>Whether <paramref name="typeSymbol"/> is a valid serializer type for <paramref name="parameterSymbol"/>'s type.</returns>
    public static bool IsValidValueSetSerializerTypeForParameterType(
        this INamedTypeSymbol typeSymbol,
        IParameterSymbol parameterSymbol,
        out ParameterOrReturnType parameterOrReturnType)
    {
        // Any INamedTypeSymbol matching the serializer is allowed
        if (parameterSymbol.Type is INamedTypeSymbol parameterTypeSymbol)
        {
            // Special case for IProgress<T> values, where the custom serializer applies to the inner T values
            if (parameterTypeSymbol is
                {
                    Name: "IProgress",
                    ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true },
                    IsGenericType: true,
                    TypeArguments.Length: 1
                })
            {
                // For this to be allowed, the type argument has to be an INamedTypeSymbol
                if (parameterTypeSymbol.TypeArguments[0] is INamedTypeSymbol progressTypeSymbol &&
                    IsValidValueSetSerializerTypeForType(typeSymbol, progressTypeSymbol))
                {
                    parameterOrReturnType = ParameterOrReturnType.IProgressOfT | ParameterOrReturnType.CustomSerializerType;

                    return true;
                }

                goto Failure;
            }

            // Handle all other cases normally (ie. the serializer applies directly to the parameter type)
            if (IsValidValueSetSerializerTypeForType(typeSymbol, parameterTypeSymbol))
            {
                parameterOrReturnType = ParameterOrReturnType.CustomSerializerType;

                return true;
            }
        }

        Failure:
        parameterOrReturnType = default;

        return false;
    }

    /// <summary>
    /// Checks whether an input <see cref="INamedTypeSymbol"/> object represents a valid serializer type for a target type.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <param name="targetTypeSymbol">The <see cref="INamedTypeSymbol"/> to validate.</param>
    /// <returns>Whether <paramref name="typeSymbol"/> is a valid serializer type for <paramref name="targetTypeSymbol"/>.</returns>
    public static bool IsValidValueSetSerializerTypeForType(this INamedTypeSymbol typeSymbol, INamedTypeSymbol targetTypeSymbol)
    {
        foreach (INamedTypeSymbol interfaceSymbol in typeSymbol.AllInterfaces)
        {
            if (interfaceSymbol.TryGetValueSetSerializerType(out INamedTypeSymbol? resultingSymbol))
            {
                if (SymbolEqualityComparer.Default.Equals(targetTypeSymbol, resultingSymbol))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Tries to get the target type symbol from an interface, if it's <c>IValueSetSerializer&lt;T<&gt;</c>.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance to check.</param>
    /// <param name="resultingSymbol">The target type symbol, if available.</param>
    /// <returns>Whether <paramref name="typeSymbol"/> was <c>IValueSetSerializer&lt;T<&gt;</c> and <paramref name="resultingSymbol"/> could be retrieved.</returns>
    public static bool TryGetValueSetSerializerType(this INamedTypeSymbol typeSymbol, [NotNullWhen(true)] out INamedTypeSymbol? resultingSymbol)
    {
        if (typeSymbol is { Name: "IValueSetSerializer", ContainingNamespace: { Name: "AppServices", ContainingNamespace: { Name: "CommunityToolkit", ContainingNamespace.IsGlobalNamespace: true } }, IsGenericType: true, TypeArguments.Length: 1 } &&
            typeSymbol.TypeArguments[0] is INamedTypeSymbol typeArgumentSymbol)
        {
            resultingSymbol = typeArgumentSymbol;

            return true;
        }

        resultingSymbol = null;

        return false;
    }
}
