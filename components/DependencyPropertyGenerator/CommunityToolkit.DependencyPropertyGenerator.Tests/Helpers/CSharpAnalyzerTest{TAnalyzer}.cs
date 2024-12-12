// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using CommunityToolkit.WinUI;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;

/// <summary>
/// A custom <see cref="CSharpAnalyzerTest{TAnalyzer, TVerifier}"/> that uses a specific C# language version to parse code.
/// </summary>
/// <typeparam name="TAnalyzer">The type of the analyzer to test.</typeparam>
internal sealed class CSharpAnalyzerTest<TAnalyzer> : CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    /// <summary>
    /// The C# language version to use to parse code.
    /// </summary>
    private readonly LanguageVersion languageVersion;

    /// <summary>
    /// Creates a new <see cref="CSharpAnalyzerWithLanguageVersionTest{TAnalyzer}"/> instance with the specified parameters.
    /// </summary>
    /// <param name="languageVersion">The C# language version to use to parse code.</param>
    private CSharpAnalyzerTest(LanguageVersion languageVersion)
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
    public static Task VerifyAnalyzerAsync(string source, LanguageVersion languageVersion, params DiagnosticResult[] expected)
    {
        CSharpAnalyzerTest<TAnalyzer> test = new(languageVersion) { TestCode = source };

        test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80;
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(ApplicationView).Assembly.Location));
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(DependencyProperty).Assembly.Location));
        test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(typeof(GeneratedDependencyPropertyAttribute).Assembly.Location));

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync(CancellationToken.None);
    }
}
