// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="IncrementalGeneratorInitializationContext"/>.
/// </summary>
internal static class IncrementalGeneratorInitializationContextExtensions
{
    /// <inheritdoc cref="SyntaxValueProvider.ForAttributeWithMetadataName"/>
    public static IncrementalValuesProvider<T> ForAttributeWithMetadataNameAndOptions<T>(
        this IncrementalGeneratorInitializationContext context,
        string fullyQualifiedMetadataName,
        Func<SyntaxNode, CancellationToken, bool> predicate,
        Func<GeneratorAttributeSyntaxContextWithOptions, CancellationToken, T> transform)
    {
        // Invoke 'ForAttributeWithMetadataName' normally, but just return the context directly
        IncrementalValuesProvider<GeneratorAttributeSyntaxContext> syntaxContext = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName,
            predicate,
            static (context, token) => context);

        // Do the same for the analyzer config options
        IncrementalValueProvider<AnalyzerConfigOptions> configOptions = context.AnalyzerConfigOptionsProvider.Select(static (provider, token) => provider.GlobalOptions);

        // Merge the two and invoke the provided transform on these two values. Neither value
        // is equatable, meaning the pipeline will always re-run until this point. This is
        // intentional: we don't want any symbols or other expensive objects to be kept alive
        // across incremental steps, especially if they could cause entire compilations to be
        // rooted, which would significantly increase memory use and introduce more GC pauses.
        // In this specific case, flowing non equatable values in a pipeline is therefore fine.
        return syntaxContext.Combine(configOptions).Select((input, token) => transform(new GeneratorAttributeSyntaxContextWithOptions(input.Left, input.Right), token));
    }

    /// <inheritdoc cref="SyntaxValueProvider.CreateSyntaxProvider"/>
    public static IncrementalValuesProvider<T> CreateSyntaxProviderWithOptions<T>(
        this IncrementalGeneratorInitializationContext context,
        Func<SyntaxNode, CancellationToken, bool> predicate,
        Func<GeneratorSyntaxContextWithOptions, CancellationToken, T> transform)
    {
        // Invoke 'ForAttributeWithMetadataName' normally, but just return the context directly
        IncrementalValuesProvider<GeneratorSyntaxContext> syntaxContext = context.SyntaxProvider.CreateSyntaxProvider(
            predicate,
            static (context, token) => context);

        // Do the same for the analyzer config options
        IncrementalValueProvider<AnalyzerConfigOptions> configOptions = context.AnalyzerConfigOptionsProvider.Select(static (provider, token) => provider.GlobalOptions);

        // Merge the two and invoke the provided transform, like the extension above
        return syntaxContext.Combine(configOptions).Select((input, token) => transform(new GeneratorSyntaxContextWithOptions(input.Left, input.Right), token));
    }
}
