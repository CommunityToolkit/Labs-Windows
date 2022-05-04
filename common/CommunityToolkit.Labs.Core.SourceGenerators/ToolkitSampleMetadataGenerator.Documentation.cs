// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Diagnostics;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace CommunityToolkit.Labs.Core.SourceGenerators;

// This part of the partial class deals with finding Markdown files
// and creating ToolkitFrontMatter metadata from it.
public partial class ToolkitSampleMetadataGenerator
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

    private const string MarkdownRegexSampleTagExpression = @"^>\s*\[!SAMPLE\s*(?<sampleid>.*)\s*\]\s*$";
    private static readonly Regex MarkdownRegexSampleTag = new Regex(MarkdownRegexSampleTagExpression, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private void DiagnoseAndGenerateDocumentRegistry(SourceProductionContext ctx, Dictionary<string, ToolkitSampleRecord> sampleMetadata, IEnumerable<AdditionalText> markdownFileData, IEnumerable<(ToolkitSampleAttribute Attribute, string AttachedQualifiedTypeName, ISymbol Symbol)> toolkitSampleAttributeData)
    {
        // Keep track of all sample ids and remove them as we reference them so we know if we have any unreferenced samples.
        var sampleIdReferences = sampleMetadata.Keys.ToList();

        var docFrontMatter = GatherDocumentFrontMatter(ctx, markdownFileData);

        foreach (var matter in docFrontMatter)
        {
            foreach (var id in matter.SampleIdReferences!)
            {
                if (!sampleMetadata.ContainsKey(id))
                {
                    var fullpath = markdownFileData.First(file => file.Path.Contains(matter.FilePath)).Path;

                    // TODO: Figure out line location
                    ctx.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.MarkdownSampleIdNotFound,
                            Location.Create(fullpath, TextSpan.FromBounds(0, 1), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
                            matter.FilePath,
                            id));
                }
                else
                {
                    sampleIdReferences.Remove(id);
                }
            }
        }

        AddDocuments(ctx, docFrontMatter);

        // Emit warnings for any unreferenced samples
        foreach (var id in sampleIdReferences)
        {
            var sample = toolkitSampleAttributeData.First(sample => sample.Attribute.Id == id);

            ctx.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptors.SampleNotReferencedInMarkdown,
                    sample.Symbol.Locations.FirstOrDefault(),
                    id));
        }
    }

    private ImmutableArray<ToolkitFrontMatter> GatherDocumentFrontMatter(SourceProductionContext ctx, IEnumerable<AdditionalText> data)
    {
        return data.Select(file =>
        {
            // We have to manually parse the YAML here for now because of
            // https://github.com/dotnet/roslyn/issues/43903

            var content = file.GetText()!.ToString();
            var matter = content.Split(new[] { "---" }, StringSplitOptions.RemoveEmptyEntries);

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
                var frontmatter = matter[0];

                // Grab all front matter fields using RegEx expressions.
                var title = ParseYamlField(ref ctx, file.Path, ref frontmatter, FrontMatterRegexTitle, "title");
                var description = ParseYamlField(ref ctx, file.Path, ref frontmatter, FrontMatterRegexDescription, "description");
                var keywords = ParseYamlField(ref ctx, file.Path, ref frontmatter, FrontMatterRegexKeywords, "keywords");
                //// TODO: Should generate the enum from these or something?
                var category = ParseYamlField(ref ctx, file.Path, ref frontmatter, FrontMatterRegexCategory, "category");
                var subcategory = ParseYamlField(ref ctx, file.Path, ref frontmatter, FrontMatterRegexSubcategory, "subcategory");

                // Check we have all the fields we expect to continue (errors will have been spit out otherwise already from the ParseYamlField method)
                if (title == null || description == null || keywords == null ||
                        category == null || subcategory == null)
                {
                    return null;
                }

                // Grab/Check Enum values
                if (!Enum.TryParse<ToolkitSampleCategory>(category, out var categoryValue))
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

                if (!Enum.TryParse<ToolkitSampleSubcategory>(subcategory, out var subcategoryValue))
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

                // Get the filepath we need to be able to load the markdown file in sample app.
                var filepath = file.Path.Split(new string[] { @"\labs\", "/labs/" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

                // Look for sample id tags
                var matches = MarkdownRegexSampleTag.Matches(content);
                var sampleids = new List<string>();

                foreach (Match match in matches)
                {
                    // Validation of these ids occurs outside this function.
                    sampleids.Add(match.Groups["sampleid"].Value.Trim());
                }

                if (sampleids.Count == 0)
                {
                    ctx.ReportDiagnostic(
                        Diagnostic.Create(
                            DiagnosticDescriptors.DocumentationHasNoSamples,
                            Location.Create(file.Path, TextSpan.FromBounds(0, 1), new LinePositionSpan(LinePosition.Zero, LinePosition.Zero)),
                            file.Path));
                }

                // Finally, construct the complete object.
                return new ToolkitFrontMatter()
                {
                    Title = title!,
                    Description = description!,
                    Keywords = keywords!,
                    Category = categoryValue,
                    Subcategory = subcategoryValue,
                    FilePath = filepath,
                    SampleIdReferences = sampleids.ToArray()
                };
            }
        }).OfType<ToolkitFrontMatter>().ToImmutableArray();
    }

    private string? ParseYamlField(ref SourceProductionContext ctx, string filepath, ref string content, Regex pattern, string fieldname)
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
            return null;
        }

        return match.Groups[fieldname].Value.Trim();
    }

    private void AddDocuments(SourceProductionContext ctx, ImmutableArray<ToolkitFrontMatter> matter)
    {
        if (matter.Length > 0)
        {
            var source = BuildRegistrationCallsFromDocuments(matter);
            ctx.AddSource($"ToolkitDocumentRegistry.g.cs", source);
        }

        // TODO: Emit a better error that no documentation is here?
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

        return @$"yield return new {typeof(ToolkitFrontMatter).FullName}() {{ Title = ""{metadata.Title}"", Author = ""{metadata.Author}"", Description = ""{metadata.Description}"", Keywords = ""{metadata.Keywords}"", Category = {categoryParam}, Subcategory = {subcategoryParam}, FilePath = @""{metadata.FilePath}"", SampleIdReferences = new string[] {{ ""{string.Join("\",\"", metadata.SampleIdReferences)}"" }} }};"; // TODO: Add list of sample ids in document
    }
}
