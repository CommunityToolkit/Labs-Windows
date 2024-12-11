// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// A <see langword="class"/> with some extension methods for C# syntax kinds.
/// </summary>
internal static partial class SyntaxKindExtensions
{
    /// <summary>
    /// Converts an <see cref="ImmutableArray{T}"/> of <see cref="SyntaxKind"/> values to one of their underlying type.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> value.</param>
    /// <returns>The resulting <see cref="ImmutableArray{T}"/> of <see cref="ushort"/> values.</returns>
    public static ImmutableArray<ushort> AsUnderlyingType(this ImmutableArray<SyntaxKind> array)
    {
        ushort[]? underlyingArray = (ushort[]?)(object?)Unsafe.As<ImmutableArray<SyntaxKind>, SyntaxKind[]?>(ref array);

        return Unsafe.As<ushort[]?, ImmutableArray<ushort>>(ref underlyingArray);
    }

    /// <summary>
    /// Converts an <see cref="ImmutableArray{T}"/> of <see cref="ushort"/> values to one of their real type.
    /// </summary>
    /// <param name="array">The input <see cref="ImmutableArray{T}"/> value.</param>
    /// <returns>The resulting <see cref="ImmutableArray{T}"/> of <see cref="SyntaxKind"/> values.</returns>
    public static ImmutableArray<SyntaxKind> AsSyntaxKindArray(this ImmutableArray<ushort> array)
    {
        SyntaxKind[]? typedArray = (SyntaxKind[]?)(object?)Unsafe.As<ImmutableArray<ushort>, ushort[]?>(ref array);

        return Unsafe.As<SyntaxKind[]?, ImmutableArray<SyntaxKind>>(ref typedArray);
    }
}
