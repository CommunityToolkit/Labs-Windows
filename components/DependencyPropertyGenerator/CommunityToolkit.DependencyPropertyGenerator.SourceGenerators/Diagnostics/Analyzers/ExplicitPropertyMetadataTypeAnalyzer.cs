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
/// A diagnostic analyzer that generates diagnostics when using <c>[GeneratedDependencyProperty]</c> with 'PropertyType' set incorrectly.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExplicitPropertyMetadataTypeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        UnnecessaryDependencyPropertyExplicitMetadataType,
        IncompatibleDependencyPropertyExplicitMetadataType
    ];

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

                // If an explicit property type isn't set, there's nothing to do
                if (!attributeData.TryGetNamedArgument("PropertyType", out TypedConstant propertyType))
                {
                    return;
                }

                // Special case for 'null': this will already warn due to nullability annotations, nothing more to do here either.
                // This will also catch invalid types, which Roslyn will already emit errors for (so we can ignore them as well).
                if (propertyType is not { Kind: TypedConstantKind.Type, IsNull: false, Value: ITypeSymbol typeSymbol })
                {
                    return;
                }

                // If the explicit type matches the property type, then it's unnecessary, so we can warn and stop here
                if (SymbolEqualityComparer.Default.Equals(propertySymbol.Type, typeSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        UnnecessaryDependencyPropertyExplicitMetadataType,
                        attributeData.GetLocation(),
                        propertySymbol,
                        propertySymbol.Type));

                    return;
                }

                // If the explicit type is not compatible (i.e. there's no implicit conversion and the type is not the underlying nullable type), emit an error.
                // We also emit the same diagnostic if the explicit type is the nullable version of the property type, as that is not a supported scenario.
                bool isPropertyTypeIncompatible =
                    typeSymbol.IsNullableValueTypeWithUnderlyingType(propertySymbol.Type) ||
                    (!context.Compilation.HasImplicitConversion(propertySymbol.Type, typeSymbol)) &&
                     !propertySymbol.Type.IsNullableValueTypeWithUnderlyingType(typeSymbol);

                // Special case: we want to also block incompatible assignments that would have an implicit conversion (eg. 'float' -> 'double')
                if (propertySymbol.Type.IsValueType &&
                    typeSymbol.IsValueType &&
                    !propertySymbol.Type.IsNullableValueType() &&
                    !typeSymbol.IsNullableValueType() &&
                    !SymbolEqualityComparer.Default.Equals(propertySymbol.Type, typeSymbol))
                {
                    isPropertyTypeIncompatible = true;
                }

                if (isPropertyTypeIncompatible)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        IncompatibleDependencyPropertyExplicitMetadataType,
                        attributeData.GetLocation(),
                        propertySymbol,
                        typeSymbol,
                        propertySymbol.Type));
                }
            }, SymbolKind.Property);
        });
    }
}
