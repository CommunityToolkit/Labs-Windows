// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using static CommunityToolkit.AppServices.SourceGenerators.Diagnostics.SuppressionDescriptors;

namespace CommunityToolkit.AppServices.SourceGenerators;

/// <summary>
/// <para>
/// A diagnostic suppressor to suppress CS1998 warnings for synchronous AppService methods using the async modifier.
/// </para>
/// <para>
/// That is, this diagnostic suppressor will suppress the following diagnostic:
/// <code>
/// public partial class MyAppService : IMyAppService
/// { 
///     public async Task&lt;string&gt; GreetUserAsync()
///     {
///         return "Hello world";
///     }
/// }
/// </code>
/// </para>
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SynchronousAppServiceMethodDiagnosticSuppressor : DiagnosticSuppressor
{
    /// <inheritdoc/>
    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(SynchronousAppServiceMethod);

    /// <inheritdoc/>
    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (Diagnostic diagnostic in context.ReportedDiagnostics)
        {
            SuppressDiagnosticIfNeeded(context, diagnostic);
        }
    }

    /// <summary>
    /// Suppresses a given diagnostic, if applicable.
    /// </summary>
    /// <param name="context">The <see cref="SuppressionAnalysisContext"/> instance currently in use.</param>
    /// <param name="diagnostic">The candidate <see cref="Diagnostic"/> to suppress, if needed.</param>
    private static void SuppressDiagnosticIfNeeded(SuppressionAnalysisContext context, Diagnostic diagnostic)
    {
        SyntaxNode? syntaxNode = diagnostic.Location.SourceTree?.GetRoot(context.CancellationToken).FindNode(diagnostic.Location.SourceSpan);

        // The target node has to be a method declaration inside a class
        if (syntaxNode is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax })
        {
            // Get the symbol for the current syntax node
            ISymbol? declaredSymbol = context.GetSemanticModel(syntaxNode.SyntaxTree).GetDeclaredSymbol(syntaxNode, context.CancellationToken);

            // Get the method symbol and the type symbol for the containing type
            if (declaredSymbol is IMethodSymbol { ContainingType: INamedTypeSymbol classSymbol } methodSymbol)
            {
                foreach (INamedTypeSymbol interfaceSymbol in classSymbol.Interfaces)
                {
                    // Check whether this interface implemented by the containing type is an AppService interface
                    if (interfaceSymbol.TryGetAppServicesNameFromAttribute(out _))
                    {
                        foreach (IMethodSymbol interfaceMethodSymbol in interfaceSymbol.GetMembers().OfType<IMethodSymbol>())
                        {
                            // For each interface method, get the implementation in the current class and check if it's the same as the current method
                            if (classSymbol.FindImplementationForInterfaceMember(interfaceMethodSymbol) is IMethodSymbol implementedMethodSymbol &&
                                SymbolEqualityComparer.Default.Equals(methodSymbol, implementedMethodSymbol))
                            {
                                context.ReportSuppression(Suppression.Create(SynchronousAppServiceMethod, diagnostic));

                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
