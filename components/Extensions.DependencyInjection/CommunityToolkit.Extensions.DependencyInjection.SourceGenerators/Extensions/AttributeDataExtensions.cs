// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="AttributeData"/> type.
/// </summary>
internal static class AttributeDataExtensions
{
    /// <summary>
    /// Tries to get the location of the input <see cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="attributeData">The input <see cref="AttributeData"/> instance to get the location for.</param>
    /// <returns>The resulting location for <paramref name="attributeData"/>, if a syntax reference is available.</returns>
    public static Location? GetLocation(this AttributeData attributeData)
    {
        if (attributeData.ApplicationSyntaxReference is { } syntaxReference)
        {
            return syntaxReference.SyntaxTree.GetLocation(syntaxReference.Span);
        }

        return null;
    }
}