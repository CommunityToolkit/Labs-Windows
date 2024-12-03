// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates an error whenever <c>[GeneratedDependencyProperty]</c> is used with an invalid default value type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyDefaultValueTypeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        InvalidPropertyDefaultValueNull,
        InvalidPropertyDefaultValueType
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

            context.RegisterOperationAction(context =>
            {
                // We only care about attributes on properties
                if (context.ContainingSymbol is not IPropertySymbol propertySymbol)
                {
                    return;
                }

                // Make sure the attribute operation is valid, and that we can get the attribute type symbol
                if (context.Operation is not IAttributeOperation { Operation: IObjectCreationOperation { Type: INamedTypeSymbol attributeTypeSymbol } objectOperation })
                {
                    return;
                }

                // Filter out all attributes of other types
                if (!generatedDependencyPropertyAttributeSymbols.Contains(attributeTypeSymbol, SymbolEqualityComparer.Default))
                {
                    return;
                }

                // Also get the actual attribute data for '[GeneratedDependencyProperty]' (this should always succeed at this point)
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // Get the default value, if present (if it's not set, nothing to do)
                if (!attributeData.TryGetNamedArgument("DefaultValue", out TypedConstant defaultValue))
                {
                    return;
                }

                bool isNullableValueType = propertySymbol.Type is INamedTypeSymbol { IsValueType: true, IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };
                bool isNullableType = !propertySymbol.Type.IsValueType || isNullableValueType;

                // If the value is 'null', handle all possible cases:
                //   - Special placeholder for 'UnsetValue'
                //   - Explicit 'null' value
                if (defaultValue.IsNull)
                {
                    // Go through all named arguments of the attribute to look for 'UnsetValue'
                    foreach (IOperation argumentOperation in objectOperation.Initializer?.Initializers ?? [])
                    {
                        // We found its assignment: check if it's the 'UnsetValue' placeholder
                        if (argumentOperation is ISimpleAssignmentOperation { Value: IFieldReferenceOperation { Field: { Name: "UnsetValue" } fieldSymbol } })
                        {
                            // Validate that the reference is actually to the special placeholder
                            if (fieldSymbol.ContainingType!.HasFullyQualifiedMetadataName(WellKnownTypeNames.GeneratedDependencyProperty))
                            {
                                return;
                            }

                            // If it's not a match, we can just stop iterating: we know for sure the value is something else explicitly set
                            break;
                        }
                    }

                    // Warn if the value is not nullable
                    if (!isNullableType)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidPropertyDefaultValueNull,
                            attributeData.GetLocation(),
                            propertySymbol,
                            propertySymbol.Type));
                    }
                }
                else
                {
                    // Get the target type with a special case for 'Nullable<T>'
                    ITypeSymbol propertyTypeSymbol = isNullableValueType
                        ? ((INamedTypeSymbol)propertySymbol.Type).TypeArguments[0]
                        : propertySymbol.Type;

                    // Warn if the type of the default value is not compatible
                    if (!SymbolEqualityComparer.Default.Equals(propertyTypeSymbol, defaultValue.Type))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidPropertyDefaultValueType,
                            attributeData.GetLocation(),
                            propertySymbol,
                            propertySymbol.Type,
                            defaultValue.Value,
                            defaultValue.Type));
                    }
                }
            }, OperationKind.Attribute);
        });
    }
}
