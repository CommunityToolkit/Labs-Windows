// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Basic.Reference.Assemblies;
using CommunityToolkit.WinUI;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;

/// <summary>
/// A helper type to run source generator tests.
/// </summary>
/// <typeparam name="TGenerator">The type of generator to test.</typeparam>
internal static class CSharpGeneratorTest<TGenerator>
    where TGenerator : IIncrementalGenerator, new()
{
    /// <summary>
    /// Verifies the resulting diagnostics from a source generator.
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="diagnosticsIds">The expected diagnostics ids to be generated.</param>
    public static void VerifyDiagnostics(string source, params string[] diagnosticsIds)
    {
        RunGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics);

        Dictionary<string, Diagnostic> diagnosticMap = diagnostics.DistinctBy(diagnostic => diagnostic.Id).ToDictionary(diagnostic => diagnostic.Id);

        // Check that the diagnostics match
        Assert.IsTrue(diagnosticMap.Keys.ToHashSet().SetEquals(diagnosticsIds), $"Diagnostics didn't match. {string.Join(", ", diagnosticMap.Values)}");

        // If the compilation was supposed to succeed, ensure that no further errors were generated
        if (diagnosticsIds.Length == 0)
        {
            // Compute diagnostics for the final compiled output (just include errors)
            List<Diagnostic> outputCompilationDiagnostics = compilation.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToList();

            Assert.IsTrue(outputCompilationDiagnostics.Count == 0, $"resultingIds: {string.Join(", ", outputCompilationDiagnostics)}");
        }
    }

    /// <summary>
    /// Verifies the resulting sources produced by a source generator.
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="result">The expected source to be generated.</param>
    /// <param name="languageVersion">The language version to use to run the test.</param>
    public static void VerifySources(string source, (string Filename, string Source) result, LanguageVersion languageVersion = LanguageVersion.CSharp13)
    {
        RunGenerator(source, out Compilation compilation, out ImmutableArray<Diagnostic> diagnostics, languageVersion);

        // Ensure that no diagnostics were generated
        CollectionAssert.AreEquivalent(Array.Empty<Diagnostic>(), diagnostics);

        // Update the assembly version using the version from the assembly of the input generators.
        // This allows the tests to not need updates whenever the version of the MVVM Toolkit changes.
        string expectedText = result.Source.Replace("<ASSEMBLY_VERSION>", $"\"{typeof(TGenerator).Assembly.GetName().Version}\"");
        string actualText = compilation.SyntaxTrees.Single(tree => Path.GetFileName(tree.FilePath) == result.Filename).ToString();

        Assert.AreEqual(expectedText, actualText);
    }

    /// <summary>
    /// Verifies the incremental generator steps for a given source generator.
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="updatedSource">The updated source to process.</param>
    /// <param name="executeReason">The reason for the first <c>"Execute"</c> step.</param>
    /// <param name="outputReason">The reason for the <c>"Output"</c> step.</param>
    /// <param name="sourceReason">The reason for the final output source.</param>
    /// <param name="languageVersion">The language version to use to run the test.</param>
    public static void VerifyIncrementalSteps(
        string source,
        string updatedSource,
        IncrementalStepRunReason executeReason,
        IncrementalStepRunReason outputReason,
        IncrementalStepRunReason sourceReason,
        LanguageVersion languageVersion = LanguageVersion.CSharp13)
    {
        Compilation compilation = CreateCompilation(source, languageVersion);

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [new TGenerator().AsSourceGenerator()],
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true));

        // Run the generator on the initial sources
        driver = driver.RunGenerators(compilation);

        // Update the compilation by replacing the source
        compilation = compilation.ReplaceSyntaxTree(
            compilation.SyntaxTrees.First(),
            CSharpSyntaxTree.ParseText(updatedSource, CSharpParseOptions.Default.WithLanguageVersion(languageVersion)));

        // Run the generators again on the updated source
        driver = driver.RunGenerators(compilation);

        GeneratorRunResult result = driver.GetRunResult().Results.Single();

        // Get the generated sources and validate them
        (object Value, IncrementalStepRunReason Reason)[] sourceOuputs =
            result.TrackedOutputSteps
            .SelectMany(outputStep => outputStep.Value)
            .SelectMany(output => output.Outputs)
            .ToArray();

        Assert.AreEqual(1, sourceOuputs.Length);
        Assert.AreEqual(sourceReason, sourceOuputs[0].Reason);
        Assert.AreEqual(executeReason, result.TrackedSteps["Execute"].Single().Outputs[0].Reason);
        Assert.AreEqual(outputReason, result.TrackedSteps["Output"].Single().Outputs[0].Reason);
    }

    /// <summary>
    /// Creates a compilation from a given source.
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="languageVersion">The language version to use to run the test.</param>
    /// <returns>The resulting <see cref="Compilation"/> object.</returns>
    private static CSharpCompilation CreateCompilation(string source, LanguageVersion languageVersion = LanguageVersion.CSharp13)
    {
        // Get all assembly references for the .NET TFM and ComputeSharp
        IEnumerable<MetadataReference> metadataReferences =
        [
            .. Net80.References.All,
            MetadataReference.CreateFromFile(typeof(Point).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ApplicationView).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(DependencyProperty).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(GeneratedDependencyPropertyAttribute).Assembly.Location)
        ];

        // Parse the source text
        SyntaxTree sourceTree = CSharpSyntaxTree.ParseText(
            source,
            CSharpParseOptions.Default.WithLanguageVersion(languageVersion));

        // Create the original compilation
        return CSharpCompilation.Create(
            "original",
            [sourceTree],
            metadataReferences,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));
    }

    /// <summary>
    /// Runs a generator and gathers the output results.
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="compilation"><inheritdoc cref="GeneratorDriver.RunGeneratorsAndUpdateCompilation" path="/param[@name='outputCompilation']/node()"/></param>
    /// <param name="diagnostics"><inheritdoc cref="GeneratorDriver.RunGeneratorsAndUpdateCompilation" path="/param[@name='diagnostics']/node()"/></param>
    /// <param name="languageVersion">The language version to use to run the test.</param>
    private static void RunGenerator(
        string source,
        out Compilation compilation,
        out ImmutableArray<Diagnostic> diagnostics,
        LanguageVersion languageVersion = LanguageVersion.CSharp13)
    {
        Compilation originalCompilation = CreateCompilation(source, languageVersion);

        // Create the generator driver with the specified generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new TGenerator()).WithUpdatedParseOptions(originalCompilation.SyntaxTrees.First().Options);

        // Run all source generators on the input source code
        _ = driver.RunGeneratorsAndUpdateCompilation(originalCompilation, out compilation, out diagnostics);
    }
}
