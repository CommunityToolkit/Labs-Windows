// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// <inheritdoc cref="GeneratorSyntaxContext" path="/summary/node()"/>
/// </summary>
/// <param name="syntaxContext">The original <see cref="GeneratorSyntaxContext"/> value.</param>
/// <param name="globalOptions">The original <see cref="AnalyzerConfigOptions"/> value.</param>
internal readonly struct GeneratorSyntaxContextWithOptions(
    GeneratorSyntaxContext syntaxContext,
    AnalyzerConfigOptions globalOptions)
{
    /// <inheritdoc cref="GeneratorSyntaxContext.Node"/>
    public SyntaxNode Node { get; } = syntaxContext.Node;

    /// <inheritdoc cref="GeneratorSyntaxContext.SemanticModel"/>
    public SemanticModel SemanticModel { get; } = syntaxContext.SemanticModel;

    /// <inheritdoc cref="AnalyzerConfigOptionsProvider.GlobalOptions"/>
    public AnalyzerConfigOptions GlobalOptions { get; } = globalOptions;
}
