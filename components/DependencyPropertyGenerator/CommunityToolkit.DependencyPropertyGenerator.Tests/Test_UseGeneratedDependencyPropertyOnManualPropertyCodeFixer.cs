// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    [DataRow("byte", "byte")]
    [DataRow("sbyte", "sbyte")]
    [DataRow("short", "short")]
    [DataRow("ushort", "ushort")]
    [DataRow("uint", "uint")]
    [DataRow("long", "long")]
    [DataRow("ulong", "ulong")]
    [DataRow("char", "char")]
    [DataRow("float", "float")]
    [DataRow("double", "double")]
    [DataRow("global::System.Numerics.Matrix3x2", "global::System.Numerics.Matrix3x2")]
    [DataRow("global::System.Numerics.Matrix4x4", "global::System.Numerics.Matrix4x4")]
    [DataRow("global::System.Numerics.Plane", "global::System.Numerics.Plane")]
    [DataRow("global::System.Numerics.Quaternion", "global::System.Numerics.Quaternion")]
    [DataRow("global::System.Numerics.Vector2", "global::System.Numerics.Vector2")]
    [DataRow("global::System.Numerics.Vector3", "global::System.Numerics.Vector3")]
    [DataRow("global::System.Numerics.Vector4", "global::System.Numerics.Vector4")]
    [DataRow("global::Windows.Foundation.Point", "global::Windows.Foundation.Point")]
    [DataRow("global::Windows.Foundation.Rect", "global::Windows.Foundation.Rect")]
    [DataRow("global::Windows.Foundation.Size", "global::Windows.Foundation.Size")]
    [DataRow("global::Windows.UI.Xaml.Visibility", "global::Windows.UI.Xaml.Visibility")]
    [DataRow("int?", "int?")]
    [DataRow("byte?", "byte?")]
    [DataRow("char?", "char?")]
    [DataRow("long?", "long?")]
    [DataRow("float?", "float?")]
    [DataRow("double?", "double?")]
    [DataRow("global::System.DateTimeOffset?", "global::System.DateTimeOffset?")]
    [DataRow("global::System.TimeSpan?", "global::System.TimeSpan?")]
    [DataRow("global::System.Guid?", "global::System.Guid?")]
    [DataRow("global::System.Collections.Generic.KeyValuePair<int, float>?", "global::System.Collections.Generic.KeyValuePair<int, float>?")]
    [DataRow("global::MyApp.MyStruct?", "global::MyApp.MyStruct?")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?")]
    [DataRow("global::MyApp.MyClass", "global::MyApp.MyClass")]
    [DataRow("global::MyApp.MyClass", "global::MyApp.MyClass?")]
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

            public struct MyStruct { public string X { get; set; } }
            public enum MyEnum { A, B, C }
            public class MyClass { }
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

            public struct MyStruct { public string X { get; set; } }
            public enum MyEnum { A, B, C }
            public class MyClass { }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
        };

        await test.RunAsync();
    }

    // These are custom value types, on properties where the metadata was set to 'null'. In this case, the
    // default value would just be 'null', as XAML can't default initialize them. To preserve behavior,
    // we must include an explicit default value. This will warn when the code is recompiled, but that
    // is expected, because this specific scenario was (1) niche, and (2) kinda busted already anyway.
    [TestMethod]
    [DataRow("global::MyApp.MyStruct", "global::MyApp.MyStruct")]
    [DataRow("global::MyApp.MyEnum", "global::MyApp.MyEnum")]
    public async Task SimpleProperty_ExplicitNull(string dependencyPropertyType, string propertyType)
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

            public struct MyStruct { public string X { get; set; } }
            public enum MyEnum { A, B, C }
            """;

        string @fixed = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = null)]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }

            public struct MyStruct { public string X { get; set; } }
            public enum MyEnum { A, B, C }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
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
    [DataRow("global::System.Numerics.Matrix3x2", "global::System.Numerics.Matrix3x2", "default(global::System.Numerics.Matrix3x2)")]
    [DataRow("global::System.Numerics.Matrix4x4", "global::System.Numerics.Matrix4x4", "default(global::System.Numerics.Matrix4x4)")]
    [DataRow("global::System.Numerics.Plane", "global::System.Numerics.Plane", "default(global::System.Numerics.Plane)")]
    [DataRow("global::System.Numerics.Quaternion", "global::System.Numerics.Quaternion", "default(global::System.Numerics.Quaternion)")]
    [DataRow("global::System.Numerics.Vector2", "global::System.Numerics.Vector2", "default(global::System.Numerics.Vector2)")]
    [DataRow("global::System.Numerics.Vector3", "global::System.Numerics.Vector3", "default(global::System.Numerics.Vector3)")]
    [DataRow("global::System.Numerics.Vector4", "global::System.Numerics.Vector4", "default(global::System.Numerics.Vector4)")]
    [DataRow("global::Windows.Foundation.Point", "global::Windows.Foundation.Point", "default(global::Windows.Foundation.Point)")]
    [DataRow("global::Windows.Foundation.Rect", "global::Windows.Foundation.Rect", "default(global::Windows.Foundation.Rect)")]
    [DataRow("global::Windows.Foundation.Size", "global::Windows.Foundation.Size", "default(global::Windows.Foundation.Size)")]
    [DataRow("global::Windows.UI.Xaml.Visibility", "global::Windows.UI.Xaml.Visibility", "default(global::Windows.UI.Xaml.Visibility)")]
    [DataRow("global::Windows.UI.Xaml.Visibility", "global::Windows.UI.Xaml.Visibility", "global::Windows.UI.Xaml.Visibility.Visible")]
    [DataRow("global::System.DateTimeOffset?", "global::System.DateTimeOffset?", "default(global::System.DateTimeOffset?)")]
    [DataRow("global::System.DateTimeOffset?", "global::System.DateTimeOffset?", "null")]
    [DataRow("global::System.TimeSpan?", "global::System.TimeSpan?", "null")]
    [DataRow("global::System.Guid?", "global::System.Guid?", "null")]
    [DataRow("global::System.Collections.Generic.KeyValuePair<int, float>?", "global::System.Collections.Generic.KeyValuePair<int, float>?", "null")]
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
            FixedCode = @fixed
        };

        await test.RunAsync();
    }

    [TestMethod]
    [DataRow("string", "string", "\"\"")]
    [DataRow("string", "string", "\"Hello\"")]
    [DataRow("int", "int", "42")]
    [DataRow("int?", "int?", "0")]
    [DataRow("int?", "int?", "42")]
    [DataRow("Visibility", "Visibility", "Visibility.Collapsed")]
    [DataRow("global::MyApp.MyEnum", "global::MyApp.MyEnum", "(global::MyApp.MyEnum)5")]
    [DataRow("global::MyApp.MyEnum", "global::MyApp.MyEnum", "(global::MyApp.MyEnum)(-5)")]
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

            public enum MyEnum { A }
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

            public enum MyEnum { A }
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
        };

        await test.RunAsync();
    }

    [TestMethod]
    public async Task SimpleProperty_WithExplicitValue_NotDefault_AddsNamespace()
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
                    propertyType: typeof(Windows.UI.Xaml.Automation.AnnotationType),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata(Windows.UI.Xaml.Automation.AnnotationType.TrackChanges));

                public Windows.UI.Xaml.Automation.AnnotationType [|Name|]
                {
                    get => (Windows.UI.Xaml.Automation.AnnotationType)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        string @fixed = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Automation;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = AnnotationType.TrackChanges)]
                public partial Windows.UI.Xaml.Automation.AnnotationType {|CS9248:Name|} { get; set; }
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
