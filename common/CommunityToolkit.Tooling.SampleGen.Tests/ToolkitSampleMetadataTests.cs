// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Attributes;
using CommunityToolkit.Tooling.SampleGen.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace CommunityToolkit.Tooling.SampleGen.Tests;

[TestClass]
public partial class ToolkitSampleMetadataTests
{
    [TestMethod]
    public void PaneOption_GeneratesWithoutDiagnostics()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""Test"", false, Title = ""Toggle y"")]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", ""Segoe UI"", ""Arial"", ""Consolas"", Title = ""Font family"")]

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                    public Sample()
                    {{
                        var x = this.Test;
                        var y = this.TextFontFamily;
                    }}
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleOptionGenerator>(source, string.Empty);
    }

    [TestMethod]
    public void PaneOption_GeneratesTitleProperty()
    {
        var source = """
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {
                [ToolkitSampleNumericOption("TextSize", 12, 8, 48, 2, false, Title = "FontSize")]
                [ToolkitSample(id: nameof(Sample), "Test Sample", description: "")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {
                    public Sample()
                    {
                        var x = this.Test;
                        var y = this.TextFontFamily;
                    }
                }
            }

            namespace Windows.UI.Xaml.Controls
            {
                public class UserControl { }
            }
        """;

        var result = """
            #nullable enable
            namespace CommunityToolkit.Tooling.SampleGen;

            public static class ToolkitSampleRegistry
            {
                public static System.Collections.Generic.Dictionary<string, CommunityToolkit.Tooling.SampleGen.Metadata.ToolkitSampleMetadata> Listing
                { get; } = new() {
                    ["Sample"] = new CommunityToolkit.Tooling.SampleGen.Metadata.ToolkitSampleMetadata("Sample", "Test Sample", "", typeof(MyApp.Sample), () => new MyApp.Sample(), null, null, new CommunityToolkit.Tooling.SampleGen.Metadata.IGeneratedToolkitSampleOptionViewModel[] { new CommunityToolkit.Tooling.SampleGen.Metadata.ToolkitSampleNumericOptionMetadataViewModel(name: "TextSize", initial: 12, min: 8, max: 48, step: 2, showAsNumberBox: false, title: "FontSize") })
                };
            }
            """;

        VerifyGenerateSources("MyApp.Tests", source, new[] { new ToolkitSampleMetadataGenerator() }, ignoreDiagnostics: true, ("ToolkitSampleRegistry.g.cs", result));
    }

    // https://github.com/CommunityToolkit/Labs-Windows/issues/175
    [TestMethod]
    public void PaneOption_GeneratesProperty_DuplicatePropNamesAcrossSampleClasses()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""Test"", false, Title = ""Toggle y"")]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", ""Segoe UI"", ""Arial"", ""Consolas"", Title = ""Font family"")]

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                    public Sample()
                    {{
                        var x = this.Test;
                        var y = this.TextFontFamily;
                    }}
                }}

                [ToolkitSampleBoolOption(""Test"", false, Title = ""Toggle y"")]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", ""Segoe UI"", ""Arial"", ""Consolas"", Title = ""Font family"")]

                [ToolkitSample(id: nameof(Sample2), ""Test Sample"", description: """")]
                public partial class Sample2 : Windows.UI.Xaml.Controls.UserControl
                {{
                    public Sample2()
                    {{
                        var x = this.Test;
                        var y = this.TextFontFamily;
                    }}
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleOptionGenerator>(source, string.Empty);
    }

    [TestMethod]
    public void PaneOptionOnNonSample()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {
                [ToolkitSampleBoolOption(""BindToMe"", false, Title =  ""Toggle visibility"")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {
                }
            }

            namespace Windows.UI.Xaml.Controls
            {
                public class UserControl { }
            }";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SamplePaneOptionAttributeOnNonSample.Id);
    }

    [DataRow("", DisplayName = "Empty string"), DataRow(" ", DisplayName = "Only whitespace"), DataRow("Test ", DisplayName = "Text with whitespace")]
    [DataRow("_", DisplayName = "Underscore"), DataRow("$", DisplayName = "Dollar sign"), DataRow("%", DisplayName = "Percent symbol")]
    [DataRow("class", DisplayName = "Reserved keyword 'class'"), DataRow("string", DisplayName = "Reserved keyword 'string'"), DataRow("sealed", DisplayName = "Reserved keyword 'sealed'"), DataRow("ref", DisplayName = "Reserved keyword 'ref'")]
    [TestMethod]
    public void PaneOptionWithBadName(string name)
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                [ToolkitSampleBoolOption(""{name}"", false, Title =  ""Toggle visibility"")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SamplePaneOptionWithBadName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void PaneOptionWithConflictingPropertyName()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""IsVisible"", false, Title =  ""Toggle x"")]
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                    public string IsVisible {{ get; set; }}
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SamplePaneOptionWithConflictingName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void PaneOptionWithConflictingInheritedPropertyName()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""IsVisible"", false, Title =  ""Toggle x"")]
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Base
                {{
                }}

                public class Base : Windows.UI.Xaml.Controls.UserControl
                {{
                    public string IsVisible {{ get; set; }}
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SamplePaneOptionWithConflictingName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void PaneOptionWithDuplicateName()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""test"", false, Title =  ""Toggle x"")]
                [ToolkitSampleBoolOption(""test"", false, Title =  ""Toggle y"")]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", ""Segoe UI"", ""Arial"", Title = ""Text foreground"")]

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SamplePaneOptionWithDuplicateName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void PaneOptionWithDuplicateName_AllowedBetweenSamples()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""test"", false, Title =  ""Toggle y"")]

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}

                [ToolkitSampleBoolOption(""test"", false, Title =  ""Toggle y"")]

                [ToolkitSample(id: nameof(Sample2), ""Test Sample"", description: """")]
                public partial class Sample2 : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void PaneMultipleChoiceOptionWithNoChoices()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", Title = ""Font family"")]

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SamplePaneMultiChoiceOptionWithNoChoices.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void SampleGeneratedOptionAttributeOnUnsupportedType()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", ""Segoe UI"", ""Arial"", ""Consolas"", Title = ""Font family"")]
                [ToolkitSampleBoolOption(""Test"", false, Title =  ""Toggle visibility"")]
                public partial class Sample
                {{
                }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SampleGeneratedOptionAttributeOnUnsupportedType.Id, DiagnosticDescriptors.SamplePaneOptionAttributeOnNonSample.Id);
    }

    [TestMethod]
    public void SampleAttributeOnUnsupportedType()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample
                {{
                }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SampleAttributeOnUnsupportedType.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void SampleOptionPaneAttributeOnUnsupportedType()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleOptionsPane(sampleId: nameof(Sample))]
                public partial class SampleOptionsPane
                {{
                }}

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SampleOptionPaneAttributeOnUnsupportedType.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void SampleAttributeValid()
    {
        var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.SampleGen;
            using CommunityToolkit.Tooling.SampleGen.Attributes;

            namespace MyApp
            {{

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

        // TODO: We should have this return the references to the registries or something so we can check the generated output?
        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, string.Empty, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="source">The input source to process.</param>
    /// <param name="markdown">The input documentation info to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics<TGenerator>(string source, string markdown, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        VerifyGeneratedDiagnostics<TGenerator>(CSharpSyntaxTree.ParseText(source), markdown, diagnosticsIds);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="syntaxTree">The input source tree to process.</param>
    /// <param name="markdown">The input documentation info to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics<TGenerator>(SyntaxTree syntaxTree, string markdown, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        var sampleAttributeType = typeof(ToolkitSampleAttribute);

        var references =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where !assembly.IsDynamic
            let reference = MetadataReference.CreateFromFile(assembly.Location)
            select reference;

        var compilation = CSharpCompilation.Create(
            "original.Samples",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationDiagnostics = compilation.GetDiagnostics();

        IIncrementalGenerator generator = new TGenerator();

        GeneratorDriver driver =
            CSharpGeneratorDriver
                .Create(generator)
                .WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        if (!string.IsNullOrWhiteSpace(markdown))
        {
            var text = new InMemoryAdditionalText(@"C:\pathtorepo\components\experiment\samples\experiment.Samples\documentation.md", markdown);

            driver = driver.AddAdditionalTexts(ImmutableArray.Create<AdditionalText>(text));
        }

        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();
        var generatedCompilationDiaghostics = outputCompilation.GetDiagnostics();

        Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds), $"Expected one of [{string.Join(", ", diagnosticsIds)}] diagnostic Ids. Got [{string.Join(", ", resultingIds)}]");
        Assert.IsTrue(generatedCompilationDiaghostics.All(x => x.Severity != DiagnosticSeverity.Error), $"Expected no generated compilation errors. Got: \n{string.Join("\n", generatedCompilationDiaghostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => $"[{x.Id}: {x.GetMessage()}]"))}");

        GC.KeepAlive(sampleAttributeType);
    }

    //// See: https://github.com/CommunityToolkit/dotnet/blob/c2053562d1a4d4829fc04b1cb86d1564c2c4a03c/tests/CommunityToolkit.Mvvm.SourceGenerators.UnitTests/Test_SourceGeneratorsCodegen.cs#L103
    /// <summary>
    /// Generates the requested sources
    /// </summary>
    /// <param name="source">The input source to process.</param>
    /// <param name="generators">The generators to apply to the input syntax tree.</param>
    /// <param name="results">The source files to compare.</param>
    private static void VerifyGenerateSources(string assemblyName, string source, IIncrementalGenerator[] generators, bool ignoreDiagnostics = false, params (string Filename, string Text)[] results)
    {
        // Ensure our types are loaded
        Type sampleattributeObjectType = typeof(ToolkitSampleAttribute);

        // Get all assembly references for the loaded assemblies (easy way to pull in all necessary dependencies)
        IEnumerable<MetadataReference> references =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where !assembly.IsDynamic
            let reference = MetadataReference.CreateFromFile(assembly.Location)
            select reference;

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10));

        // Create a syntax tree with the input source
        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            new SyntaxTree[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators).WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        // Run all source generators on the input source code
        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        // Ensure that no diagnostics were generated
        if (!ignoreDiagnostics)
        {
            CollectionAssert.AreEquivalent(Array.Empty<Diagnostic>(), diagnostics);
        }

        foreach ((string filename, string text) in results)
        {
            SyntaxTree generatedTree = outputCompilation.SyntaxTrees.Single(tree => Path.GetFileName(tree.FilePath) == filename);

            Assert.AreEqual(text, generatedTree.ToString());
        }

        GC.KeepAlive(sampleattributeObjectType);
    }

    // From: https://github.com/dotnet/roslyn/blob/main/src/Compilers/Test/Core/SourceGeneration/TestGenerators.cs
    internal class InMemoryAdditionalText : AdditionalText
    {
        private readonly SourceText _content;

        public InMemoryAdditionalText(string path, string content)
        {
            Path = path;
            _content = SourceText.From(content, Encoding.UTF8);
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default) => _content;
    }
}
