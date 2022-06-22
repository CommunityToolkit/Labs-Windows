// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Tests;

[TestClass]
public partial class ToolkitSampleMetadataTests
{
    [TestMethod]
    public void PaneOptionOnNonSample()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {
                [ToolkitSampleBoolOption(""BindToMe"", ""Toggle visibility"", false)]
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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                [ToolkitSampleBoolOption(""{name}"", ""Toggle visibility"", false)]
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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""IsVisible"", ""Toggle x"", false)]
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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""IsVisible"", ""Toggle x"", false)]
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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""test"", ""Toggle x"", false)]
                [ToolkitSampleBoolOption(""test"", ""Toggle y"", false)]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", title: ""Text foreground"", ""Segoe UI"", ""Arial"")]

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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleBoolOption(""test"", ""Toggle y"", false)]

                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}

                [ToolkitSampleBoolOption(""test"", ""Toggle y"", false)]

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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", title: ""Text foreground"")]

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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", title: ""Text foreground"", ""Segoe UI"", ""Arial"")]
                [ToolkitSampleBoolOption(""Test"", ""Toggle visibility"", false)]
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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

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
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

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
            "original.Sample",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        IIncrementalGenerator generator = new TGenerator();

        GeneratorDriver driver =
            CSharpGeneratorDriver
                .Create(generator)
                .WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        if (!string.IsNullOrWhiteSpace(markdown))
        {
            var text = new InMemoryAdditionalText(@"C:\pathtorepo\labs\experiment\samples\experiment.Sample\documentation.md", markdown);

            driver = driver.AddAdditionalTexts(ImmutableArray.Create<AdditionalText>(text));
        }

        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();

        Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds), $"Expected one of [{string.Join(", ", diagnosticsIds)}] diagnostic Ids. Got [{string.Join(", ", resultingIds)}]");

        GC.KeepAlive(sampleAttributeType);
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
