// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpCodeFixTest = CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers.CSharpCodeFixTest<
    CommunityToolkit.GeneratedDependencyProperty.UseFieldDeclarationCorrectlyAnalyzer,
    CommunityToolkit.GeneratedDependencyProperty.UseFieldDeclarationCorrectlyCodeFixer>;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

[TestClass]
public class Test_UseFieldDeclarationCorrectlyCodeFixer
{
    [TestMethod]
    [DataRow("private static readonly DependencyProperty")]
    [DataRow("public readonly DependencyProperty")]
    [DataRow("public static DependencyProperty")]
    [DataRow("public static volatile DependencyProperty")]
    [DataRow("public static readonly DependencyProperty?")]
    public async Task SingleField(string fieldDeclaration)
    {
        string original = $$"""
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                {{fieldDeclaration}} [|TestProperty|];
            }
            """;

        const string @fixed = """
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty TestProperty;
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
    [DataRow("private static readonly DependencyProperty")]
    [DataRow("public readonly DependencyProperty")]
    [DataRow("public static DependencyProperty")]
    [DataRow("public static volatile DependencyProperty")]
    [DataRow("public static readonly DependencyProperty?")]
    public async Task SingleField_WithInitializer(string fieldDeclaration)
    {
        string original = $$"""
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                {{fieldDeclaration}} [|TestProperty|] = DependencyProperty.Register(
                    "Test",
                    typeof(string),
                    typeof(MyObject),
                    null);
            }
            """;

        const string @fixed = """
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty TestProperty = DependencyProperty.Register(
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
    public async Task MultipleFields()
    {
        string original = $$"""
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                private static readonly DependencyProperty [|Test1Property|];
                public readonly DependencyProperty [|Test2Property|];
                public static DependencyProperty [|Test3Property|];
                public static readonly DependencyProperty Test4Property;
                public static volatile DependencyProperty [|Test5Property|];
                public static readonly DependencyProperty? [|Test6Property|];
                public static readonly DependencyProperty Test7Property;
                public static readonly DependencyProperty Test8Property;
            }
            """;

        const string @fixed = """
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty Test1Property;
                public static readonly DependencyProperty Test2Property;
                public static readonly DependencyProperty Test3Property;
                public static readonly DependencyProperty Test4Property;
                public static readonly DependencyProperty Test5Property;
                public static readonly DependencyProperty Test6Property;
                public static readonly DependencyProperty Test7Property;
                public static readonly DependencyProperty Test8Property;
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
    public async Task MultipleFields_WithInitializers()
    {
        string original = $$"""
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                private static readonly DependencyProperty [|Test1Property|] = DependencyProperty.Register(
                    "Test1",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public readonly DependencyProperty [|Test2Property|] = DependencyProperty.Register(
                    "Test2",
                    typeof(string),
                    typeof(MyObject),
                    null);

                public static DependencyProperty [|Test3Property|];
                public static readonly DependencyProperty Test4Property = DependencyProperty.Register("Test4", typeof(string), typeof(MyObject), null);
                public static volatile DependencyProperty [|Test5Property|];
                public static readonly DependencyProperty? [|Test6Property|] = DependencyProperty.Register("Test6", typeof(string), typeof(MyObject), null);
                public static readonly DependencyProperty Test7Property;
                public static readonly DependencyProperty Test8Property = DependencyProperty.Register(
                    "Test8",
                    typeof(string),
                    typeof(MyObject),
                    null);
            }
            """;

        const string @fixed = """
            using Windows.UI.Xaml;

            #nullable enable

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

                public static readonly DependencyProperty Test3Property;
                public static readonly DependencyProperty Test4Property = DependencyProperty.Register("Test4", typeof(string), typeof(MyObject), null);
                public static readonly DependencyProperty Test5Property;
                public static readonly DependencyProperty Test6Property = DependencyProperty.Register("Test6", typeof(string), typeof(MyObject), null);
                public static readonly DependencyProperty Test7Property;
                public static readonly DependencyProperty Test8Property = DependencyProperty.Register(
                    "Test8",
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
}
