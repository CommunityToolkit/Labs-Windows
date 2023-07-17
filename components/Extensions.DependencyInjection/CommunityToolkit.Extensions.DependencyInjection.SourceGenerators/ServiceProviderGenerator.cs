// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Extensions;
using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Models;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators;

/// <summary>
/// A source generator for <see cref="System.IServiceProvider"/> instances.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class ServiceProviderGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Gather info on all singleton service providers to generate
        IncrementalValuesProvider<ServiceCollectionInfo> singletonServices =
            context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "CommunityToolkit.Extensions.DependencyInjection.SingletonAttribute",
            predicate: Execute.IsSyntaxTarget,
            transform: Execute.GetSingletonInfo)
            .Where(static info => info is not null)!;

        // Do the same for all transient services
        IncrementalValuesProvider<ServiceCollectionInfo> transientServices =
            context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: "CommunityToolkit.Extensions.DependencyInjection.TransientAttribute",
            predicate: Execute.IsSyntaxTarget,
            transform: Execute.GetTransientInfo)
            .Where(static info => info is not null)!;

        // Merge the two registration types
        IncrementalValuesProvider<ServiceCollectionInfo> serviceCollections = singletonServices.Concat(transientServices);

        // Aggregate all discovered services (both singleton and transient) and group them by target method
        IncrementalValuesProvider<ServiceCollectionInfo> groupedServiceCollections =
            serviceCollections.GroupBy<ServiceCollectionInfo, ServiceProviderMethodInfo, RegisteredServiceInfo>(
                keySelector: static item => item.Method,
                elementsSelector: static item => item.Services,
                resultSelector: static (key, elements) => new ServiceCollectionInfo(key, elements));

        // Generate all service provider methods
        context.RegisterSourceOutput(groupedServiceCollections, static (context, info) =>
        {
            CompilationUnitSyntax compilationUnit = Execute.GetSyntax(info);

            context.AddSource($"{info.Method.Hierarchy.FilenameHint}.{info.Method.MethodName}.g.cs", compilationUnit.GetText(Encoding.UTF8));
        });
    }
}
