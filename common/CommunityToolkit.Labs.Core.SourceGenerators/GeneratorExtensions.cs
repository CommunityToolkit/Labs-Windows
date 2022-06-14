// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CommunityToolkit.Labs.Core.SourceGenerators;

public static class GeneratorExtensions
{
    /// <summary>
    /// Crawls a namespace and all child namespaces for all contained types.
    /// </summary>
    /// <returns>A flattened enumerable of <see cref="INamedTypeSymbol"/>s.</returns>
    public static IEnumerable<INamedTypeSymbol> CrawlForAllNamedTypes(this INamespaceSymbol namespaceSymbol)
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            if (member is INamespaceSymbol nestedNamespace)
            {
                foreach (var item in CrawlForAllNamedTypes(nestedNamespace))
                    yield return item;
            }

            if (member is INamedTypeSymbol typeSymbol)
                yield return typeSymbol;
        }
    }

    /// <summary>
    /// Crawls an object tree for nested properties of the same type and returns the first instance that matches the <paramref name="filterPredicate"/>. 
    /// </summary>
    /// <remarks>
    /// Does not filter against or return the <paramref name="root"/> object.
    /// </remarks>
    public static T? CrawlBy<T>(this T? root, Func<T?, T?> selectPredicate, Func<T?, bool> filterPredicate)
    {
        crawl:
        var current = selectPredicate(root);

        if (filterPredicate(current))
        {
            return current;
        }

        if (current is null)
        {
            return default;
        }

        root = current;
        goto crawl;
    }

    /// <summary>
    /// Reconstructs an attribute instance as the given type.
    /// </summary>
    /// <typeparam name="T">The attribute type to create.</typeparam>
    /// <param name="attributeData">The attribute data used to construct the instance of <typeparamref name="T"/></param>
    public static T ReconstructAs<T>(this AttributeData attributeData)
    {
        // Reconstructing the attribute instance provides some safety against changes to the attribute's constructor signature.
        var attributeArgs = attributeData.ConstructorArguments.Select(PrepareParameterTypeForActivator).ToArray();
        return (T)Activator.CreateInstance(typeof(T), attributeArgs);
    }


    /// <summary>
    /// Attempts to reconstruct an attribute instance as the given type, returning null if <paramref name="attributeData"/> and <typeparamref name="T"/> are mismatched.
    /// </summary>
    /// <typeparam name="T">The attribute type to create.</typeparam>
    /// <param name="attributeData">The attribute data used to construct the instance of <typeparamref name="T"/></param>
    public static T? TryReconstructAs<T>(this AttributeData attributeData)
        where T : Attribute
    {
        var attributeMatchesType = attributeData.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == $"global::{typeof(T).FullName}";

        if (attributeMatchesType)
            return attributeData.ReconstructAs<T>();

        return null;
    }

    /// <summary>
    /// Checks whether or not a given type symbol has a specified full name.
    /// </summary>
    /// <param name="symbol">The input <see cref="ISymbol"/> instance to check.</param>
    /// <param name="name">The full name to check.</param>
    /// <returns>Whether <paramref name="symbol"/> has a full name equals to <paramref name="name"/>.</returns>
    public static bool HasFullyQualifiedName(this ISymbol symbol, string name)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == name;
    }

    /// <summary>
    /// Performs any data transforms needed for using <paramref name="parameterTypedConstant"/> as a parameter in <see cref="Activator.CreateInstance(Type, object[])"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException">The <paramref name="parameterTypedConstant"/>'s <see cref="TypedConstant.Type"/> was null.</exception>
    public static object? PrepareParameterTypeForActivator(this TypedConstant parameterTypedConstant)
    {
        if (parameterTypedConstant.Type is null)
            throw new ArgumentNullException(nameof(parameterTypedConstant.Type));

        // Types prefixed with global:: do not work with Type.GetType and must be stripped away.
        var assemblyQualifiedName = parameterTypedConstant.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "")
            .Replace("string", "System.String");

        var argType = Type.GetType(assemblyQualifiedName);

        // Enums arrive as the underlying integer type, which doesn't work as a param for Activator.CreateInstance()
        if (argType != null && parameterTypedConstant.Kind == TypedConstantKind.Enum)
            return Enum.Parse(argType, parameterTypedConstant.Value?.ToString());

        if (parameterTypedConstant.Kind == TypedConstantKind.Array)
        {
            // Cannot use actual value to get item type b/c array can be empty.
            var paramAssemblyQualifiedName = parameterTypedConstant.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("[]", "")
                .Replace("global::", "")
                .Replace("string", "System.String");

            var paramArgType = paramAssemblyQualifiedName is null ? null : Type.GetType(paramAssemblyQualifiedName);

            // Prepare each value in the array.
            var arr = parameterTypedConstant.Values.Select(PrepareParameterTypeForActivator).ToArray();

            // This code path will always return object?[]
            if (paramArgType is null)
                return arr;

            // Prepare the array as the correct type.
            var result = Array.CreateInstance(paramArgType, arr.Length);
            arr.CopyTo(result, 0);

            return result;
        }

        if (argType is not null)
            return Convert.ChangeType(parameterTypedConstant.Value, argType);

        return parameterTypedConstant.Value;
    }
}
