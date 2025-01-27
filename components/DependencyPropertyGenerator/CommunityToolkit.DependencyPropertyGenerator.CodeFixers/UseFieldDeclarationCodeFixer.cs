// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Text;
using static CommunityToolkit.GeneratedDependencyProperty.Diagnostics.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.GeneratedDependencyProperty;

/// <summary>
/// A code fixer that updates property declarations to be fields instead, for dependency properties.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class UseFieldDeclarationCodeFixer : CodeFixProvider
{
    /// <inheritdoc/>
    public override ImmutableArray<string> FixableDiagnosticIds { get; } = [DependencyPropertyFieldDeclarationId];

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

        // Get the property declaration from the target diagnostic
        if (root!.FindNode(diagnosticSpan) is PropertyDeclarationSyntax propertyDeclaration)
        {
            // We only support this code fix for static properties without modifiers and attributes
            if (!IsCodeFixSupportedForPropertyDeclaration(propertyDeclaration))
            {
                return;
            }

            // We can now register the code fix to convert the property into a field
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Declare dependency property as field",
                    createChangedDocument: token => ConvertDependencyPropertyToFieldDeclaration(context.Document, root, propertyDeclaration),
                    equivalenceKey: "Declare dependency property as field"),
                diagnostic);
        }
    }

    /// <summary>
    /// Checks whether the code fixer can be applied to a target property declaration.
    /// </summary>
    /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/> to update.</param>
    /// <returns>Whether the code fixer can be applied to <paramref name="propertyDeclaration"/>.</returns>
    private static bool IsCodeFixSupportedForPropertyDeclaration(PropertyDeclarationSyntax propertyDeclaration)
    {
        // We don't support properties with attributes, as those might not work on fields and need special handling
        if (propertyDeclaration.AttributeLists.Count > 0)
        {
            return false;
        }

        foreach (SyntaxToken modifier in propertyDeclaration.Modifiers)
        {
            // Accessibility modifiers are allowed (the property will however become public)
            if (SyntaxFacts.IsAccessibilityModifier(modifier.Kind()))
            {
                continue;
            }

            // If the property is abstract or an override, or other weird things (which shouldn't really happen), we don't support it
            if (modifier.Kind() is SyntaxKind.AbstractKeyword or SyntaxKind.OverrideKeyword or SyntaxKind.PartialKeyword or SyntaxKind.ExternKeyword)
            {
                return false;
            }
        }

        // Properties with an expression body are supported and will be converted to field initializers
        if (propertyDeclaration.ExpressionBody is not null)
        {
            return true;
        }

        // The property must have at least an accessor
        if (propertyDeclaration.AccessorList is not { Accessors.Count: > 0 } accessorList)
        {
            return false;
        }

        // One of the accessors must be a getter
        if (!accessorList.Accessors.Any(accessor => accessor.IsKind(SyntaxKind.GetAccessorDeclaration)))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Applies the code fix to a target property declaration and returns an updated document.
    /// </summary>
    /// <param name="document">The original document being fixed.</param>
    /// <param name="root">The original tree root belonging to the current document.</param>
    /// <param name="propertyDeclaration">The <see cref="PropertyDeclarationSyntax"/> to update.</param>
    /// <returns>An updated document with the applied code fix.</returns>
    private static async Task<Document> ConvertDependencyPropertyToFieldDeclaration(Document document, SyntaxNode root, PropertyDeclarationSyntax propertyDeclaration)
    {
        await Task.CompletedTask;

        SyntaxEditor syntaxEditor = new(root, document.Project.Solution.Workspace.Services);

        syntaxEditor.ReplaceNode(propertyDeclaration, (node, generator) =>
        {
            // If the property had an initializer, carry that over
            ExpressionSyntax? initializerExpression = propertyDeclaration switch
            {
                { ExpressionBody.Expression: { } arrowExpression } => arrowExpression,
                { Initializer.Value: { } equalsExpression } => equalsExpression,
                _ => null
            };

            // Create the field declaration and make it 'public static readonly' (same as the other analyzer)
            SyntaxNode updatedNode = generator.FieldDeclaration(
                name: propertyDeclaration.Identifier.Text,
                type: propertyDeclaration.Type,
                accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Static | DeclarationModifiers.ReadOnly,
                initializer: initializerExpression);

            // Keep the 'new' modifier, if needed
            if (propertyDeclaration.Modifiers.Any(SyntaxKind.NewKeyword))
            {
                updatedNode = generator.WithModifiers(updatedNode, generator.GetModifiers(updatedNode).WithIsNew(true));
            }

            return updatedNode.WithTriviaFrom(propertyDeclaration);
        });

        return document.WithSyntaxRoot(syntaxEditor.GetChangedRoot());
    }
}
