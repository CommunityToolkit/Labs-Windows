// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunityToolkit.Labs.Core.Generators
{
    internal static class GeneratorExtensions
    {
        internal static IEnumerable<INamedTypeSymbol> CrawlForAllNamedTypes(this INamespaceSymbol namespaceSymbol)
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
        /// Reconstructs an attribute instance as the given type.
        /// </summary>
        /// <typeparam name="T">The attribute type to create.</typeparam>
        /// <param name="attributeData">The attribute data used to construct the instance of <typeparamref name="T"/></param>
        /// <returns></returns>
        internal static T ReconstructAs<T>(this AttributeData attributeData)
        {
            // Reconstructing the attribute instance provides some safety against changes to the attribute's constructor signature.
            var attributeArgs = attributeData.ConstructorArguments.Select(PrepareTypeForActivator).ToArray();
            return (T)Activator.CreateInstance(typeof(T), attributeArgs);
        }

        internal static object? PrepareTypeForActivator(this TypedConstant typedConstant)
        {
            if (typedConstant.Type is null)
                throw new ArgumentNullException(nameof(typedConstant.Type));

            // Types prefixed with global:: do not work with Type.GetType and must be stripped away.
            var assemblyQualifiedName = typedConstant.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");

            var argType = Type.GetType(assemblyQualifiedName);

            // Enums arrive as the underlying integer type, which doesn't work as a param for Activator.CreateInstance()
            if (argType != null && typedConstant.Kind == TypedConstantKind.Enum)
                return Enum.Parse(argType, typedConstant.Value?.ToString());

            return typedConstant.Value;
        }
    }
}
