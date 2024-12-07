// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A diagnostic analyzer that generates an error whenever <c>[GeneratedDependencyProperty]</c> is used with an invalid default value callback argument.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidPropertyDefaultValueCallbackTypeAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
    [
        InvalidPropertyDeclarationDefaultValueCallbackMixed,
        InvalidPropertyDeclarationDefaultValueCallbackNoMethodFound,
        InvalidPropertyDeclarationDefaultValueCallbackInvalidMethod
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
                IPropertySymbol propertySymbol = (IPropertySymbol)context.Symbol;

                // If the property is not using '[GeneratedDependencyProperty]', there's nothing to do
                if (!propertySymbol.TryGetAttributeWithAnyType(generatedDependencyPropertyAttributeSymbols, out AttributeData? attributeData))
                {
                    return;
                }

                // If 'DefaultValueCallback' is not set, there's nothing to do
                if (!attributeData.TryGetNamedArgument("DefaultValueCallback", out string? defaultValueCallback))
                {
                    return;
                }

                // Emit a diagnostic if 'DefaultValue' is also being set
                if (attributeData.TryGetNamedArgument("DefaultValue", out _))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidPropertyDeclarationDefaultValueCallbackMixed,
                        attributeData.GetLocation(),
                        propertySymbol));
                }

                // If 'DefaultValueCallback' is 'null', ignore it (Roslyn will already warn here)
                if (defaultValueCallback is null)
                {
                    return;
                }

                // Emit a diagnostic if we can't find a candidate method
                if (!TryFindDefaultValueCallbackMethod(propertySymbol, defaultValueCallback, out IMethodSymbol? methodSymbol))
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidPropertyDeclarationDefaultValueCallbackNoMethodFound,
                        attributeData.GetLocation(),
                        propertySymbol,
                        defaultValueCallback));
                }
                else if (!IsDefaultValueCallbackValid(propertySymbol, methodSymbol))
                {
                    // Emit a diagnostic if the candidate method is not valid
                    context.ReportDiagnostic(Diagnostic.Create(
                        InvalidPropertyDeclarationDefaultValueCallbackInvalidMethod,
                        attributeData.GetLocation(),
                        propertySymbol,
                        defaultValueCallback));
                }

            }, SymbolKind.Property);
        });
    }

    /// <summary>
    /// Tries to find a candidate default value callback method for a given property.
    /// </summary>
    /// <param name="propertySymbol">The <see cref="IPropertySymbol"/> currently being targeted by the analyzer.</param>
    /// <param name="methodName">The name of the default value callback method to look for.</param>
    /// <param name="methodSymbol">The <see cref="IMethodSymbol"/> for the resulting default value callback candidate method, if found.</param>
    /// <returns>Whether <paramref name="methodSymbol"/> could be found.</returns>
    public static bool TryFindDefaultValueCallbackMethod(IPropertySymbol propertySymbol, string methodName, [NotNullWhen(true)] out IMethodSymbol? methodSymbol)
    {
        ImmutableArray<ISymbol> memberSymbols = propertySymbol.ContainingType!.GetMembers(methodName);

        foreach (ISymbol member in memberSymbols)
        {
            // Ignore all other member types
            if (member is not IMethodSymbol candidateSymbol)
            {
                continue;
            }

            // Match the exact method name too
            if (candidateSymbol.Name == methodName)
            {
                methodSymbol = candidateSymbol;

                return true;
            }
        }

        methodSymbol = null;

        return false;
    }

    /// <summary>
    /// Checks whether a given default value callback method is valid for a given property.
    /// </summary>
    /// <param name="propertySymbol">The <see cref="IPropertySymbol"/> currently being targeted by the analyzer.</param>
    /// <param name="methodSymbol">The <see cref="IMethodSymbol"/> for the candidate default value callback method to validate.</param>
    /// <returns>Whether <paramref name="methodSymbol"/> is a valid default value callback method for <paramref name="propertySymbol"/>.</returns>
    public static bool IsDefaultValueCallbackValid(IPropertySymbol propertySymbol, IMethodSymbol methodSymbol)
    {
        // We need methods which are static and with no parameters (and that are not explicitly implemented)
        if (methodSymbol is not { IsStatic: true, Parameters: [], ExplicitInterfaceImplementations: [] })
        {
            return false;
        }

        // We have a candidate, now we need to match the return type. First,
        // we just check whether the return is 'object', or an exact match.
        if (methodSymbol.ReturnType.SpecialType is SpecialType.System_Object ||
            SymbolEqualityComparer.Default.Equals(propertySymbol.Type, methodSymbol.ReturnType))
        {
            return true;
        }

        bool isNullableValueType = propertySymbol.Type is INamedTypeSymbol { IsValueType: true, IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T };

        // Otherwise, try to see if the return is the type argument of a nullable value type
        if (isNullableValueType &&
            methodSymbol.ReturnType.TypeKind is TypeKind.Struct &&
            SymbolEqualityComparer.Default.Equals(((INamedTypeSymbol)propertySymbol.Type).TypeArguments[0], methodSymbol.ReturnType))
        {
            return true;
        }

        return false;
    }
}
