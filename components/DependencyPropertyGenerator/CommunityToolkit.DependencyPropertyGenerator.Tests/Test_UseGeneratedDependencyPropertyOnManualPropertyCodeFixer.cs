// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using CSharpCodeFixTest = CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers.CSharpCodeFixTest<
    CommunityToolkit.GeneratedDependencyProperty.UseGeneratedDependencyPropertyOnManualPropertyAnalyzer,
    CommunityToolkit.GeneratedDependencyProperty.UseGeneratedDependencyPropertyOnManualPropertyCodeFixer>;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

[TestClass]
public class Test_UseGeneratedDependencyPropertyOnManualPropertyCodeFixer
{
    [TestMethod]
    [DataRow("string", "string")]
    [DataRow("string", "string?")]
    [DataRow("object", "object")]
    [DataRow("object", "object?")]
    [DataRow("int", "int")]
    [DataRow("int?", "int?")]
    public async Task SimpleProperty(string dependencyPropertyType, string propertyType)
    {
        string original = $$"""
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: nameof(Name),
                    propertyType: typeof({{dependencyPropertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public {{propertyType}} [|Name|]
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        string @fixed = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            TestState = { AdditionalReferences =
            {
                MetadataReference.CreateFromFile(typeof(ApplicationView).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DependencyProperty).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(GeneratedDependencyPropertyAttribute).Assembly.Location)
            }}
        };

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("string", "string", "null")]
    [DataRow("string", "string", "default(string)")]
    [DataRow("string", "string", "(string)null")]
    [DataRow("string", "string?", "null")]
    [DataRow("object", "object", "null")]
    [DataRow("object", "object?", "null")]
    [DataRow("int", "int", "0")]
    [DataRow("int", "int", "default(int)")]
    [DataRow("int?", "int?", "null")]
    [DataRow("int?", "int?", "default(int?)")]
    [DataRow("int?", "int?", "null")]
    [DataRow("System.TimeSpan", "System.TimeSpan", "default(System.TimeSpan)")]
    public async Task SimpleProperty_WithExplicitValue_DefaultValue(
        string dependencyPropertyType,
        string propertyType,
        string defaultValueExpression)
    {
        string original = $$"""
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{dependencyPropertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{defaultValueExpression}}));

                public {{propertyType}} [|Name|]
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        string @fixed = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            TestState = { AdditionalReferences =
            {
                MetadataReference.CreateFromFile(typeof(ApplicationView).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DependencyProperty).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(GeneratedDependencyPropertyAttribute).Assembly.Location)
            }}
        };

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("string", "string", "\"\"")]
    [DataRow("string", "string", "\"Hello\"")]
    [DataRow("int", "int", "42")]
    [DataRow("int?", "int?", "0")]
    [DataRow("int?", "int?", "42")]
    public async Task SimpleProperty_WithExplicitValue_NotDefault(
        string dependencyPropertyType,
        string propertyType,
        string defaultValueExpression)
    {
        string original = $$"""
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{dependencyPropertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{defaultValueExpression}}));

                public {{propertyType}} [|Name|]
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        string @fixed = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = {{defaultValueExpression}})]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            TestState = { AdditionalReferences =
            {
                MetadataReference.CreateFromFile(typeof(ApplicationView).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DependencyProperty).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(GeneratedDependencyPropertyAttribute).Assembly.Location)
            }}
        };

        await test.RunAsync();
    }
}
