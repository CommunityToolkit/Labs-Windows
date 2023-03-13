// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Tooling.SampleGen.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
public static class DiagnosticDescriptors
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a derived <see cref="Attributes.ToolkitSampleOptionBaseAttribute"/> used on a member that isn't a toolkit sample.
    /// <para>
    /// Format: <c>"Cannot generate sample pane options for type {0} as it does not use ToolkitSampleAttribute"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SamplePaneOptionAttributeOnNonSample = new(
        id: "TKSMPL0001",
        title: $"Invalid sample option declaration",
        messageFormat: $"Cannot generate sample pane options for type {{0}} as it does not use {nameof(Attributes.ToolkitSampleAttribute)}",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample pane options for a type which does not use {nameof(Attributes.ToolkitSampleAttribute)}.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a derived <see cref="Attributes.ToolkitSampleOptionBaseAttribute"/> with an empty or invalid name.
    /// <para>
    /// Format: <c>"Cannot generate sample pane options for type {0} as it contains an empty or invalid name."</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SamplePaneOptionWithBadName = new(
        id: "TKSMPL0002",
        title: $"Invalid sample option declaration",
        messageFormat: $"Cannot generate sample pane options for type {{0}} as the provided name is empty or invalid",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample pane options when the provided name is empty or invalid.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a <see cref="Attributes.ToolkitSampleOptionsPaneAttribute"/> with a <see cref="Attributes.ToolkitSampleOptionsPaneAttribute.SampleId"/> that doesn't have a corresponding <see cref="Attributes.ToolkitSampleAttribute.Id"/>.
    /// <para>
    /// Format: <c>"Cannot link sample options pane to type {0} as the provided sample ID does not match any known {nameof(Attributes.ToolkitSampleAttribute)}"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor OptionsPaneAttributeWithMissingOrInvalidSampleId = new(
        id: "TKSMPL0003",
        title: $"Missing or invalid sample Id",
        messageFormat: $"Cannot link sample options pane to type {{0}} as the provided sample ID does not match any known {nameof(Attributes.ToolkitSampleAttribute)}",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot link sample options pane to a provided sample ID that does not match any known {nameof(Attributes.ToolkitSampleAttribute)}.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a derived <see cref="Attributes.ToolkitSampleOptionBaseAttribute"/> that contains a name which is already in use by another sample option.
    /// <para>
    /// Format: <c>"Cannot generate sample pane options with name {0} as the provided name is already in use by another sample option"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SamplePaneOptionWithDuplicateName = new(
        id: "TKSMPL0004",
        title: $"Duplicate sample option name",
        messageFormat: $"Cannot generate sample pane option with name {{0}} as the provided name is already in use by another sample option",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample pane option when the provided name is used by another sample option.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a derived <see cref="Attributes.ToolkitSampleOptionBaseAttribute"/> that contains a name which is already defined as a member in the attached class.
    /// <para>
    /// Format: <c>"Cannot generate sample pane options with name {0} the provided name is already defined as a member in the attached class"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SamplePaneOptionWithConflictingName = new(
        id: "TKSMPL0005",
        title: $"Conflicting sample option name",
        messageFormat: $"Cannot generate sample pane option with name {{0}} as the provided name is already defined as a member in the attached class",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample pane option when the provided name is already defined as a member in the attached class.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a <see cref="Attributes.ToolkitSampleMultiChoiceOptionAttribute"/> that has no defined options to present to the user.
    /// <para>
    /// Format: <c>"Cannot generate multiple choice sample pane option with title {{0}} as no choices were provided"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SamplePaneMultiChoiceOptionWithNoChoices = new(
        id: "TKSMPL0006",
        title: $"Missing choices in multi-choice sample option",
        messageFormat: $"Cannot generate multiple choice sample pane option with title {{0}} as no choices were provided.",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate multiple choice sample pane option as there are no choices provided.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a <see cref="Attributes.ToolkitSampleAttribute"/> that was used on an unsupported type.
    /// <para>
    /// Format: <c>"Cannot generate sample metadata as the attribute was used on an unsupported type."</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SampleAttributeOnUnsupportedType = new(
        id: "TKSMPL0007",
        title: $"ToolkitSampleAttribute declared on an invalid type",
        messageFormat: $"Cannot generate sample metadata as the attribute was used on an unsupported type",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample metadata as the attribute was used on an unsupported type.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a <see cref="Attributes.ToolkitSampleOptionsPaneAttribute"/> that was used on an unsupported type.
    /// <para>
    /// Format: <c>"Cannot generate options pane metadata as the attribute was used on an unsupported type."</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SampleOptionPaneAttributeOnUnsupportedType = new(
        id: "TKSMPL0008",
        title: $"Toolkit sample options pane declared on an invalid type",
        messageFormat: $"Cannot generate options pane metadata as the attribute was used on an unsupported type",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate options pane metadata as the attribute was used on an unsupported type.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a derived <see cref="Attributes.ToolkitSampleOptionBaseAttribute"/> that was used on an unsupported type.
    /// <para>
    /// Format: <c>"Cannot generate sample option metadata as the attribute was used on an unsupported type."</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SampleGeneratedOptionAttributeOnUnsupportedType = new(
        id: "TKSMPL0009",
        title: $"Toolkit sample option declared on an invalid type",
        messageFormat: $"Cannot generate sample option metadata as the attribute was used on an unsupported type",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample option metadata as the attribute was used on an unsupported type.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating an exception occured while parsing the front matter of a markdown sample file.
    /// </summary>
    public static readonly DiagnosticDescriptor MarkdownYAMLFrontMatterException = new(
        id: "TKSMPL0010",
        title: $"Invalid YAML Front Matter",
        messageFormat: $"Cannot generate sample page info for file {{0}} as an error was encountered parsing its YAML front matter: {{1}}",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample page info due to a YAML Front Matter parsing exception.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating an expected piece of metadata was missing from the front matter of a markdown sample file.
    /// </summary>
    public static readonly DiagnosticDescriptor MarkdownYAMLFrontMatterMissingField = new(
        id: "TKSMPL0011",
        title: $"Missing YAML Front Matter",
        messageFormat: $"Cannot generate sample page info for file {{0}} as no '{{1}}' field was found in its YAML front matter.",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample page info due to missing YAML Front Matter.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating the sample referenced in the Markdown file couldn't be found.
    /// </summary>
    public static readonly DiagnosticDescriptor MarkdownSampleIdNotFound = new(
        id: "TKSMPL0012",
        title: $"Sample Id Not Found from Markdown Reference",
        messageFormat: $"Cannot find the sample page referenced in file {{0}} with sample id '{{1}}'.",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot link sample page info due to invalid/unknown id.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating the sample is not referenced in any Markdown documentation files.
    /// </summary>
    public static readonly DiagnosticDescriptor SampleNotReferencedInMarkdown = new(
        id: "TKSMPL0013",
        title: $"Sample Not Referenced In Documentation",
        messageFormat: $"The sample with id '{{0}}' is not referenced in any documentation files and will not appear in the full sample app.",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: $"Cannot find reference to the sample within the documentation.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating the documentation file contains no sample references.
    /// </summary>
    public static readonly DiagnosticDescriptor DocumentationHasNoSamples = new(
        id: "TKSMPL0014",
        title: $"Documentation Has No Samples",
        messageFormat: $"The documentation page '{{0}}' does not reference any sample examples, it is recommended to have at least one code sample per document page.",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: $"Document contains no interactive sample code.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating a sample ID is already in use by another sample.
    /// <para>
    /// Format: <c>"Cannot generate sample with id {0} as the provided id is already in use by another sample"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor SampleIdAlreadyInUse = new(
        id: "TKSMPL0015",
        title: $"Sample ID already in use",
        messageFormat: $"Cannot generate sample with id {{0}} as the provided id is already in use by another sample",
        category: typeof(ToolkitSampleMetadataGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate sample as the id is already in use by another sample.");
}
