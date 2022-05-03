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
    public partial class ToolkitSampleMetadataTests
    {
        //// We currently need at least one sample to test the document registry, so we'll have this for the base cases to share.
        private static readonly string SimpleSource = $@"
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

        [TestMethod]
        public void MissingFrontMatterSection()
        {
            string markdown = @"
            # This is some test documentation...
            Without any front matter.
            ";

            VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(SimpleSource, markdown, DiagnosticDescriptors.MarkdownYAMLFrontMatterException.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
        }
    }
}
