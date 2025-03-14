// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates an error whenever <c>[GeneratedDependencyProperty]</c> is used on an invalid property declaration.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertySyntaxDeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [InvalidPropertyDeclaration];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the '[GeneratedDependencyProperty]' symbol (there might be multiples, due to embedded mode)
            ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute);

            context.RegisterSymbolAction(context =>
            {
                IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;

                // If the property isn't using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // Check that the property has valid syntax
                foreach (SyntaxReference propertyReference in propertySymbol.DeclaringSyntaxReferences)
                {
                    SyntaxNode propertyNode = propertyReference.GetSyntax(context.CancellationToken);

                    if (!IsValidPropertyDeclaration(propertyNode))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidPropertyDeclaration,
                            attributeData.GetLocation(),
                            propertySymbol));

                        return;
                    }
                }
            }, SymbolKind.Property);
        });
    }

    /// <summary>
    /// Checks whether a given property declaration has valid syntax.
    /// </summary>
    /// <param name="node">The input node to validate.</param>
    /// <returns>Whether <paramref name="node"/> is a valid property.</returns>
    internal static bool IsValidPropertyDeclaration(SyntaxNode node)
    {
        // The node must be a property declaration with two accessors
        if (node is not PropertyDeclarationSyntax { AccessorList.Accessors: { Count: 2 } accessors, AttributeLists.Count: > 0 } property)
        {
            return false;
        }

        // The property must be partial (we'll check that it's a declaration from its symbol)
        if (!property.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            return false;
        }

        // Static properties are not supported
        if (property.Modifiers.Any(SyntaxKind.StaticKeyword))
        {
            return false;
        }

        // The accessors must be a get and a set (with any accessibility)
        if (accessors[0].Kind() is not (SyntaxKind.GetAccessorDeclaration or SyntaxKind.SetAccessorDeclaration) ||
            accessors[1].Kind() is not (SyntaxKind.GetAccessorDeclaration or SyntaxKind.SetAccessorDeclaration))
        {
            return false;
        }

        return true;
    }
}
