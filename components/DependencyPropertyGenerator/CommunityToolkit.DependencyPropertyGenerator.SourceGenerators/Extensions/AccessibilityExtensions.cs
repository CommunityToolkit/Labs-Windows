// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for the <see cref="Accessibility"/> type.
/// </summary>
internal static class AccessibilityExtensions
{
    /// <summary>
    /// Gets the expression for a given <see cref="Accessibility"/> value.
    /// </summary>
    /// <param name="accessibility">The input <see cref="Accessibility"/> value.</param>
    /// <returns>The expression for <paramref name="accessibility"/>.</returns>
    public static string GetExpression(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "private protected",
            Accessibility.Protected => "protected",
            Accessibility.Internal => "internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.Public => "public",
            _ => ""
        };
    }
}
