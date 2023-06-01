// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using CommunityToolkit.AppServices.SourceGenerators.Helpers;

namespace CommunityToolkit.AppServices.SourceGenerators.Models;

/// <summary>
/// A model with gathered info on a given method.
/// </summary>
/// <param name="MethodName">The name of the target method.</param>
/// <param name="Parameters">The method parameters, if any.</param>
/// <param name="ReturnType">The return type of the method.</param>
/// <param name="FullyQualifiedReturnTypeName">The fully qualified type name of the return type being serialized, if any.</param>
/// <param name="FullyQualifiedValueSetSerializerTypeName">The fully qualified type name of the custom ValueSet serializer to use, if any.</param>
internal sealed record MethodInfo(
    string MethodName,
    EquatableArray<ParameterInfo> Parameters,
    ParameterOrReturnType ReturnType,
    string? FullyQualifiedReturnTypeName,
    string? FullyQualifiedValueSetSerializerTypeName)
{
    /// <summary>
    /// Creates <see cref="MethodInfo"/> instances from methods in a given <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">The input <see cref="INamedTypeSymbol"/> instance to gather info for.</param>
    /// <param name="token">The cancellation token for the operation.</param>
    /// <returns>A collection of <see cref="MethodInfo"/> instances from <paramref name="typeSymbol"/>.</returns>
    public static ImmutableArray<MethodInfo> From(INamedTypeSymbol typeSymbol, CancellationToken token)
    {
        using ImmutableArrayBuilder<MethodInfo> builder = ImmutableArrayBuilder<MethodInfo>.Rent();

        foreach (ISymbol symbol in typeSymbol.GetMembers())
        {
            token.ThrowIfCancellationRequested();

            if (symbol.IsIgnoredAppServicesMember())
            {
                continue;
            }

            // If the current method is not valid, also just skip it. This will avoid crashes in the generator
            // and it will keep the rest of the code simpler. The analyzer will just emit the correct diagnostics
            // separately, so the user will easily understand why these methods have not been generated.
            if (symbol is not IMethodSymbol methodSymbol ||
                !methodSymbol.IsValidAppServicesMethod())
            {
                continue;
            }

            // Get the return type (this will always succeed, as the call above will validate the whole signature)
            _ = methodSymbol.TryGetParameterOrReturnType(out ParameterOrReturnType returnType);

            // Get the return type name, if needed (for custom serializer types, and for enum types)
            string? fullyQualifiedReturnTypeName = returnType.HasAnyFlags(ParameterOrReturnType.CustomSerializerType | ParameterOrReturnType.Enum) switch
            {
                true => ((INamedTypeSymbol)methodSymbol.ReturnType).TypeArguments[0].GetFullyQualifiedName(),
                false => null
            };

            // Try to get the serializer name, in case there is one (validation has already been performed)
            _ = methodSymbol.TryGetValueSetSerializerTypeFromAttribute(out INamedTypeSymbol? serializerType);

            builder.Add(new MethodInfo(
                MethodName: symbol.Name,
                Parameters: ParameterInfo.From(methodSymbol),
                ReturnType: returnType,
                FullyQualifiedReturnTypeName: fullyQualifiedReturnTypeName,
                FullyQualifiedValueSetSerializerTypeName: serializerType?.GetFullyQualifiedName()));
        }

        return builder.ToImmutable();
    }

    /// <summary>
    /// Gets whether or not the method has a custom serializer.
    /// </summary>
    [MemberNotNullWhen(true, nameof(FullyQualifiedReturnTypeName))]
    [MemberNotNullWhen(true, nameof(FullyQualifiedValueSetSerializerTypeName))]
    public bool HasCustomValueSetSerializer => FullyQualifiedValueSetSerializerTypeName is not null;
}
