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
/// A diagnostic analyzer that generates a warning whenever a dependency property is declared as a property.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseFieldDeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [DependencyPropertyFieldDeclaration];

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

            // Check whether the current project is a WinRT component (modern .NET uses CsWinRT, legacy .NET produces .winmd files directly)
            bool isWinRTComponent =
                context.Options.AnalyzerConfigOptionsProvider.GlobalOptions.GetMSBuildBooleanPropertyValue(WellKnownPropertyNames.CsWinRTComponent) ||
                context.Compilation.Options.OutputKind is OutputKind.WindowsRuntimeMetadata;

            context.RegisterSymbolAction(context =>
            {
                IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;

                // We only care about properties which are of type 'DependencyProperty'
                if (!SymbolEqualityComparer.Default.Equals(propertySymbol.Type, dependencyPropertySymbol))
                {
                    return;
                }

                // If the property is an explicit interface implementation, allow it
                if (propertySymbol.ExplicitInterfaceImplementations.Length > 0)
                {
                    return;
                }

                // Next, make sure this property isn't (implicitly) implementing any interface properties.
                // If that's the case, we'll also allow it, as otherwise fixing this would break things.
                foreach (INamedTypeSymbol interfaceSymbol in propertySymbol.ContainingType.AllInterfaces)
                {
                    // Go over all properties (we can filter to just those with the same name) in each interface
                    foreach (IPropertySymbol interfacePropertySymbol in interfaceSymbol.GetMembers(propertySymbol.Name).OfType<IPropertySymbol>())
                    {
                        // The property must have the same type to possibly be an interface implementation
                        if (!SymbolEqualityComparer.Default.Equals(interfacePropertySymbol.Type, propertySymbol.Type))
                        {
                            continue;
                        }

                        // If the property is not implemented at all, ignore it
                        if (propertySymbol.ContainingType.FindImplementationForInterfaceMember(interfacePropertySymbol) is not IPropertySymbol implementationSymbol)
                        {
                            continue;
                        }

                        // If the current property is the one providing the implementation, then we allow it and stop here
                        if (SymbolEqualityComparer.Default.Equals(implementationSymbol, propertySymbol))
                        {
                            return;
                        }
                    }
                }

                // Make an exception for WinRT components: in this case declaring properties is valid, as they're needed for WinRT
                if (isWinRTComponent && propertySymbol.GetEffectiveAccessibility() is Accessibility.Public)
                {
                    return;
                }

                // At this point, we know for sure the property isn't valid, so emit a diagnostic
                context.ReportDiagnostic(Diagnostic.Create(
                    DependencyPropertyFieldDeclaration,
                    propertySymbol.Locations.First(),
                    propertySymbol));
            }, SymbolKind.Property);
        });
    }
}
