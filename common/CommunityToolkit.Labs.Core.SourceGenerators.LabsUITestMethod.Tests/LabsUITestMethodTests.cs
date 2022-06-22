// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;
using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Tests;

[TestClass]
public partial class LabsUITestMethodTests
{
    private const string DispatcherQueueDefinition = @"
namespace MyApp
{
    public partial class Test
    {
        public System.Threading.Tasks.Task EnqueueAsync<T>(System.Func<System.Threading.Tasks.Task<T>> function) => System.Threading.Tasks.Task.Run(function);

        public System.Threading.Tasks.Task EnqueueAsync(System.Action function) => System.Threading.Tasks.Task.Run(function);
    }
}
";

    [TestMethod]
    public void TestControlHasConstructorWithParameters()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [LabsUITestMethod]
                    public void TestMethod(MyControl control)
                    {
                    }
                }

                public class MyControl : Microsoft.UI.Xaml.FrameworkElement
                {
                    public MyControl(string id)
                    {
                    }
                }
            }

            namespace Microsoft.UI.Xaml
            {
                public class FrameworkElement { }
            }";

        VerifyGeneratedDiagnostics<LabsUITestMethodGenerator>(source + DispatcherQueueDefinition, DiagnosticDescriptors.TestControlHasConstructorWithParameters.Id);
    }

    [TestMethod]
    public void Async_Mux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [LabsUITestMethod]
                    public async System.Threading.Tasks.Task TestMethod(MyControl control)
                    {
                    }
                }

                public class MyControl : Microsoft.UI.Xaml.FrameworkElement
                {
                }
            }

            namespace Microsoft.UI.Xaml
            {
                public class FrameworkElement { }
            }";

        VerifyGeneratedDiagnostics<LabsUITestMethodGenerator>(source + DispatcherQueueDefinition);
    }

    [TestMethod]
    public void Async_Wux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [LabsUITestMethod]
                    public async System.Threading.Tasks.Task TestMethod(MyControl control)
                    {
                    }
                }

                public class MyControl : Windows.UI.Xaml.FrameworkElement
                {
                }
            }

            namespace Windows.UI.Xaml
            {
                public class FrameworkElement { }
            }";

        VerifyGeneratedDiagnostics<LabsUITestMethodGenerator>(source + DispatcherQueueDefinition);
    }

    [TestMethod]
    public void Async_NoMethodParams_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    [LabsUITestMethod]
                    public async System.Threading.Tasks.Task TestMethod()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<LabsUITestMethodGenerator>(source + DispatcherQueueDefinition);
    }

    [TestMethod]
    public void Synchronous_Mux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [LabsUITestMethod]
                    public void TestMethod(MyControl control)
                    {
                    }
                }

                public class MyControl : Microsoft.UI.Xaml.FrameworkElement
                {
                }
            }

            namespace Microsoft.UI.Xaml
            {
                public class FrameworkElement { }
            }";

        VerifyGeneratedDiagnostics<LabsUITestMethodGenerator>(source + DispatcherQueueDefinition);
    }

    [TestMethod]
    public void Synchronous_Wux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [LabsUITestMethod]
                    public void TestMethod(MyControl control)
                    {
                    }
                }

                public class MyControl : Windows.UI.Xaml.FrameworkElement
                {
                }
            }

            namespace Windows.UI.Xaml
            {
                public class FrameworkElement { }
            }";

        VerifyGeneratedDiagnostics<LabsUITestMethodGenerator>(source + DispatcherQueueDefinition);
    }

    [TestMethod]
    public void Synchronous_NoMethodParams_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    [LabsUITestMethod]
                    public void TestMethod()
                    {
                    }
                }
            }";

        VerifyGeneratedDiagnostics<LabsUITestMethodGenerator>(source + DispatcherQueueDefinition);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="source">The input source to process.</param>
    /// <param name="markdown">The input documentation info to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics<TGenerator>(string source, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        VerifyGeneratedDiagnostics<TGenerator>(CSharpSyntaxTree.ParseText(source), diagnosticsIds);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="syntaxTree">The input source tree to process.</param>
    /// <param name="markdown">The input documentation info to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static void VerifyGeneratedDiagnostics<TGenerator>(SyntaxTree syntaxTree, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        var attributeType = typeof(LabsUITestMethodAttribute);

        var references =
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where !assembly.IsDynamic
            let reference = MetadataReference.CreateFromFile(assembly.Location)
            select reference;

        var compilation = CSharpCompilation.Create(
            "original.Sample",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationDiagnostics = compilation.GetDiagnostics();

        Assert.IsTrue(compilationDiagnostics.All(x => x.Severity != DiagnosticSeverity.Error), $"Expected no compilation errors before source generation. Got: \n{string.Join("\n", compilationDiagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => $"[{x.Id}: {x.GetMessage()}]"))}");

        IIncrementalGenerator generator = new TGenerator();

        GeneratorDriver driver =
            CSharpGeneratorDriver
                .Create(generator)
                .WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

        _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();
        var generatedCompilationDiaghostics = outputCompilation.GetDiagnostics();

        Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds), $"Expected one of [{string.Join(", ", diagnosticsIds)}] diagnostic Ids. Got [{string.Join(", ", resultingIds)}]");
        Assert.IsTrue(generatedCompilationDiaghostics.All(x => x.Severity != DiagnosticSeverity.Error), $"Expected no generated compilation errors. Got: \n{string.Join("\n", generatedCompilationDiaghostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => $"[{x.Id}: {x.GetMessage()}]"))}");

        GC.KeepAlive(attributeType);
    }
}
