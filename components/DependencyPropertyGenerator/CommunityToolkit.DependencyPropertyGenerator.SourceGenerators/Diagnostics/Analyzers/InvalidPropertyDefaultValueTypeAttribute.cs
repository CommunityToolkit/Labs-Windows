// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Threading;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates an error whenever <c>[GeneratedDependencyProperty]</c> is used with an invalid default value type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyDefaultValueTypeAttribute : DiagnosticAnalyzer
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
            ImmutableArray<INamedTypeSymbol> generatedDependencyPropertyAttributeSymbols = context.Compilation.GetTypesByMetadataName("CommunityToolkit.WinUI.GeneratedDependencyPropertyAttribute");

            context.RegisterSymbolAction(context =>
            {
                // We're intentionally only looking for properties here
                if (context.Symbol is not IPropertySymbol propertySymbol)
                {
                    return;
                }

                // If the property isn't using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // Get the default value, if present (if it's not set, nothing to do)
                if (!attributeData.TryGetNamedArgument("DefaultValue", out TypedConstant defaultValue))
                {
                    return;
                }

                // Skip 'UnsetValue', that's special

                bool isNullableValueType = propertySymbol.Type is { IsValueType: true } and not INamedTypeSymbol { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };
                bool isNullableType = !propertySymbol.Type.IsValueType || isNullableValueType;

                // Check for invalid 'null' default values
                if (defaultValue.IsNull && !isNullableType)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidPropertyDefaultValueNull,
                        attributeData.GetLocation(),
                        propertySymbol,
                        propertySymbol.Type));

                    return;
                }


                foreach (SyntaxReference propertyReference in propertySymbol.DeclaringSyntaxReferences)
                {
                    SyntaxNode propertyNode = propertyReference.GetSyntax(context.CancellationToken);

                    // if (!IsValidPropertyDeclaration(propertyNode))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            InvalidPropertyDeclaration,
                            attributeData.GetLocation(),
                            propertySymbol));

                        return;
                    }
                }
            }, SymbolKind.Property);
        });
    }

    internal static bool IsDependencyPropertyUnsetValue(
        AttributeData attributeData,
        SemanticModel semanticModel,
        CancellationToken token)
    {
        // If we do have a default value, we also want to check whether it's the special 'UnsetValue' placeholder.
        // To do so, we get the application syntax, find the argument, then get the operation and inspect it.
        if (attributeData.ApplicationSyntaxReference?.GetSyntax(token) is AttributeSyntax attributeSyntax)
        {
            foreach (AttributeArgumentSyntax attributeArgumentSyntax in attributeSyntax.ArgumentList?.Arguments ?? [])
            {
                // Let's see whether the current argument is the one that set the 'DefaultValue' property
                if (attributeArgumentSyntax.NameEquals?.Name.Identifier.Text is "DefaultValue")
                {
                    IOperation? operation = semanticModel.GetOperation(attributeArgumentSyntax.Expression, token);

                    // Double check that it's a constant field reference (it could also be a literal of some kind, etc.)
                    if (operation is IFieldReferenceOperation { Field: { Name: "UnsetValue" } fieldSymbol })
                    {
                        // Last step: we want to validate that the reference is actually to the special placeholder
                        if (fieldSymbol.ContainingType!.HasFullyQualifiedMetadataName("CommunityToolkit.WinUI.GeneratedDependencyProperty"))
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}
