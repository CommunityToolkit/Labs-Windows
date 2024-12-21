// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Models;

/// <inheritdoc/>
partial record TypedConstantInfo
{
    /// <summary>
    /// Creates a new <see cref="TypedConstantInfo"/> instance from a given <see cref="TypedConstant"/> value.
    /// </summary>
    /// <param name="arg">The input <see cref="TypedConstant"/> value.</param>
    /// <returns>A <see cref="TypedConstantInfo"/> instance representing <paramref name="arg"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    public static TypedConstantInfo Create(TypedConstant arg)
    {
        if (arg.IsNull)
        {
            return new Null();
        }

        if (arg.Kind == TypedConstantKind.Array)
        {
            string elementTypeName = ((IArrayTypeSymbol)arg.Type!).ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            ImmutableArray<TypedConstantInfo> items = arg.Values.Select(Create).ToImmutableArray();

            return new Array(elementTypeName, items);
        }

        return (arg.Kind, arg.Value) switch
        {
            (TypedConstantKind.Primitive, string text) => new Primitive.String(text),
            (TypedConstantKind.Primitive, bool flag) => new Primitive.Boolean(flag),
            (TypedConstantKind.Primitive, object value) => value switch
            {
                byte b => new Primitive.Of<byte>(b),
                char c => new Primitive.Of<char>(c),
                double d => new Primitive.Of<double>(d),
                float f => new Primitive.Of<float>(f),
                int i => new Primitive.Of<int>(i),
                long l => new Primitive.Of<long>(l),
                sbyte sb => new Primitive.Of<sbyte>(sb),
                short sh => new Primitive.Of<short>(sh),
                uint ui => new Primitive.Of<uint>(ui),
                ulong ul => new Primitive.Of<ulong>(ul),
                ushort ush => new Primitive.Of<ushort>(ush),
                _ => throw new ArgumentException("Invalid primitive type")
            },
            (TypedConstantKind.Type, ITypeSymbol type)
                => new Type(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
            (TypedConstantKind.Enum, object value) when arg.Type!.TryGetEnumFieldName(value, out string? fieldName)
                => new KnownEnum(arg.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), fieldName),
            (TypedConstantKind.Enum, object value)
                => new Enum(arg.Type!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), value),
            _ => throw new ArgumentException("Invalid typed constant type"),
        };
    }

    /// <summary>
    /// Creates a new <see cref="TypedConstantInfo"/> instance from a given <see cref="TypedConstant"/> value.
    /// </summary>
    /// <param name="arg">The input <see cref="TypedConstant"/> value.</param>
    /// <returns>A <see cref="TypedConstantInfo"/> instance representing <paramref name="arg"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    public static bool TryCreate(IOperation operation, [NotNullWhen(true)] out TypedConstantInfo? result)
    {
        // Validate that we do have some constant value
        if (operation is not { Type: { } operationType, ConstantValue.HasValue: true })
        {
            result = null;

            return false;
        }

        if (operation.ConstantValue.Value is null)
        {
            result = new Null();

            return true;
        }

        // Handle all known possible constant values
        result = (operationType, operation.ConstantValue.Value) switch
        {
            ({ SpecialType: SpecialType.System_String }, string text) => new Primitive.String(text),
            ({ SpecialType: SpecialType.System_Boolean}, bool flag) => new Primitive.Boolean(flag),
            (INamedTypeSymbol { TypeKind: TypeKind.Enum }, object value) when operationType.TryGetEnumFieldName(value, out string? fieldName)
                => new KnownEnum(operationType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), fieldName),
            (INamedTypeSymbol { TypeKind: TypeKind.Enum }, object value)
                => new Enum(operationType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), value),
            (_, byte b) => new Primitive.Of<byte>(b),
            (_, char c) => new Primitive.Of<char>(c),
            (_, double d) => new Primitive.Of<double>(d),
            (_, float f) => new Primitive.Of<float>(f),
            (_, int i) => new Primitive.Of<int>(i),
            (_, long l) => new Primitive.Of<long>(l),
            (_, sbyte sb) => new Primitive.Of<sbyte>(sb),
            (_, short sh) => new Primitive.Of<short>(sh),
            (_, uint ui) => new Primitive.Of<uint>(ui),
            (_, ulong ul) => new Primitive.Of<ulong>(ul),
            (_, ushort ush) => new Primitive.Of<ushort>(ush),
            (_, ITypeSymbol type) => new Type(type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)),
            _ => throw new ArgumentException("Invalid typed constant type"),
        };

        return true;
    }
}
