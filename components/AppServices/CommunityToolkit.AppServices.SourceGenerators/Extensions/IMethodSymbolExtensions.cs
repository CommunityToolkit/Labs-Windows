// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using CommunityToolkit.AppServices.SourceGenerators.Models;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="IMethodSymbol"/> type.
/// </summary>
internal static partial class IMethodSymbolExtensions
{
    /// <summary>
    /// Tries to parse the <see cref="ParameterOrReturnType"/> value from an input method symbol.
    /// </summary>
    /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance.</param>
    /// <param name="parameterOrReturnType">The <see cref="ParameterOrReturnType"/> for <paramref name="methodSymbol"/>, if valid.</param>
    /// <returns>Whether <paramref name="parameterOrReturnType"/> could successfully be retrieved from <paramref name="methodSymbol"/>.</returns>
    public static bool TryGetParameterOrReturnType(this IMethodSymbol methodSymbol, out ParameterOrReturnType parameterOrReturnType)
    {
        // Check if the method has a custom serializer for its return
        if (methodSymbol.TryGetValueSetSerializerTypeFromAttribute(out INamedTypeSymbol? serializerType))
        {
            // If the custom serializer doesn't match the return type, the return type is invalid
            if (!serializerType.IsValidValueSetSerializerTypeForReturnType(methodSymbol))
            {
                parameterOrReturnType = default;

                return false;
            }

            // Otherwise, the return type has to be a Task<T> with a custom serializer, which is valid
            parameterOrReturnType = ParameterOrReturnType.TaskOfT | ParameterOrReturnType.CustomSerializerType;

            return true;
        }

        // If there is no custom serializer, the usual logic is used
        return methodSymbol.ReturnType.TryGetParameterOrReturnType(out parameterOrReturnType);
    }

    /// <summary>
    /// Gets all member symbols from a given <see cref="INamedTypeSymbol"/> instance, including inherited ones.
    /// </summary>
    /// <param name="methodSymbol">The input <see cref="INamedTypeSymbol"/> instance.</param>
    /// <returns>A sequence of all member symbols for <paramref name="symbol"/>.</returns>
    /// <remarks>
    /// The logic here is very similar to that of <see cref="InvalidAppServicesMemberAnalyzer"/>, with the difference being
    /// that the logic here is simple as no diagnostics need to be generated, the method only has to check for correctness.
    /// </remarks>
    public static bool IsValidAppServicesMethod(this IMethodSymbol methodSymbol)
    {
        // All [AppServices] methods must be non-generic instance methods
        if (methodSymbol is not { IsStatic: false, IsGenericMethod: false, ReturnType: INamedTypeSymbol })
        {
            return false;
        }

        // Validate the return type
        if (!methodSymbol.TryGetParameterOrReturnType(out ParameterOrReturnType returnType) ||
            !returnType.IsValidReturnType())
        {
            return false;
        }

        bool hasProgress = false;
        bool hasCancellationToken = false;

        // Validate the method parameters
        foreach (IParameterSymbol parameter in methodSymbol.Parameters)
        {
            // First validate types that could possibly be allowed at all (ie. valid types)
            if (!parameter.TryGetParameterOrReturnType(out ParameterOrReturnType parameterType) ||
                !parameterType.IsValidParameterType())
            {
                return false;
            }

            // Then check that the type is not an IProgress<T>, if one has already been discovered
            if (parameterType.HasFlag(ParameterOrReturnType.IProgressOfT))
            {
                if (hasProgress)
                {
                    return false;
                }

                hasProgress = true;
            }

            // Lastly, check that the type is not a CancellationToken, if one has already been discovered
            if (parameterType.HasFlag(ParameterOrReturnType.CancellationToken))
            {
                if (hasCancellationToken)
                {
                    return false;
                }

                hasCancellationToken = true;
            }
        }

        return true;
    }
}
