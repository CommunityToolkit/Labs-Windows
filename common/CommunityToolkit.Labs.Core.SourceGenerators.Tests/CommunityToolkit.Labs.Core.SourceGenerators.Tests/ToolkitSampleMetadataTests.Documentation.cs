using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Tests;

public partial class ToolkitSampleMetadataTests
{
    // We currently need at least one sample to test the document registry, so we'll have this for the base cases to share.
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

    [DataRow(1, DisplayName = "Title")]
    [DataRow(3, DisplayName = "Description")]
    [DataRow(4, DisplayName = "Keywords")]
    [DataRow(7, DisplayName = "Category")]
    [DataRow(8, DisplayName = "Subcategory")]
    [TestMethod]
    public void MissingFrontMatterField(int removeline)
    {
        string markdown = @"---
title: Canvas Layout
author: mhawker
description: A canvas-like VirtualizingLayout for use in an ItemsRepeater
keywords: CanvasLayout, ItemsRepeater, VirtualizingLayout, Canvas, Layout, Panel, Arrange
dev_langs:
    - csharp
category: Controls
subcategory: Layout
---
# This is some test documentation...
> [!SAMPLE Sample]
Without any front matter.";

        // Remove the field we want to test is missing.
        var lines = markdown.Split('\n').ToList();
        lines.RemoveAt(removeline);
        markdown = String.Join('\n', lines);

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(SimpleSource, markdown, DiagnosticDescriptors.MarkdownYAMLFrontMatterMissingField.Id, DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id); // We won't see the sample reference as we bail out when the front matter fails to be complete...
    }

    [TestMethod]
    public void MarkdownInvalidSampleReference()
    {
        string markdown = @"---
title: Canvas Layout
author: mhawker
description: A canvas-like VirtualizingLayout for use in an ItemsRepeater
keywords: CanvasLayout, ItemsRepeater, VirtualizingLayout, Canvas, Layout, Panel, Arrange
dev_langs:
    - csharp
category: Controls
subcategory: Layout
---
# This is some test documentation...
> [!SAMPLE SampINVALIDle]
Without any front matter.
";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(SimpleSource, markdown,
            DiagnosticDescriptors.MarkdownSampleIdNotFound.Id,
            DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void DocumentationMissingSample()
    {
        string markdown = @"---
title: Canvas Layout
author: mhawker
description: A canvas-like VirtualizingLayout for use in an ItemsRepeater
keywords: CanvasLayout, ItemsRepeater, VirtualizingLayout, Canvas, Layout, Panel, Arrange
dev_langs:
    - csharp
category: Controls
subcategory: Layout
---
# This is some test documentation...
Without any front matter.";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(SimpleSource, markdown,
            DiagnosticDescriptors.DocumentationHasNoSamples.Id,
            DiagnosticDescriptors.SampleNotReferencedInMarkdown.Id);
    }

    [TestMethod]
    public void DocumentationValid()
    {
        string markdown = @"---
title: Canvas Layout
author: mhawker
description: A canvas-like VirtualizingLayout for use in an ItemsRepeater
keywords: CanvasLayout, ItemsRepeater, VirtualizingLayout, Canvas, Layout, Panel, Arrange
dev_langs:
    - csharp
category: Controls
subcategory: Layout
---
# This is some test documentation...
Without any front matter.
> [!SAMPLE Sample]";

        VerifyGeneratedDiagnostics<ToolkitSampleMetadataGenerator>(SimpleSource, markdown);
    }
}
