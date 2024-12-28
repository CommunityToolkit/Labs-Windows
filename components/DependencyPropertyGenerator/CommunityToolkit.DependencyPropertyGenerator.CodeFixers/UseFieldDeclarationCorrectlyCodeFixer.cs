// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A code fixer that updates field declarations to ensure they follow the recommended rules for dependency properties.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class UseFieldDeclarationCorrectlyCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [IncorrectDependencyPropertyFieldDeclarationId];

    /// <inheritdoc/>
    public override FixAllProvider? GetFixAllProvider()
    {
        return WellKnownFixAllProviders.BatchFixer;
    }

    /// <inheritdoc/>
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        Diagnostic diagnostic = context.Diagnostics[0];
        TextSpan diagnosticSpan = context.Span;

        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        // Get the property declaration and the field declaration from the target diagnostic
        if (root!.FindNode(diagnosticSpan).FirstAncestorOrSelf<FieldDeclarationSyntax>() is { } fieldDeclaration)
        {
            // Register the code fix to update the field to be correctly declared
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Declare dependency property field correctly",
                    createChangedDocument: token => FixDependencyPropertyFieldDeclaration(context.Document, root, fieldDeclaration),
                    equivalenceKey: "Declare dependency property field correctly"),
                diagnostic);
        }
    }

    /// <summary>
    /// Applies the code fix to a target field declaration and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="fieldDeclaration">The <see cref="FieldDeclarationSyntax"/> to update.</param>
    /// <returns>An updated document with the applied code fix.</returns>
    private static async Task<Document> FixDependencyPropertyFieldDeclaration(Document document, SyntaxNode root, FieldDeclarationSyntax fieldDeclaration)
    {
        await Task.CompletedTask;

        SyntaxEditor syntaxEditor = new(root, document.Project.Solution.Workspace.Services);

        // We use the lambda overload mostly for convenient, so we can easily get a generator to use
        syntaxEditor.ReplaceNode(fieldDeclaration, (node, generator) =>
        {
            // Update the field to ensure it's declared as 'public static readonly'
            node = generator.WithAccessibility(node, Accessibility.Public);
            node = generator.WithModifiers(node, DeclarationModifiers.Static | DeclarationModifiers.ReadOnly);

            // If the type is declared as nullable, unwrap it and remove the annotation.
            // We need to make sure to carry the space after the element type. When the
            // type is nullable, that space is attached to the question mark token.
            if (((FieldDeclarationSyntax)node).Declaration is { Type: NullableTypeSyntax { ElementType: { } fieldElementType } nullableType } variableDeclaration)
            {
                TypeSyntax typeDeclaration = fieldElementType.WithTrailingTrivia(nullableType.QuestionToken.TrailingTrivia);

                node = ((FieldDeclarationSyntax)node).WithDeclaration(variableDeclaration.WithType(typeDeclaration));
            }

            return node;
        });

        // Create the new document with the single change
        return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
    }
}
