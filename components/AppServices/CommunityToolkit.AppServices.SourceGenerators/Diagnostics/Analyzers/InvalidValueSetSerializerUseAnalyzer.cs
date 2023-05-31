// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using static CommunityToolkit.AppServices.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.AppServices.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that emits diagnostics whenever a value set serializer type is not valid.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidValueSetSerializerUseAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(InvalidValueSetSerializerType, InvalidValueSetSerializerLocation);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        // Register a callback for all named type symbols (ie. user defined types)
        context.RegisterSyntaxNodeAction(static context =>
        {
            // Try to get the associated symbol for the current node. There are two cases we are interested in:
            //   - int Foo([Attribute] int bar), ie. an attribute on a parameter node
            //   - [return: Attribute] int Foo(), ie. an attribute on a return value
            ISymbol? associatedSymbol = context.Node switch
            {
                { Parent.Parent: ParameterSyntax parameter }
                    => context.SemanticModel.GetDeclaredSymbol(parameter, context.CancellationToken),
                { Parent: AttributeListSyntax { Target.Identifier: SyntaxToken(SyntaxKind.ReturnKeyword), Parent: MethodDeclarationSyntax method } }
                    => context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken),
                _ => null
            };

            // We're only looking for methods and parameters
            if (associatedSymbol is not (IMethodSymbol or IParameterSymbol))
            {
                return;
            }

            // We only care about cases where [ValueSetSerializer] is used
            if (!associatedSymbol.TryGetValueSetSerializerTypeFromAttribute(out INamedTypeSymbol? serializerType))
            {
                return;
            }

            // Validate the attribute location
            if ((associatedSymbol as IMethodSymbol ?? ((IParameterSymbol)associatedSymbol).ContainingSymbol) is not IMethodSymbol methodSymbol ||
                methodSymbol.ContainingType is not INamedTypeSymbol { TypeKind: TypeKind.Interface } interfaceSymbol ||
                !interfaceSymbol.TryGetAppServicesNameFromAttribute(out _))
            {
                // If the location is invalid, the attribute has no effect, so we emit this diagnostic and just stop here
                context.ReportDiagnostic(Diagnostic.Create(InvalidValueSetSerializerLocation, context.Node.GetLocation(), associatedSymbol));

                return;
            }

            // Look for a public parameterless constructor
            foreach (IMethodSymbol constructor in serializerType.InstanceConstructors)
            {
                if (constructor is { DeclaredAccessibility: Accessibility.Public, Parameters.IsEmpty: true })
                {
                    return;
                }
            }

            // The type isn't valid, emit a diagnostic
            context.ReportDiagnostic(Diagnostic.Create(InvalidValueSetSerializerType, context.Node.GetLocation(), serializerType));
        }, ImmutableArray.Create(SyntaxKind.Attribute));
    }
}
