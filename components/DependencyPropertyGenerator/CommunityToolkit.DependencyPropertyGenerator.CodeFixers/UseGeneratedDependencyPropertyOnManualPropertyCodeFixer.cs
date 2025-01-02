// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
using CommunityToolkit.GeneratedDependencyProperty.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A code fixer that converts manual properties into partial properties using <c>[GeneratedDependencytProperty]</c>.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class UseGeneratedDependencyPropertyOnManualPropertyCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [UseGeneratedDependencyPropertyForManualPropertyId];

    /// <inheritdoc/>
    public override Microsoft.CodeAnalysis.CodeFixes.FixAllProvider? GetFixAllProvider()
    {
        return new FixAllProvider();
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];
        TextSpan diagnosticSpan = context.Span;

        // This code fixer needs the semantic model, so check that first
        if (!context.Document.SupportsSemanticModel)
        {
            return;
        }

        // Get all additional locations we expect from the analyzer
        if (!TryGetAdditionalLocations(
            diagnostic,
            out Location? fieldLocation,
            out Location? propertyTypeExpressionLocation,
            out Location? defaultValueExpressionLocation))
        {
            return;
        }

        // Retrieve the properties passed by the analyzer
        string? defaultValue = diagnostic.Properties[UseGeneratedDependencyPropertyOnManualPropertyAnalyzer.DefaultValuePropertyName];
        string? defaultValueTypeReferenceId = diagnostic.Properties[UseGeneratedDependencyPropertyOnManualPropertyAnalyzer.DefaultValueTypeReferenceIdPropertyName];

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        // Get the property declaration and the field declaration from the target diagnostic
        if (root!.FindNode(diagnosticSpan) is PropertyDeclarationSyntax propertyDeclaration &&
            root.FindNode(fieldLocation.SourceSpan) is FieldDeclarationSyntax fieldDeclaration)
        {
            // Get the semantic model, as we need to resolve symbols
            SemanticModel semanticModel = (await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false))!;

            // Register the code fix to update the semi-auto property to a partial property
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Use a partial property",
                    createChangedDocument: token => ConvertToPartialProperty(
                        context.Document,
                        semanticModel,
                        root,
                        propertyDeclaration,
                        fieldDeclaration,
                        propertyTypeExpressionLocation,
                        defaultValue,
                        defaultValueTypeReferenceId,
                        defaultValueExpressionLocation),
                    equivalenceKey: "Use a partial property"),
                diagnostic);
        }
    }

    /// <summary>
    /// Tries to get an <see cref="AttributeListSyntax"/> for the <c>[GeneratedDependencyProperty]</c> attribute.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current compilation.</param>
    /// <param name="generatedDependencyPropertyAttributeList">The resulting attribute list, if successfully retrieved.</param>
    /// <returns>Whether <paramref name="generatedDependencyPropertyAttributeList"/> could be retrieved successfully.</returns>
    private static bool TryGetGeneratedDependencyPropertyAttributeList(
        Document document,
        SemanticModel semanticModel,
        [NotNullWhen(true)] out AttributeListSyntax? generatedDependencyPropertyAttributeList)
    {
        // Make sure we can resolve the '[GeneratedDependencyProperty]' attribute
        if (semanticModel.Compilation.GetTypeByMetadataName(WellKnownTypeNames.GeneratedDependencyPropertyAttribute) is not INamedTypeSymbol attributeSymbol)
        {
            generatedDependencyPropertyAttributeList = null;

            return false;
        }

        SyntaxGenerator syntaxGenerator = SyntaxGenerator.GetGenerator(document);

        // Create the attribute syntax for the new '[GeneratedDependencyProperty]' attribute here too
        SyntaxNode attributeTypeSyntax = syntaxGenerator.TypeExpression(attributeSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);

        generatedDependencyPropertyAttributeList = (AttributeListSyntax)syntaxGenerator.Attribute(attributeTypeSyntax);

        return true;
    }

    /// <summary>
    /// Updates an <see cref="AttributeListSyntax"/> for the <c>[GeneratedDependencyProperty]</c> attribute with the right default value.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current compilation.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="propertyTypeExpressionLocation">The location of the property type expression to use in metadata, if available.</param>
    /// <param name="defaultValueExpression">The expression for the default value of the property, if present</param>
    /// <param name="defaultValueTypeReferenceId">The documentation comment reference id for type of the default value, if present.</param>
    /// <param name="defaultValueExpressionLocation">The location for the default value, if available.</param>
    /// <returns>The updated attribute syntax.</returns>
    private static AttributeListSyntax UpdateGeneratedDependencyPropertyAttributeList(
        Document document,
        SemanticModel semanticModel,
        SyntaxNode root,
        AttributeListSyntax generatedDependencyPropertyAttributeList,
        Location propertyTypeExpressionLocation,
        string? defaultValueExpression,
        string? defaultValueTypeReferenceId,
        Location defaultValueExpressionLocation)
    {
        void HandlePropertyType(ref AttributeListSyntax generatedDependencyPropertyAttributeList)
        {
            // If the property needs an explicit type in metadata, just carry it over from the original field declaration initializer
            if (root.FindNode(propertyTypeExpressionLocation.SourceSpan) is ArgumentSyntax { Expression: { } propertyTypeOriginalExpression })
            {
                SyntaxGenerator syntaxGenerator = SyntaxGenerator.GetGenerator(document);

                generatedDependencyPropertyAttributeList = (AttributeListSyntax)syntaxGenerator.AddAttributeArguments(
                    generatedDependencyPropertyAttributeList,
                    [syntaxGenerator.AttributeArgument("PropertyType", propertyTypeOriginalExpression)]);
            }
        }

        void HandleDefaultValue(ref AttributeListSyntax generatedDependencyPropertyAttributeList)
        {
            // If we do have a default value expression, set it in the attribute.
            // We extract the generated attribute so we can add the new argument.
            // It's important to reuse it, as it has the "add usings" annotation.
            if (defaultValueExpression is not null)
            {
                SyntaxGenerator syntaxGenerator = SyntaxGenerator.GetGenerator(document);

                // Special case if we have a location for the original expression, and we can resolve the node.
                // In this case, we want to just carry that over with no changes (this is used for named constants).
                // See notes below for how this method is constructing the new attribute argument to insert.
                if (root.FindNode(defaultValueExpressionLocation.SourceSpan) is ArgumentSyntax { Expression: { } defaultValueOriginalExpression })
                {
                    generatedDependencyPropertyAttributeList = (AttributeListSyntax)syntaxGenerator.AddAttributeArguments(
                        generatedDependencyPropertyAttributeList,
                        [syntaxGenerator.AttributeArgument("DefaultValue", defaultValueOriginalExpression)]);

                    return;
                }

                ExpressionSyntax parsedExpression = ParseExpression(defaultValueExpression);

                // Special case values which are simple enum member accesses, like 'global::Windows.UI.Xaml.Visibility.Collapsed'.
                // We have two cases to handle, which require different logic to ensure the correct tree is always generated.
                // For nested enum types, we'll have a reference id. In this case, we manually insert annotations.
                if (defaultValueTypeReferenceId is not null)
                {
                    // Here we're relying on the special 'SymbolId' annotation, which is used internally by Roslyn to track
                    // necessary imports for type expressions. We need to rely on this implementation detail here because
                    // there is no public API to correctly produce a tree for an enum member access on a nested type.
                    // This internal detail is one that many generators take a dependency on already, so it's safe-ish.
                    parsedExpression = parsedExpression.WithAdditionalAnnotations(
                        Simplifier.Annotation,
                        Simplifier.AddImportsAnnotation,
                        new SyntaxAnnotation("SymbolId", defaultValueTypeReferenceId));

                    // Create the attribute argument to insert
                    SyntaxNode attributeArgumentSyntax = syntaxGenerator.AttributeArgument("DefaultValue", parsedExpression);

                    // Actually add the argument to the existing attribute syntax
                    generatedDependencyPropertyAttributeList = (AttributeListSyntax)syntaxGenerator.AddAttributeArguments(generatedDependencyPropertyAttributeList, [attributeArgumentSyntax]);

                    return;
                }

                // For normal enum member accesses, we resolve the type and then construct the tree from that expression.
                if (parsedExpression is MemberAccessExpressionSyntax { Expression: { } expressionSyntax, Name: IdentifierNameSyntax { Identifier.Text: { } memberName } })
                {
                    string fullyQualifiedMetadataName = expressionSyntax.ToFullString();

                    // Ensure we strip the global prefix, if present (it should always be present if we didn't have a metadata name)
                    if (fullyQualifiedMetadataName.StartsWith("global::"))
                    {
                        fullyQualifiedMetadataName = fullyQualifiedMetadataName["global::".Length..];
                    }

                    // Try to resolve the attribute type, if present. This API takes a fully qualified metadata name, not
                    // a fully qualified type name. However, for virtually all cases for enum types, the two should match.
                    // That is, they will be the same if the type is not nested, and not generic, which is what we expect.
                    if (semanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedMetadataName) is INamedTypeSymbol enumTypeSymbol)
                    {
                        // Create the identifier syntax for the enum type, with the right annotations
                        SyntaxNode enumTypeSyntax = syntaxGenerator.TypeExpression(enumTypeSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);

                        // Create the member access expression for the target enum type
                        SyntaxNode enumMemberAccessExpressionSyntax = syntaxGenerator.MemberAccessExpression(enumTypeSyntax, memberName);

                        // Create the attribute argument, like in the previous case
                        generatedDependencyPropertyAttributeList = (AttributeListSyntax)syntaxGenerator.AddAttributeArguments(
                            generatedDependencyPropertyAttributeList,
                            [syntaxGenerator.AttributeArgument("DefaultValue", enumMemberAccessExpressionSyntax)]);

                        return;
                    }
                }

                // Otherwise, just add the new default value normally
                generatedDependencyPropertyAttributeList =
                    AttributeList(SingletonSeparatedList(
                        generatedDependencyPropertyAttributeList.Attributes[0]
                        .AddArgumentListArguments(
                            AttributeArgument(ParseExpression(defaultValueExpression))
                            .WithNameEquals(NameEquals(IdentifierName("DefaultValue"))))));
            }
        }

        HandlePropertyType(ref generatedDependencyPropertyAttributeList);
        HandleDefaultValue(ref generatedDependencyPropertyAttributeList);

        return generatedDependencyPropertyAttributeList;
    }

    /// <summary>
    /// Applies the code fix to a target property declaration and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current compilation.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/> for the property being updated.</param>
    /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/> for the declared property to remove.</param>
    /// <param name="propertyTypeExpressionLocation">The location of the property type expression to use in metadata, if available.</param>
    /// <param name="defaultValueExpression">The expression for the default value of the property, if present</param>
    /// <param name="defaultValueTypeReferenceId">The documentation comment reference id for type of the default value, if present.</param>
    /// <param name="defaultValueExpressionLocation">The location for the default value, if available.</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="propertyDeclaration"/> being replaced with a partial property.</returns>
    private static async Task<Document> ConvertToPartialProperty(
        Document document,
        SemanticModel semanticModel,
        SyntaxNode root,
        PropertyDeclarationSyntax propertyDeclaration,
        FieldDeclarationSyntax fieldDeclaration,
        Location propertyTypeExpressionLocation,
        string? defaultValueExpression,
        string? defaultValueTypeReferenceId,
        Location defaultValueExpressionLocation)
    {
        await Task.CompletedTask;

        // If we can't generate the new attribute list, bail (this should never happen)
        if (!TryGetGeneratedDependencyPropertyAttributeList(document, semanticModel, out AttributeListSyntax? generatedDependencyPropertyAttributeList))
        {
            return document;
        }

        // Create an editor to perform all mutations
        SyntaxEditor syntaxEditor = new(root, document.Project.Solution.Workspace.Services);

        ConvertToPartialProperty(
            document,
            semanticModel,
            root,
            propertyDeclaration,
            fieldDeclaration,
            generatedDependencyPropertyAttributeList,
            syntaxEditor,
            propertyTypeExpressionLocation,
            defaultValueExpression,
            defaultValueTypeReferenceId,
            defaultValueExpressionLocation);

        RemoveLeftoverLeadingEndOfLines([fieldDeclaration], syntaxEditor);

        // Create the new document with the single change
        return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
    }

    /// <summary>
    /// Applies the code fix to a target identifier and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current compilation.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/> for the property being updated.</param>
    /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/> for the declared property to remove.</param>
    /// <param name="generatedDependencyPropertyAttributeList">The <see cref="AttributeListSyntax"/> with the attribute to add.</param>
    /// <param name="syntaxEditor">The <see cref="SyntaxEditor"/> instance to use.</param>
    /// <param name="propertyTypeExpressionLocation">The location of the property type expression to use in metadata, if available.</param>
    /// <param name="defaultValueExpression">The expression for the default value of the property, if present</param>
    /// <param name="defaultValueTypeReferenceId">The documentation comment reference id for type of the default value, if present.</param>
    /// <param name="defaultValueExpressionLocation">The location for the default value, if available.</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="propertyDeclaration"/> being replaced with a partial property.</returns>
    private static void ConvertToPartialProperty(
        Document document,
        SemanticModel semanticModel,
        SyntaxNode root,
        PropertyDeclarationSyntax propertyDeclaration,
        FieldDeclarationSyntax fieldDeclaration,
        AttributeListSyntax generatedDependencyPropertyAttributeList,
        SyntaxEditor syntaxEditor,
        Location propertyTypeExpressionLocation,
        string? defaultValueExpression,
        string? defaultValueTypeReferenceId,
        Location defaultValueExpressionLocation)
    {
        // Replace the property with the partial property using the attribute. Note that it's important to use the
        // lambda 'ReplaceNode' overload here, rather than creating a modifier property declaration syntax node and
        // replacing the original one. Doing that would cause the following 'ReplaceNode' call to adjust the leading
        // trivia of trailing members after the fields being removed to not work incorrectly, and fail to be resolved.
        syntaxEditor.ReplaceNode(propertyDeclaration, (node, _) =>
        {
            PropertyDeclarationSyntax propertyDeclaration = (PropertyDeclarationSyntax)node;

            // Update the attribute to insert with the default value, if present
            generatedDependencyPropertyAttributeList = UpdateGeneratedDependencyPropertyAttributeList(
                document,
                semanticModel,
                root,
                generatedDependencyPropertyAttributeList,
                propertyTypeExpressionLocation,
                defaultValueExpression,
                defaultValueTypeReferenceId,
                defaultValueExpressionLocation);

            // Start setting up the updated attribute lists
            SyntaxList<AttributeListSyntax> attributeLists = propertyDeclaration.AttributeLists;

            if (attributeLists is [AttributeListSyntax firstAttributeListSyntax, ..])
            {
                // Remove the trivia from the original first attribute
                attributeLists = attributeLists.Replace(
                    nodeInList: firstAttributeListSyntax,
                    newNode: firstAttributeListSyntax.WithoutTrivia());

                // If the property has at least an attribute list, move the trivia from it to the new attribute
                generatedDependencyPropertyAttributeList = generatedDependencyPropertyAttributeList.WithTriviaFrom(firstAttributeListSyntax);

                // Insert the new attribute
                attributeLists = attributeLists.Insert(0, generatedDependencyPropertyAttributeList);
            }
            else
            {
                // Otherwise (there are no attribute lists), transfer the trivia to the new (only) attribute list
                generatedDependencyPropertyAttributeList = generatedDependencyPropertyAttributeList.WithTriviaFrom(propertyDeclaration);

                // Save the new attribute list
                attributeLists = attributeLists.Add(generatedDependencyPropertyAttributeList);
            }

            // Append any attributes we want to forward (any attributes on the field, they've already been validated).
            // We also need to strip all trivia, to avoid accidentally carrying over XML docs from the field declaration.
            foreach (AttributeListSyntax fieldAttributeList in fieldDeclaration.AttributeLists)
            {
                attributeLists = attributeLists.Add(fieldAttributeList.WithTarget(AttributeTargetSpecifier(Token(SyntaxKind.StaticKeyword))).WithoutTrivia());
            }

            // Get a new property that is partial and with semicolon token accessors
            return
                propertyDeclaration
                .AddModifiers(Token(SyntaxKind.PartialKeyword))
                .WithoutLeadingTrivia()
                .WithAttributeLists(attributeLists)
                .WithAdditionalAnnotations(Formatter.Annotation)
                .WithAccessorList(AccessorList(List(
                [
                    // Keep the accessors (so we can easily keep all trivia, modifiers, attributes, etc.) but make them semicolon only
                    propertyDeclaration.AccessorList!.Accessors[0]
                    .WithBody(null)
                    .WithExpressionBody(null)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    .WithAdditionalAnnotations(Formatter.Annotation),
                    propertyDeclaration.AccessorList!.Accessors[1]
                    .WithBody(null)
                    .WithExpressionBody(null)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                    .WithTrailingTrivia(propertyDeclaration.AccessorList.Accessors[1].GetTrailingTrivia())
                    .WithAdditionalAnnotations(Formatter.Annotation)
                ])).WithTrailingTrivia(propertyDeclaration.AccessorList.GetTrailingTrivia()));
        });

        // Also remove the field declaration (it'll be generated now)
        syntaxEditor.RemoveNode(fieldDeclaration);

        // Find the parent type for the property (we need to do this for all ancestor types, as the type might be bested)
        for (TypeDeclarationSyntax? typeDeclaration = propertyDeclaration.FirstAncestor<TypeDeclarationSyntax>();
             typeDeclaration is not null;
             typeDeclaration = typeDeclaration.FirstAncestor<TypeDeclarationSyntax>())
        {
            // Make sure it's partial (we create the updated node in the function to preserve the updated property declaration).
            // If we created it separately and replaced it, the whole tree would also be replaced, and we'd lose the new property.
            if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                syntaxEditor.ReplaceNode(typeDeclaration, static (node, generator) => generator.WithModifiers(node, generator.GetModifiers(node).WithPartial(true)));
            }
        }
    }

    /// <summary>
    /// Removes any leftover leading end of lines on remaining members following any removed fields.
    /// </summary>
    /// <param name="fieldDeclarations">The collection of all fields that have been removed.</param>
    /// <param name="syntaxEditor">The <see cref="SyntaxEditor"/> instance to use.</param>
    private static void RemoveLeftoverLeadingEndOfLines(IReadOnlyCollection<FieldDeclarationSyntax> fieldDeclarations, SyntaxEditor syntaxEditor)
    {
        foreach (FieldDeclarationSyntax fieldDeclaration in fieldDeclarations)
        {
            // Special handling for the leading trivia of members following the field declaration we are about to remove.
            // There is an edge case that can happen when a type declaration is as follows:
            //
            // class ContainingType
            // {
            //     public static readonly DependencyProperty NameProperty = ...;
            //
            //     public void SomeOtherMember() { }
            //
            //     public string? Name { ... }
            // }
            //
            // In this case, just removing the target field for the dependency property being rewritten (that is, 'NameProperty')
            // will cause an extra blank line to be left after the edits, right above the member immediately following the field.
            // To work around this, we look for such a member and check its trivia, and then manually remove a leading blank line.
            if (fieldDeclaration.Parent is not TypeDeclarationSyntax fieldParentTypeDeclaration)
            {
                continue;
            }

            int fieldDeclarationIndex = fieldParentTypeDeclaration.Members.IndexOf(fieldDeclaration);

            // Check whether there is a member immediatley following the field
            if (fieldDeclarationIndex == -1 || fieldDeclarationIndex >= fieldParentTypeDeclaration.Members.Count - 1)
            {
                continue;
            }

            MemberDeclarationSyntax nextMember = fieldParentTypeDeclaration.Members[fieldDeclarationIndex + 1];

            // It's especially important to skip members that have been rmeoved. This would otherwise fail when computing
            // the final document. We only care about fixing trivia for members that will still be present after all edits.
            if (fieldDeclarations.Contains(nextMember))
            {
                continue;
            }

            SyntaxTriviaList leadingTrivia = nextMember.GetLeadingTrivia();

            // Check whether this member has a first leading trivia that's just a blank line: we want to remove this one
            if (leadingTrivia is not [SyntaxTrivia(SyntaxKind.EndOfLineTrivia), ..])
            {
                continue;
            }

            bool hasAnyPersistentPrecedingMemberDeclarations = false;

            // Last check: we only want to actually remove the end of line if there are no other members before the current
            // one, that have persistend in the containing type after all edits. If that is not the case, that is, if there
            // are other members before the current one, we want to keep that end of line. Otherwise, we'd end up with the
            // current member being incorrectly declared right after the previous one, without a separating blank line.
            for (int i = 0; i < fieldDeclarationIndex + 1; i++)
            {
                hasAnyPersistentPrecedingMemberDeclarations |= !fieldDeclarations.Contains(fieldParentTypeDeclaration.Members[i]);
            }

            // If there's any other persistent members, stop here
            if (hasAnyPersistentPrecedingMemberDeclarations)
            {
                continue;
            }

            // Finally, we can actually remove this end of line trivia, as we're sure it's not actually intended
            syntaxEditor.ReplaceNode(nextMember, (nextMember, _) => nextMember.WithLeadingTrivia(leadingTrivia.RemoveAt(0)));
        }
    }

    /// <summary>
    /// Gets the additional locations provided by the analyzer.
    /// </summary>
    /// <param name="diagnostic">The <see cref="Diagnostic"/> instance currently being processed.</param>
    /// <param name="fieldLocation">The location of the field to remove.</param>
    /// <param name="propertyTypeExpressionLocation">The location of the property type expression to use in metadata.</param>
    /// <param name="defaultValueExpressionLocation">The location for the default value.</param>
    /// <returns>Whether the additional locations were retrieved correctly.</returns>
    private static bool TryGetAdditionalLocations(
        Diagnostic diagnostic,
        [NotNullWhen(true)] out Location? fieldLocation,
        [NotNullWhen(true)] out Location? propertyTypeExpressionLocation,
        [NotNullWhen(true)] out Location? defaultValueExpressionLocation)
    {
        // We always expect 3 additional locations, as per contract with the analyzer.
        // Do a sanity check just in case, as we've seen sporadic issues with this.
        if (diagnostic.AdditionalLocations is not [{ } location1, { } location2, { } location3])
        {
            fieldLocation = null;
            propertyTypeExpressionLocation = null;
            defaultValueExpressionLocation = null;

            return false;
        }

        fieldLocation = location1;
        propertyTypeExpressionLocation = location2;
        defaultValueExpressionLocation = location3;

        return true;
    }

    /// <summary>
    /// A custom <see cref="FixAllProvider"/> with the logic from <see cref="UsePartialPropertyForSemiAutoPropertyCodeFixer"/>.
    /// </summary>
    private sealed class FixAllProvider : DocumentBasedFixAllProvider
    {
        /// <inheritdoc/>
        protected override async Task<Document?> FixAllAsync(FixAllContext fixAllContext, Document document, ImmutableArray<Diagnostic> diagnostics)
        {
            // Get the semantic model, as we need to resolve symbols
            if (await document.GetSemanticModelAsync(fixAllContext.CancellationToken).ConfigureAwait(false) is not SemanticModel semanticModel)
            {
                return document;
            }

            // Get the document root (this should always succeed)
            if (await document.GetSyntaxRootAsync(fixAllContext.CancellationToken).ConfigureAwait(false) is not SyntaxNode root)
            {
                return document;
            }

            // If we can't generate the new attribute list, bail (this should never happen)
            if (!TryGetGeneratedDependencyPropertyAttributeList(document, semanticModel, out AttributeListSyntax? generatedDependencyPropertyAttributeList))
            {
                return document;
            }

            // Create an editor to perform all mutations (across all edits in the file)
            SyntaxEditor syntaxEditor = new(root, fixAllContext.Solution.Services);

            // Create the set to track all fields being removed, to adjust whitespaces
            HashSet<FieldDeclarationSyntax> fieldDeclarations = [];

            // Step 1: rewrite all properties and remove the fields
            foreach (Diagnostic diagnostic in diagnostics)
            {
                // Get the current property declaration for the diagnostic
                if (root.FindNode(diagnostic.Location.SourceSpan) is not PropertyDeclarationSyntax propertyDeclaration)
                {
                    continue;
                }

                // Get all additional locations we expect from the analyzer
                if (!TryGetAdditionalLocations(
                    diagnostic,
                    out Location? fieldLocation,
                    out Location? propertyTypeExpressionLocation,
                    out Location? defaultValueExpressionLocation))
                {
                    continue;
                }

                // Also check that we can find the target field to remove
                if (root.FindNode(fieldLocation.SourceSpan) is not FieldDeclarationSyntax fieldDeclaration)
                {
                    continue;
                }

                // Retrieve the properties passed by the analyzer
                string? defaultValue = diagnostic.Properties[UseGeneratedDependencyPropertyOnManualPropertyAnalyzer.DefaultValuePropertyName];
                string? defaultValueTypeReferenceId = diagnostic.Properties[UseGeneratedDependencyPropertyOnManualPropertyAnalyzer.DefaultValueTypeReferenceIdPropertyName];

                ConvertToPartialProperty(
                    document,
                    semanticModel,
                    root,
                    propertyDeclaration,
                    fieldDeclaration,
                    generatedDependencyPropertyAttributeList,
                    syntaxEditor,
                    propertyTypeExpressionLocation,
                    defaultValue,
                    defaultValueTypeReferenceId,
                    defaultValueExpressionLocation);

                fieldDeclarations.Add(fieldDeclaration);
            }

            // Step 2: remove any leftover leading end of lines on members following fields that have been removed
            RemoveLeftoverLeadingEndOfLines(fieldDeclarations, syntaxEditor);

            return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
        }
    }
}
