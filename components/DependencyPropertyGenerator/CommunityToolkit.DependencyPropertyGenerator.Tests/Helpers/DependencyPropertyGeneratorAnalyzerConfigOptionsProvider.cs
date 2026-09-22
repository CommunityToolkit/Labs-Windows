// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;

/// <summary>
/// A custom <see cref="AnalyzerConfigOptionsProvider"/> providing the MSBuild properties needed by the dependency property generator.
/// </summary>
internal sealed class DependencyPropertyGeneratorAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
    /// <summary>
    /// The singleton <see cref="DependencyPropertyGeneratorAnalyzerConfigOptionsProvider"/> instance.
    /// </summary>
    public static DependencyPropertyGeneratorAnalyzerConfigOptionsProvider Instance { get; } = new();

    /// <inheritdoc/>
    public override AnalyzerConfigOptions GlobalOptions { get; } = new SimpleAnalyzerConfigOptions(
        ImmutableDictionary<string, string>.Empty.Add("build_property.DependencyPropertyGeneratorUseWindowsUIXaml", "true"));

    /// <inheritdoc/>
    public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
    {
        return SimpleAnalyzerConfigOptions.Empty;
    }

    /// <inheritdoc/>
    public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
    {
        return SimpleAnalyzerConfigOptions.Empty;
    }

    /// <summary>
    /// A simple <see cref="AnalyzerConfigOptions"/> implementation backed by an immutable dictionary.
    /// </summary>
    /// <param name="options">The dictionary of options.</param>
    private sealed class SimpleAnalyzerConfigOptions(ImmutableDictionary<string, string> options) : AnalyzerConfigOptions
    {
        /// <summary>
        /// An empty <see cref="SimpleAnalyzerConfigOptions"/> instance.
        /// </summary>
        public static SimpleAnalyzerConfigOptions Empty { get; } = new(ImmutableDictionary<string, string>.Empty);

        /// <inheritdoc/>
        public override bool TryGetValue(string key, [NotNullWhen(true)] out string? value)
        {
            return options.TryGetValue(key, out value);
        }
    }
}
