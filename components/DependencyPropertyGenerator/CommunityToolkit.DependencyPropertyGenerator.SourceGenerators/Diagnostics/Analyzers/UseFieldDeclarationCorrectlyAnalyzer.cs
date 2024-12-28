// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates a warning whenever a dependency property is declared as an incorrect field.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseFieldDeclarationCorrectlyAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [IncorrectDependencyPropertyFieldDeclaration];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the XAML mode to use
            bool useWindowsUIXaml = context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.GetMSBuildBooleanPropertyValue(WellKnownPropertyNames.DependencyPropertyGeneratorUseWindowsUIXaml);

            // Get the 'DependencyProperty' symbol
            if (context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.DependencyProperty(useWindowsUIXaml)) is not { } dependencyPropertySymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                IFieldSymbol fieldSymbol = (IFieldSymbol)context.Symbol;

                // We only care about fields which are of type 'DependencyProperty'
                if (!SymbolEqualityComparer.Default.Equals(fieldSymbol.Type, dependencyPropertySymbol))
                {
                    return;
                }

                // Fields should always be public, static, readonly, and with nothing else on them
                if (fieldSymbol is
                    { DeclaredAccessibility: not Accessibility.Public } or
                    { IsStatic: false } or
                    { IsRequired: true } or
                    { IsReadOnly: false } or
                    { IsVolatile: true } or
                    { NullableAnnotation: NullableAnnotation.Annotated })
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        IncorrectDependencyPropertyFieldDeclaration,
                        fieldSymbol.Locations.FirstOrDefault(),
                        fieldSymbol));
                }
            }, SymbolKind.Field);
        });
    }
}
