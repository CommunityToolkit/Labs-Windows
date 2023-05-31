// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using CommunityToolkit.AppServices.SourceGenerators.Models;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="IParameterSymbol"/> type.
/// </summary>
internal static partial class IParameterSymbolExtensions
{
    /// <summary>
    /// Tries to parse the <see cref="ParameterOrReturnType"/> value from an input parameter symbol.
    /// </summary>
    /// <param name="parameterSymbol">The input <see cref="IParameterSymbol"/> instance.</param>
    /// <param name="parameterOrReturnType">The <see cref="ParameterOrReturnType"/> for <paramref name="parameterSymbol"/>, if valid.</param>
    /// <returns>Whether <paramref name="parameterOrReturnType"/> could successfully be retrieved from <paramref name="parameterSymbol"/>.</returns>
    public static bool TryGetParameterOrReturnType(this IParameterSymbol parameterSymbol, out ParameterOrReturnType parameterOrReturnType)
    {
        // Check if the parameter has a custom serializer specified
        if (parameterSymbol.TryGetValueSetSerializerTypeFromAttribute(out INamedTypeSymbol? serializerType))
        {
            // If that is the case, validate the serializer against the current parameter type.
            // This includes logic to also validate IProgress<T> parameters with custom serializers.
            return serializerType.IsValidValueSetSerializerTypeForParameterType(parameterSymbol, out parameterOrReturnType);
        }

        // If there is no custom serializer, the usual logic is used
        return parameterSymbol.Type.TryGetParameterOrReturnType(out parameterOrReturnType);
    }
}
