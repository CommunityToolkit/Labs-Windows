// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for <see cref="IOperation"/> types.
/// </summary>
internal static class IOperationExtensions
{
    /// <summary>
    /// Checks whether a given operation represents a default constant value.
    /// </summary>
    /// <param name="operation">The input <see cref="IOperation"/> instance.</param>
    /// <returns>Whether <paramref name="operation"/> represents a default constant value.</returns>
    public static bool IsConstantValueDefault(this IOperation operation)
    {
        if (operation is not { Type: not null, ConstantValue.HasValue: true })
        {
            return false;
        }

        // Easy check for reference types
        if (operation is { Type.IsReferenceType: true, ConstantValue.Value: null })
        {
            return true;
        }

        // Equivalent check for nullable value types too
        if (operation is { Type.SpecialType: SpecialType.System_Nullable_T, ConstantValue.Value: null })
        {
            return true;
        }

        // Special case enum types as well (enum types only support a subset of primitive types)
        if (operation.Type is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: { } underlyingType })
        {
            return (underlyingType.SpecialType, operation.ConstantValue.Value) switch
            {
                (SpecialType.System_Byte, default(byte)) or
                (SpecialType.System_SByte, default(sbyte)) or
                (SpecialType.System_Int16, default(short)) or
                (SpecialType.System_UInt16, default(ushort)) or
                (SpecialType.System_Int32, default(int)) or
                (SpecialType.System_UInt32, default(uint)) or
                (SpecialType.System_Int64, default(long)) or
                (SpecialType.System_UInt64, default(ulong)) => true,
                _ => false
            };
        }

        // Manually match for known primitive types (this should be kept in sync with 'IsWellKnownWinRTProjectedValueType')
        return (operation.Type.SpecialType, operation.ConstantValue.Value) switch
        {
            (SpecialType.System_Byte, default(byte)) or
            (SpecialType.System_SByte, default(sbyte)) or
            (SpecialType.System_Int16, default(short)) or
            (SpecialType.System_UInt16, default(ushort)) or
            (SpecialType.System_Char, default(char)) or
            (SpecialType.System_Int32, default(int)) or
            (SpecialType.System_UInt32, default(uint)) or
            (SpecialType.System_Int64, default(long)) or
            (SpecialType.System_UInt64, default(ulong)) or
            (SpecialType.System_Boolean, default(bool)) => true,
            (SpecialType.System_Single, float x) when BitConverter.DoubleToInt64Bits(x) == 0 => true,
            (SpecialType.System_Double, double x) when BitConverter.DoubleToInt64Bits(x) == 0 => true,
            _ => false
        };
    }
}
