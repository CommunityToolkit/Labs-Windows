// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.GeneratedDependencyProperty.Constants;
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

        // We can only possibly fix diagnostics with an additional location
        if (diagnostic.AdditionalLocations is not [{ } fieldLocation])
        {
            return;
        }

        // This code fixer needs the semantic model, so check that first
        if (!context.Document.SupportsSemanticModel)
        {
            return;
        }

        // Retrieve the properties passed by the analyzer
        string? defaultValue = diagnostic.Properties[UseGeneratedDependencyPropertyOnManualPropertyAnalyzer.DefaultValuePropertyName];

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
                        defaultValue),
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
    /// <param name="defaultValueExpression">The expression for the default value of the property, if present</param>
    /// <returns>The updated attribute syntax.</returns>
    private static AttributeListSyntax UpdateGeneratedDependencyPropertyAttributeList(
        Document document,
        SemanticModel semanticModel,
        AttributeListSyntax generatedDependencyPropertyAttributeList,
        string? defaultValueExpression)
    {
        // If we do have a default value expression, set it in the attribute.
        // We extract the generated attribute so we can add the new argument.
        // It's important to reuse it, as it has the "add usings" annotation.
        if (defaultValueExpression is not null)
        {
            ExpressionSyntax parsedExpression = ParseExpression(defaultValueExpression);

            // Special case values which are simple enum member accesses, like 'global::Windows.UI.Xaml.Visibility.Collapsed'
            if (parsedExpression is MemberAccessExpressionSyntax { Expression: { } expressionSyntax, Name: IdentifierNameSyntax { Identifier.Text: { } memberName } })
            {
                string fullyQualifiedTypeName = expressionSyntax.ToFullString();

                // Ensure we strip the global prefix, if present (it should always be present)
                if (fullyQualifiedTypeName.StartsWith("global::"))
                {
                    fullyQualifiedTypeName = fullyQualifiedTypeName["global::".Length..];
                }

                // Try to resolve the attribute type, if present. This API takes a fully qualified metadata name, not
                // a fully qualified type name. However, for virtually all cases for enum types, the two should match.
                // That is, they will be the same if the type is not nested, and not generic, which is what we expect.
                if (semanticModel.Compilation.GetTypeByMetadataName(fullyQualifiedTypeName) is INamedTypeSymbol enumTypeSymbol)
                {
                    SyntaxGenerator syntaxGenerator = SyntaxGenerator.GetGenerator(document);

                    // Create the identifier syntax for the enum type, with the right annotations
                    SyntaxNode enumTypeSyntax = syntaxGenerator.TypeExpression(enumTypeSymbol).WithAdditionalAnnotations(Simplifier.AddImportsAnnotation);

                    // Create the member access expression for the target enum type
                    SyntaxNode enumMemberAccessExpressionSyntax = syntaxGenerator.MemberAccessExpression(enumTypeSyntax, memberName);

                    // Create the attribute argument to insert
                    SyntaxNode attributeArgumentSyntax = syntaxGenerator.AttributeArgument("DefaultValue", enumMemberAccessExpressionSyntax);

                    // Actually add the argument to the existing attribute syntax
                    return (AttributeListSyntax)syntaxGenerator.AddAttributeArguments(generatedDependencyPropertyAttributeList, [attributeArgumentSyntax]);
                }
            }

            // Otherwise, just add the new default value normally
            return
                AttributeList(SingletonSeparatedList(
                    generatedDependencyPropertyAttributeList.Attributes[0]
                    .AddArgumentListArguments(
                        AttributeArgument(ParseExpression(defaultValueExpression))
                        .WithNameEquals(NameEquals(IdentifierName("DefaultValue"))))));
        }

        // If we have no value expression, we can just reuse the attribute with no changes
        return generatedDependencyPropertyAttributeList;
    }

    /// <summary>
    /// Applies the code fix to a target identifier and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current compilation.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/> for the property being updated.</param>
    /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/> for the declared property to remove.</param>
    /// <param name="defaultValueExpression">The expression for the default value of the property, if present</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="propertyDeclaration"/> being replaced with a partial property.</returns>
    private static async Task<Document> ConvertToPartialProperty(
        Document document,
        SemanticModel semanticModel,
        SyntaxNode root,
        PropertyDeclarationSyntax propertyDeclaration,
        FieldDeclarationSyntax fieldDeclaration,
        string? defaultValueExpression)
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
            propertyDeclaration,
            fieldDeclaration,
            generatedDependencyPropertyAttributeList,
            syntaxEditor,
            defaultValueExpression);

        // Create the new document with the single change
        return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
    }

    /// <summary>
    /// Applies the code fix to a target identifier and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current compilation.</param>
    /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/> for the property being updated.</param>
    /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/> for the declared property to remove.</param>
    /// <param name="generatedDependencyPropertyAttributeList">The <see cref="AttributeListSyntax"/> with the attribute to add.</param>
    /// <param name="syntaxEditor">The <see cref="SyntaxEditor"/> instance to use.</param>
    /// <param name="defaultValueExpression">The expression for the default value of the property, if present</param>
    /// <returns>An updated document with the applied code fix, and <paramref name="propertyDeclaration"/> being replaced with a partial property.</returns>
    private static void ConvertToPartialProperty(
        Document document,
        SemanticModel semanticModel,
        PropertyDeclarationSyntax propertyDeclaration,
        FieldDeclarationSyntax fieldDeclaration,
        AttributeListSyntax generatedDependencyPropertyAttributeList,
        SyntaxEditor syntaxEditor,
        string? defaultValueExpression)
    {
        // Update the attribute to insert with the default value, if present
        generatedDependencyPropertyAttributeList = UpdateGeneratedDependencyPropertyAttributeList(
            document,
            semanticModel,
            generatedDependencyPropertyAttributeList,
            defaultValueExpression);

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

        // Get a new property that is partial and with semicolon token accessors
        PropertyDeclarationSyntax updatedPropertyDeclaration =
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

        syntaxEditor.ReplaceNode(propertyDeclaration, updatedPropertyDeclaration);

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
        if (fieldDeclaration.Parent is TypeDeclarationSyntax fieldParentTypeDeclaration)
        {
            int fieldDeclarationIndex = fieldParentTypeDeclaration.Members.IndexOf(fieldDeclaration);

            // Check whether there is a member immediatley following the field
            if (fieldDeclarationIndex >= 0 && fieldDeclarationIndex < fieldParentTypeDeclaration.Members.Count - 1)
            {
                MemberDeclarationSyntax nextMember = fieldParentTypeDeclaration.Members[fieldDeclarationIndex + 1];
                SyntaxTriviaList leadingTrivia = nextMember.GetLeadingTrivia();

                // Check whether this member has a first leading trivia that's just a blank line: we want to remove this one
                if (leadingTrivia.Count > 0 && leadingTrivia[0].IsKind(SyntaxKind.EndOfLineTrivia))
                {
                    syntaxEditor.ReplaceNode(nextMember, (nextMember, _) => nextMember.WithLeadingTrivia(leadingTrivia.RemoveAt(0)));
                }
            }
        }

        // Also remove the field declaration (it'll be generated now)
        syntaxEditor.RemoveNode(fieldDeclaration);

        // Find the parent type for the property
        TypeDeclarationSyntax typeDeclaration = propertyDeclaration.FirstAncestorOrSelf<TypeDeclarationSyntax>()!;

        // Make sure it's partial (we create the updated node in the function to preserve the updated property declaration).
        // If we created it separately and replaced it, the whole tree would also be replaced, and we'd lose the new property.
        if (!typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
        {
            syntaxEditor.ReplaceNode(typeDeclaration, static (node, generator) => generator.WithModifiers(node, generator.GetModifiers(node).WithPartial(true)));
        }
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

            foreach (Diagnostic diagnostic in diagnostics)
            {
                // Get the current property declaration for the diagnostic
                if (root.FindNode(diagnostic.Location.SourceSpan) is not PropertyDeclarationSyntax propertyDeclaration)
                {
                    continue;
                }

                // Also check that we can find the target field to remove
                if (diagnostic.AdditionalLocations is not [{ } fieldLocation] ||
                    root.FindNode(fieldLocation.SourceSpan) is not FieldDeclarationSyntax fieldDeclaration)
                {
                    continue;
                }

                // Retrieve the properties passed by the analyzer
                string? defaultValue = diagnostic.Properties[UseGeneratedDependencyPropertyOnManualPropertyAnalyzer.DefaultValuePropertyName];

                ConvertToPartialProperty(
                    document,
                    semanticModel,
                    propertyDeclaration,
                    fieldDeclaration,
                    generatedDependencyPropertyAttributeList,
                    syntaxEditor,
                    defaultValue);
            }

            return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
        }
    }
}
