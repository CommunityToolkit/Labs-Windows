// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="Compilation"/> type.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// Tries to build a map of <see cref="INamedTypeSymbol"/> instances form the input mapping of names.
    /// </summary>
    /// <typeparam name="T">The type of keys for each symbol.</typeparam>
    /// <param name="compilation">The <see cref="Compilation"/> to consider for analysis.</param>
    /// <param name="typeNames">The input mapping of <typeparamref name="T"/> keys to fully qualified type names.</param>
    /// <param name="typeSymbols">The resulting mapping of <typeparamref name="T"/> keys to resolved <see cref="INamedTypeSymbol"/> instances.</param>
    /// <returns>Whether all requested <see cref="INamedTypeSymbol"/> instances could be resolved.</returns>
    public static bool TryBuildNamedTypeSymbolMap<T>(
        this Compilation compilation,
        IEnumerable<KeyValuePair<T, string>> typeNames,
        [NotNullWhen(true)] out ImmutableDictionary<T, INamedTypeSymbol>? typeSymbols)
        where T : IEquatable<T>
    {
        ImmutableDictionary<T, INamedTypeSymbol>.Builder builder = ImmutableDictionary.CreateBuilder<T, INamedTypeSymbol>();

        foreach (KeyValuePair<T, string> pair in typeNames)
        {
            if (compilation.GetTypeByMetadataName(pair.Value) is not INamedTypeSymbol attributeSymbol)
            {
                typeSymbols = null;

                return false;
            }

            builder.Add(pair.Key, attributeSymbol);
        }

        typeSymbols = builder.ToImmutable();

        return true;
    }
}
