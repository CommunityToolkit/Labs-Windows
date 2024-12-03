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
/// A diagnostic analyzer that generates a warning when a property with <c>[GeneratedDependencyProperty]</c> would generate a nullability annotations violation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyNonNullableDeclarationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [NonNullablePropertyDeclarationIsNotEnforced];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the '[GeneratedDependencyProperty]' symbol (there might be multiples, due to embedded mode)
            ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute);

            // Attempt to also get the '[MaybeNull]' symbols (there might be multiples, due to polyfills)
            ImmutableArray<INamedTypeSymbol> maybeNullAttributeSymbol = context.Compilation.GetTypesByMetadataName("System.Diagnostics.CodeAnalysis.MaybeNullAttribute");

            context.RegisterSymbolAction(context =>
            {
                // Validate that we do have a property, and that it is of some type that can be explicitly nullable.
                // We're intentionally ignoring 'Nullable<T>' values here, since those are by defintiion nullable.
                // Additionally, we only care about properties that are explicitly marked as not nullable.
                // Lastly, we can skip required properties, since for those it's completely fine to be non-nullable.
                if (context.Symbol is not IPropertySymbol { Type.IsValueType: false, NullableAnnotation: NullableAnnotation.NotAnnotated, IsRequired: false } propertySymbol)
                {
                    return;
                }

                // If the property is not using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // If the property has '[MaybeNull]', we never need to emit a diagnostic
                if (propertySymbol.HasAttributeWithAnyType(maybeNullAttributeSymbol))
                {
                    return;
                }

                // Emit a diagnostic if there is no default value, or if it's 'null'
                if (!attributeData.TryGetNamedArgument("DefaultValue", out TypedConstant defaultValue) || defaultValue.IsNull)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        NonNullablePropertyDeclarationIsNotEnforced,
                        attributeData.GetLocation(),
                        propertySymbol));
                }
            }, SymbolKind.Property);
        });
    }
}
