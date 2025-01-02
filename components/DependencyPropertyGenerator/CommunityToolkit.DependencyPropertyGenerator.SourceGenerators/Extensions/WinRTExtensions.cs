// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.GeneratedDependencyProperty.Constants;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for WinRT scenarios.
/// </summary>
internal static class WinRTExtensions
{
    /// <summary>
    /// Checks whether a given type is a well known WinRT projected value type (ie. a type that XAML can default).
    /// </summary>
    /// <param name="symbol">The input <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="useWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
    /// <returns>Whether <paramref name="symbol"/> is a well known WinRT projected value type..</returns>
    public static bool IsWellKnownWinRTProjectedValueType(this ITypeSymbol symbol, bool useWindowsUIXaml)
    {
        // Early check: we don't care about type parameters, only about named types
        if (symbol is not INamedTypeSymbol)
        {
            return false;
        }

        // This method only cares about non nullable value types
        if (symbol.IsDefaultValueNull())
        {
            return false;
        }

        // There is a special case for this: if the type of the property is a built-in WinRT
        // projected enum type or struct type (ie. some projected value type in general, except
        // for 'Nullable<T>' values), then we can just use 'null' and bypass creating the property
        // metadata. The WinRT runtime will automatically instantiate a default value for us.
        if (symbol.IsContainedInNamespace(WellKnownTypeNames.XamlNamespace(useWindowsUIXaml)))
        {
            return true;
        }

        // Special case for projected numeric types
        if (symbol.Name is "Matrix3x2" or "Matrix4x4" or "Plane" or "Quaternion" or "Vector2" or "Vector3" or "Vector4" &&
            symbol.IsContainedInNamespace("System.Numerics"))
        {
            return true;
        }

        // Special case a few more well known value types that are mapped for WinRT
        if (symbol.Name is "Point" or "Rect" or "Size" &&
            symbol.IsContainedInNamespace("Windows.Foundation"))
        {
            return true;
        }

        // Special case two more system types
        if (symbol is { MetadataName: "TimeSpan" or "DateTimeOffset", ContainingNamespace.MetadataName: "System" })
        {
            return true;
        }

        // Lastly, special case the well known primitive types
        if (symbol.SpecialType is
            SpecialType.System_Byte or
            SpecialType.System_SByte or
            SpecialType.System_Int16 or
            SpecialType.System_UInt16 or
            SpecialType.System_Char or
            SpecialType.System_Int32 or
            SpecialType.System_UInt32 or
            SpecialType.System_Int64 or
            SpecialType.System_UInt64 or
            SpecialType.System_Boolean or
            SpecialType.System_Single or
            SpecialType.System_Double)
        {
            return true;
        }

        return false;
    }
}
