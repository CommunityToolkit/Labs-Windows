// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.TestGen;
using CommunityToolkit.Tooling.TestGen.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Tooling.TestGen.Tests;

[TestClass]
public partial class UIThreadTestMethodTests
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
            using CommunityToolkit.Tooling.TestGen;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIThreadTestMethod]
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

        VerifyGeneratedDiagnostics<UIThreadTestMethodGenerator>(source + DispatcherQueueDefinition, DiagnosticDescriptors.TestControlHasConstructorWithParameters.Id);
    }

    [TestMethod]
    public void Async_Mux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.TestGen;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIThreadTestMethod]
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

        var result = VerifyGeneratedDiagnostics<UIThreadTestMethodGenerator>(source + DispatcherQueueDefinition);

        Assert.AreEqual(1, result.GeneratedTrees.Length, "More trees generated than expected.");
        // To do, should probably inspect tree more directly.
        var generatedSource = result.GeneratedTrees.First().ToString();
        Assert.IsTrue(generatedSource.Contains("await LoadTestContentAsync(testControl);"), "Didn't see expected loading call.");
        Assert.IsTrue(generatedSource.Contains("await UnloadTestContentAsync(testControl);"), "Didn't see expected unloading call.");
        Assert.IsTrue(generatedSource.Contains("EnqueueAsync(async () => {"), "Unexpected sync lambda in Enqueue call.");
        Assert.IsTrue(generatedSource.Contains("var testControl = new global::MyApp.MyControl();"), "Didn't see expected creation of test control.");
        Assert.IsTrue(generatedSource.Contains("await TestMethod(testControl);"), "Didn't see expected running of test.");
    }

    [TestMethod]
    public void Async_Wux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.TestGen;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIThreadTestMethod]
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

        var result = VerifyGeneratedDiagnostics<UIThreadTestMethodGenerator>(source + DispatcherQueueDefinition);

        Assert.AreEqual(1, result.GeneratedTrees.Length, "More trees generated than expected.");
        // To do, should probably inspect tree more directly.
        var generatedSource = result.GeneratedTrees.First().ToString();
        Assert.IsTrue(generatedSource.Contains("await LoadTestContentAsync(testControl);"), "Didn't see expected loading call.");
        Assert.IsTrue(generatedSource.Contains("await UnloadTestContentAsync(testControl);"), "Didn't see expected unloading call.");
        Assert.IsTrue(generatedSource.Contains("EnqueueAsync(async () => {"), "Unexpected sync lambda in Enqueue call.");
        Assert.IsTrue(generatedSource.Contains("var testControl = new global::MyApp.MyControl();"), "Didn't see expected creation of test control.");
        Assert.IsTrue(generatedSource.Contains("await TestMethod(testControl);"), "Didn't see expected running of test.");
    }

    [TestMethod]
    public void Async_NoMethodParams_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.TestGen;

            namespace MyApp
            {
                public partial class Test
                {
                    [UIThreadTestMethod]
                    public async System.Threading.Tasks.Task TestMethod()
                    {
                    }
                }
            }";

        var result = VerifyGeneratedDiagnostics<UIThreadTestMethodGenerator>(source + DispatcherQueueDefinition);

        Assert.AreEqual(1, result.GeneratedTrees.Length, "More trees generated than expected.");
        // To do, should probably inspect tree more directly.
        var generatedSource = result.GeneratedTrees.First().ToString();
        Assert.IsFalse(generatedSource.Contains("await LoadTestContentAsync"), "Saw a loading call.");
        Assert.IsFalse(generatedSource.Contains("await UnloadTestContentAsync"), "Saw an unloading call.");
        Assert.IsTrue(generatedSource.Contains("EnqueueAsync(async () => {"), "Unexpected sync lambda in Enqueue call.");
        Assert.IsTrue(generatedSource.Contains("await TestMethod();"), "Didn't see expected running of test.");
    }

    [TestMethod]
    public void Synchronous_Mux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.TestGen;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIThreadTestMethod]
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

        var result = VerifyGeneratedDiagnostics<UIThreadTestMethodGenerator>(source + DispatcherQueueDefinition);

        Assert.AreEqual(1, result.GeneratedTrees.Length, "More trees generated than expected.");
        // To do, should probably inspect tree more directly.
        var generatedSource = result.GeneratedTrees.First().ToString();
        Assert.IsTrue(generatedSource.Contains("await LoadTestContentAsync(testControl);"), "Didn't see expected loading call.");
        Assert.IsTrue(generatedSource.Contains("await UnloadTestContentAsync(testControl);"), "Didn't see expected unloading call.");
        Assert.IsTrue(generatedSource.Contains("EnqueueAsync(async () => {"), "Unexpected sync lambda in Enqueue call.");
        Assert.IsTrue(generatedSource.Contains("var testControl = new global::MyApp.MyControl();"), "Didn't see expected creation of test control.");
        Assert.IsTrue(generatedSource.Contains("TestMethod(testControl);"), "Didn't see expected running of test.");
    }

    [TestMethod]
    public void Synchronous_Wux_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.TestGen;

            namespace MyApp
            {
                public partial class Test
                {
                    public System.Threading.Tasks.Task LoadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;
                    public System.Threading.Tasks.Task UnloadTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIThreadTestMethod]
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

        var result = VerifyGeneratedDiagnostics<UIThreadTestMethodGenerator>(source + DispatcherQueueDefinition);

        Assert.AreEqual(1, result.GeneratedTrees.Length, "More trees generated than expected.");
        // To do, should probably inspect tree more directly.
        var generatedSource = result.GeneratedTrees.First().ToString();
        Assert.IsTrue(generatedSource.Contains("await LoadTestContentAsync(testControl);"), "Didn't see expected loading call.");
        Assert.IsTrue(generatedSource.Contains("await UnloadTestContentAsync(testControl);"), "Didn't see expected unloading call.");
        Assert.IsTrue(generatedSource.Contains("EnqueueAsync(async () => {"), "Unexpected sync lambda in Enqueue call.");
        Assert.IsTrue(generatedSource.Contains("var testControl = new global::MyApp.MyControl();"), "Didn't see expected creation of test control.");
        Assert.IsTrue(generatedSource.Contains("TestMethod(testControl);"), "Didn't see expected running of test.");
    }

    [TestMethod]
    public void Synchronous_NoMethodParams_NoErrors()
    {
        string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Tooling.TestGen;

            namespace MyApp
            {
                public partial class Test
                {
                    [UIThreadTestMethod]
                    public void TestMethod()
                    {
                    }
                }
            }";

        var result = VerifyGeneratedDiagnostics<UIThreadTestMethodGenerator>(source + DispatcherQueueDefinition);

        Assert.AreEqual(1, result.GeneratedTrees.Length, "More trees generated than expected.");
        // To do, should probably inspect tree more directly.
        var generatedSource = result.GeneratedTrees.First().ToString();
        Assert.IsFalse(generatedSource.Contains("await LoadTestContentAsync"), "Saw a loading call.");
        Assert.IsFalse(generatedSource.Contains("await UnloadTestContentAsync"), "Saw an unloading call.");
        Assert.IsTrue(generatedSource.Contains("EnqueueAsync(() => {"), "Unexpected async lambda in Enqueue call.");
        Assert.IsTrue(generatedSource.Contains("TestMethod();"), "Didn't see expected running of test.");
        Assert.IsFalse(generatedSource.Contains("await TestMethod();"), "Sync method ran async instead.");
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="source">The input source to process.</param>
    /// <param name="markdown">The input documentation info to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static GeneratorDriverRunResult VerifyGeneratedDiagnostics<TGenerator>(string source, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        return VerifyGeneratedDiagnostics<TGenerator>(CSharpSyntaxTree.ParseText(source), diagnosticsIds);
    }

    /// <summary>
    /// Verifies the output of a source generator.
    /// </summary>
    /// <typeparam name="TGenerator">The generator type to use.</typeparam>
    /// <param name="syntaxTree">The input source tree to process.</param>
    /// <param name="markdown">The input documentation info to process.</param>
    /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
    private static GeneratorDriverRunResult VerifyGeneratedDiagnostics<TGenerator>(SyntaxTree syntaxTree, params string[] diagnosticsIds)
        where TGenerator : class, IIncrementalGenerator, new()
    {
        var attributeType = typeof(UIThreadTestMethodAttribute);

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

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

        HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();
        var generatedCompilationDiaghostics = outputCompilation.GetDiagnostics();

        Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds), $"Expected one of [{string.Join(", ", diagnosticsIds)}] diagnostic Ids. Got [{string.Join(", ", resultingIds)}]");
        Assert.IsTrue(generatedCompilationDiaghostics.All(x => x.Severity != DiagnosticSeverity.Error), $"Expected no generated compilation errors. Got: \n{string.Join("\n", generatedCompilationDiaghostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => $"[{x.Id}: {x.GetMessage()}]"))}");

        GC.KeepAlive(attributeType);

        var result = driver.GetRunResult();

        if (diagnosticsIds.Length == 0)
        {
            Assert.IsTrue(result.GeneratedTrees.Length > 0, "Generator did not produce any output!");
        }

        return result;
    }
}
