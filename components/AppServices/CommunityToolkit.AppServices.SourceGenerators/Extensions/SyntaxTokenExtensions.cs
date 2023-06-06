// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="SyntaxToken"/> type.
/// </summary>
internal static partial class SyntaxTokenExtensions
{
    /// <summary>
    /// Deconstructs a <see cref="SyntaxToken"/> into its <see cref="SyntaxKind"/> value.
    /// </summary>
    /// <param name="token">The input <see cref="SyntaxToken"/> instance.</param>
    /// <param name="kind">The <see cref="SyntaxKind"/> for <paramref name="token"/>.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void Deconstruct(this SyntaxToken token, out SyntaxKind kind)
    {
        kind = token.Kind();
    }
}
