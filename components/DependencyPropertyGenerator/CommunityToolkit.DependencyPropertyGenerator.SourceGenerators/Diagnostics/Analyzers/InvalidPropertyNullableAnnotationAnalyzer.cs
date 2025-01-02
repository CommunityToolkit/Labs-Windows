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
/// A diagnostic analyzer that generates a warning when a property with <c>[GeneratedDependencyProperty]</c> would generate a nullability annotations violation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyNullableAnnotationAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        NonNullablePropertyDeclarationIsNotEnforced,
        NotNullResilientAccessorsForNullablePropertyDeclaration,
        NotNullResilientAccessorsForNullablePropertyDeclaration
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

            // Attempt to also get the '[MaybeNull]', '[NotNull]', '[AllowNull]' and '[DisallowNull]' symbols (there might be multiples, due to polyfills)
            ImmutableArray<INamedTypeSymbol> maybeNullAttributeSymbols = context.Compilation.GetTypesByMetadataName("System.Diagnostics.CodeAnalysis.MaybeNullAttribute");
            ImmutableArray<INamedTypeSymbol> notNullAttributeSymbols = context.Compilation.GetTypesByMetadataName("System.Diagnostics.CodeAnalysis.NotNullAttribute");
            ImmutableArray<INamedTypeSymbol> allowNullAttributeSymbols = context.Compilation.GetTypesByMetadataName("System.Diagnostics.CodeAnalysis.AllowNullAttribute");
            ImmutableArray<INamedTypeSymbol> disallowNullAttributeSymbols = context.Compilation.GetTypesByMetadataName("System.Diagnostics.CodeAnalysis.DisallowNullAttribute");

            context.RegisterSymbolAction(context =>
            {
                // Validate that we have a property that is of some type that could potentially become 'null'
                if (context.Symbol is not IPropertySymbol { Type.IsValueType: false, NullableAnnotation: not NullableAnnotation.None } propertySymbol)
                {
                    return;
                }

                // If the property is not using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // Handle nullable and non-null properties differently
                if (propertySymbol.NullableAnnotation is NullableAnnotation.Annotated)
                {
                    // If we don't have '[NotNull]', we'll never need to emit a diagnostic.
                    // That is, the default nullable state will always be correct already.
                    if (!propertySymbol.HasAttributeWithAnyType(notNullAttributeSymbols))
                    {
                        return;
                    }

                    // If we have '[NotNull]', it means the property getter must always ensure that a non-null value is returned.
                    // This can be achieved in two different ways:
                    //   1) By implementing one of the 'On___Get' methods, and adding '[NotNull]' on the parameter.
                    //   2) By having '[DisallowNull]' on the property or implementing one of the 'On___Set' methods with '[NotNull]'
                    //      on the parameter, and either marking the property as required, or providing a non-null default value.
                    if (!IsAccessorMethodMarkedAsNotNull(propertySymbol, SyntaxKind.GetAccessorDeclaration, notNullAttributeSymbols) &&
                        !((propertySymbol.HasAttributeWithAnyType(disallowNullAttributeSymbols) || IsAccessorMethodMarkedAsNotNull(propertySymbol, SyntaxKind.SetAccessorDeclaration, notNullAttributeSymbols)) &&
                          (propertySymbol.IsRequired || IsDefaultValueNotNull(propertySymbol, attributeData, maybeNullAttributeSymbols, notNullAttributeSymbols))))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            NotNullResilientAccessorsForNullablePropertyDeclaration,
                            attributeData.GetLocation(),
                            propertySymbol));
                    }
                }
                else
                {
                    // If the property is not nullable and it has '[MaybeNull]', we never need to emit a diagnostic.
                    // That is, setting 'null' is valid, and the initial state doesn't matter, as the return is nullable.
                    if (propertySymbol.HasAttributeWithAnyType(maybeNullAttributeSymbols))
                    {
                        return;
                    }

                    // If setting 'null' values is allowed, then the initial state (and the default value) don't matter anymore.
                    // In order to be correct, we must have '[NotNull]' on any implemented getter or setter methods (same as above).
                    if (propertySymbol.HasAttributeWithAnyType(allowNullAttributeSymbols))
                    {
                        if (!IsAccessorMethodMarkedAsNotNull(propertySymbol, SyntaxKind.GetAccessorDeclaration, notNullAttributeSymbols) &&
                            !IsAccessorMethodMarkedAsNotNull(propertySymbol, SyntaxKind.SetAccessorDeclaration, notNullAttributeSymbols))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                NotNullResilientAccessorsForNullablePropertyDeclaration,
                                attributeData.GetLocation(),
                                propertySymbol));
                        }
                    }
                    else
                    {
                        // Otherwise, we need to check that either the property is required, or that the default value is not 'null'.
                        // This is because when the nullability of the setter is correct, then the default value takes precedence.
                        if (!propertySymbol.IsRequired && !IsDefaultValueNotNull(propertySymbol, attributeData, maybeNullAttributeSymbols, notNullAttributeSymbols))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                NonNullablePropertyDeclarationIsNotEnforced,
                                attributeData.GetLocation(),
                                propertySymbol));
                        }
                    }
                }
            }, SymbolKind.Property);
        });
    }
    /// <summary>
    /// Checks whether a given generated accessor method has <c>[NotNull]</c> on its parameter.
    /// </summary>
    /// <param name="propertySymbol">The <see cref="IPropertySymbol"/> instance to inspect.</param>
    /// <param name="accessorKind">The syntax kind for the accessor method to look for.</param>
    /// <param name="notNullAttributeSymbols">The <see cref="INamedTypeSymbol"/> instances for <c>[NotNull]</c>.</param>
    /// <returns>Whether <paramref name="propertySymbol"/> has a generated accessor method with its parameter marked with <c>[NotNull]</c>.</returns>
    private static bool IsAccessorMethodMarkedAsNotNull(IPropertySymbol propertySymbol, SyntaxKind accessorKind, ImmutableArray<INamedTypeSymbol> notNullAttributeSymbols)
    {
        string suffix = accessorKind == SyntaxKind.GetAccessorDeclaration ? "Get" : "Set";

        foreach (ISymbol symbol in propertySymbol.ContainingType.GetMembers($"On{propertySymbol.Name}{suffix}"))
        {
            // We really only expect to match our own generated methods, but do some basic filtering just in case
            if (symbol is not IMethodSymbol { IsStatic: false, ReturnsVoid: true, Parameters: [{ Type: INamedTypeSymbol, RefKind: RefKind.Ref } propertyValue] })
            {
                continue;
            }

            // Check if the parameter has '[NotNull]' on it
            if (propertyValue.HasAttributeWithAnyType(notNullAttributeSymbols))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks whether a given property has a default value that is not <see langword="null"/>.
    /// </summary>
    /// <param name="propertySymbol">The <see cref="IPropertySymbol"/> instance to inspect.</param>
    /// <param name="attributeData">The <see cref="AttributeData"/> instance on <paramref name="propertySymbol"/>.</param>
    /// <param name="maybeNullAttributeSymbols">The <see cref="INamedTypeSymbol"/> instances for <c>[MaybeNull]</c>.</param>
    /// <param name="notNullAttributeSymbols">The <see cref="INamedTypeSymbol"/> instances for <c>[NotNull]</c>.</param>
    /// <returns></returns>
    private static bool IsDefaultValueNotNull(
        IPropertySymbol propertySymbol,
        AttributeData attributeData,
        ImmutableArray<INamedTypeSymbol> maybeNullAttributeSymbols,
        ImmutableArray<INamedTypeSymbol> notNullAttributeSymbols)
    {
        // If we have a default value, check that it's not 'null'
        if (attributeData.TryGetNamedArgument("DefaultValue", out TypedConstant defaultValue))
        {
            return !defaultValue.IsNull;
        }

        // If we have a callback, validate its return type
        if (attributeData.TryGetNamedArgument("DefaultValueCallback", out TypedConstant defaultValueCallback))
        {
            // Find the target method (same logic here as in the generator)
            if (defaultValueCallback is { Type.SpecialType: SpecialType.System_String, Value: string { Length: > 0 } methodName } &&
                InvalidPropertyDefaultValueCallbackTypeAnalyzer.TryFindDefaultValueCallbackMethod(propertySymbol, methodName, out IMethodSymbol? methodSymbol) &&
                InvalidPropertyDefaultValueCallbackTypeAnalyzer.IsDefaultValueCallbackValid(propertySymbol, methodSymbol))
            {
                // Verify that the return type can't possibly be 'null', including using attributes
                return
                    (methodSymbol.ReturnNullableAnnotation is NullableAnnotation.NotAnnotated && !methodSymbol.HasReturnAttributeWithAnyType(maybeNullAttributeSymbols)) ||
                    (methodSymbol.ReturnNullableAnnotation is NullableAnnotation.Annotated && methodSymbol.HasReturnAttributeWithAnyType(notNullAttributeSymbols));
            }
        }

        return false;
    }
}
