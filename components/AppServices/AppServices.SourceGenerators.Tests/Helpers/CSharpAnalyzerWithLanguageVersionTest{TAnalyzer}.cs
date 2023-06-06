// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.AppServices.SourceGenerators.Tests.Helpers;

/// <summary>
/// A custom <see cref="CSharpAnalyzerTest{TAnalyzer, TVerifier}"/> that uses a specific C# language version to parse code.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the analyzer to test.</typeparam>
internal sealed class CSharpAnalyzerWithLanguageVersionTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, MSTestVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// The C# language version to use to parse code.
    /// </summary>
    private readonly LanguageVersion languageVersion;

    /// <summary>
    /// Creates a new <see cref="CSharpAnalyzerWithLanguageVersionTest{TAnalyzer}"/> instance with the specified paramaters.
    /// </summary>
    /// <param name="languageVersion">The C# language version to use to parse code.</param>
    private CSharpAnalyzerWithLanguageVersionTest(LanguageVersion languageVersion)
    {
        this.languageVersion = languageVersion;
    }

    /// <inheritdoc/>
    protected override ParseOptions CreateParseOptions()
    {
        return new CSharpParseOptions(this.languageVersion, DocumentationMode.Diagnose);
    }

    /// <inheritdoc cref="AnalyzerVerifier{TAnalyzer, TTest, TVerifier}.VerifyAnalyzerAsync"/>
    /// <param name="languageVersion">The language version to use to run the test.</param>
    public static Task VerifyAnalyzerAsync(string source, LanguageVersion languageVersion = LanguageVersion.CSharp8)
    {
        CSharpAnalyzerWithLanguageVersionTest<TAnalyzer> test = new(languageVersion) { TestCode = source };

        test.TestState.ReferenceAssemblies =
            ReferenceAssemblies.NetFramework.Net472.Default
            .AddPackages(ImmutableArray.Create(new PackageIdentity("Microsoft.Windows.SDK.Contracts", "10.0.22621.2")));
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(AppServiceAttribute).Assembly.Location));

        return test.RunAsync(CancellationToken.None);
    }
}
