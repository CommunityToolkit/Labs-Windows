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
    public async Task SimpleProperty_WithExplicitValue_EmptyString()
    {
        const string original = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata(string.Empty));

                public string [|Name|]
                {
                    get => (string)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = "")]
                public partial string {|CS9248:Name|} { get; set; }
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
    public async Task SimpleProperty_WithExplicitValue_NestedEnumType()
    {
        const string original = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(MyContainingType.MyEnum),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata(MyContainingType.MyEnum.B));

                public MyContainingType.MyEnum [|Name|]
                {
                    get => (MyContainingType.MyEnum)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public class MyContainingType
            {
                public enum MyEnum { A, B }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = MyContainingType.MyEnum.B)]
                public partial MyContainingType.MyEnum {|CS9248:Name|} { get; set; }
            }

            public class MyContainingType
            {
                public enum MyEnum { A, B }
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
    public async Task SimpleProperty_WithExplicitValue_NestedEnumType_WithUsingStatic()
    {
        const string original = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            using static MyApp.MyContainingType;

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(MyEnum),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata(MyEnum.B));

                public MyEnum [|Name|]
                {
                    get => (MyEnum)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public class MyContainingType
            {
                public enum MyEnum { A, B }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            using static MyApp.MyContainingType;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = MyEnum.B)]
                public partial MyEnum {|CS9248:Name|} { get; set; }
            }

            public class MyContainingType
            {
                public enum MyEnum { A, B }
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
    public async Task SimpleProperty_WithExplicitValue_NotDefault_AddsNamespace()
    {
        const string original = """
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

        const string @fixed = """
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

    [TestMethod]
    [DataRow("[A]", "[static: A]")]
    [DataRow("""[Test(42, "Hello")]""", """[static: Test(42, "Hello")]""")]
    [DataRow("""[field: Test(42, "Hello")]""", """[static: Test(42, "Hello")]""")]
    [DataRow("""[A, Test(42, "Hello")]""", """[static: A, Test(42, "Hello")]""")]
    [DataRow("""
        [A]
            [Test(42, "Hello")]
        """, """
        [static: A]
            [static: Test(42, "Hello")]
        """)]
    public async Task SimpleProperty_WithForwardedAttributes(
        string attributeDefinition,
        string attributeForwarding)
    {
        string original = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public class MyControl : Control
            {
                {{attributeDefinition}}
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: nameof(Name),
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? [|Name|]
                {
                    get => (string?)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public class AAttribute : Attribute;
            public class TestAttribute(int X, string Y) : Attribute;
            """;

        string @fixed = $$"""            
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                {{attributeForwarding}}
                public partial string? {|CS9248:Name|} { get; set; }
            }

            public class AAttribute : Attribute;
            public class TestAttribute(int X, string Y) : Attribute;
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
        };

        await test.RunAsync();
    }

    [TestMethod]
    public async Task MultipleProperties_HandlesSpacingCorrectly()
    {
        const string original = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty Name1Property = DependencyProperty.Register(
                    name: "Name1",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public static readonly DependencyProperty Name2Property = DependencyProperty.Register(
                    name: "Name2",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? [|Name1|]
                {
                    get => (string?)GetValue(Name1Property);
                    set => SetValue(Name1Property, value);
                }

                public string? [|Name2|]
                {
                    get => (string?)GetValue(Name2Property);
                    set => SetValue(Name2Property, value);
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name1|} { get; set; }

                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name2|} { get; set; }
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
    public async Task MultipleProperties_WithXmlDocs_HandlesSpacingCorrectly()
    {
        const string original = """
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public abstract partial class MyObject<TElement, TValue> : DependencyObject
            {
                /// <summary>
                /// Blah.
                /// </summary>
                public static readonly DependencyProperty TargetObjectProperty = DependencyProperty.Register(
                    nameof(TargetObject),
                    typeof(TElement?),
                    typeof(MyObject<TElement, TValue>),
                    null);

                /// <summary>
                /// Blah.
                /// </summary>
                public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                    nameof(Value),
                    typeof(TValue?),
                    typeof(MyObject<TElement, TValue>),
                    null);

                /// <summary>
                /// Blah.
                /// </summary>
                public TValue? [|Value|]
                {
                    get => (TValue?)GetValue(ValueProperty);
                    set => SetValue(ValueProperty, value);
                }

                /// <summary>
                /// Blah.
                /// </summary>
                public TElement? [|TargetObject|]
                {
                    get => (TElement?)GetValue(TargetObjectProperty);
                    set => SetValue(TargetObjectProperty, value);
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public abstract partial class MyObject<TElement, TValue> : DependencyObject
            {
                /// <summary>
                /// Blah.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial TValue? {|CS9248:Value|} { get; set; }

                /// <summary>
                /// Blah.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial TElement? {|CS9248:TargetObject|} { get; set; }
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
    public async Task MultipleProperties_WithInterspersedMembers_HandlesSpacingCorrectly()
    {
        const string original = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty Name1Property = DependencyProperty.Register(
                    name: "Name1",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public static readonly DependencyProperty Name2Property = DependencyProperty.Register(
                    name: "Name2",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                /// <summary>This is another member</summary>
                public int Blah => 42;

                public string? [|Name1|]
                {
                    get => (string?)GetValue(Name1Property);
                    set => SetValue(Name1Property, value);
                }

                public string? [|Name2|]
                {
                    get => (string?)GetValue(Name2Property);
                    set => SetValue(Name2Property, value);
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                /// <summary>This is another member</summary>
                public int Blah => 42;

                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name1|} { get; set; }

                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name2|} { get; set; }
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
    public async Task MultipleProperties_WithLeadingPersistentMembers_HandlesSpacingCorrectly()
    {
        const string original = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty Name1Property = DependencyProperty.Register(
                    name: "Name1",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public static readonly DependencyProperty Name2Property = DependencyProperty.Register(
                    name: "Name2",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? Name1
                {
                    get => (string?)GetValue(Name1Property) ?? string.Empty;
                    set => SetValue(Name1Property, value);
                }

                public string? [|Name2|]
                {
                    get => (string?)GetValue(Name2Property);
                    set => SetValue(Name2Property, value);
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty Name1Property = DependencyProperty.Register(
                    name: "Name1",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? Name1
                {
                    get => (string?)GetValue(Name1Property) ?? string.Empty;
                    set => SetValue(Name1Property, value);
                }

                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name2|} { get; set; }
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
    public async Task MultipleProperties_WithLeadingPersistentMembers_WithXmlDocs_HandlesSpacingCorrectly()
    {
        const string original = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                /// <summary>Blah</summary>
                public static readonly DependencyProperty Name1Property = DependencyProperty.Register(
                    name: "Name1",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                /// <summary>Blah</summary>
                public static readonly DependencyProperty Name2Property = DependencyProperty.Register(
                    name: "Name2",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                /// <summary>Blah</summary>
                public string? Name1
                {
                    get => (string?)GetValue(Name1Property) ?? string.Empty;
                    set => SetValue(Name1Property, value);
                }

                /// <summary>Blah</summary>
                public string? [|Name2|]
                {
                    get => (string?)GetValue(Name2Property);
                    set => SetValue(Name2Property, value);
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                /// <summary>Blah</summary>
                public static readonly DependencyProperty Name1Property = DependencyProperty.Register(
                    name: "Name1",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                /// <summary>Blah</summary>
                public string? Name1
                {
                    get => (string?)GetValue(Name1Property) ?? string.Empty;
                    set => SetValue(Name1Property, value);
                }

                /// <summary>Blah</summary>
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name2|} { get; set; }
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
    [DataRow("float", "0.0F", "1.0F", "0.123F")]
    [DataRow("double", "0.0", "4.0", "0.123")]
    public async Task MultipleProperties_HandlesWellKnownLiterals(string propertyType, string zeroExpression, string literalExpression, string decimalLiteralExpression)
    {
        string original = $$"""
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty P1Property = DependencyProperty.Register(
                    name: "P1",
                    propertyType: typeof({{propertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{zeroExpression}}));

                public static readonly DependencyProperty P2Property = DependencyProperty.Register(
                    name: "P2",
                    propertyType: typeof({{propertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{propertyType}}.MinValue));

                public static readonly DependencyProperty P3Property = DependencyProperty.Register(
                    name: "P3",
                    propertyType: typeof({{propertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{propertyType}}.NaN));

                public static readonly DependencyProperty P4Property = DependencyProperty.Register(
                    name: "P4",
                    propertyType: typeof({{propertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{propertyType}}.Pi));

                public static readonly DependencyProperty P5Property = DependencyProperty.Register(
                    name: "P5",
                    propertyType: typeof({{propertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{literalExpression}}));

                public static readonly DependencyProperty P6Property = DependencyProperty.Register(
                    name: "P6",
                    propertyType: typeof({{propertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: new PropertyMetadata({{decimalLiteralExpression}}));

                public {{propertyType}} [|P1|]
                {
                    get => ({{propertyType}})GetValue(P1Property);
                    set => SetValue(P1Property, value);
                }

                public {{propertyType}} [|P2|]
                {
                    get => ({{propertyType}})GetValue(P2Property);
                    set => SetValue(P2Property, value);
                }

                public {{propertyType}} [|P3|]
                {
                    get => ({{propertyType}})GetValue(P3Property);
                    set => SetValue(P3Property, value);
                }

                public {{propertyType}} [|P4|]
                {
                    get => ({{propertyType}})GetValue(P4Property);
                    set => SetValue(P4Property, value);
                }

                public {{propertyType}} [|P5|]
                {
                    get => ({{propertyType}})GetValue(P5Property);
                    set => SetValue(P5Property, value);
                }

                public {{propertyType}} [|P6|]
                {
                    get => ({{propertyType}})GetValue(P6Property);
                    set => SetValue(P6Property, value);
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
                public partial {{propertyType}} {|CS9248:P1|} { get; set; }

                [GeneratedDependencyProperty(DefaultValue = {{propertyType}}.MinValue)]
                public partial {{propertyType}} {|CS9248:P2|} { get; set; }

                [GeneratedDependencyProperty(DefaultValue = {{propertyType}}.NaN)]
                public partial {{propertyType}} {|CS9248:P3|} { get; set; }

                [GeneratedDependencyProperty(DefaultValue = {{propertyType}}.Pi)]
                public partial {{propertyType}} {|CS9248:P4|} { get; set; }

                [GeneratedDependencyProperty(DefaultValue = {{literalExpression}})]
                public partial {{propertyType}} {|CS9248:P5|} { get; set; }

                [GeneratedDependencyProperty(DefaultValue = {{decimalLiteralExpression}})]
                public partial {{propertyType}} {|CS9248:P6|} { get; set; }
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
    public async Task MultipleProperties_WithXmlDocs_WithForwardedAttributes_TrimsAttributTrivia()
    {
        const string original = """
            using System;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                /// <summary>
                /// Identifies the <seealso cref="Expression"/> dependency property.
                /// </summary>
                public static readonly DependencyProperty ExpressionProperty = DependencyProperty.Register(
                    nameof(Expression),
                    typeof(string),
                    typeof(MyObject),
                    null);

                /// <summary>
                /// Identifies the <seealso cref="Input" /> dependency property.
                /// </summary>
                [Test(42, "Test")]
                public static readonly DependencyProperty InputProperty = DependencyProperty.Register(
                    nameof(Input),
                    typeof(object),
                    typeof(MyObject),
                    null);

                /// <summary>
                /// Blah.
                /// </summary>
                public string? [|Expression|]
                {
                    get => (string?)GetValue(ExpressionProperty);
                    set => SetValue(ExpressionProperty, value);
                }

                /// <summary>
                /// Blah.
                /// </summary>
                public object? [|Input|]
                {
                    get => (object?)GetValue(InputProperty);
                    set => SetValue(InputProperty, value);
                }
            }

            public class TestAttribute(int X, string Y) : Attribute;
            """;

        const string @fixed = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                /// <summary>
                /// Blah.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Expression|} { get; set; }

                /// <summary>
                /// Blah.
                /// </summary>
                [GeneratedDependencyProperty]
                [static: Test(42, "Test")]
                public partial object? {|CS9248:Input|} { get; set; }
            }

            public class TestAttribute(int X, string Y) : Attribute;
            """;

        CSharpCodeFixTest test = new(LanguageVersion.Preview)
        {
            TestCode = original,
            FixedCode = @fixed
        };

        await test.RunAsync();
    }

    // Using 'object' for dependency properties is sometimes needed to work around an 'IReference<T>' issue in some binding scenarios
    [TestMethod]
    public async Task MultipleProperties_WithInterspersedNonFixableProprty_HandlesAllPossibleProperties()
    {
        const string original = """
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                /// <summary>
                /// Identifies the <see cref="DisableAnimation" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty DisableAnimationProperty = DependencyProperty.Register(
                    nameof(DisableAnimation),
                    typeof(bool),
                    typeof(MyObject),
                    new PropertyMetadata(false));

                /// <summary>
                /// Identifies the <see cref="HorizontalOffset" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
                    nameof(HorizontalOffset),
                    typeof(object),
                    typeof(MyObject),
                    null);

                /// <summary>
                /// Identifies the <see cref="IsHorizontalOffsetRelative" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty IsHorizontalOffsetRelativeProperty = DependencyProperty.Register(
                    nameof(IsHorizontalOffsetRelative),
                    typeof(bool),
                    typeof(MyObject),
                    new PropertyMetadata(false));

                /// <summary>
                /// Identifies the <see cref="IsVerticalOffsetRelative" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty IsVerticalOffsetRelativeProperty = DependencyProperty.Register(
                    nameof(IsVerticalOffsetRelative),
                    typeof(bool),
                    typeof(MyObject),
                    new PropertyMetadata(false));

                /// <summary>
                /// Identifies the <see cref="TargetScrollViewer" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty TargetScrollViewerProperty = DependencyProperty.Register(
                    nameof(TargetScrollViewer),
                    typeof(ScrollViewer),
                    typeof(MyObject),
                    null);

                /// <summary>
                /// Identifies the <see cref="VerticalOffset" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
                    nameof(VerticalOffset),
                    typeof(object),
                    typeof(MyObject),
                    null);

                /// <summary>
                /// Gets or sets a value indicating whether the animation is disabled. The default value is <see langword="false" />.
                /// </summary>
                public bool [|DisableAnimation|]
                {
                    get => (bool)GetValue(DisableAnimationProperty);
                    set => SetValue(DisableAnimationProperty, value);
                }

                /// <summary>
                /// Gets or sets the distance should be scrolled horizontally.
                /// </summary>
                public double? HorizontalOffset
                {
                    get => (double?)GetValue(HorizontalOffsetProperty);
                    set => SetValue(HorizontalOffsetProperty, value);
                }

                /// <summary>
                /// Gets or sets a value indicating whether the horizontal offset is relative to the current offset. The default value is <see langword="false" />.
                /// </summary>
                public bool [|IsHorizontalOffsetRelative|]
                {
                    get => (bool)GetValue(IsHorizontalOffsetRelativeProperty);
                    set => SetValue(IsHorizontalOffsetRelativeProperty, value);
                }

                /// <summary>
                /// Gets or sets a value indicating whether the vertical offset is relative to the current offset. The default value is <see langword="false" />.
                /// </summary>
                public bool [|IsVerticalOffsetRelative|]
                {
                    get => (bool)GetValue(IsVerticalOffsetRelativeProperty);
                    set => SetValue(IsVerticalOffsetRelativeProperty, value);
                }

                /// <summary>
                /// Gets or sets the target <see cref="ScrollViewer" />.
                /// </summary>
                public ScrollViewer? [|TargetScrollViewer|]
                {
                    get => (ScrollViewer?)GetValue(TargetScrollViewerProperty);
                    set => SetValue(TargetScrollViewerProperty, value);
                }

                /// <summary>
                /// Gets or sets the distance should be scrolled vertically.
                /// </summary>
                public double? VerticalOffset
                {
                    get => (double?)GetValue(VerticalOffsetProperty);
                    set => SetValue(VerticalOffsetProperty, value);
                }
            }
            """;

        const string @fixed = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                /// <summary>
                /// Identifies the <see cref="HorizontalOffset" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register(
                    nameof(HorizontalOffset),
                    typeof(object),
                    typeof(MyObject),
                    null);

                /// <summary>
                /// Identifies the <see cref="VerticalOffset" /> dependency property.
                /// </summary>
                public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register(
                    nameof(VerticalOffset),
                    typeof(object),
                    typeof(MyObject),
                    null);

                /// <summary>
                /// Gets or sets a value indicating whether the animation is disabled. The default value is <see langword="false" />.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial bool {|CS9248:DisableAnimation|} { get; set; }

                /// <summary>
                /// Gets or sets the distance should be scrolled horizontally.
                /// </summary>
                public double? HorizontalOffset
                {
                    get => (double?)GetValue(HorizontalOffsetProperty);
                    set => SetValue(HorizontalOffsetProperty, value);
                }

                /// <summary>
                /// Gets or sets a value indicating whether the horizontal offset is relative to the current offset. The default value is <see langword="false" />.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial bool {|CS9248:IsHorizontalOffsetRelative|} { get; set; }
 
                /// <summary>
                /// Gets or sets a value indicating whether the vertical offset is relative to the current offset. The default value is <see langword="false" />.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial bool {|CS9248:IsVerticalOffsetRelative|} { get; set; }
            
                /// <summary>
                /// Gets or sets the target <see cref="ScrollViewer" />.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial ScrollViewer? {|CS9248:TargetScrollViewer|} { get; set; }

                /// <summary>
                /// Gets or sets the distance should be scrolled vertically.
                /// </summary>
                public double? VerticalOffset
                {
                    get => (double?)GetValue(VerticalOffsetProperty);
                    set => SetValue(VerticalOffsetProperty, value);
                }
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
    public async Task SimpleProperty_NestedType_AddsAllRequiredPartialModifiers()
    {
        const string original = """
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public class MyNestedObject : DependencyObject
                {
                    public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                        name: "Name",
                        propertyType: typeof(string),
                        ownerType: typeof(MyNestedObject),
                        typeMetadata: null);

                    public string? [|Name|]
                    {
                        get => (string?)GetValue(NameProperty);
                        set => SetValue(NameProperty, value);
                    }
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                public partial class MyNestedObject : DependencyObject
                {
                    [GeneratedDependencyProperty]
                    public partial string? {|CS9248:Name|} { get; set; }
                }
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
    public async Task SimpleProperty_ExplicitNullCallbackArgument_IsHandledCorrectly1()
    {
        const string original = """
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public class MyNestedObject : DependencyObject
                {
                    public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                        name: "Name",
                        propertyType: typeof(string),
                        ownerType: typeof(MyNestedObject),
                        typeMetadata: new PropertyMetadata(null, null));

                    public string? [|Name|]
                    {
                        get => (string?)GetValue(NameProperty);
                        set => SetValue(NameProperty, value);
                    }
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                public partial class MyNestedObject : DependencyObject
                {
                    [GeneratedDependencyProperty]
                    public partial string? {|CS9248:Name|} { get; set; }
                }
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
    public async Task SimpleProperty_ExplicitNullCallbackArgument_IsHandledCorrectly2()
    {
        const string original = """
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public class MyNestedObject : DependencyObject
                {
                    public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                        name: "Name",
                        propertyType: typeof(string),
                        ownerType: typeof(MyNestedObject),
                        typeMetadata: new PropertyMetadata("", null));

                    public string? [|Name|]
                    {
                        get => (string?)GetValue(NameProperty);
                        set => SetValue(NameProperty, value);
                    }
                }
            }
            """;

        const string @fixed = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                public partial class MyNestedObject : DependencyObject
                {
                    [GeneratedDependencyProperty(DefaultValue = "")]
                    public partial string? {|CS9248:Name|} { get; set; }
                }
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