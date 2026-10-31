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
/// A diagnostic analyzer that generates an error when a property with <c>[GeneratedDependencyProperty]</c> is in an invalid type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyContainingTypeDeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [InvalidPropertyDeclarationContainingTypeIsNotDependencyObject];

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

            // Get the 'DependencyObject' symbol
            if (context.Compilation.GetTypeByMetadataName(WellKnownTypeNames.DependencyObject(useWindowsUIXaml)) is not { } dependencyObjectSymbol)
            {
                return;
            }

            context.RegisterSymbolAction(context =>
            {
                IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;

                // If the property is not using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // Emit the diagnostic if the target is not valid
                if (!IsDependencyObject(propertySymbol.ContainingType, dependencyObjectSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                       InvalidPropertyDeclarationContainingTypeIsNotDependencyObject,
                       attributeData.GetLocation(),
                       propertySymbol));
                }
            }, SymbolKind.Property);
        });
    }

    /// <summary>
    /// Checks whether a class derives from or implements the selected XAML framework's dependency object type.
    /// </summary>
    /// <param name="typeSymbol">The target <see cref="ITypeSymbol"/> instance to check.</param>
    /// <param name="dependencyObjectSymbol">The resolved dependency object type for the selected XAML framework.</param>
    /// <returns>Whether <paramref name="typeSymbol"/> can contain generated dependency properties.</returns>
    public static bool IsDependencyObject(ITypeSymbol typeSymbol, INamedTypeSymbol dependencyObjectSymbol)
    {
        if (typeSymbol is not { TypeKind: TypeKind.Class, IsStatic: false })
        {
            return false;
        }

        // WinUI 3, UWP, and Uno 7 expose 'DependencyObject' as a class
        if (dependencyObjectSymbol.TypeKind is not TypeKind.Interface)
        {
            return typeSymbol.InheritsFromType(dependencyObjectSymbol);
        }

        return typeSymbol.ImplementsInterface(dependencyObjectSymbol);
    }
}
