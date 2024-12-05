// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates an error when a property with <c>[GeneratedDependencyProperty]</c> would generate conflicts.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyConflictingDeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [InvalidPropertyDeclarationWouldCauseConflicts];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the XAML mode to use
            bool useWindowsUIXaml = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.GetMSBuildBooleanPropertyValue(WellKnownPropertyNames.DependencyPropertyGeneratorUseWindowsUIXaml);

            // Get the '[GeneratedDependencyProperty]' symbol (there might be multiples, due to embedded mode)
            ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute);

            // Get the 'DependencyPropertyChangedEventArgs' symbol
            if (context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.DependencyPropertyChangedEventArgs(useWindowsUIXaml)) is not { } dependencyPropertyChangedEventArgsSymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                // Validate that we do have a property
                if (context.Symbol is not IPropertySymbol propertySymbol)
                {
                    return;
                }

                // If the property is not using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // Same logic as 'IsCandidateSymbolValid' in the generator
                if (propertySymbol.Name == "Property")
                {
                    // Check for collisions with the generated helpers and the property, only happens with these 2 types
                    if (propertySymbol.Type.SpecialType == SpecialType.System_Object ||
                        SymbolEqualityComparer.Default.Equals(propertySymbol.Type, dependencyPropertyChangedEventArgsSymbol))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidPropertyDeclarationWouldCauseConflicts,
                            attributeData.GetLocation(),
                            propertySymbol));
                    }
                }
            }, SymbolKind.Property);
        });
    }
}
