// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeAnalysis;
using CommunityToolkit.AppServices.SourceGenerators.Helpers;
using CommunityToolkit.AppServices.SourceGenerators.Models;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="ITypeSymbol"/> type.
/// </summary>
internal static class ITypeSymbolExtensions
{
    /// <summary>
    /// Gets a valid filename for a given <see cref="ISymbol"/> instance.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="ISymbol"/> instance.</param>
    /// <returns>The full metadata name for <paramref name="symbol"/> that is also a valid filename.</returns>
    public static string GetFullMetadataNameForFileName(this ITypeSymbol typeSymbol)
    {
        using ImmutableArrayBuilder<char> builder = ImmutableArrayBuilder<char>.Rent();

        static void BuildFrom(ISymbol? symbol, in ImmutableArrayBuilder<char> builder)
        {
            switch (symbol)
            {
                // Namespaces that are nested also append a leading '.'
                case INamespaceSymbol { ContainingNamespace.IsGlobalNamespace: false }:
                    BuildFrom(symbol.ContainingNamespace, in builder);
                    builder.Add('.');
                    builder.AddRange(symbol.MetadataName.AsSpan());
                    break;

                // Other namespaces (ie. the one right before global) skip the leading '.'
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

        BuildFrom(typeSymbol, in builder);

        return builder.ToString();
    }

    /// <summary>
    /// Tries to parse the <see cref="ParameterOrReturnType"/> value from an input type symbol.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <param name="parameterOrReturnType">The <see cref="ParameterOrReturnType"/> for <paramref name="typeSymbol"/>, if valid.</param>
    /// <returns>Whether <paramref name="parameterOrReturnType"/> could successfully be retrieved from <paramref name="typeSymbol"/>.</returns>
    public static bool TryGetParameterOrReturnType(this ITypeSymbol typeSymbol, out ParameterOrReturnType parameterOrReturnType)
    {
        return TryGetParameterOrReturnType(typeSymbol, associatedTypeSymbol: null, out parameterOrReturnType);
    }

    /// <summary>
    /// Tries to parse the <see cref="ParameterOrReturnType"/> value from an input type symbol.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="ITypeSymbol"/> instance.</param>
    /// <param name="associatedTypeSymbol">The associated type symbol (ie. the containing type, if the parent is generic).</param>
    /// <param name="parameterOrReturnType">The <see cref="ParameterOrReturnType"/> for <paramref name="typeSymbol"/>, if valid.</param>
    /// <returns>Whether <paramref name="parameterOrReturnType"/> could successfully be retrieved from <paramref name="typeSymbol"/>.</returns>
    private static bool TryGetParameterOrReturnType(this ITypeSymbol typeSymbol, ITypeSymbol? associatedTypeSymbol, out ParameterOrReturnType parameterOrReturnType)
    {
        // There are only two allowed kinds of type symbols:
        //   - IArrayTypeSymbol, for SZ arrays
        //   - INamedTypeSymbol, for all other possible types.
        if (typeSymbol is not (INamedTypeSymbol or IArrayTypeSymbol))
        {
            goto Failure;
        }

        // First, consider the case where the type symbol is an array type
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            // In order to be valid, an array type must match these conditions:
            //   - It must be either a top level parameter, or its associated type symbol must be Task<T>
            //   - It has to be an SZ array
            //   - The element type must be one of the simple types supported (eg. not an array, Task or other complex types)
            if ((associatedTypeSymbol is null || IsTaskOfT(associatedTypeSymbol)) &&
                arrayTypeSymbol is { IsSZArray: true, ElementType: INamedTypeSymbol elementTypeSymbol } &&
                TryGetParameterOrReturnType(elementTypeSymbol, associatedTypeSymbol: typeSymbol, out ParameterOrReturnType elementType))
            {
                parameterOrReturnType = elementType | ParameterOrReturnType.Array;

                return true;
            }

            goto Failure;
        }

        // Try to match against all allowed types that have a corresponding special type
        ParameterOrReturnType? parameterOrReturnTypeFromSpecialType = typeSymbol.SpecialType switch
        {
            SpecialType.System_Byte => ParameterOrReturnType.UInt8,
            SpecialType.System_Int16 => ParameterOrReturnType.Int16,
            SpecialType.System_UInt16 => ParameterOrReturnType.UInt16,
            SpecialType.System_Int32 => ParameterOrReturnType.Int32,
            SpecialType.System_UInt32 => ParameterOrReturnType.UInt32,
            SpecialType.System_Int64 => ParameterOrReturnType.Int64,
            SpecialType.System_UInt64 => ParameterOrReturnType.UInt64,
            SpecialType.System_Single => ParameterOrReturnType.Single,
            SpecialType.System_Double => ParameterOrReturnType.Double,
            SpecialType.System_Char => ParameterOrReturnType.Char16,
            SpecialType.System_Boolean => ParameterOrReturnType.Boolean,
            SpecialType.System_String => ParameterOrReturnType.String,
            SpecialType.System_DateTime => ParameterOrReturnType.DateTime,
            _ => null
        };

        // If a type has been found, we can stop here
        if (parameterOrReturnTypeFromSpecialType is not null)
        {
            parameterOrReturnType = parameterOrReturnTypeFromSpecialType.Value;

            return true;
        }

        // If the type is an enum, that is also explicitly allowed
        if (typeSymbol.TypeKind == TypeKind.Enum)
        {
            parameterOrReturnType = ParameterOrReturnType.Enum;

            return true;
        }

        // The following types have to be checked explicitly, as they don't have a special type:
        //   - System.TimeSpan
        //   - System.Guid
        //   - Windows.Foundation.Size
        //   - Windows.Foundation.Point
        //   - Windows.Foundation.Rect            
        if (typeSymbol is { Name: "TimeSpan", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } })
        {
            parameterOrReturnType = ParameterOrReturnType.TimeSpan;

            return true;
        }

        // System.Guid
        if (typeSymbol is { Name: "Guid", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } })
        {
            parameterOrReturnType = ParameterOrReturnType.Guid;

            return true;
        }

        // Windows.Foundation.Size
        if (typeSymbol is { Name: "Size", ContainingNamespace: { Name: "Foundation", ContainingNamespace: { Name: "Windows", ContainingNamespace.IsGlobalNamespace: true } } })
        {
            parameterOrReturnType = ParameterOrReturnType.Size;

            return true;
        }

        // Windows.Foundation.Point
        if (typeSymbol is { Name: "Point", ContainingNamespace: { Name: "Foundation", ContainingNamespace: { Name: "Windows", ContainingNamespace.IsGlobalNamespace: true } } })
        {
            parameterOrReturnType = ParameterOrReturnType.Point;

            return true;
        }

        // Windows.Foundation.Rect
        if (typeSymbol is { Name: "Rect", ContainingNamespace: { Name: "Foundation", ContainingNamespace: { Name: "Windows", ContainingNamespace.IsGlobalNamespace: true } } })
        {
            parameterOrReturnType = ParameterOrReturnType.Rect;

            return true;
        }

        // This is the end of the possible types that are allowed if an associated type is present.
        // That is, if there is an associated type available, it means matching has failed.
        // If that's not the case, the following types must be checked manually after that:
        //   - T[] arrays
        //   - System.Threading.Tasks.Task
        //   - System.Threading.Tasks.Task<T>
        //   - System.IProgress<T>
        //   - System.Threading.CancellationToken
        if (associatedTypeSymbol is not null)
        {
            goto Failure;
        }

        // System.Threading.Tasks.Task<T> (this has to be tested first for correctness, as System.Threading.Tasks.Task would also match)
        if (IsTaskOfT(typeSymbol))
        {
            // Only validate a Task<T> type if the type argument is actually allowed
            if (((INamedTypeSymbol)typeSymbol).TypeArguments[0] is INamedTypeSymbol taskResultTypeSymbol &&
                TryGetParameterOrReturnType(taskResultTypeSymbol, associatedTypeSymbol: typeSymbol, out ParameterOrReturnType resultType))
            {
                parameterOrReturnType = resultType | ParameterOrReturnType.TaskOfT;

                return true;
            }

            goto Failure;
        }

        // System.Threading.Tasks.Task (this branch is also taken for Task<T> as the type name without type parameters is the same)
        if (typeSymbol is { Name: "Task", ContainingNamespace: { Name: "Tasks", ContainingNamespace: { Name: "Threading", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } } })
        {
            parameterOrReturnType = ParameterOrReturnType.Task;

            return true;
        }

        // System.IProgress<T>
        if (typeSymbol is INamedTypeSymbol
            {
                Name: "IProgress",
                ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true },
                IsGenericType: true,
                TypeArguments.Length: 1
            })
        {
            // If the type argument is valid, then confirm the IProgress<T> parameter
            if (((INamedTypeSymbol)typeSymbol).TypeArguments[0] is INamedTypeSymbol progressTypeSymbol &&
                TryGetParameterOrReturnType(progressTypeSymbol, associatedTypeSymbol: typeSymbol, out ParameterOrReturnType progressType))
            {
                parameterOrReturnType = progressType | ParameterOrReturnType.IProgressOfT;

                return true;
            }

            goto Failure;
        }

        // System.Threading.CancellationToken
        if (typeSymbol is { Name: "CancellationToken", ContainingNamespace: { Name: "Threading", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } })
        {
            parameterOrReturnType = ParameterOrReturnType.CancellationToken;

            return true;
        }

        Failure:
        parameterOrReturnType = default;

        return false;
    }

    /// <summary>
    /// Checks whether the input <see cref="ITypeSymbol"/> represents the <see cref="System.Threading.Tasks.Task{TResult}"/> type.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <returns>Whether <paramref name="typeSymbol"/> represents the <see cref="System.Threading.Tasks.Task{TResult}"/> type.</returns>
    private static bool IsTaskOfT(ITypeSymbol typeSymbol)
    {
        return typeSymbol is INamedTypeSymbol
        {
            Name: "Task",
            ContainingNamespace: { Name: "Tasks", ContainingNamespace: { Name: "Threading", ContainingNamespace: { Name: "System", ContainingNamespace.IsGlobalNamespace: true } } },
            IsGenericType: true,
            TypeArguments.Length: 1
        };
    }
}