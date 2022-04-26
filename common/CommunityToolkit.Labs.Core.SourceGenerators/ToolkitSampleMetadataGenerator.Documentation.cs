// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

/// <summary>
/// This part of the partial class deals with finding Markdown files and creating
/// <see cref="ToolkitFrontMatter"/> metadata from it.
/// </summary>
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

    private void GenerateDocumentRegistry(IncrementalGeneratorInitializationContext context)
    {
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

                    var filepath = file.Path.Split(new string[] { @"\samples\" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

                    return new ToolkitFrontMatter()
                    {
                        Title = title.Text,
                        Description = description.Text,
                        Keywords = keywords.Text,
                        Category = categoryValue,
                        Subcategory = subcategoryValue,
                        FilePath = filepath
                    };
                }
            }).OfType<ToolkitFrontMatter>().ToImmutableArray());
        });
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

        return @$"yield return new {typeof(ToolkitFrontMatter).FullName}() {{ Title = ""{metadata.Title}"", Author = ""{metadata.Author}"", Description = ""{metadata.Description}"", Keywords = ""{metadata.Keywords}"", Category = {categoryParam}, Subcategory = {subcategoryParam}, FilePath = @""{metadata.FilePath}""}};";
    }
}
