// Licensnsed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Extensions;
using static CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that emits diagnostics for invalid service registrations.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidServiceRegistrationAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// The mapping of target attributes that will trigger the analyzer.
    /// </summary>
    private static readonly ImmutableDictionary<string, string> RegistrationAttributeNamesToFullyQualifiedNamesMap = ImmutableDictionary.CreateRange(new[]
    {
        new KeyValuePair<string, string>("SingletonAttribute", "CommunityToolkit.Extensions.DependencyInjection.SingletonAttribute"),
        new KeyValuePair<string, string>("TransientAttribute", "CommunityToolkit.Extensions.DependencyInjection.TransientAttribute"),
    });

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        InvalidRegistrationImplementationType,
        InvalidRegistrationServiceType,
        DuplicateImplementationTypeRegistration,
        DuplicateServiceTypeRegistration);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Try to get all necessary type symbols
            if (!context.Compilation.TryBuildNamedTypeSymbolMap(RegistrationAttributeNamesToFullyQualifiedNamesMap, out ImmutableDictionary<string, INamedTypeSymbol>? typeSymbols))
            {
                return;
            }

            // Register a callback for all methpds
            context.RegisterSymbolAction(context =>
            {
                IMethodSymbol methodSymbol = (IMethodSymbol)context.Symbol;

                HashSet<ISymbol> implementationTypeRegistrations = new(SymbolEqualityComparer.Default);
                HashSet<ISymbol> serviceTypeRegistrations = new(SymbolEqualityComparer.Default);

                foreach (AttributeData attributeData in context.Symbol.GetAttributes())
                {
                    // Go over each attribute on the target method and find the ones indicating a service registration
                    if (attributeData.AttributeClass is { Name: string attributeName } attributeClass &&
                        typeSymbols.TryGetValue(attributeName, out INamedTypeSymbol? attributeSymbol) &&
                        SymbolEqualityComparer.Default.Equals(attributeClass, attributeSymbol))
                    {
                        // Ensure the attribute arguments are present, and retrieve them
                        if (attributeData.ConstructorArguments is not [
                            { Kind: TypedConstantKind.Type, Value: ITypeSymbol implementationType },
                            { Kind: TypedConstantKind.Array, Values: ImmutableArray<TypedConstant> serviceTypes }])
                        {
                            continue;
                        }

                        // Check the implementation type is valid (note: not checking constructors just yet)
                        if (implementationType is not INamedTypeSymbol
                            {
                                TypeKind: TypeKind.Class,
                                IsStatic: false,
                                IsAbstract: false,
                                InstanceConstructors: [IMethodSymbol, ..] constructors
                            })
                        {
                            // The type is not valid, emit a diagnostic
                            context.ReportDiagnostic(Diagnostic.Create(
                                InvalidRegistrationImplementationType,
                                attributeData.GetLocation(),
                                implementationType));
                        }

                        // Check if the implementation type has not been seen before
                        if (!implementationTypeRegistrations.Add(implementationType))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(
                                DuplicateImplementationTypeRegistration,
                                attributeData.GetLocation(),
                                implementationType));
                        }

                        // If no service types are present, the service is registered as the implementation type
                        if (serviceTypes.IsEmpty)
                        {
                            // Since we're tracking all registered service types, we need to track that case as well.
                            // This covers cases such as:
                            //
                            // [Singleton(typeof(A))]
                            // [Singleton(typeof(BDerivesFromA), typeof(A), typeof(IB))]
                            //
                            // That is, the first attribute will trigger this code path and the implementation type
                            // will be registered, and the second attribute will go through the explicit list of
                            // service types to register, see A being present again, and correctly emit the diagnostic.
                            if (!serviceTypeRegistrations.Add(implementationType))
                            {
                                context.ReportDiagnostic(Diagnostic.Create(
                                    DuplicateServiceTypeRegistration,
                                    attributeData.GetLocation(),
                                    implementationType));
                            }
                        }
                        else
                        {
                            // Go over all declared service types and validate them as well
                            foreach (TypedConstant serviceType in serviceTypes)
                            {
                                // For a service type to be valid, there has to be an implicit or identity conversion between that and the service type
                                if (serviceType.Value is not INamedTypeSymbol { TypeKind: TypeKind.Class or TypeKind.Interface, IsStatic: false } targetServiceType ||
                                    context.Compilation.ClassifyCommonConversion(implementationType, targetServiceType) is not ({ IsIdentity: true } or { IsImplicit: true }))
                                {
                                    // The service type is not valid, emit a diagnostic
                                    context.ReportDiagnostic(Diagnostic.Create(
                                        InvalidRegistrationServiceType,
                                        attributeData.GetLocation(),
                                        implementationType,
                                        serviceType.Value));
                                }

                                // Check if the service type has not been seen before
                                if (serviceType.Value is ITypeSymbol typeSymbol &&
                                    !serviceTypeRegistrations.Add(typeSymbol))
                                {
                                    context.ReportDiagnostic(Diagnostic.Create(
                                       DuplicateServiceTypeRegistration,
                                       attributeData.GetLocation(),
                                       typeSymbol));
                                }
                            }
                        }
                    }
                }
            }, SymbolKind.Method);
        });
    }
}
