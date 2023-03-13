// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommunityToolkit.Tooling.XamlNamedPropertyRelay;

/// <summary>
/// Generates code that provides access to XAML elements with <c>x:Name</c> from code-behind by wrapping an instance of a control, without the need to use <c>x:FieldProvider="public"</c> directly in markup.
/// </summary>
[Generator]
public class XamlNamedPropertyRelayGenerator : IIncrementalGenerator
{
    private readonly HashSet<INamedTypeSymbol> _handledConstructors = new(SymbolEqualityComparer.Default);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // UWP generates fields for x:Name.
        var fieldSymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is FieldDeclarationSyntax,
                static (context, _) => ((FieldDeclarationSyntax)context.Node).Declaration.Variables.Select(v => (IFieldSymbol?)context.SemanticModel.GetDeclaredSymbol(v)))
            .SelectMany(static (item, _) => item)
            .Where(IsDeclaredXamlXNameProperty);

        // Uno generates private properties for x:Name.
        var propertySymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MemberDeclarationSyntax,
                static (context, _) => context.SemanticModel.GetDeclaredSymbol((MemberDeclarationSyntax)context.Node) as IPropertySymbol)
            .Where(IsDeclaredXamlXNameProperty);

        context.RegisterSourceOutput(fieldSymbols, (ctx, sym) => GenerateNamedPropertyRelay(ctx, sym, sym?.Type, _handledConstructors));
        context.RegisterSourceOutput(propertySymbols, (ctx, sym) => GenerateNamedPropertyRelay(ctx, sym, sym?.Type, _handledConstructors));
    }

    private static void GenerateNamedPropertyRelay(SourceProductionContext context, ISymbol? symbol, ITypeSymbol? type, HashSet<INamedTypeSymbol> handledConstructors)
    {
        if (symbol?.ContainingType is null || type is null)
            return;

        GenerateConstructor(context, symbol, handledConstructors);

        var source = $@"namespace {symbol.ContainingType.ContainingNamespace}
{{
    partial class {symbol.ContainingType.Name}
    {{
        /// <summary>
        /// Provides the same functionality as using <c>&lt;SomeElement x:FieldProvider=""public"" x:Name=""someName""&gt;</c>
        /// on an element in XAML, without the need for the extra <c>x:FieldProvider</c> markup.
        /// </summary>
        public partial record XamlNamedPropertyRelay
        {{
            public {type} {symbol.Name} => _{symbol.ContainingType.Name}.{symbol.Name};
        }}
    }}
}}
";

        context.AddSource($"{symbol}.g", source);
    }

    private static void GenerateConstructor(SourceProductionContext context, ISymbol? symbol, HashSet<INamedTypeSymbol> handledConstructors)
    {
        if (symbol?.ContainingType is null)
            return;

        if (!handledConstructors.Add(symbol.ContainingType))
            return;

        var source = $@"#nullable enable
namespace {symbol.ContainingType.ContainingNamespace}
{{
    partial class {symbol.ContainingType.Name}
    {{
        /// <summary>
        /// Provides the same functionality as using <c>&lt;SomeElement x:FieldProvider=""public"" x:Name=""someName""&gt;</c>
        /// on an element in XAML, without the need for the extra <c>x:FieldProvider</c> markup.
        /// </summary>
        public partial record XamlNamedPropertyRelay
        {{
            private readonly {symbol.ContainingType} _{symbol.ContainingType.Name};

            public XamlNamedPropertyRelay({symbol.ContainingType} {symbol.ContainingType.Name.ToLower()})
            {{
                _{symbol.ContainingType.Name} = {symbol.ContainingType.Name.ToLower()};
            }}
        }}
    }}
}}
";

        context.AddSource($"{symbol.ContainingType}.ctor.g", source);
    }

    /// <summary>
    /// Checks if a symbol's <see cref="ISymbol.ContainingType"/> is or inherits from a type representing a XAML framework, and that the <paramref name="symbol"/> is a generated x:Name.
    /// </summary>
    /// <returns><see langwork="true"/> if the <paramref name="symbol"/> is or inherits from a type representing a XAML framework.</returns>
    public static bool IsDeclaredXamlXNameProperty<T>(T? symbol)
        where T : ISymbol
    {
        if (symbol is null)
            return false;

        // In UWP, Page inherits UserControl
        // In Uno, Page doesn't appear to inherit UserControl.
        var validSimpleTypeNames = new[] { "UserControl", "Page" };

        // UWP / Uno / WinUI 3 namespaces.
        var validNamespaceRoots = new[] { "Microsoft", "Windows" };

        // Recursively crawl the base types until either UserControl or Page is found.
        var validInheritedSymbol = CrawlBy(
            symbol.ContainingType,
            x => x?.BaseType,
            baseType => validNamespaceRoots.Any(x => $"{baseType}".StartsWith(x)) &&
                                                     $"{baseType}".Contains(".UI.Xaml.Controls.") &&
                                                     validSimpleTypeNames.Any(x => $"{baseType}".EndsWith(x)));

        var containerIsPublic = symbol.ContainingType?.DeclaredAccessibility == Accessibility.Public;
        var isPrivate = symbol.DeclaredAccessibility == Accessibility.Private;
        var typeIsAccessible = symbol is IFieldSymbol field && field.Type.DeclaredAccessibility == Accessibility.Public || symbol is IPropertySymbol prop && prop.Type.DeclaredAccessibility == Accessibility.Public;

        return validInheritedSymbol != default && isPrivate && containerIsPublic && typeIsAccessible && !symbol.IsStatic;
    }

    /// <summary>
    /// Crawls an object tree for nested properties of the same type and returns the first instance that matches the <paramref name="filterPredicate"/>.
    /// </summary>
    /// <remarks>
    /// Does not filter against or return the <paramref name="root"/> object.
    /// </remarks>
    public static T? CrawlBy<T>(T? root, Func<T?, T?> selectPredicate, Func<T?, bool> filterPredicate)
    {
        crawl:
        var current = selectPredicate(root);

        if (filterPredicate(current))
        {
            return current;
        }

        if (current is null)
        {
            return default;
        }

        root = current;
        goto crawl;
    }
}

