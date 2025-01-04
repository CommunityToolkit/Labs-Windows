// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates an error when using <c>[GeneratedDependencyProperty]</c> without the right C# version.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnsupportedCSharpLanguageVersionAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        PropertyDeclarationRequiresCSharp13,
        LocalCachingRequiresCSharpPreview
    ];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // If we're using C# 'preview', we'll never emit any errors
            if (context.Compilation.IsLanguageVersionPreview())
            {
                return;
            }

            // Get the '[GeneratedDependencyProperty]' symbol (there might be multiples, due to embedded mode)
            ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute);

            context.RegisterSymbolAction(context =>
            {
                IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;

                // If the property is not using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                bool isLocalCachingEnabled = attributeData.GetNamedArgument("IsLocalCacheEnabled", defaultValue: false);

                // Emit only up to one diagnostic, for whichever the highest required C# version would be
                if (isLocalCachingEnabled && !context.Compilation.IsLanguageVersionPreview())
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        LocalCachingRequiresCSharpPreview,
                        attributeData.GetLocation(),
                        propertySymbol));
                }
                else if (!isLocalCachingEnabled && !context.Compilation.HasLanguageVersionAtLeastEqualTo(LanguageVersion.CSharp13))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        PropertyDeclarationRequiresCSharp13,
                        attributeData.GetLocation(),
                        propertySymbol));
                }
            }, SymbolKind.Property);
        });
    }
}
