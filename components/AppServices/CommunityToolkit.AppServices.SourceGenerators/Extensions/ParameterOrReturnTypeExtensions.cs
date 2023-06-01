// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.AppServices.SourceGenerators.Models;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="Paramet"/> type.
/// </summary>
internal static partial class ParameterOrReturnTypeExtensions
{
    /// <summary>
    /// Checks whether an input <see cref="ParameterOrReturnType"/> value can be used as a parameter type.
    /// </summary>
    /// <param name="parameterType">The <see cref="ParameterOrReturnType"/> value to check.</param>
    /// <returns>Whether <paramref name="parameterType"/> can be used as a return type.</returns>
    public static bool IsValidParameterType(this ParameterOrReturnType parameterType)
    {
        // The sets of valid return types and parameter types are disjointed, so we can just invert the condition and
        // reuse the same logic. That is, return types only allow Task and Task<T>, which can't be used as parameters.
        return !IsValidReturnType(parameterType);
    }

    /// <summary>
    /// Checks whether an input <see cref="ParameterOrReturnType"/> value can be used as a return type.
    /// </summary>
    /// <param name="returnType">The <see cref="ParameterOrReturnType"/> value to check.</param>
    /// <returns>Whether <paramref name="returnType"/> can be used as a return type.</returns>
    public static bool IsValidReturnType(this ParameterOrReturnType returnType)
    {
        return returnType.HasAnyFlags(ParameterOrReturnType.Task | ParameterOrReturnType.TaskOfT);
    }
}
