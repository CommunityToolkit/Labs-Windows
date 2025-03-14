// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for <see cref="ITypeParameterSymbol"/> types.
/// </summary>
internal static class ITypeParameterSymbolExtensions
{
    /// <summary>
    /// Checks whether a given type parameter is a reference type.
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeParameterSymbol"/> instance to check.</param>
    /// <returns>Whether the input type parameter is a reference type.</returns>
    public static bool IsReferenceTypeOrIndirectlyConstrainedToReferenceType(this ITypeParameterSymbol symbol)
    {
        // The type is definitely a reference type (e.g. it has the 'class' constraint)
        if (symbol.IsReferenceType)
        {
            return true;
        }

        // The type is definitely a value type (e.g. it has the 'struct' constraint)
        if (symbol.IsValueType)
        {
            return false;
        }

        foreach (ITypeSymbol constraintType in symbol.ConstraintTypes)
        {
            // Recurse on the type parameter first (e. g. we might indirectly be getting a 'class' constraint)
            if (constraintType is ITypeParameterSymbol typeParameter &&
                typeParameter.IsReferenceTypeOrIndirectlyConstrainedToReferenceType())
            {
                return true;
            }

            // Special constraint type that type parameters can derive from. Note that for concrete enum
            // types, the 'Enum' constraint isn't sufficient, they'd also have e.g. 'struct', which is
            // already checked before. If a type parameter only has 'Enum', then it should be considered
            // a reference type.
            if (constraintType.SpecialType is SpecialType.System_Delegate or SpecialType.System_Enum)
            {
                return true;
            }

            // Only check for classes (an interface doesn't guarantee the type argument will be a reference type)
            if (constraintType.TypeKind is TypeKind.Class)
            {
                return true;
            }
        }

        return false;
    }
}
