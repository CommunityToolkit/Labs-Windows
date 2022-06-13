using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;
using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Tests
{
    [TestClass]
    public partial class UIControlTestMethodTests
    {
        private const string AppDispatcherQueueDefinition = @"
namespace MyApp
{
    public class DispatcherQueue
    {
        public System.Threading.Tasks.Task EnqueueAsync(System.Action function)
        {
            return System.Threading.Tasks.Task.Run(function);
        }
    }

    public class App
    {
        public static DispatcherQueue DispatcherQueue { get; } = new DispatcherQueue();
    }
}
";

        [TestMethod]
        public void TypeDoesNotInheritFrameworkElement()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Windows.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(Test))]
                    public void TestMethod()
                    {
                    }
                }

                public class MyControl : Windows.UI.Xaml.FrameworkElement
                {
                }

                namespace Windows.UI.Xaml
                {
                    public class FrameworkElement { }
                }
            }";

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition, DiagnosticDescriptors.TypeDoesNotInheritFrameworkElement.Id);
        }

        [TestMethod]
        public void TypeDoesInheritFrameworkElement_Wux()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Windows.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Windows.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(MyControl))]
                    public void TestMethod()
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

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition);
        }

        [TestMethod]
        public void TypeDoesInheritFrameworkElement_Mux()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Microsoft.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(MyControl))]
                    public void TestMethod()
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

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition);
        }

        [TestMethod]
        public void TestControlHasNoConstructorWithParameters()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Microsoft.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(MyControl))]
                    public void TestMethod()
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

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition);
        }

        [TestMethod]
        public void TestControlHasConstructorWithParameters()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Microsoft.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(MyControl))]
                    public void TestMethod()
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

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition, DiagnosticDescriptors.TestControlHasConstructorWithParameters.Id);
        }

        [TestMethod]
        public void TestMethodHasParameterlessConstructor()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Microsoft.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(MyControl))]
                    public void TestMethod()
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

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition);
        }

        [TestMethod]
        public void TestMethodDoesNotHaveParameterlessConstructor()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Microsoft.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(MyControl))]
                    public void TestMethod(string id)
                    {
                    }
                }

                public class MyControl : Microsoft.UI.Xaml.FrameworkElement
                {
                    public MyControl()
                    {
                    }
                }
            }

            namespace Microsoft.UI.Xaml
            {
                public class FrameworkElement { }
            }";

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition, DiagnosticDescriptors.TestMethodIsNotParameterless.Id);
        }

        [TestMethod]
        public void AsyncMethod_NoErrors()
        {
            string source = @"
            using System.ComponentModel;
            using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

            namespace MyApp
            {
                public partial class Test
                {
                    public Microsoft.UI.Xaml.FrameworkElement? TestPage { get; private set; }
                    public System.Threading.Tasks.Task SetTestContentAsync(Microsoft.UI.Xaml.FrameworkElement content) => System.Threading.Tasks.Task.CompletedTask;

                    [UIControlTestMethod(typeof(MyControl))]
                    public async System.Threading.Tasks.Task TestMethod()
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

            VerifyGeneratedDiagnostics<UIControlTestMethodGenerator>(source + AppDispatcherQueueDefinition);
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
            var attributeType = typeof(UIControlTestMethodAttribute);

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

            Assert.IsTrue(compilationDiagnostics.All(x => x.Severity != DiagnosticSeverity.Error), $"Expected no compilation errors. Got: \n[{string.Join("\n", compilationDiagnostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => $"{x.Id}: {x.GetMessage()}"))}]");

            IIncrementalGenerator generator = new TGenerator();

            GeneratorDriver driver =
                CSharpGeneratorDriver
                    .Create(generator)
                    .WithUpdatedParseOptions((CSharpParseOptions)syntaxTree.Options);

            _ = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

            HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();
            var generatedCompilationDiaghostics = outputCompilation.GetDiagnostics();

            Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds), $"Expected one of [{string.Join(", ", diagnosticsIds)}] diagnostic Ids. Got [{string.Join(", ", resultingIds)}]");
            Assert.IsTrue(generatedCompilationDiaghostics.All(x => x.Severity != DiagnosticSeverity.Error), $"Expected no generated compilation errors. Got: \n[{string.Join("\n", generatedCompilationDiaghostics.Where(x => x.Severity == DiagnosticSeverity.Error).Select(x => $"{x.Id}: {x.GetMessage()}"))}]");

            GC.KeepAlive(attributeType);
        }
    }
}
