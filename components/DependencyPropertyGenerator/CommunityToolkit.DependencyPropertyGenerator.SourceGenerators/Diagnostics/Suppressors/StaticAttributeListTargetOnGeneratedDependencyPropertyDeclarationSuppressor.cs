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
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.SuppressionDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// <para>
/// A diagnostic suppressor to suppress CS0658 warnings for properties with [GeneratedDependencyProperty] using a [static:] attribute list.
/// </para>
/// <para>
/// That is, this diagnostic suppressor will suppress the following diagnostic:
/// <code>
/// public partial class MyControl : Control
/// {
///     [GeneratedDependencyProperty]
///     [static: JsonIgnore]
///     public partial string? Name { get; set; }
/// }
/// </code>
/// </para>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class StaticAttributeListTargetOnGeneratedDependencyPropertyDeclarationSuppressor : DiagnosticSuppressor
{
    /// <inheritdoc/>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } = [StaticPropertyAttributeListForGeneratedDependencyPropertyDeclaration];

    /// <inheritdoc/>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        // Get the '[GeneratedDependencyProperty]' symbol (there might be multiples, due to embedded mode)
        ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute);

        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SyntaxNode? syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

            // Check that the target is effectively [static:] over a property declaration, which is the only case we are interested in
            if (syntaxNode is AttributeTargetSpecifierSyntax { Parent.Parent: PropertyDeclarationSyntax propertyDeclaration, Identifier: SyntaxToken(SyntaxKind.StaticKeyword) })
            {
                SemanticModel semanticModel = context.GetSemanticModel(syntaxNode.SyntaxTree);

                // Get the property symbol from the property declaration
                ISymbol? declaredSymbol = semanticModel.GetDeclaredSymbol(propertyDeclaration, context.CancellationToken);

                // Check if the property is using [GeneratedDependencyProperty], in which case we should suppress the warning
                if (declaredSymbol is IPropertySymbol propertySymbol && propertySymbol.HasAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols))
                {
                    context.ReportSuppression(Suppression.Create(StaticPropertyAttributeListForGeneratedDependencyPropertyDeclaration, diagnostic));
                }
            }
        }
    }
}
