// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using CommunityToolkit.GeneratedDependencyProperty.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates an error when a property with <c>[GeneratedDependencyProperty]</c> is using invalid forwarded attributes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyForwardedAttributeDeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        InvalidDependencyPropertyTargetedAttributeType,
        InvalidDependencyPropertyTargetedAttributeTypeArgumentExpression
    ];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the XAML mode to use
            bool useWindowsUIXaml = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.GetMSBuildBooleanPropertyValue(WellKnownPropertyNames.DependencyPropertyGeneratorUseWindowsUIXaml);

            // Get the '[GeneratedDependencyProperty]' symbol (there might be multiples, due to embedded mode)
            ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute);

            // Get the 'DependencyObject' symbol
            if (context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.DependencyObject(useWindowsUIXaml)) is not { } dependencyObjectSymbol)
            {
                return;
            }

            context.RegisterSymbolStartAction(context =>
            {
                // Ensure that we have some target property to analyze (also skip implementation parts)
                if (context.Symbol is not IPropertySymbol { PartialDefinitionPart: null } propertySymbol)
                {
                    return;
                }

                // If the property is not using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                context.RegisterSyntaxNodeAction(context =>
                {
                    foreach (AttributeListSyntax attributeList in ((PropertyDeclarationSyntax)context.Node).AttributeLists)
                    {
                        // Only target attributes that would be forwarded, ignore all others
                        if (attributeList.Target?.Identifier is not SyntaxToken(SyntaxKind.StaticKeyword))
                        {
                            continue;
                        }

                        foreach (AttributeSyntax attribute in attributeList.Attributes)
                        {
                            // Emit a diagnostic (and stop here for this attribute) if we can't resolve the symbol for the attribute to forward
                            if (!context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken).TryGetAttributeTypeSymbol(out INamedTypeSymbol? attributeTypeSymbol))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(
                                    InvalidDependencyPropertyTargetedAttributeType,
                                    attribute.GetLocation(),
                                    propertySymbol,
                                    attribute.Name.ToFullString()));

                                continue;
                            }

                            IEnumerable<AttributeArgumentSyntax> attributeArguments = attribute.ArgumentList?.Arguments ?? [];

                            // Also emit a diagnostic if we fail to create the object model for the forwarded attribute
                            if (!AttributeInfo.TryCreate(attributeTypeSymbol, context.SemanticModel, attributeArguments, context.CancellationToken, out _))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(
                                    InvalidDependencyPropertyTargetedAttributeTypeArgumentExpression,
                                    attribute.GetLocation(),
                                    propertySymbol,
                                    attributeTypeSymbol));
                            }
                        }
                    }
                }, SyntaxKind.PropertyDeclaration);
            }, SymbolKind.Property);
        });
    }
}
