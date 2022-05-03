using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Tests
{
    [TestClass]
    public class ToolkitSampleMetadataTests
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

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SamplePaneOptionAttributeOnNonSample.Id);
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

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SamplePaneOptionWithBadName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
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

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SamplePaneOptionWithConflictingName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
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

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SamplePaneOptionWithConflictingName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
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
                
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SamplePaneOptionWithDuplicateName.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
        }

        [TestMethod]
        public void PaneOptionWithDuplicateName_AllowedForMultiChoice()
        {
            var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", label: ""Segoe UI"", value: ""Segoe UI"", title: ""Font"")]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", label: ""Arial"", value: ""Arial"")]

                [ToolkitSampleBoolOption(""test"", ""Toggle y"", false)]
                
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
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

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
        }

        [TestMethod]
        public void PaneMultipleChoiceOptionWithMultipleTitles()
        {
            var source = $@"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators;
            using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

            namespace MyApp
            {{
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", label: ""Segoe UI"", value: ""Segoe UI"", title: ""Font"")]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", label: ""Arial"", value: ""Arial"", title: ""Other font"")]
                
                [ToolkitSample(id: nameof(Sample), ""Test Sample"", description: """")]
                public partial class Sample : Windows.UI.Xaml.Controls.UserControl
                {{
                }}
            }}

            namespace Windows.UI.Xaml.Controls
            {{
                public class UserControl {{ }}
            }}";

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SamplePaneMultiChoiceOptionWithMultipleTitles.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
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
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", label: ""Segoe UI"", value: ""Segoe UI"", title: ""Font"")]
                [ToolkitSampleMultiChoiceOption(""TextFontFamily"", label: ""Arial"", value: ""Arial"")]
                [ToolkitSampleBoolOption(""Test"", ""Toggle visibility"", false)]
                public partial class Sample
                {{
                }}
            }}";

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SampleGeneratedOptionAttributeOnUnsupportedType.Id, DiagnosticDescriptors.SamplePaneOptionAttributeOnNonSample.Id);
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

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SampleAttributeOnUnsupportedType.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
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

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(source, DiagnosticDescriptors.SampleOptionPaneAttributeOnUnsupportedType.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
        }

        /// <summary>
        /// Verifies the output of a source generator.
        /// </summary>
        /// <typeparam name="TGenerator">The generator type to use.</typeparam>
        /// <param name="source">The input source to process.</param>
        /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
        private static void VerifyGeneratedDiagnostics<TGenerator>(string source, params string[] diagnosticsIds)
            where TGenerator : class, IIncrementalGenerator, new()
        {
            VerifyGeneratedDiagnostics<TGenerator>(CSharpSyntaxTree.ParseText(source), diagnosticsIds);
        }

        /// <summary>
        /// Verifies the output of a source generator.
        /// </summary>
        /// <typeparam name="TGenerator">The generator type to use.</typeparam>
        /// <param name="syntaxTree">The input source tree to process.</param>
        /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
        private static void VerifyGeneratedDiagnostics<TGenerator>(SyntaxTree syntaxTree, params string[] diagnosticsIds)
            where TGenerator : class, IIncrementalGenerator, new()
        {
            var sampleAttributeType = typeof(ToolkitSampleAttribute);

            var references =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where !assembly.IsDynamic
                let reference = MetadataReference.CreateFromFile(assembly.Location)
                select reference;

            var compilation = CSharpCompilation.Create(
                "original",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            IIncrementalGenerator generator = new TGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator).WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

            _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

            HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();

            Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds), $"Expected one of [{string.Join(", ", diagnosticsIds)}] diagnostic Ids. Got [{string.Join(", ", resultingIds)}]");

            GC.KeepAlive(sampleAttributeType);
        }
    }
}
