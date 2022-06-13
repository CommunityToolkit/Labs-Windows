// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod.Diagnostics;
using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

/// <summary>
/// Generates code that provides access to XAML elements with <c>x:Name</c> from code-behind by wrapping an instance of a control, without the need to use <c>x:FieldProvider="public"</c> directly in markup.
/// </summary>
[Generator]
public class UIControlTestMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Get all method declarations with at least one attribute
        var methodSymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax { Parent: ClassDeclarationSyntax, AttributeLists.Count: > 0 },
                static (context, token) => context.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)context.Node, cancellationToken: token))
            .Where(x => x is not null)
            .Select((x, _) => x!);

        // Filter the methods using [UIControlTestMethod]
        var methodAndPageTypeSymbols = methodSymbols
            .Select(static (item, _) =>
            (
                Symbol: item,
                Attribute: item.GetAttributes().FirstOrDefault(a => a.AttributeClass?.HasFullyQualifiedName("global::CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod.UIControlTestMethodAttribute") ?? false)
            ))
            
            .Where(static item => item.Attribute is not null && item.Symbol is IMethodSymbol)
            .Select(static (x, _) => (MethodSymbol: (IMethodSymbol)x.Symbol, Attribute: x.Attribute!))
            
            .Where(static x => !x.Attribute.ConstructorArguments.IsEmpty)
            .Select(static (x, _) => (x.MethodSymbol, ControlTypeSymbol: GetControlTypeSymbolFromAttribute(x.Attribute)))
            
            .Where(static x => x.ControlTypeSymbol is not null)
            .Select(static (x, _) => (x.MethodSymbol, ControlTypeSymbol: x.ControlTypeSymbol!));

        // Generate source
        context.RegisterSourceOutput(methodAndPageTypeSymbols, (x, y) => GenerateTestMethod(x, y.MethodSymbol, y.ControlTypeSymbol));
    }

    private static void GenerateTestMethod(SourceProductionContext context, IMethodSymbol methodSymbol, INamedTypeSymbol controlTypeSymbol)
    {
        if (!ControlTypeInheritsFrameworkElement(controlTypeSymbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TypeDoesNotInheritFrameworkElement, methodSymbol.Locations.FirstOrDefault(), controlTypeSymbol.Name));            
            return;
        }

        if (controlTypeSymbol.Constructors.Any(x => !x.Parameters.IsEmpty))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TestControlHasConstructorWithParameters, methodSymbol.Locations.FirstOrDefault(), controlTypeSymbol.Name));
            return;
        }

        if (!methodSymbol.Parameters.IsEmpty)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.TestMethodIsNotParameterless, methodSymbol.Locations.FirstOrDefault(), controlTypeSymbol.Name));
            return;
        }

        var isAsync = methodSymbol.ReturnType.HasFullyQualifiedName("global::System.Threading.Tasks.Task") ||
                      methodSymbol.ReturnType.InheritsFromFullyQualifiedName("global::System.Threading.Tasks.Task");

        var source = $@"using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace {methodSymbol.ContainingType.ContainingNamespace}
{{
    partial class {methodSymbol.ContainingType.Name}
    {{
        [TestMethod]
        public Task {methodSymbol.Name}_Test()
        {{
            return App.DispatcherQueue.EnqueueAsync(async () => {{
                TestPage = new {controlTypeSymbol.GetFullyQualifiedName()}();

                // Set content
                Task result = SetTestContentAsync(TestPage);
                await result;

                Assert.IsTrue(result.IsCompletedSuccessfully, $""Failed to load page {controlTypeSymbol.GetFullyQualifiedName()} for test { methodSymbol.Name } with Exception: {{ result.Exception?.Message}} "");

                // Call original
                {(isAsync ? "await " : string.Empty)}{methodSymbol.Name}();

                TestPage = null;
            }});
        }}
    }}
}}
";

        context.AddSource($"{methodSymbol}.g", source);
    }

    private static bool ControlTypeInheritsFrameworkElement(INamedTypeSymbol controlType)
    {
        return controlType.HasOrInheritsFromFullyQualifiedName("global::Windows.UI.Xaml.FrameworkElement") ||
               controlType.HasOrInheritsFromFullyQualifiedName("global::Microsoft.UI.Xaml.FrameworkElement");
    }

    private static INamedTypeSymbol? GetControlTypeSymbolFromAttribute(AttributeData attribute)
    {
        return attribute.ConstructorArguments.FirstOrDefault(x => x.Kind == TypedConstantKind.Type).Value as INamedTypeSymbol;
    }
}

