// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

partial class Test_DependencyPropertyGenerator
{
    [TestMethod]
    [DataRow("Uno.UI", true, false, false)]
    [DataRow("Uno.UI", true, true, false)]
    [DataRow("Uno.WinUI", false, false, false)]
    [DataRow("Uno.WinUI", false, true, false)]
    [DataRow("Uno.UI", false, false, false)]
    [DataRow("Uno.UI", false, true, false)]
    [DataRow("Microsoft.WinUI", false, false, false)]
    [DataRow("Uno.UI", true, false, true)]
    [DataRow("Uno.UI", true, true, true)]
    public async Task XamlFramework_ValidOwners_CompileAndDoNotWarn(string assemblyName, bool isInterface, bool renamedReference, bool useWindowsUIXaml)
    {
        string xamlNamespace = useWindowsUIXaml ? "Windows.UI.Xaml" : "Microsoft.UI.Xaml";
        PortableExecutableReference frameworkReference = XamlFrameworkTest.CreateFrameworkReference(
            assemblyName,
            isInterface,
            xamlNamespace,
            filePath: renamedReference ? @"C:\References\Unrelated.dll" : $@"C:\References\{assemblyName}.dll");

        const string properties = """
            [GeneratedDependencyProperty]
            public partial int Count { get; set; }

            [GeneratedDependencyProperty(DefaultValue = 42)]
            public partial int Answer { get; set; }

            [GeneratedDependencyProperty]
            public partial string? Caption { get; set; }

            [GeneratedDependencyProperty(DefaultValue = "Ready")]
            public partial string Text { get; set; }

            [GeneratedDependencyProperty]
            public partial int? OptionalCount { get; set; }

            [GeneratedDependencyProperty(DefaultValueCallback = nameof(CreateValue))]
            public partial int FactoryValue { get; set; }

            [GeneratedDependencyProperty(PropertyType = typeof(object))]
            public partial bool? Selection { get; set; }

            private static int CreateValue() => 7;

            partial void OnCountPropertyChanged(DependencyPropertyChangedEventArgs e) { }
            partial void OnPropertyChanged(DependencyPropertyChangedEventArgs e) { }
            partial void OnCountSet(ref int value) { value++; }
            partial void OnCountChanged(int value) { }
            """;

        string source = $$"""
            using CommunityToolkit.WinUI;
            using {{xamlNamespace}};
            using {{xamlNamespace}}.Controls;

            namespace MyApp;

            public partial class DirectOwner : DependencyObject
            {
                {{(isInterface ? "public object GetValue(DependencyProperty property) => null!;" : "")}}
                {{(isInterface ? "public void SetValue(DependencyProperty property, object value) { }" : "")}}
                {{properties}}
            }

            public partial class ControlOwner : Control
            {
                {{properties}}
            }

            public class IntermediateControl : Control { }

            public partial class IndirectOwner : IntermediateControl
            {
                {{properties}}
            }
            """;

        Compilation compilation = XamlFrameworkTest.RunGenerator(source, frameworkReference, useWindowsUIXaml);

        XamlFrameworkTest.AssertEmits(compilation);

        Assert.AreEqual(0, (await XamlFrameworkTest.GetAnalyzerDiagnosticsAsync(compilation, useWindowsUIXaml)).Length);

        foreach (string ownerName in new[] { "DirectOwner", "ControlOwner", "IndirectOwner" })
        {
            string generatedSource = compilation.SyntaxTrees.Single(tree => Path.GetFileName(tree.FilePath) == $"MyApp.{ownerName}.g.cs").ToString();

            StringAssert.Contains(generatedSource, $"global::{xamlNamespace}.DependencyProperty.Register(");
            StringAssert.Contains(generatedSource, $"ownerType: typeof({ownerName})");
            StringAssert.Contains(generatedSource, $"new global::{xamlNamespace}.PropertyMetadata(");
            StringAssert.Contains(generatedSource, "defaultValue: \"Ready\"");
            StringAssert.Contains(generatedSource, "defaultValue: 42");
            StringAssert.Contains(generatedSource, "defaultValue: default(int)");
            StringAssert.Contains(generatedSource, "defaultValue: null");
            StringAssert.Contains(generatedSource, "propertyType: typeof(int?)");
            StringAssert.Contains(generatedSource, "propertyType: typeof(object)");
            StringAssert.Contains(generatedSource, "CreateDefaultValueCallback(static () => CreateValue())");
            StringAssert.Contains(generatedSource, "GetValue(CountProperty)");
            StringAssert.Contains(generatedSource, "XamlBindingHelper.SetPropertyFromInt32(this, CountProperty, value)");
            StringAssert.Contains(generatedSource, "OnCountSet(ref value)");
            StringAssert.Contains(generatedSource, "OnCountChanged(value)");
            StringAssert.Contains(generatedSource, "PropertyChangedUnsafeAccessors.OnCountPropertyChanged(__this, e)");
            StringAssert.Contains(generatedSource, "PropertyChangedUnsafeAccessors.OnPropertyChanged(__this, e)");
        }
    }

    [TestMethod]
    [DataRow("Uno.UI", true)]
    [DataRow("Uno.WinUI", false)]
    [DataRow("Uno.UI", false)]
    public async Task XamlFramework_UnrelatedOwners_AreRejected(string assemblyName, bool isInterface)
    {
        const string source = """
            using CommunityToolkit.WinUI;

            namespace Unrelated
            {
                public class DependencyObject { }
            }

            namespace MyApp
            {
                public partial class PlainOwner
                {
                    [GeneratedDependencyProperty]
                    public partial int Value { get; set; }
                }

                public partial class UnrelatedOwner : Unrelated.DependencyObject
                {
                    [GeneratedDependencyProperty]
                    public partial int Value { get; set; }
                }
            }
            """;

        Compilation compilation = XamlFrameworkTest.RunGenerator(source, XamlFrameworkTest.CreateFrameworkReference(assemblyName, isInterface));
        ImmutableArray<Diagnostic> diagnostics = await XamlFrameworkTest.GetAnalyzerDiagnosticsAsync(compilation);

        Assert.AreEqual(2, diagnostics.Length);
        Assert.IsTrue(diagnostics.All(static diagnostic => diagnostic.Id == "WCTDPG0005"));
        Assert.IsFalse(compilation.SyntaxTrees.Any(tree => Path.GetFileName(tree.FilePath) is "MyApp.PlainOwner.g.cs" or "MyApp.UnrelatedOwner.g.cs"));
    }

    [TestMethod]
    [DataRow("struct")]
    [DataRow("record struct")]
    public async Task XamlFramework_InterfaceDependencyObject_ValueTypeOwners_AreRejected(string ownerKind)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Microsoft.UI.Xaml;

            namespace MyApp;

            public partial {{ownerKind}} Owner : DependencyObject
            {
                public object GetValue(DependencyProperty property) => null!;
                public void SetValue(DependencyProperty property, object value) { }

                [GeneratedDependencyProperty]
                public partial int Value { get; set; }
            }
            """;

        Compilation compilation = XamlFrameworkTest.RunGenerator(source, XamlFrameworkTest.CreateFrameworkReference("Uno.UI", isInterface: true));
        ImmutableArray<Diagnostic> diagnostics = await XamlFrameworkTest.GetAnalyzerDiagnosticsAsync(compilation);

        Assert.AreEqual("WCTDPG0005", diagnostics.Single().Id);
        Assert.IsFalse(compilation.SyntaxTrees.Any(tree => Path.GetFileName(tree.FilePath) == "MyApp.Owner.g.cs"));
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task XamlFramework_ExplicitNamespaceSelection_IsRespected(bool useWindowsUIXaml)
    {
        string xamlNamespace = useWindowsUIXaml ? "Windows.UI.Xaml" : "Microsoft.UI.Xaml";
        string source = $$"""
            using CommunityToolkit.WinUI;
            using {{xamlNamespace}};

            public partial class Owner : DependencyObject
            {
                [GeneratedDependencyProperty]
                public partial int Value { get; set; }
            }
            """;

        PortableExecutableReference reference = XamlFrameworkTest.CreateFrameworkReference("Framework", isInterface: false, xamlNamespace);
        Compilation matchingCompilation = XamlFrameworkTest.RunGenerator(source, reference, useWindowsUIXaml);

        XamlFrameworkTest.AssertEmits(matchingCompilation);

        Assert.AreEqual(0, (await XamlFrameworkTest.GetAnalyzerDiagnosticsAsync(matchingCompilation, useWindowsUIXaml)).Length);

        Compilation mismatchedCompilation = XamlFrameworkTest.RunGenerator(source, reference, !useWindowsUIXaml);

        Assert.IsFalse(mismatchedCompilation.SyntaxTrees.Any(tree => Path.GetFileName(tree.FilePath) == "Owner.g.cs"));
    }
}
