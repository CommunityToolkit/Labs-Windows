// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

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
    /// Creates a new <see cref="TypedConstantInfo"/> instance from a given <see cref="IOperation"/> instance.
    /// </summary>
    /// <param name="operation">The input <see cref="IOperation"/> instance.</param>
    /// <returns>A <see cref="TypedConstantInfo"/> instance representing <paramref name="arg"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    /// <remarks>This method only supports constant values.</remarks>
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

    /// <summary>
    /// Creates a new <see cref="TypedConstantInfo"/> instance from a given <see cref="IOperation"/> instance.
    /// </summary>
    /// <param name="operation">The input <see cref="IOperation"/> instance.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> that was used to retrieve <paramref name="operation"/>.</param>
    /// <param name="expression">The <see cref="ExpressionSyntax"/> that <paramref name="operation"/> was retrieved from.</param>
    /// <param name="token">The cancellation token for the current operation.</param>
    /// <param name="result">The resulting <see cref="TypedConstantInfo"/> instance, if available.</param>
    /// <returns>Whether a resulting <see cref="TypedConstantInfo"/> instance could be created.</returns>
    /// <exception cref="ArgumentException">Thrown if the input argument is not valid.</exception>
    public static bool TryCreate(
        IOperation operation,
        SemanticModel semanticModel,
        ExpressionSyntax expression,
        CancellationToken token,
        [NotNullWhen(true)] out TypedConstantInfo? result)
    {
        if (TryCreate(operation, out result))
        {
            return true;
        }

        if (operation is ITypeOfOperation typeOfOperation)
        {
            result = new Type(typeOfOperation.TypeOperand.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

            return true;
        }

        if (operation is IArrayCreationOperation)
        {
            string? elementTypeName = ((IArrayTypeSymbol?)operation.Type)?.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            // If the element type is not available (since the attribute wasn't checked), just default to object
            elementTypeName ??= "object";

            // Handle all possible ways of initializing arrays in attributes
            IEnumerable<ExpressionSyntax>? arrayElementExpressions = expression switch
            {
                ImplicitArrayCreationExpressionSyntax { Initializer.Expressions: { } expressions } => expressions,
                ArrayCreationExpressionSyntax { Initializer.Expressions: { } expressions } => expressions,
                CollectionExpressionSyntax { Elements: { } elements } => elements.OfType<ExpressionElementSyntax>().Select(static element => element.Expression),
                _ => null
            };

            // No element expressions found, just return an empty array
            if (arrayElementExpressions is null)
            {
                result = new Array(elementTypeName, ImmutableArray<TypedConstantInfo>.Empty);

                return true;
            }

            using ImmutableArrayBuilder<TypedConstantInfo> items = new();

            // Enumerate all array elements and extract serialized info for them
            foreach (ExpressionSyntax elementExpressions in arrayElementExpressions)
            {
                if (semanticModel.GetOperation(elementExpressions, token) is not IOperation initializationOperation)
                {
                    goto Failure;
                }

                if (!TryCreate(initializationOperation, semanticModel, elementExpressions, token, out TypedConstantInfo? elementInfo))
                {
                    goto Failure;
                }

                items.Add(elementInfo);
            }

            result = new Array(elementTypeName, items.ToImmutable());

            return true;
        }

        Failure:
        result = null;

        return false;
    }
}
