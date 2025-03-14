// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for the <see cref="SyntaxTrivia"/> type.
/// </summary>
internal static class SyntaxTriviaExtensions
{
    /// <summary>
    /// Deconstructs a <see cref="SyntaxTrivia"/> into its <see cref="SyntaxKind"/> value.
    /// </summary>
    /// <param name="syntaxTrivia">The input <see cref="SyntaxTrivia"/> value.</param>
    /// <param name="syntaxKind">The resulting <see cref="SyntaxKind"/> value for <paramref name="syntaxTrivia"/>.</param>
    public static void Deconstruct(this SyntaxTrivia syntaxTrivia, out SyntaxKind syntaxKind)
    {
        syntaxKind = syntaxTrivia.Kind();
    }
}
