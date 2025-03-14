// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.GeneratedDependencyProperty.Models;

/// <summary>
/// A model representing an attribute declaration.
/// </summary>
/// <param name="TypeName">The type name of the attribute.</param>
/// <param name="ConstructorArgumentInfo">The <see cref="TypedConstantInfo"/> values for all constructor arguments for the attribute.</param>
/// <param name="NamedArgumentInfo">The <see cref="TypedConstantInfo"/> values for all named arguments for the attribute.</param>
internal sealed record AttributeInfo(
    string TypeName,
    EquatableArray<(string? Name, TypedConstantInfo Value)> ConstructorArgumentInfo,
    EquatableArray<(string Name, TypedConstantInfo Value)> NamedArgumentInfo)
{
    /// <summary>
    /// Creates a new <see cref="AttributeInfo"/> instance from a given syntax node.
    /// </summary>
    /// <param name="typeSymbol">The symbol for the attribute type.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current run.</param>
    /// <param name="arguments">The sequence of <see cref="AttributeArgumentSyntax"/> instances to process.</param>
    /// <param name="token">The cancellation token for the current operation.</param>
    /// <param name="info">The resulting <see cref="AttributeInfo"/> instance, if available</param>
    /// <returns>Whether a resulting <see cref="AttributeInfo"/> instance could be created.</returns>
    public static bool TryCreate(
        INamedTypeSymbol typeSymbol,
        SemanticModel semanticModel,
        IEnumerable<AttributeArgumentSyntax> arguments,
        CancellationToken token,
        [NotNullWhen(true)] out AttributeInfo? info)
    {
        string typeName = typeSymbol.GetFullyQualifiedName();

        using ImmutableArrayBuilder<(string?, TypedConstantInfo)> constructorArguments = new();
        using ImmutableArrayBuilder<(string, TypedConstantInfo)> namedArguments = new();

        foreach (AttributeArgumentSyntax argument in arguments)
        {
            // The attribute expression has to have an available operation to extract information from
            if (semanticModel.GetOperation(argument.Expression, token) is not IOperation operation)
            {
                continue;
            }

            // Try to get the info for the current argument
            if (!TypedConstantInfo.TryCreate(operation, semanticModel, argument.Expression, token, out TypedConstantInfo? argumentInfo))
            {
                info = null;

                return false;
            }

            // Try to get the identifier name if the current expression is a named argument expression. If it
            // isn't, then the expression is a normal attribute constructor argument, so no extra work is needed.
            if (argument.NameEquals is { Name.Identifier.ValueText: string nameEqualsName })
            {
                namedArguments.Add((nameEqualsName, argumentInfo));
            }
            else if (argument.NameColon is { Name.Identifier.ValueText: string nameColonName })
            {
                // This special case also handles named constructor parameters (i.e. '[Test(value: 42)]', not '[Test(Value = 42)]')
                constructorArguments.Add((nameColonName, argumentInfo));
            }
            else
            {
                constructorArguments.Add((null, argumentInfo));
            }
        }

        info = new AttributeInfo(
            typeName,
            constructorArguments.ToImmutable(),
            namedArguments.ToImmutable());

        return true;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        // Helper to format constructor parameters
        static AttributeArgumentSyntax CreateConstructorArgument(string? name, TypedConstantInfo value)
        {
            AttributeArgumentSyntax argument = AttributeArgument(ParseExpression(value.ToString()));

            // The name color expression is not guaranteed to be present (in fact, it's more common for it to be missing)
            if (name is not null)
            {
                argument = argument.WithNameColon(NameColon(IdentifierName(name)));
            }

            return argument;
        }

        // Gather the constructor arguments
        IEnumerable<AttributeArgumentSyntax> arguments =
            ConstructorArgumentInfo
            .Select(static arg => CreateConstructorArgument(arg.Name, arg.Value));

        // Gather the named arguments
        IEnumerable<AttributeArgumentSyntax> namedArguments =
            NamedArgumentInfo.Select(static arg =>
                AttributeArgument(ParseExpression(arg.Value.ToString()))
                .WithNameEquals(NameEquals(IdentifierName(arg.Name))));

        // Get the attribute to emit
        AttributeSyntax attributeDeclaration = Attribute(IdentifierName(TypeName), AttributeArgumentList(SeparatedList(arguments.Concat(namedArguments))));

        return attributeDeclaration.NormalizeWhitespace(eol: "\n").ToFullString();
    }
}
