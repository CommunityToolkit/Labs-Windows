// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

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
        if (attributeData.ApplicationSyntaxReference is not { } syntaxReference)
        {
            return null;
        }

        return syntaxReference.SyntaxTree.GetLocation(syntaxReference.Span);
    }

    /// <summary>
    /// Tries to get the location of a named argument in an input <see cref="AttributeData"/> instance.
    /// </summary>
    /// <param name="attributeData">The input <see cref="AttributeData"/> instance to get the location for.</param>
    /// <param name="name">The name of the argument to look for.</param>
    /// <param name="token">The cancellation token for the operation.</param>
    /// <returns>The resulting location for <paramref name="attributeData"/>, if a syntax reference is available.</returns>
    public static Location? GetNamedArgumentOrAttributeLocation(this AttributeData attributeData, string name, CancellationToken token = default)
    {
        if (attributeData.ApplicationSyntaxReference is not { } syntaxReference)
        {
            return null;
        }

        // If we can recover the syntax node, look for the target named argument
        if (syntaxReference.GetSyntax(token) is AttributeSyntax { ArgumentList: { } argumentList })
        {
            foreach (AttributeArgumentSyntax argument in argumentList.Arguments)
            {
                if (argument.NameEquals?.Name.Identifier.Text == name)
                {
                    return argument.GetLocation();
                }
            }
        }

        // Otherwise, fallback to the location of the whole attribute
        return syntaxReference.SyntaxTree.GetLocation(syntaxReference.Span);
    }

    /// <summary>
    /// Tries to get a constructor argument at a given index from the input <see cref="AttributeData"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of constructor argument to retrieve.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to get the argument from.</param>
    /// <param name="index">The index of the argument to try to retrieve.</param>
    /// <param name="result">The resulting argument, if it was found.</param>
    /// <returns>Whether or not an argument of type <typeparamref name="T"/> at position <paramref name="index"/> was found.</returns>
    public static bool TryGetConstructorArgument<T>(this AttributeData attributeData, int index, [NotNullWhen(true)] out T? result)
    {
        if (attributeData.ConstructorArguments.Length > index &&
            attributeData.ConstructorArguments[index].Value is T argument)
        {
            result = argument;

            return true;
        }

        result = default;

        return false;
    }

    /// <summary>
    /// Tries to get a given named argument value from an <see cref="AttributeData"/> instance, or a default value.
    /// </summary>
    /// <typeparam name="T">The type of argument to check.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
    /// <param name="name">The name of the argument to check.</param>
    /// <param name="defaultValue">The default value to return if the argument is not found.</param>
    /// <returns>The argument value, or <paramref name="defaultValue"/>.</returns>
    public static T? GetNamedArgument<T>(this AttributeData attributeData, string name, T? defaultValue = default)
    {
        if (TryGetNamedArgument(attributeData, name, out T? value))
        {
            return value;
        }

        return defaultValue;
    }

    /// <summary>
    /// Tries to get a given named argument value from an <see cref="AttributeData"/> instance, if present.
    /// </summary>
    /// <typeparam name="T">The type of argument to check.</typeparam>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
    /// <param name="name">The name of the argument to check.</param>
    /// <param name="value">The resulting argument value, if present.</param>
    /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with a valid value.</returns>
    public static bool TryGetNamedArgument<T>(this AttributeData attributeData, string name, out T? value)
    {
        if (TryGetNamedArgument(attributeData, name, out TypedConstant constantValue))
        {
            value = (T?)constantValue.Value;

            return true;
        }

        value = default;

        return false;
    }

    /// <summary>
    /// Tries to get a given named argument value from an <see cref="AttributeData"/> instance, if present.
    /// </summary>
    /// <param name="attributeData">The target <see cref="AttributeData"/> instance to check.</param>
    /// <param name="name">The name of the argument to check.</param>
    /// <param name="value">The resulting argument value, if present.</param>
    /// <returns>Whether or not <paramref name="attributeData"/> contains an argument named <paramref name="name"/> with a valid value.</returns>
    public static bool TryGetNamedArgument(this AttributeData attributeData, string name, out TypedConstant value)
    {
        foreach (KeyValuePair<string, TypedConstant> argument in attributeData.NamedArguments)
        {
            if (argument.Key == name)
            {
                value = argument.Value;

                return true;
            }
        }

        value = default;

        return false;
    }
}
