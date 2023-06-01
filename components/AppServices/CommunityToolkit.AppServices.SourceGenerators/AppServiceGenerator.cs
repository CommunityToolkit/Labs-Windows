// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CommunityToolkit.AppServices.SourceGenerators.Extensions;
using CommunityToolkit.AppServices.SourceGenerators.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CommunityToolkit.AppServices.SourceGenerators;

/// <summary>
/// A source generator for the <c>AppServiceAttribute</c> type.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed partial class AppServiceGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all app service class implementations, and only enable this branch if the target is not a UWP app (the component)
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, AppServiceInfo Info)> appServiceComponentInfo =
            context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax classDeclaration && classDeclaration.HasOrPotentiallyHasBaseTypes(),
                static (context, token) =>
                {
                    // Only retrieve host info if the target is not a UWP application
                    if (Helpers.IsUwpTarget(context.SemanticModel.Compilation))
                    {
                        return default;
                    }

                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;

                    // Only select the first declaration of a given item, to avoid issues with partial types
                    if (!context.Node.IsFirstSyntaxDeclarationForSymbol(typeSymbol))
                    {
                        return default;
                    }

                    token.ThrowIfCancellationRequested();

                    // Try to get the info on the current component
                    (INamedTypeSymbol? serviceSymbol, string? appServiceName) = Component.GetInfo(typeSymbol, token);

                    // If there's no app service interface, do nothing
                    if (serviceSymbol is null)
                    {
                        return default;
                    }

                    HierarchyInfo hierarchy = HierarchyInfo.From(typeSymbol);

                    token.ThrowIfCancellationRequested();

                    ImmutableArray<MethodInfo> methods = MethodInfo.From(serviceSymbol, token);

                    token.ThrowIfCancellationRequested();

                    return (Hierarchy: hierarchy, new AppServiceInfo(methods, appServiceName!, typeSymbol.GetFullyQualifiedName()));
                })
            .Where(static pair => pair.Hierarchy is not null);

        // Produce the component type
        context.RegisterSourceOutput(appServiceComponentInfo, static (context, item) =>
        {
            ConstructorDeclarationSyntax constructorSyntax = Component.GetSyntax(item.Hierarchy, item.Info);
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(
                ImmutableArray.Create<MemberDeclarationSyntax>(constructorSyntax),
                ImmutableArray.Create<BaseTypeSyntax>(SimpleBaseType(IdentifierName("global::CommunityToolkit.AppServices.AppServiceComponent"))),
                "/// <inheritdoc/>");

            context.AddSource($"{item.Hierarchy.FilenameHint}.g.cs", compilationUnit.GetText(Encoding.UTF8));
        });

        // Gather all interfaces, and only enable this branch if the target is a UWP app (the host)
        IncrementalValuesProvider<(HierarchyInfo Hierarchy, AppServiceInfo Info)> appServiceHostInfo =
            context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "CommunityToolkit.AppServices.AppServiceAttribute",
                static (node, _) => node is InterfaceDeclarationSyntax,
                static (context, token) =>
                {
                    // Only retrieve host info if the target is a UWP application
                    if (!Helpers.IsUwpTarget(context.SemanticModel.Compilation))
                    {
                        return default;
                    }

                    // Check if the current interface is in fact an app service type
                    if (!Host.TryGetAppServiceName(context.Attributes[0], out string? appServiceName))
                    {
                        return default;
                    }

                    token.ThrowIfCancellationRequested();

                    INamedTypeSymbol typeSymbol = (INamedTypeSymbol)context.TargetSymbol;

                    // Get the info on the host implementation
                    HierarchyInfo hierarchy = HierarchyInfo.From(typeSymbol, typeSymbol.Name.Substring(1));

                    token.ThrowIfCancellationRequested();

                    ImmutableArray<MethodInfo> methods = MethodInfo.From(typeSymbol, token);

                    token.ThrowIfCancellationRequested();

                    return (Hierarchy: hierarchy, new AppServiceInfo(methods, appServiceName, typeSymbol.GetFullyQualifiedName()));
                })
            .Where(static item => item.Hierarchy is not null);

        // Produce the host type
        context.RegisterSourceOutput(appServiceHostInfo, static (context, item) =>
        {
            ConstructorDeclarationSyntax constructorSyntax = Host.GetConstructorSyntax(item.Hierarchy, item.Info);
            ImmutableArray<MethodDeclarationSyntax> methodDeclarations = Host.GetMethodDeclarationsSyntax(item.Info);
            CompilationUnitSyntax compilationUnit = item.Hierarchy.GetCompilationUnit(
                ImmutableArray.Create<MemberDeclarationSyntax>(constructorSyntax).AddRange(methodDeclarations),
                ImmutableArray.Create<BaseTypeSyntax>(
                    SimpleBaseType(IdentifierName("global::CommunityToolkit.AppServices.AppServiceHost")),
                    SimpleBaseType(IdentifierName(item.Info.InterfaceFullyQualifiedName))),
                $"/// <summary>A generated host implementation for the <see cref=\"{item.Info.InterfaceFullyQualifiedName}\"/> interface.</summary>");

            context.AddSource($"{item.Hierarchy.FilenameHint}.g.cs", compilationUnit.GetText(Encoding.UTF8));
        });
    }
}
