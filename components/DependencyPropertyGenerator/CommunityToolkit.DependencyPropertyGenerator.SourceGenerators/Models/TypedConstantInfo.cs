// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.GeneratedDependencyProperty.Models;

/// <summary>
/// A model representing a typed constant item.
/// </summary>
internal abstract partial record TypedConstantInfo
{
    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing a <see langword="null"/> value.
    /// </summary>
    public sealed record Null : TypedConstantInfo
    {
        /// <summary>
        /// The shared <see cref="Null"/> instance (the type is stateless).
        /// </summary>
        public static Null Instance { get; } = new();

        /// <inheritdoc/>
        public override string ToString()
        {
            return "null";
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing an array.
    /// </summary>
    /// <param name="ElementTypeName">The type name for array elements.</param>
    /// <param name="Items">The sequence of contained elements.</param>
    public sealed record Array(string ElementTypeName, EquatableArray<TypedConstantInfo> Items) : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            ArrayCreationExpressionSyntax arrayCreationExpressionSyntax =
                ArrayCreationExpression(
                ArrayType(IdentifierName(ElementTypeName))
                .AddRankSpecifiers(ArrayRankSpecifier(SingletonSeparatedList<ExpressionSyntax>(OmittedArraySizeExpression()))))
                .WithInitializer(InitializerExpression(SyntaxKind.ArrayInitializerExpression)
                .AddExpressions(Items.Select(static c => ParseExpression(c.ToString())).ToArray()));

            return arrayCreationExpressionSyntax.NormalizeWhitespace(eol: "\n").ToFullString();
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing a primitive value.
    /// </summary>
    public abstract record Primitive : TypedConstantInfo
    {
        /// <summary>
        /// A <see cref="TypedConstantInfo"/> type representing a <see cref="string"/> value.
        /// </summary>
        /// <param name="Value">The input <see cref="string"/> value.</param>
        public sealed record String(string Value) : TypedConstantInfo
        {
            /// <inheritdoc/>
            public override string ToString()
            {
                return '"' + Value + '"';
            }
        }

        /// <summary>
        /// A <see cref="TypedConstantInfo"/> type representing a <see cref="bool"/> value.
        /// </summary>
        /// <param name="Value">The input <see cref="bool"/> value.</param>
        public sealed record Boolean(bool Value) : TypedConstantInfo
        {
            /// <inheritdoc/>
            public override string ToString()
            {
                return Value ? "true" : "false";
            }
        }

        /// <summary>
        /// A <see cref="TypedConstantInfo"/> type representing a generic primitive value.
        /// </summary>
        /// <typeparam name="T">The primitive type.</typeparam>
        /// <param name="Value">The input primitive value.</param>
        public sealed record Of<T>(T Value) : TypedConstantInfo
            where T : unmanaged, IEquatable<T>
        {
            /// <summary>
            /// The cached map of constant fields for the type.
            /// </summary>
            private static readonly FrozenDictionary<T, string> ConstantFields = GetConstantFields();

            /// <inheritdoc/>
            public override string ToString()
            {
                static ExpressionSyntax GetExpression(T value)
                {
                    // Try to match named constants first
                    if (TryGetConstantExpression(value, out ExpressionSyntax? expression))
                    {
                        return expression;
                    }

                    // Special logic for doubles
                    if (value is double d)
                    {
                        // Handle 'double.NaN' explicitly, as 'ToString()' won't work on it at all
                        if (double.IsNaN(d))
                        {
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                PredefinedType(Token(SyntaxKind.DoubleKeyword)), IdentifierName("NaN"));
                        }

                        // Handle 0, to avoid matching against positive/negative zeros
                        if (d == 0)
                        {
                            return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal("0.0", 0.0));
                        }

                        string rawLiteral = d.ToString("R", CultureInfo.InvariantCulture);

                        // For doubles, we need to manually format it and always add the trailing "D" suffix.
                        // This ensures that the correct type is produced if the expression was assigned to
                        // an object (eg. the literal was used in an attribute object parameter/property).
                        string literal = rawLiteral.Contains(".") ? rawLiteral : $"{rawLiteral}.0";

                        return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(literal, d));
                    }

                    // Same special handling for floats as well
                    if (value is float f)
                    {
                        // Handle 'float.NaN' as above
                        if (float.IsNaN(f))
                        {
                            return MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                PredefinedType(Token(SyntaxKind.FloatKeyword)), IdentifierName("NaN"));
                        }

                        // Handle 0, same as above too
                        if (f == 0)
                        {
                            return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal("0.0F", 0.0f));
                        }

                        string rawLiteral = f.ToString("R", CultureInfo.InvariantCulture);

                        // For floats, Roslyn will automatically add the "F" suffix, so no extra work is needed.
                        // However, we still format it manually to ensure we consistently add ".0" as suffix.
                        string literal = rawLiteral.Contains(".") ? $"{rawLiteral}F" : $"{rawLiteral}.0F";

                        return LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(literal, f));
                    }

                    // Handle all other supported types as well
                    return LiteralExpression(SyntaxKind.NumericLiteralExpression, value switch
                    {
                        byte b => Literal(b),
                        char c => Literal(c),
                        int i => Literal(i),
                        long l => Literal(l),
                        sbyte sb => Literal(sb),
                        short sh => Literal(sh),
                        uint ui => Literal(ui),
                        ulong ul => Literal(ul),
                        ushort ush => Literal(ush),
                        _ => throw new ArgumentException("Invalid primitive type")
                    });
                }

                return GetExpression(Value).NormalizeWhitespace(eol: "\n").ToFullString();
            }

            /// <summary>
            /// Tries to get a constant expression for a given value.
            /// </summary>
            /// <param name="value">The value to try to get an expression for.</param>
            /// <param name="expression">The resulting expression, if successfully retrieved.</param>
            /// <returns>The expression for <paramref name="value"/>, if available.</returns>
            /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not of a supported type.</exception>
            private static bool TryGetConstantExpression(T value, [NotNullWhen(true)] out ExpressionSyntax? expression)
            {
                if (ConstantFields.TryGetValue(value, out string? name))
                {
                    SyntaxKind syntaxKind = value switch
                    {
                        byte => SyntaxKind.ByteKeyword,
                        char => SyntaxKind.CharKeyword,
                        double => SyntaxKind.DoubleKeyword,
                        float => SyntaxKind.FloatKeyword,
                        int => SyntaxKind.IntKeyword,
                        long => SyntaxKind.LongKeyword,
                        sbyte => SyntaxKind.SByteKeyword,
                        short => SyntaxKind.ShortKeyword,
                        uint => SyntaxKind.UIntKeyword,
                        ulong => SyntaxKind.ULongKeyword,
                        ushort => SyntaxKind.UShortKeyword,
                        _ => throw new ArgumentException("Invalid primitive type")
                    };

                    expression = MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        PredefinedType(Token(syntaxKind)), IdentifierName(name));

                    return true;
                }

                expression = null;

                return false;
            }

            /// <summary>
            /// Gets a mapping of all well known constant fields for the current type.
            /// </summary>
            /// <returns>The mapping of all well known constant fields for the current type.</returns>
            private static FrozenDictionary<T, string> GetConstantFields()
            {
                return typeof(T)
                    .GetFields()
                    .Where(static info => info.IsLiteral)
                    .Where(static info => info.FieldType == typeof(T))
                    .Select(static info => (Value: (T)info.GetRawConstantValue(), info.Name))
                    .Where(static info => !EqualityComparer<T>.Default.Equals(info.Value, default))
                    .ToFrozenDictionary(
                        keySelector: static info => info.Value,
                        elementSelector: static info => info.Name);


            }
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing a type.
    /// </summary>
    /// <param name="TypeName">The input type name.</param>
    public sealed record Type(string TypeName) : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"typeof({TypeName})";
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing a known enum value.
    /// </summary>
    /// <param name="TypeName">The enum type name.</param>
    /// <param name="FieldName">The enum field name.</param>
    public sealed record KnownEnum(string TypeName, string FieldName) : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{TypeName}.{FieldName}";
        }
    }

    /// <summary>
    /// A <see cref="TypedConstantInfo"/> type representing an enum value.
    /// </summary>
    /// <param name="TypeName">The enum type name.</param>
    /// <param name="Value">The boxed enum value.</param>
    public sealed record Enum(string TypeName, object Value) : TypedConstantInfo
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            // We let Roslyn parse the value expression, so that it can automatically handle both positive and negative values. This
            // is needed because negative values have a different syntax tree (UnaryMinusExpression holding the numeric expression).
            ExpressionSyntax valueExpression = ParseExpression(Value.ToString());

            // If the value is negative, we have to put parentheses around them (to avoid CS0075 errors)
            if (valueExpression is PrefixUnaryExpressionSyntax unaryExpression && unaryExpression.IsKind(SyntaxKind.UnaryMinusExpression))
            {
                valueExpression = ParenthesizedExpression(valueExpression);
            }

            // Now we can safely return the cast expression for the target enum type (with optional parentheses if needed)
            return $"({TypeName}){valueExpression.NormalizeWhitespace(eol: "\n").ToFullString()}";
        }
    }
}
