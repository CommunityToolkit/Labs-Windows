// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Diagnostics;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommunityToolkit.Labs.Core.SourceGenerators;

/// <summary>
/// Crawls all referenced projects for <see cref="ToolkitSampleAttribute"/>s and generates a static method that returns metadata for each one found.
/// </summary>
[Generator]
public partial class ToolkitSampleMetadataGenerator : IIncrementalGenerator
{
    private const string FrontMatterRegexTitleExpression = @"^title:\s*(?<title>.*)$";
    private static readonly Regex FrontMatterRegexTitle = new Regex(FrontMatterRegexTitleExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private const string FrontMatterRegexDescriptionExpression = @"^description:\s*(?<description>.*)$";
    private static readonly Regex FrontMatterRegexDescription = new Regex(FrontMatterRegexDescriptionExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private const string FrontMatterRegexKeywordsExpression = @"^keywords:\s*(?<keywords>.*)$";
    private static readonly Regex FrontMatterRegexKeywords = new Regex(FrontMatterRegexKeywordsExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private const string FrontMatterRegexCategoryExpression = @"^category:\s*(?<category>.*)$";
    private static readonly Regex FrontMatterRegexCategory = new Regex(FrontMatterRegexCategoryExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private const string FrontMatterRegexSubcategoryExpression = @"^subcategory:\s*(?<subcategory>.*)$";
    private static readonly Regex FrontMatterRegexSubcategory = new Regex(FrontMatterRegexSubcategoryExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TODO: Let's further break this down into more functions and split into two partial files?
        var markdown = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(".md")); // TODO: file.Path.Contains("samples") - this seems to break things?

        context.RegisterSourceOutput(markdown.Collect(), (ctx, data) =>
        {
            AddDocuments(ctx, data.Select(file =>
            {
                // We have to manually parse the YAML here for now because of
                // https://github.com/dotnet/roslyn/issues/43903

                var matter = file.GetText()!.ToString().Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries);

                if (matter.Length <= 1)
                {
                    ctx.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.MarkdownYAMLFrontMatterException,
                            Location.Create(file.Path, TextSpan.FromBounds(0, 1), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
                            file.Path,
                            "No Front Matter Section Found. Exclude md file from AdditionalFiles in csproj if this is not a documentation file."));
                    return null;
                }
                else
                {
                    var content = matter[0];

                    var title = ParseYamlField(ref ctx, file.Path, ref content, FrontMatterRegexTitle, "title");
                    var description = ParseYamlField(ref ctx, file.Path, ref content, FrontMatterRegexDescription, "description");
                    var keywords = ParseYamlField(ref ctx, file.Path, ref content, FrontMatterRegexKeywords, "keywords");
                    /// TODO: Should generate the enum from these or something?
                    var category = ParseYamlField(ref ctx, file.Path, ref content, FrontMatterRegexCategory, "category");
                    var subcategory = ParseYamlField(ref ctx, file.Path, ref content, FrontMatterRegexSubcategory, "subcategory");

                    if (!(title.Success && description.Success && keywords.Success &&
                          category.Success && subcategory.Success))
                    {
                        return null;
                    }

                    if (!Enum.TryParse<ToolkitSampleCategory>(category.Text, out var categoryValue))
                    {
                        // TODO: extract index to get proper line number?
                        ctx.ReportDiagnostic(
                            Diagnostic.Create(
                                DiagnosticDescriptors.MarkdownYAMLFrontMatterException,
                                Location.Create(file.Path, TextSpan.FromBounds(0, 1), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
                                file.Path,
                                "Can't parse category field, use value from ToolkitSampleCategory enum."));
                        return null;
                    }

                    if (!Enum.TryParse<ToolkitSampleSubcategory>(subcategory.Text, out var subcategoryValue))
                    {
                        // TODO: extract index to get proper line number?
                        ctx.ReportDiagnostic(
                            Diagnostic.Create(
                                DiagnosticDescriptors.MarkdownYAMLFrontMatterException,
                                Location.Create(file.Path, TextSpan.FromBounds(0, 1), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
                                file.Path,
                                "Can't parse subcategory field, use value from ToolkitSampleSubcategory enum."));
                        return null;
                    }

                    return new ToolkitFrontMatter()
                    {
                        Title = title.Text,
                        Description = description.Text,
                        Keywords = keywords.Text,
                        Category = categoryValue,
                        Subcategory = subcategoryValue,
                    };
                }
            }).OfType<ToolkitFrontMatter>().ToImmutableArray());
        });

        var classes = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is ClassDeclarationSyntax c && c.AttributeLists.Count > 0,
                static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node))
            .Where(static m => m is not null)
            .Select(static (x, _) => x!);

        var referencedTypes = context.CompilationProvider
                              .SelectMany((x, _) => x.SourceModule.ReferencedAssemblySymbols)
                              .SelectMany((asm, _) => asm.GlobalNamespace.CrawlForAllNamedTypes())
                              .Where(x => x.TypeKind == TypeKind.Class && x.CanBeReferencedByName)
                              .Select((x, _) => (ISymbol)x);

        Execute(classes);
        Execute(referencedTypes);

        void Execute(IncrementalValuesProvider<ISymbol> types)
        {
            // Get all attributes + the original type symbol.
            var allAttributeData = types.SelectMany(static (sym, _) => sym.GetAttributes().Select(x => (sym, x)));

            // Find and reconstruct generated pane option attributes + the original type symbol.
            var generatedPaneOptions = allAttributeData.Select(static (x, _) =>
            {
                if (x.Item2.TryReconstructAs<ToolkitSampleBoolOptionAttribute>() is ToolkitSampleBoolOptionAttribute boolOptionAttribute)
                    return (x.Item1, (ToolkitSampleOptionBaseAttribute)boolOptionAttribute);

                if (x.Item2.TryReconstructAs<ToolkitSampleMultiChoiceOptionAttribute>() is ToolkitSampleMultiChoiceOptionAttribute multiChoiceOptionAttribute)
                    return (x.Item1, (ToolkitSampleOptionBaseAttribute)multiChoiceOptionAttribute);

                return default;
            }).Collect();

            // Find and reconstruct sample attributes
            var toolkitSampleAttributeData = allAttributeData.Select(static (data, _) =>
            {
                if (data.Item2.TryReconstructAs<ToolkitSampleAttribute>() is ToolkitSampleAttribute sampleAttribute)
                    return (Attribute: sampleAttribute, AttachedQualifiedTypeName: data.Item1.ToString(), Symbol: data.Item1);

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

                ReportDiagnostics(ctx, toolkitSampleAttributeData, optionsPaneAttribute, generatedOptionPropertyData);

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

                // TODO: Check that samples referencesd in Markdown files match known samples.
                // Throw error if markdown references one that's not found
                // Throw warning? if sample exists but isn't referenced in markdown

                if (!sampleMetadata.Any())
                    return;

                // Build source string
                var source = BuildRegistrationCallsFromMetadata(sampleMetadata);
                ctx.AddSource($"ToolkitSampleRegistry.g.cs", source);
            });
        }
    }

    private (bool Success, string? Text) ParseYamlField(ref SourceProductionContext ctx, string filepath, ref string content, Regex pattern, string fieldname)
    {
        var match = pattern.Match(content);

        if (!match.Success)
        {
            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.MarkdownYAMLFrontMatterMissingField,
                    Location.Create(filepath, TextSpan.FromBounds(0, 1), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
                    filepath,
                    fieldname));
            return (false, null);
        }

        return (true, match.Groups[fieldname].Value.Trim());
    }

    private void AddDocuments(SourceProductionContext ctx, ImmutableArray<ToolkitFrontMatter> matter)
    {
        if (matter.Length > 0)
        {
            var source = BuildRegistrationCallsFromDocuments(matter);
            ctx.AddSource($"ToolkitDocumentRegistry.g.cs", source);
        }
    }

    private static void ReportDiagnostics(SourceProductionContext ctx,
                                          IEnumerable<(ToolkitSampleAttribute Attribute, string AttachedQualifiedTypeName, ISymbol Symbol)> toolkitSampleAttributeData,
                                          IEnumerable<(ToolkitSampleOptionsPaneAttribute?, ISymbol)> optionsPaneAttribute,
                                          IEnumerable<(ISymbol, ToolkitSampleOptionBaseAttribute)> generatedOptionPropertyData)
    {
        ReportDiagnosticsForInvalidAttributeUsage(ctx, toolkitSampleAttributeData, optionsPaneAttribute, generatedOptionPropertyData);
        ReportDiagnosticsForLinkedOptionsPane(ctx, toolkitSampleAttributeData, optionsPaneAttribute);
        ReportDiagnosticsGeneratedOptionsPane(ctx, toolkitSampleAttributeData, generatedOptionPropertyData);
    }

    private static void ReportDiagnosticsForInvalidAttributeUsage(SourceProductionContext ctx,
                                                                  IEnumerable<(ToolkitSampleAttribute Attribute, string AttachedQualifiedTypeName, ISymbol Symbol)> toolkitSampleAttributeData,
                                                                  IEnumerable<(ToolkitSampleOptionsPaneAttribute?, ISymbol)> optionsPaneAttribute,
                                                                  IEnumerable<(ISymbol, ToolkitSampleOptionBaseAttribute)> generatedOptionPropertyData)
    {
        var toolkitAttributesOnUnsupportedType = toolkitSampleAttributeData.Where(x => x.Symbol is not INamedTypeSymbol namedSym || !IsValidXamlControl(namedSym));
        var optionsAttributeOnUnsupportedType = optionsPaneAttribute.Where(x => x.Item2 is not INamedTypeSymbol namedSym || !IsValidXamlControl(namedSym));
        var generatedOptionAttributeOnUnsupportedType = generatedOptionPropertyData.Where(x => x.Item1 is not INamedTypeSymbol namedSym || !IsValidXamlControl(namedSym));


        foreach (var item in toolkitAttributesOnUnsupportedType)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SampleAttributeOnUnsupportedType, item.Symbol.Locations.FirstOrDefault()));


        foreach (var item in optionsAttributeOnUnsupportedType)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SampleOptionPaneAttributeOnUnsupportedType, item.Item2.Locations.FirstOrDefault()));


        foreach (var item in generatedOptionAttributeOnUnsupportedType)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SampleGeneratedOptionAttributeOnUnsupportedType, item.Item1.Locations.FirstOrDefault()));

    }

    private static void ReportDiagnosticsForLinkedOptionsPane(SourceProductionContext ctx,
                                                              IEnumerable<(ToolkitSampleAttribute Attribute, string AttachedQualifiedTypeName, ISymbol Symbol)> toolkitSampleAttributeData,
                                                              IEnumerable<(ToolkitSampleOptionsPaneAttribute?, ISymbol)> optionsPaneAttribute)
    {
        // Check for options pane attributes with no matching sample ID
        var optionsPaneAttributeWithMissingOrInvalidSampleId = optionsPaneAttribute.Where(x => !toolkitSampleAttributeData.Any(sample => sample.Attribute.Id == x.Item1?.SampleId));

        foreach (var item in optionsPaneAttributeWithMissingOrInvalidSampleId)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.OptionsPaneAttributeWithMissingOrInvalidSampleId, item.Item2.Locations.FirstOrDefault()));
    }

    private static void ReportDiagnosticsGeneratedOptionsPane(SourceProductionContext ctx,
                                                              IEnumerable<(ToolkitSampleAttribute Attribute, string AttachedQualifiedTypeName, ISymbol Symbol)> toolkitSampleAttributeData,
                                                              IEnumerable<(ISymbol, ToolkitSampleOptionBaseAttribute)> generatedOptionPropertyData)
    {
        ReportGeneratedMultiChoiceOptionsPaneDiagnostics(ctx, generatedOptionPropertyData);

        // Check for generated options which don't have a valid sample attribute
        var generatedOptionsWithMissingSampleAttribute = generatedOptionPropertyData.Where(x => !toolkitSampleAttributeData.Any(sample => ReferenceEquals(sample.Symbol, x.Item1)));

        foreach (var item in generatedOptionsWithMissingSampleAttribute)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SamplePaneOptionAttributeOnNonSample, item.Item1.Locations.FirstOrDefault()));

        // Check for generated options with an empty or invalid name.
        var generatedOptionsWithBadName = generatedOptionPropertyData.Where(x => string.IsNullOrWhiteSpace(x.Item2.Name) || // Must not be null or empty
                                                                                !x.Item2.Name.Any(char.IsLetterOrDigit) || // Must be alphanumeric
                                                                                x.Item2.Name.Any(char.IsWhiteSpace) || // Must not have whitespace
                                                                                SyntaxFacts.GetKeywordKind(x.Item2.Name) != SyntaxKind.None); // Must not be a reserved keyword

        foreach (var item in generatedOptionsWithBadName)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SamplePaneOptionWithBadName, item.Item1.Locations.FirstOrDefault(), item.Item1.ToString()));

        // Check for generated options with duplicate names.
        var generatedOptionsWithDuplicateName = generatedOptionPropertyData.GroupBy(x => x.Item1, SymbolEqualityComparer.Default) // Group by containing symbol (allow reuse across samples)
                                                                           .SelectMany(y => y.GroupBy(x => x.Item2.Name) // In this symbol, group options by name.
                                                                                             .Where(x => x.Any(x => x.Item2 is not ToolkitSampleMultiChoiceOptionAttribute)) // Exclude Multichoice. 
                                                                                             .Where(x => x.Count() > 1)); // Options grouped by name should only contain 1 item.

        foreach (var item in generatedOptionsWithDuplicateName)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SamplePaneOptionWithDuplicateName, item.SelectMany(x => x.Item1.Locations).FirstOrDefault(), item.Key));

        // Check for generated options that conflict with an existing property name
        var generatedOptionsWithConflictingPropertyNames = generatedOptionPropertyData.Where(x => GetAllMembers((INamedTypeSymbol)x.Item1).Any(y => x.Item2.Name == y.Name));

        foreach (var item in generatedOptionsWithConflictingPropertyNames)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SamplePaneOptionWithConflictingName, item.Item1.Locations.FirstOrDefault(), item.Item2.Name));
    }

    private static void ReportGeneratedMultiChoiceOptionsPaneDiagnostics(SourceProductionContext ctx, IEnumerable<(ISymbol, ToolkitSampleOptionBaseAttribute)> generatedOptionPropertyData)
    {
        var generatedMultipleChoiceOptionWithMultipleTitles = new List<(ISymbol, ToolkitSampleOptionBaseAttribute)>();

        var multiChoiceOptionsGroupedBySymbol = generatedOptionPropertyData.GroupBy(x => x.Item1, SymbolEqualityComparer.Default)
                                                                           .Where(x => x.Any(x => x.Item2 is ToolkitSampleMultiChoiceOptionAttribute));

        foreach (var symbolGroup in multiChoiceOptionsGroupedBySymbol)
        {
            var optionsGroupedByName = symbolGroup.GroupBy(x => x.Item2.Name);

            foreach (var nameGroup in optionsGroupedByName)
            {
                var optionsGroupedByTitle = nameGroup.Where(x => !string.IsNullOrWhiteSpace(x.Item2?.Title))
                                                     .GroupBy(x => x.Item2.Title)
                                                     .SelectMany(x => x);

                if (optionsGroupedByTitle.Count() > 1)
                    generatedMultipleChoiceOptionWithMultipleTitles.Add(optionsGroupedByTitle.First());
            }
        }

        foreach (var item in generatedMultipleChoiceOptionWithMultipleTitles)
            ctx.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.SamplePaneMultiChoiceOptionWithMultipleTitles, item.Item1.Locations.FirstOrDefault(), item.Item2.Title));
    }

    private static string BuildRegistrationCallsFromDocuments(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        return $@"#nullable enable
namespace CommunityToolkit.Labs.Core.SourceGenerators;

public static class ToolkitDocumentRegistry
{{
    public static System.Collections.Generic.IEnumerable<{typeof(ToolkitFrontMatter).FullName}> Execute()
    {{
        {
        string.Join("\n        ", sampleMetadata.Select(FrontMatterToRegistryCall).ToArray())
    }
    }}
}}";
    }

    private static string FrontMatterToRegistryCall(ToolkitFrontMatter metadata)
    {
        var categoryParam = $"{nameof(ToolkitSampleCategory)}.{metadata.Category}";
        var subcategoryParam = $"{nameof(ToolkitSampleSubcategory)}.{metadata.Subcategory}";

        return @$"yield return new {typeof(ToolkitFrontMatter).FullName}() {{ Title = ""{metadata.Title}"", Author = ""{metadata.Author}"", Description = ""{metadata.Description}"", Keywords = ""{metadata.Keywords}"", Category = {categoryParam}, Subcategory = {subcategoryParam}}};";
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
        var categoryParam = $"{nameof(ToolkitSampleCategory)}.{metadata.Category}";
        var subcategoryParam = $"{nameof(ToolkitSampleSubcategory)}.{metadata.Subcategory}";
        var sampleControlTypeParam = $"typeof({metadata.SampleAssemblyQualifiedName})";
        var sampleControlFactoryParam = $"() => new {metadata.SampleAssemblyQualifiedName}()";
        var generatedSampleOptionsParam = $"new {typeof(IGeneratedToolkitSampleOptionViewModel).FullName}[] {{ {string.Join(", ", BuildNewGeneratedSampleOptionMetadataSource(metadata).ToArray())} }}";
        var sampleOptionsParam = metadata.SampleOptionsAssemblyQualifiedName is null ? "null" : $"typeof({metadata.SampleOptionsAssemblyQualifiedName})";
        var sampleOptionsPaneFactoryParam = metadata.SampleOptionsAssemblyQualifiedName is null ? "null" : $"x => new {metadata.SampleOptionsAssemblyQualifiedName}(({metadata.SampleAssemblyQualifiedName})x)";

        return @$"yield return new {typeof(ToolkitSampleMetadata).FullName}({categoryParam}, {subcategoryParam}, ""{metadata.DisplayName}"", ""{metadata.Description}"", {sampleControlTypeParam}, {sampleControlFactoryParam}, {sampleOptionsParam}, {sampleOptionsPaneFactoryParam}, {generatedSampleOptionsParam});";
    }

    private static IEnumerable<string> BuildNewGeneratedSampleOptionMetadataSource(ToolkitSampleRecord sample)
    {
        // Handle group-able items
        var multiChoice = sample.GeneratedSampleOptions.Where(x => x is ToolkitSampleMultiChoiceOptionAttribute)
                                                       .Cast<ToolkitSampleMultiChoiceOptionAttribute>()
                                                       .GroupBy(x => x.Name);

        foreach (var item in multiChoice)
            yield return $@"new {typeof(ToolkitSampleMultiChoiceOptionMetadataViewModel).FullName}(name: ""{item.Key}"", options: new[] {{ {string.Join(",", item.Select(x => $@"new {typeof(MultiChoiceOption).FullName}(""{x.Label}"", ""{x.Value}"")").ToArray())} }}, title: ""{item.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Title))?.Title}"")";

        // Handle non-grouped items
        var remainingItems = sample.GeneratedSampleOptions?.Except(multiChoice.SelectMany(x => x));

        foreach (var item in remainingItems ?? Enumerable.Empty<ToolkitSampleOptionBaseAttribute>())
        {
            if (item is ToolkitSampleBoolOptionAttribute boolAttribute)
                yield return $@"new {typeof(ToolkitSampleBoolOptionMetadataViewModel).FullName}(id: ""{boolAttribute.Name}"", label: ""{boolAttribute.Label}"", defaultState: {boolAttribute.DefaultState?.ToString().ToLower()}, title: ""{boolAttribute.Title}"")";
            else
                throw new NotSupportedException($"Unsupported or unhandled type {item.GetType()}.");
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

    private static IEnumerable<ISymbol> GetAllMembers(INamedTypeSymbol symbol)
    {
        foreach (var item in symbol.GetMembers())
            yield return item;

        if (symbol.BaseType is not null)
            foreach (var item in GetAllMembers(symbol.BaseType))
                yield return item;
    }
}
