// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using CommunityToolkit.AppServices.SourceGenerators.Models;
using static CommunityToolkit.AppServices.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.AppServices.SourceGenerators;

/// <summary>
/// A diagnostic analyzer that emits diagnostics whenever an app service has an invalid member.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidAppServicesMemberAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(
        InvalidAppServicesMemberType,
        InvalidAppServicesMethodReturnType,
        InvalidAppServicesMethodParameterType,
        InvalidRepeatedAppServicesMethodIProgressParameter,
        InvalidRepeatedAppServicesMethodCancellationTokenParameter);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            // Get the symbol for [AppService]
            if (context.Compilation.GetTypeByMetadataName("CommunityToolkit.AppServices.AppServiceAttribute") is not INamedTypeSymbol appServicesAttributeSymbol)
            {
                return;
            }

            // Register a callback for all named type symbols (ie. user defined types)
            context.RegisterSymbolAction(context =>
            {
                // The symbol must be an interface
                if (context.Symbol is not INamedTypeSymbol { TypeKind: TypeKind.Interface } interfaceSymbol)
                {
                    return;
                }

                // Check whether the interface is an app services interface
                if (!interfaceSymbol.HasOrInheritsAttribute(appServicesAttributeSymbol))
                {
                    return;
                }

                // Go through all interface members to analyze them. Here we need to go through all members, not just the ones immediately
                // declared, as it's possible an interface with an invalid member will be inherited by another one that adds [AppServices].
                // In that case, the base interface will not be analyzed (as it doesn't have [AppServices]), so the derived one will need
                // to also go through inherited members to ensure that all members that the generator will process will actually be valid.
                foreach (ISymbol memberSymbol in interfaceSymbol.GetAllMembers())
                {
                    // If a method is not abstract nor virtual (ie. a DIM or static non-virtual interface member), it can just be ignored.
                    // The generated service type will not have to consider it as far as registering endpoints and generating members goes.
                    if (memberSymbol.IsIgnoredAppServicesMember())
                    {
                        continue;
                    }

                    // Skip property accessors for validation (the property itself will be detected below)
                    if (memberSymbol is IMethodSymbol { MethodKind: MethodKind.PropertyGet or MethodKind.PropertySet })
                    {
                        continue;
                    }

                    // All remaining members must be non-generic instance methods, which the generator will emit
                    if (memberSymbol is not IMethodSymbol { IsStatic: false, IsGenericMethod: false } methodSymbol)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(InvalidAppServicesMemberType, memberSymbol.Locations.FirstOrDefault(), memberSymbol, interfaceSymbol));

                        continue;
                    }

                    // Validate the return type for the current method
                    if (methodSymbol.ReturnType is not INamedTypeSymbol returnTypeSymbol ||
                        !methodSymbol.TryGetParameterOrReturnType(out ParameterOrReturnType returnType) ||
                        !returnType.IsValidReturnType())
                    {
                        context.ReportDiagnostic(Diagnostic.Create(InvalidAppServicesMethodReturnType, memberSymbol.Locations.FirstOrDefault(), methodSymbol, interfaceSymbol, methodSymbol.ReturnType));
                    }

                    bool isProgressParameterFound = false;
                    bool isCancellationTokenParameterFound = false;

                    // Validate the method parameters
                    foreach (IParameterSymbol parameter in methodSymbol.Parameters)
                    {
                        // First validate types that could possibly be allowed at all (ie. valid types)
                        if (!parameter.TryGetParameterOrReturnType(out ParameterOrReturnType parameterType) ||
                            !parameterType.IsValidParameterType())
                        {
                            context.ReportDiagnostic(Diagnostic.Create(InvalidAppServicesMethodParameterType, parameter.Locations.FirstOrDefault(), parameter.Name, methodSymbol, interfaceSymbol, parameter.Type));

                            continue;
                        }

                        // Then check that the type is not an IProgress<T>, if one has already been discovered
                        if (parameterType.HasFlag(ParameterOrReturnType.IProgressOfT))
                        {
                            if (isProgressParameterFound)
                            {
                                context.ReportDiagnostic(Diagnostic.Create(InvalidRepeatedAppServicesMethodIProgressParameter, parameter.Locations.FirstOrDefault(), parameter.Name, methodSymbol, interfaceSymbol, parameter.Type));
                            }

                            isProgressParameterFound = true;
                        }

                        // Lastly, check that the type is not a CancellationToken, if one has already been discovered
                        if (parameterType.HasFlag(ParameterOrReturnType.CancellationToken))
                        {
                            if (isCancellationTokenParameterFound)
                            {
                                context.ReportDiagnostic(Diagnostic.Create(InvalidRepeatedAppServicesMethodCancellationTokenParameter, parameter.Locations.FirstOrDefault(), parameter.Name, methodSymbol, interfaceSymbol, parameter.Type));
                            }

                            isCancellationTokenParameterFound = true;
                        }
                    }
                }
            }, SymbolKind.NamedType);
        });
    }
}
