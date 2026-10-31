// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;

/// <summary>
/// Runs generation and analysis against an independently referenced XAML framework.
/// </summary>
/// <remarks>
/// Both compilations use .NET 8 reference assemblies to cover the oldest supported modern .NET target,
/// independently of the SDK running the tests. When raising that baseline, update both reference sets,
/// the Basic.Reference.Assemblies.Net80 package reference, and the other test helpers together.
/// </remarks>
internal static class XamlFrameworkTest
{
    public static PortableExecutableReference CreateFrameworkReference(string assemblyName, bool isInterface, string xamlNamespace = "Microsoft.UI.Xaml", string? filePath = null)
    {
        string source = $$"""
            using System;

            namespace {{xamlNamespace}}
            {
                public {{(isInterface ? "interface" : "class")}} DependencyObject
                {
                    {{(isInterface ? "object GetValue(DependencyProperty property);" : "public object GetValue(DependencyProperty property) => null;")}}
                    {{(isInterface ? "void SetValue(DependencyProperty property, object value);" : "public void SetValue(DependencyProperty property, object value) { }")}}
                }

                {{(isInterface ? "public interface IIntermediateDependencyObject : DependencyObject { }" : "")}}

                public delegate void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs args);
                public delegate object CreateDefaultValueCallback();

                public class DependencyProperty
                {
                    public static object UnsetValue { get; } = new();
                    public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata typeMetadata) => new();
                }

                public class DependencyPropertyChangedEventArgs
                {
                    public object OldValue { get; }
                    public object NewValue { get; }
                    public DependencyProperty Property { get; }
                }

                public class PropertyMetadata
                {
                    public PropertyMetadata(object defaultValue) { }
                    public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback) { }
                    public static PropertyMetadata Create(CreateDefaultValueCallback createDefaultValueCallback, PropertyChangedCallback propertyChangedCallback = null) => new(null);
                }
            }

            namespace {{xamlNamespace}}.Controls
            {
                public class Control : {{(isInterface ? "IIntermediateDependencyObject" : "DependencyObject")}}
                {
                    {{(isInterface ? "public object GetValue(DependencyProperty property) => null;" : "")}}
                    {{(isInterface ? "public void SetValue(DependencyProperty property, object value) { }" : "")}}
                }
            }

            namespace {{xamlNamespace}}.Markup
            {
                public static class XamlBindingHelper
                {
                    public static void SetPropertyFromInt32(DependencyObject owner, DependencyProperty property, int value) => owner.SetValue(property, value);
                    public static void SetPropertyFromBoolean(DependencyObject owner, DependencyProperty property, bool value) => owner.SetValue(property, value);
                    public static void SetPropertyFromString(DependencyObject owner, DependencyProperty property, string value) => owner.SetValue(property, value);
                }
            }
            """;

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp13))],
            Net80.References.All,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using MemoryStream stream = new();

        AssertEmits(compilation, stream);

        return MetadataReference.CreateFromImage(stream.ToArray(), filePath: filePath);
    }

    public static Compilation RunGenerator(string source, MetadataReference frameworkReference, bool useWindowsUIXaml = false)
    {
        string[] symbols = useWindowsUIXaml
            ? ["GENERATED_DEPENDENCY_PROPERTY_ATTRIBUTE_EMBEDDED_MODE", "GENERATED_DEPENDENCY_PROPERTY_EMBEDDED_MODE", "GENERATED_DEPENDENCY_PROPERTY_USE_WINDOWS_UI_XAML"]
            : ["GENERATED_DEPENDENCY_PROPERTY_ATTRIBUTE_EMBEDDED_MODE", "GENERATED_DEPENDENCY_PROPERTY_EMBEDDED_MODE"];

        CSharpParseOptions parseOptions = new CSharpParseOptions(LanguageVersion.CSharp13).WithPreprocessorSymbols(symbols);
        CSharpCompilation compilation = CSharpCompilation.Create(
            "App",
            [CSharpSyntaxTree.ParseText(source, parseOptions)],
            [.. Net80.References.All, frameworkReference],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: [new DependencyPropertyGenerator().AsSourceGenerator()],
            parseOptions: parseOptions,
            optionsProvider: new DependencyPropertyGeneratorAnalyzerConfigOptionsProvider(useWindowsUIXaml));

        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        Assert.AreEqual(0, diagnostics.Length, string.Join(Environment.NewLine, diagnostics));

        return outputCompilation;
    }

    public static async Task<ImmutableArray<Diagnostic>> GetAnalyzerDiagnosticsAsync(Compilation compilation, bool useWindowsUIXaml = false)
    {
        AnalyzerOptions options = new([], new DependencyPropertyGeneratorAnalyzerConfigOptionsProvider(useWindowsUIXaml));

        return await compilation.WithAnalyzers([new InvalidPropertyContainingTypeDeclarationAnalyzer()], options).GetAnalyzerDiagnosticsAsync();
    }

    public static void AssertEmits(Compilation compilation)
    {
        using MemoryStream stream = new();

        AssertEmits(compilation, stream);
    }

    private static void AssertEmits(Compilation compilation, Stream stream)
    {
        EmitResult result = compilation.Emit(stream);

        Assert.IsTrue(result.Success, string.Join(Environment.NewLine, result.Diagnostics.Where(static diagnostic => diagnostic.Severity is DiagnosticSeverity.Error)));
    }
}
