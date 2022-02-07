// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Diagnostics;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CommunityToolkit.Labs.Core.SourceGenerators;

/// <summary>
/// Crawls all referenced projects for <see cref="ToolkitSampleAttribute"/>s and generates a static method that returns metadata for each one found.
/// </summary>
[Generator]
public partial class ToolkitSampleMetadataGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is ClassDeclarationSyntax c && c.AttributeLists.Count > 0,
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node))
            .Where(static m => m is not null)
            .Select(static (x, _) => x!);

        var referencedTypes = context.CompilationProvider
                              .SelectMany((x, _) => x.SourceModule.ReferencedAssemblySymbols)
                              .SelectMany((asm, _) => asm.GlobalNamespace.CrawlForAllNamedTypes())
                              .Where(x => x.TypeKind == TypeKind.Class && x.CanBeReferencedByName)/*
                              .Where(IsValidXamlControl)*/
                              .Select((x, _) => (ISymbol)x);

        Execute(classes);
        Execute(referencedTypes);

        void Execute(IncrementalValuesProvider<ISymbol> types)
        {
            // Get all attributes + the original type symbol.
            var allAttributeData = types.SelectMany(static (sym, _) => sym.GetAttributes().Select(x => (sym, x)));

            // Get all generated pane option attributes + the original type symbol.
            var generatedPaneOptions = allAttributeData.Select(static (x, _) =>
            {
                if (x.Item2.TryReconstructAs<ToolkitSampleBoolOptionAttribute>() is ToolkitSampleBoolOptionAttribute boolOptionAttribute)
                    return (x.Item1, (ToolkitSampleOptionBaseAttribute)boolOptionAttribute);

                if (x.Item2.TryReconstructAs<ToolkitSampleMultiChoiceOptionAttribute>() is ToolkitSampleMultiChoiceOptionAttribute multiChoiceOptionAttribute)
                    return (x.Item1, (ToolkitSampleOptionBaseAttribute)multiChoiceOptionAttribute);

                return default;
            }).Collect();

            // Find and reconstruct relevant attributes (with pane options)
            var toolkitSampleAttributeData = allAttributeData.Select(static (data, _) =>
            {
                if (data.Item2.TryReconstructAs<ToolkitSampleAttribute>() is ToolkitSampleAttribute sampleAttribute)
                {
                    return (Attribute: sampleAttribute, AttachedQualifiedTypeName: data.Item1.ToString(), Symbol: data.Item1);
                }

                return default;
            }).Collect();

            var optionsPaneAttributes = allAttributeData
                .Select(static (x, _) => (x.Item2.TryReconstructAs<ToolkitSampleOptionsPaneAttribute>(), x.Item1))
                .Where(static x => x.Item1 is not null)
                .Collect();

            var all = optionsPaneAttributes
                .Combine(toolkitSampleAttributeData)
                .Combine(generatedPaneOptions);

            context.RegisterSourceOutput(all, (ctx, data) =>
            {
                var toolkitSampleAttributeData = data.Left.Right.Where(x => x != default).Distinct();
                var optionsPaneAttribute = data.Left.Left.Where(x => x != default).Distinct();
                var generatedOptionPropertyData = data.Right.Where(x => x != default).Distinct();

                // Check for generated options which don't have a valid sample attribute
                var generatedOptionsWithMissingSampleAttribute = generatedOptionPropertyData.Where(x => !toolkitSampleAttributeData.Any(sample => ReferenceEquals(sample.Symbol, x.Item1)));

                foreach (var item in generatedOptionsWithMissingSampleAttribute)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SamplePaneOptionAttributeOnNonSample, item.Item1.Locations.FirstOrDefault()));

                // Check for generated options with an empty or invalid name.
                var generatedOptionsWithBadName = generatedOptionPropertyData.Where(x => string.IsNullOrWhiteSpace(x.Item2.Name) ||
                                                                                        !x.Item2.Name.Any(char.IsLetterOrDigit) ||
                                                                                        x.Item2.Name.Any(char.IsWhiteSpace) ||
                                                                                        SyntaxFacts.GetKeywordKind(x.Item2.Name) != SyntaxKind.None);

                foreach (var item in generatedOptionsWithBadName)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SamplePaneOptionWithBadName, item.Item1.Locations.FirstOrDefault()));

                // Check for options pane attributes with no matching sample ID
                var optionsPaneAttributeWithMissingOrInvalidSampleId = optionsPaneAttribute.Where(x => !toolkitSampleAttributeData.Any(sample => sample.Attribute.Id == x.Item1?.SampleId));

                foreach (var item in optionsPaneAttributeWithMissingOrInvalidSampleId)
                    ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.OptionsPaneAttributeWithMissingOrInvalidSampleId, item.Item2.Locations.FirstOrDefault()));

                // Reconstruct sample metadata from attributes
                var sampleMetadata = toolkitSampleAttributeData
                     .Select(sample =>
                        new ToolkitSampleRecord(
                            sample.Attribute.Category,
                            sample.Attribute.Subcategory,
                            sample.Attribute.DisplayName,
                            sample.Attribute.Description,
                            sample.AttachedQualifiedTypeName,
                            optionsPaneAttribute.FirstOrDefault(x => x.Item1?.SampleId == sample.Attribute.Id).Item2?.ToString(),
                            generatedOptionPropertyData.Where(x => ReferenceEquals(x.Item1, sample.Symbol)).Select(x => x.Item2)
                            )
                    );

                if (!sampleMetadata.Any())
                    return;

                // Build source string
                var source = BuildRegistrationCallsFromMetadata(sampleMetadata);
                ctx.AddSource($"ToolkitSampleRegistry.g.cs", source);
            });
        }
    }

    private static string BuildRegistrationCallsFromMetadata(IEnumerable<ToolkitSampleRecord> sampleMetadata)
    {
        return $@"#nullable enable
namespace CommunityToolkit.Labs.Core.SourceGenerators;

public static class ToolkitSampleRegistry
{{
    public static System.Collections.Generic.IEnumerable<{typeof(ToolkitSampleMetadata).FullName}> Execute()
    {{
        {
        string.Join("\n        ", sampleMetadata.Select(MetadataToRegistryCall).ToArray())
    }
    }}
}}";
    }

    private static string MetadataToRegistryCall(ToolkitSampleRecord metadata)
    {
        var sampleOptionsParam = metadata.SampleOptionsAssemblyQualifiedName is null ? "null" : $"typeof({metadata.SampleOptionsAssemblyQualifiedName})";
        var categoryParam = $"{nameof(ToolkitSampleCategory)}.{metadata.Category}";
        var subcategoryParam = $"{nameof(ToolkitSampleSubcategory)}.{metadata.Subcategory}";
        var containingClassTypeParam = $"typeof({metadata.SampleAssemblyQualifiedName})";
        var generatedSampleOptionsParam = $"new {typeof(IToolkitSampleOptionViewModel).FullName}[] {{ {string.Join(", ", BuildNewGeneratedSampleOptionMetadataSource(metadata).ToArray())} }}";

        return @$"yield return new {typeof(ToolkitSampleMetadata).FullName}({categoryParam}, {subcategoryParam}, ""{metadata.DisplayName}"", ""{metadata.Description}"", {containingClassTypeParam}, {sampleOptionsParam}, {generatedSampleOptionsParam});";
    }

    private static IEnumerable<string> BuildNewGeneratedSampleOptionMetadataSource(ToolkitSampleRecord sample)
    {
        // Handle group-able items
        var multiChoice = sample.GeneratedSampleOptions.Where(x => x is ToolkitSampleMultiChoiceOptionAttribute)
                                                       .Cast<ToolkitSampleMultiChoiceOptionAttribute>()
                                                       .GroupBy(x => x.Name);

        foreach (var item in multiChoice)
        {
            yield return $@"new {typeof(ToolkitSampleMultiChoiceOptionMetadataViewModel).FullName}(name: ""{item.Key}"", options: new[] {{ {string.Join(",", item.Select(x => $@"new {typeof(MultiChoiceOption).FullName}(""{x.Label}"", ""{x.Value}"")").ToArray())} }}, title: ""{item.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Title)).Title}"")";
        }

        var remainingItems = sample.GeneratedSampleOptions?.Except(multiChoice.SelectMany(x => x));

        // Handle non-grouped items
        foreach (var item in remainingItems ?? Enumerable.Empty<ToolkitSampleOptionBaseAttribute>())
        {
            if (item is ToolkitSampleBoolOptionAttribute boolAttribute)
            {
                yield return $@"new {typeof(ToolkitSampleBoolOptionMetadataViewModel).FullName}(id: ""{boolAttribute.Name}"", label: ""{boolAttribute.Label}"", defaultState: {boolAttribute.DefaultState?.ToString().ToLower()}, title: ""{boolAttribute.Title}"")";
            }
            else
            {
                throw new NotSupportedException($"Unsupported or unhandled type {item.GetType()}.");
            }
        }
    }

    /// <summary>
    /// Checks if a symbol's is or inherits from a type representing a XAML framework.
    /// </summary>
    /// <returns><see langwork="true"/> if the <paramref name="symbol"/> is or inherits from a type representing a XAML framework.</returns>
    private static bool IsValidXamlControl(INamedTypeSymbol symbol)
    {
        // In UWP, Page inherits UserControl
        // In Uno, Page doesn't appear to inherit UserControl.
        var validSimpleTypeNames = new[] { "UserControl", "Page" };

        // UWP / Uno / WinUI 3 namespaces.
        var validNamespaceRoots = new[] { "Microsoft", "Windows" };

        // Recursively crawl the base types until either UserControl or Page is found.
        var validInheritedSymbol = symbol.CrawlBy(x => x?.BaseType, baseType => validNamespaceRoots.Any(x => $"{baseType}".StartsWith(x)) &&
                                                                    $"{baseType}".Contains(".UI.Xaml.Controls.") &&
                                                                    validSimpleTypeNames.Any(x => $"{baseType}".EndsWith(x)));

        var typeIsAccessible = symbol.DeclaredAccessibility == Accessibility.Public;

        return validInheritedSymbol != default && typeIsAccessible && !symbol.IsStatic;
    }
}
