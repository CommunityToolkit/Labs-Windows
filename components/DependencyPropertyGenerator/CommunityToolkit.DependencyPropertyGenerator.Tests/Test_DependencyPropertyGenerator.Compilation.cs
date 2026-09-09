// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

partial class Test_DependencyPropertyGenerator
{
    [TestMethod]
    [DataRow("int", "42")]
    [DataRow("int?", "42")]
    [DataRow("bool", "true")]
    [DataRow("string", "\"Hello world!\"")]
    [DataRow("int", null)]
    [DataRow("int?", null)]
    [DataRow("bool", null)]
    [DataRow("string", null)]
    public void SingleProperty_WithLocalCache_Compiles(string propertyType, string? defaultValue)
    {
        string defaultValueArgument = defaultValue is null ? "" : $", DefaultValue = {defaultValue}";

        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty(IsLocalCacheEnabled = true{{defaultValueArgument}})]
                public partial {{propertyType}} Value { get; set; }
            }
            """;

        Compilation compilation = CSharpGeneratorTest<DependencyPropertyGenerator>.VerifyCompiles(source, LanguageVersion.Preview);

        PropertyDeclarationSyntax property = compilation.SyntaxTrees
            .Single(tree => Path.GetFileName(tree.FilePath) == "MyNamespace.MyControl.g.cs")
            .GetRoot()
            .DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Single();

        Assert.AreEqual(defaultValue, property.Initializer?.Value.ToString());
    }

    [TestMethod]
    [DynamicData(nameof(GetDefaultValueCallbackCompilationData), DynamicDataSourceType.Method)]
    public void SingleProperty_WithDefaultValueCallback_Compiles(
        string propertyType,
        string returnType,
        string returnValue,
        string constraints,
        bool requiresBoxing,
        bool isGeneric,
        bool hasPropertyChangedCallback,
        bool hasSharedPropertyChangedCallback)
    {
        string typeParameters = isGeneric ? "<T>" : "";
        string propertyChangedCallback = hasPropertyChangedCallback
            ? "partial void OnValuePropertyChanged(DependencyPropertyChangedEventArgs e) { }"
            : "";
        string sharedPropertyChangedCallback = hasSharedPropertyChangedCallback
            ? "partial void OnPropertyChanged(DependencyPropertyChangedEventArgs e) { }"
            : "";

        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyNamespace;

            public partial class MyControl{{typeParameters}} : DependencyObject {{constraints}}
            {
                [GeneratedDependencyProperty(DefaultValueCallback = nameof(CreateValue))]
                public partial {{propertyType}} Value { get; set; }

                private static {{returnType}} CreateValue() => {{returnValue}};

                {{propertyChangedCallback}}
                {{sharedPropertyChangedCallback}}
            }
            """;

        Compilation compilation = CSharpGeneratorTest<DependencyPropertyGenerator>.VerifyCompiles(source);

        ArgumentSyntax callbackArgument = compilation.SyntaxTrees
            .SelectMany(tree => tree.GetRoot().DescendantNodes())
            .OfType<ArgumentSyntax>()
            .Single(argument => argument.NameColon?.Name.Identifier.ValueText == "createDefaultValueCallback");

        string expectedCallback = requiresBoxing ? "static () => CreateValue()" : "CreateValue";

        Assert.AreEqual($"new Windows.UI.Xaml.CreateDefaultValueCallback({expectedCallback})", callbackArgument.Expression.ToString());
    }

    public static IEnumerable<object[]> GetDefaultValueCallbackCompilationData()
    {
        (string PropertyType, string ReturnType, string ReturnValue, string Constraints, bool RequiresBoxing)[] cases =
        [
            ("int", "int", "42", "", true),
            ("int", "object", "42", "", false),
            ("int?", "int?", "42", "", true),
            ("int?", "int?", "null", "", true),
            ("int?", "int", "42", "", true),
            ("int?", "object?", "null", "", false),
            ("string", "string", "\"Hello world!\"", "", false),
            ("string", "object", "\"Hello world!\"", "", false),
            ("T", "T", "default!", "", true),
            ("T", "T", "default", "where T : struct", true),
            ("T?", "T?", "null", "where T : struct", true),
            ("T?", "T", "default", "where T : struct", true),
            ("T", "T", "default!", "where T : System.IComparable<T>", true),
            ("T", "T", "default!", "where T : class", false),
            ("T", "T", "default!", "where T : System.IO.Stream", false)
        ];

        foreach (var (propertyType, returnType, returnValue, constraints, requiresBoxing) in cases)
        {
            foreach (bool isGeneric in new[] { false, true })
            {
                if (!isGeneric && propertyType is "T" or "T?")
                {
                    continue;
                }

                foreach (bool hasPropertyChangedCallback in new[] { false, true })
                {
                    foreach (bool hasSharedPropertyChangedCallback in new[] { false, true })
                    {
                        yield return
                        [
                            propertyType,
                            returnType,
                            returnValue,
                            constraints,
                            requiresBoxing,
                            isGeneric,
                            hasPropertyChangedCallback,
                            hasSharedPropertyChangedCallback
                        ];
                    }
                }
            }
        }
    }
}
