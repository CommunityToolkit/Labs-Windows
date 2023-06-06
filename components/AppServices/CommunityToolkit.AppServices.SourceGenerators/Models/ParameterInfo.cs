// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using CommunityToolkit.AppServices.SourceGenerators.Helpers;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.AppServices.SourceGenerators.Models;

/// <summary>
/// A model for a method parameter.
/// </summary>
/// <param name="Name">The parameter name.</param>
/// <param name="Type">The parameter type.</param>
/// <param name="FullyQualifiedTypeName">The fully qualified type name of the type being serialized, if any.</param>
/// <param name="FullyQualifiedValueSetSerializerTypeName">The fully qualified type name of the custom ValueSet serializer to use, if any.</param>
internal sealed record ParameterInfo(
    string Name,
    ParameterOrReturnType Type,
    string? FullyQualifiedTypeName,
    string? FullyQualifiedValueSetSerializerTypeName)
{
    /// <summary>
    /// Creates <see cref="ParameterInfo"/> instances from methods in a given <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="methodSymbol">The input <see cref="IMethodSymbol"/> instance to gather info for.</param>
    /// <returns>A collection of <see cref="ParameterInfo"/> instances from <paramref name="methodSymbol"/>.</returns>
    public static ImmutableArray<ParameterInfo> From(IMethodSymbol methodSymbol)
    {
        using ImmutableArrayBuilder<ParameterInfo> builder = ImmutableArrayBuilder<ParameterInfo>.Rent();

        foreach (IParameterSymbol parameterInfo in methodSymbol.Parameters)
        {
            // This method is only invoked from MethodInfo.From, which already does parameter validation first
            _ = parameterInfo.TryGetParameterOrReturnType(out ParameterOrReturnType type);

            // Get the parameter type name, if a serializer is used or if the type is an enum
            string? fullyQualifiedTypeName = type.HasAnyFlags(ParameterOrReturnType.CustomSerializerType | ParameterOrReturnType.Enum) switch
            {
                true => parameterInfo.Type.GetFullyQualifiedName(),
                false => null
            };

            // Same as above for the ValueSet serializer, if any
            _ = parameterInfo.TryGetValueSetSerializerTypeFromAttribute(out INamedTypeSymbol? serializerType);

            builder.Add(new ParameterInfo(
                Name: parameterInfo.Name,
                Type: type,
                FullyQualifiedTypeName: fullyQualifiedTypeName,
                FullyQualifiedValueSetSerializerTypeName: serializerType?.GetFullyQualifiedName()));
        }

        return builder.ToImmutable();
    }

    /// <summary>
    /// Creates a <see cref="TypeSyntax"/> instance representing a given parameter.
    /// </summary>
    /// <param name="type">The parameter type.</param>
    /// <param name="fullyQualifiedTypeName">The fully qualified type name, in case a custom serializer is used or the type is an enum.</param>
    /// <returns>A <see cref="TypeSyntax"/> instance representing a given parameter.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the type is not valid.</exception>
    public static TypeSyntax GetSyntax(ParameterOrReturnType type, string? fullyQualifiedTypeName)
    {
        // If a custom serializer is used or if the type is an enum, return the fully qualified type name directly
        if (fullyQualifiedTypeName is not null)
        {
            return IdentifierName(fullyQualifiedTypeName);
        }

        // If the type is a cancellation token, handle it first as it has no element type
        if (type.HasFlag(ParameterOrReturnType.CancellationToken))
        {
            return
                QualifiedName(
                    QualifiedName(
                        AliasQualifiedName(
                            IdentifierName(Token(SyntaxKind.GlobalKeyword)),
                            IdentifierName("System")),
                        IdentifierName("Threading")),
                    IdentifierName("CancellationToken"));
        }

        // Get the type syntax for the inner parameter type
        TypeSyntax typeSyntax = (type & ParameterOrReturnType.ElementTypeMask) switch
        {
            ParameterOrReturnType.UInt8 => PredefinedType(Token(SyntaxKind.ByteKeyword)),
            ParameterOrReturnType.Int16 => PredefinedType(Token(SyntaxKind.ShortKeyword)),
            ParameterOrReturnType.UInt16 => PredefinedType(Token(SyntaxKind.UShortKeyword)),
            ParameterOrReturnType.Int32 => PredefinedType(Token(SyntaxKind.IntKeyword)),
            ParameterOrReturnType.UInt32 => PredefinedType(Token(SyntaxKind.UIntKeyword)),
            ParameterOrReturnType.Int64 => PredefinedType(Token(SyntaxKind.LongKeyword)),
            ParameterOrReturnType.UInt64 => PredefinedType(Token(SyntaxKind.ULongKeyword)),
            ParameterOrReturnType.Single => PredefinedType(Token(SyntaxKind.FloatKeyword)),
            ParameterOrReturnType.Double => PredefinedType(Token(SyntaxKind.DoubleKeyword)),
            ParameterOrReturnType.Char16 => PredefinedType(Token(SyntaxKind.CharKeyword)),
            ParameterOrReturnType.Boolean => PredefinedType(Token(SyntaxKind.BoolKeyword)),
            ParameterOrReturnType.String => PredefinedType(Token(SyntaxKind.StringKeyword)),
            ParameterOrReturnType.DateTime => QualifiedName(AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName("System")), IdentifierName("DateTime")),
            ParameterOrReturnType.TimeSpan => QualifiedName(AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName("System")), IdentifierName("TimeSpan")),
            ParameterOrReturnType.Guid => QualifiedName(AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName("System")), IdentifierName("Guid")),
            ParameterOrReturnType.Point => QualifiedName(QualifiedName(AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName("Windows")), IdentifierName("Foundation")), IdentifierName("Point")),
            ParameterOrReturnType.Size => QualifiedName(QualifiedName(AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName("Windows")), IdentifierName("Foundation")), IdentifierName("Size")),
            ParameterOrReturnType.Rect => QualifiedName(QualifiedName(AliasQualifiedName(IdentifierName(Token(SyntaxKind.GlobalKeyword)), IdentifierName("Windows")), IdentifierName("Foundation")), IdentifierName("Rect")),
            _ => throw new ArgumentOutOfRangeException("Invalid special type.")
        };

        if (type.HasFlag(ParameterOrReturnType.Array))
        {
            // Return an array type where the element type is the current type syntax
            return
                ArrayType(typeSyntax)
                .AddRankSpecifiers(ArrayRankSpecifier(
                    SingletonSeparatedList<ExpressionSyntax>(
                        OmittedArraySizeExpression())));
        }

        if (type.HasFlag(ParameterOrReturnType.IProgressOfT))
        {
            // Return an IProgress<T> type where the type argument is the current type syntax
            return
                QualifiedName(
                    AliasQualifiedName(
                        IdentifierName(Token(SyntaxKind.GlobalKeyword)),
                        IdentifierName("System")),
                    GenericName(Identifier("IProgress"))
                    .AddTypeArgumentListArguments(typeSyntax));
        }

        return typeSyntax;
    }

    /// <summary>
    /// Gets whether or not the parameter has a custom serializer.
    /// </summary>
    [MemberNotNullWhen(true, nameof(FullyQualifiedTypeName))]
    [MemberNotNullWhen(true, nameof(FullyQualifiedValueSetSerializerTypeName))]
    public bool HasCustomValueSetSerializer => FullyQualifiedValueSetSerializerTypeName is not null;

    /// <summary>
    /// Creates a <see cref="TypeSyntax"/> instance representing the current parameter.
    /// </summary>
    /// <returns>A <see cref="TypeSyntax"/> instance representing the current parameter.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the type is not valid.</exception>
    public TypeSyntax GetSyntax()
    {
        return GetSyntax(Type, FullyQualifiedTypeName);
    }
}