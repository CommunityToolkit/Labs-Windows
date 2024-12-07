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
/// A diagnostic analyzer that generates a diagnostic whenever <c>[GeneratedDependencyProperty]</c> is used on a property with the 'Property' suffix in its name.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PropertyDeclarationWithPropertyNameSuffixAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [PropertyDeclarationWithPropertySuffix];

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

                // We only want to lookup the attribute if the property name actually ends with the 'Property' suffix
                if (!propertySymbol.Name.EndsWith("Property"))
                {
                    return;
                }

                // Emit a diagnostic if the property is using '[GeneratedDependencyProperty]'
                if (propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        PropertyDeclarationWithPropertySuffix,
                        attributeData.GetLocation(),
                        propertySymbol));
                }
            }, SymbolKind.Property);
        });
    }
}
