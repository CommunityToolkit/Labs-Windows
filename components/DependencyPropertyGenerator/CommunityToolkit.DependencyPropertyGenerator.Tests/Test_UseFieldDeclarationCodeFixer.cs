// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers.CSharpCodeFixTest<
    CommunityToolkit.GeneratedDependencyProperty.UseFieldDeclarationAnalyzer,
    CommunityToolkit.GeneratedDependencyProperty.UseFieldDeclarationCodeFixer>;
using CSharpCodeFixVerifier = Microsoft.CodeAnalysis.CSharp.Testing.CSharpCodeFixVerifier<
    CommunityToolkit.GeneratedDependencyProperty.UseFieldDeclarationAnalyzer,
    CommunityToolkit.GeneratedDependencyProperty.UseFieldDeclarationCodeFixer,
    Microsoft.CodeAnalysis.Testing.DefaultVerifier>;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

[TestClass]
public class Test_UseFieldDeclarationCodeFixer
{
    [TestMethod]
    [DataRow("private static DependencyProperty", "public static readonly DependencyProperty")]
    [DataRow("public static DependencyProperty", "public static readonly DependencyProperty")]
    [DataRow("public static new DependencyProperty", "public new static readonly DependencyProperty")]
    public async Task SingleProperty(string propertyDeclaration, string fieldDeclaration)
    {
        string original = $$"""
            using Windows.UI.Xaml;

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                {{propertyDeclaration}} [|TestProperty|] { get; } = DependencyProperty.Register(
                    "Test",
                    typeof(string),
                    typeof(MyObject),
                    null);
            }
            """;

        string @fixed = $$"""
            using Windows.UI.Xaml;

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                {{fieldDeclaration}} TestProperty = DependencyProperty.Register(
                    "Test",
                    typeof(string),
                    typeof(MyObject),
                    null);
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
        };

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("private static DependencyProperty", "public static readonly DependencyProperty")]
    [DataRow("public static DependencyProperty", "public static readonly DependencyProperty")]
    [DataRow("public static new DependencyProperty", "public new static readonly DependencyProperty")]
    public async Task SingleProperty_WithExpressionBody(string propertyDeclaration, string fieldDeclaration)
    {
        string original = $$"""
            using Windows.UI.Xaml;

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                {{propertyDeclaration}} [|TestProperty|] => DependencyProperty.Register(
                    "Test",
                    typeof(string),
                    typeof(MyObject),
                    null);
            }
            """;

        string @fixed = $$"""
            using Windows.UI.Xaml;

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                {{fieldDeclaration}} TestProperty = DependencyProperty.Register(
                    "Test",
                    typeof(string),
                    typeof(MyObject),
                    null);
            }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
        };

        await test.RunAsync();
    }

    [TestMethod]
    public async Task MultipleProperties_WithInitializers()
    {
        const string original = """
            using System;
            using Windows.UI.Xaml;

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                private static DependencyProperty [|Test1Property|] => DependencyProperty.Register(
                    "Test1",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public static DependencyProperty [|Test2Property|] => DependencyProperty.Register(
                    "Test2",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public static DependencyProperty [|Test3Property|] { get; } = DependencyProperty.Register(
                    "Test3",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public static DependencyProperty [|Test4Property|] { get; }

                [Test]
                public static DependencyProperty [|Test5Property|] { get; }
            }

            public class TestAttribute : Attribute;
            """;

        const string @fixed = """
            using System;
            using Windows.UI.Xaml;

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty Test1Property = DependencyProperty.Register(
                    "Test1",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public static readonly DependencyProperty Test2Property = DependencyProperty.Register(
                    "Test2",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public static readonly DependencyProperty Test3Property = DependencyProperty.Register(
                    "Test3",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public static readonly DependencyProperty Test4Property;

                [Test]
                public static DependencyProperty Test5Property { get; }
            }

            public class TestAttribute : Attribute;
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
        };

        test.FixedState.ExpectedDiagnostics.AddRange(
        [
            // /0/Test0.cs(29,38): warning WCTDP0021: The property 'MyApp.MyObject.Test5Property' is a dependency property, which is not the correct declaration type (all dependency properties should be declared as fields, unless implementing interface members or in authored WinRT component types)
            CSharpCodeFixVerifier.Diagnostic().WithSpan(29, 38, 29, 51).WithArguments("MyApp.MyObject.Test5Property")
        ]);

        await test.RunAsync();
    }
}
