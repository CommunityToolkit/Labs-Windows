// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

[TestClass]
public class Test_Analyzers
{
    [TestMethod]
    public async Task InvalidPropertySyntaxDeclarationAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public class MyControl : Control
            {
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySyntaxDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySyntaxDeclarationAnalyzer_ValidAttribute_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySyntaxDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySyntaxDeclarationAnalyzer_NotPartial_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0001:GeneratedDependencyProperty|}]
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySyntaxDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySyntaxDeclarationAnalyzer_NoSetter_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0001:GeneratedDependencyProperty|}]
                public partial string? {|CS9248:Name|} { get; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySyntaxDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySyntaxDeclarationAnalyzer_NoGetter_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0001:GeneratedDependencyProperty|}]
                public partial string? {|CS9248:Name|} { set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySyntaxDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySyntaxDeclarationAnalyzer_InitOnlySetter_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0001:GeneratedDependencyProperty|}]
                public partial string? {|CS9248:Name|} { get; init; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySyntaxDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySyntaxDeclarationAnalyzer_Static_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0001:GeneratedDependencyProperty|}]
                public static partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySyntaxDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public class MyControl : Control
            {
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_ValidAttribute_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_OnUnannotatedPartialPropertyWithImplementation_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                public partial string? Name { get; set; }

                public partial string? Name
                {
                    get => field;
                    set { }
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_OnImplementedProperty_GeneratedByToolkit_DoesNotWarn()
    {
        const string source = """
            using System.CodeDom.Compiler;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string Name { get; set; }

                [GeneratedCode("CommunityToolkit.WinUI.DependencyPropertyGenerator", "1.0.0")]
                public partial string Name
                {
                    get => field;
                    set { }
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_OnImplementedProperty_GeneratedByAnotherGenerator_Warns()
    {
        const string source = """
            using System.CodeDom.Compiler;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0002:GeneratedDependencyProperty|}]
                public partial string Name { get; set; }

                [GeneratedCode("Some.Other.Generator", "1.0.0")]
                public partial string Name
                {
                    get => field;
                    set { }
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_OnImplementedProperty_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0002:GeneratedDependencyProperty|}]
                public partial string Name { get; set; }

                public partial string Name
                {
                    get => field;
                    set { }
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.Preview);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_ReturnsRef_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0003:GeneratedDependencyProperty|}]
                public partial ref int {|CS9248:Name|} { get; {|CS8147:set|}; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_ReturnsRefReadOnly_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0003:GeneratedDependencyProperty|}]
                public partial ref readonly int {|CS9248:Name|} { get; {|CS8147:set|}; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_ReturnsByRefLike_Warns()
    {
        const string source = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0004:GeneratedDependencyProperty|}]
                public partial Span<int> {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_ReturnsPointerType_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public unsafe partial class MyControl : Control
            {
                [{|WCTDP0012:GeneratedDependencyProperty|}]
                public partial int* {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertySymbolDeclarationAnalyzer_ReturnsFunctionPointerType_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public unsafe partial class MyControl : Control
            {
                [{|WCTDP0012:GeneratedDependencyProperty|}]
                public partial delegate* unmanaged[Stdcall]<int, void> {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertySymbolDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyContainingTypeDeclarationAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl
            {
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyContainingTypeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyContainingTypeDeclarationAnalyzer_ValidType1_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyContainingTypeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyContainingTypeDeclarationAnalyzer_ValidType2_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyContainingTypeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyContainingTypeDeclarationAnalyzer_InvalidType_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;

            namespace MyApp;

            public partial class MyControl
            {
                [{|WCTDP0005:GeneratedDependencyProperty|}]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyContainingTypeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UnsupportedCSharpLanguageVersionAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<UnsupportedCSharpLanguageVersionAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp10);
    }

    [TestMethod]
    [DataRow(LanguageVersion.CSharp13)]
    [DataRow(LanguageVersion.Preview)]
    public async Task UnsupportedCSharpLanguageVersionAnalyzer_ValidLanguageVersion_DoesNotWarn(LanguageVersion languageVersion)
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<UnsupportedCSharpLanguageVersionAnalyzer>.VerifyAnalyzerAsync(source, languageVersion);
    }

    [TestMethod]
    [DataRow(LanguageVersion.CSharp10)]
    [DataRow(LanguageVersion.CSharp12)]
    public async Task UnsupportedCSharpLanguageVersionAnalyzer_RequiresCSharp13_Warns(LanguageVersion languageVersion)
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0006:GeneratedDependencyProperty|}]
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<UnsupportedCSharpLanguageVersionAnalyzer>.VerifyAnalyzerAsync(source, languageVersion);
    }

    [TestMethod]
    [DataRow(LanguageVersion.CSharp10)]
    [DataRow(LanguageVersion.CSharp13)]
    public async Task UnsupportedCSharpLanguageVersionAnalyzer_CSharp10_RequiresPreview_Warns(LanguageVersion languageVersion)
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0007:GeneratedDependencyProperty(IsLocalCacheEnabled = true)|}]
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<UnsupportedCSharpLanguageVersionAnalyzer>.VerifyAnalyzerAsync(source, languageVersion);
    }

    [TestMethod]
    public async Task InvalidPropertyConflictingDeclarationAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyConflictingDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyConflictingDeclarationAnalyzer_ValidProperty_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyConflictingDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("object")]
    [DataRow("DependencyPropertyChangedEventArgs")]
    public async Task InvalidPropertyConflictingDeclarationAnalyzer_InvalidPropertyType_ValidName_DoesNotWarn(string propertyType)
    {
        string source = $$"""
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

        await CSharpAnalyzerTest<InvalidPropertyConflictingDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("object")]
    [DataRow("DependencyPropertyChangedEventArgs")]
    public async Task InvalidPropertyConflictingDeclarationAnalyzer_InvalidPropertyType_NamedProperty_Warns(string propertyType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0008:GeneratedDependencyProperty|}]
                public partial {{propertyType}} {|CS9248:Property|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyConflictingDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public class MyControl : Control
            {
                public string Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("int")]
    [DataRow("int?")]
    [DataRow("string?")]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableOrNotApplicableType_DoesNotWarn(string propertyType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithMaybeNullAttribute_DoesNotWarn()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [MaybeNull]
                public partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_Required_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public required partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_NullableDisabled_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithNonNullDefaultValue_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = "Bob")]
                public partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithNonNullDefaultValueCallback_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetDefaultName))]
                public partial string {|CS9248:Name|} { get; set; }

                private static string GetDefaultName() => "Bob";
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithNonNullDefaultValueCallback_WithAttribute_DoesNotWarn()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetDefaultName))]
                public partial string {|CS9248:Name|} { get; set; }

                [return: NotNull]
                private static string? GetDefaultName() => "Bob";
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithMaybeNull_DoesNotWarn()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [MaybeNull]
                public partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:GeneratedDependencyProperty|}]
                public partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithNullDefaultValue_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:GeneratedDependencyProperty(DefaultValue = null)|}]
                public partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithNullDefaultValueCallback_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:GeneratedDependencyProperty(DefaultValueCallback = nameof(GetDefaultName))|}]
                public partial string {|CS9248:Name|} { get; set; }

                private static string? GetDefaultName() => "Bob";
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_WithNullDefaultValueCallback_WithAttribute_Warns()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:GeneratedDependencyProperty(DefaultValueCallback = nameof(GetDefaultName))|}]
                public partial string {|CS9248:Name|} { get; set; }

                [return: MaybeNull]
                private static string GetDefaultName() => "Bob";
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_AllowNull_WithNullResilientSetter_Object_Warns()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:GeneratedDependencyProperty|}]
                [AllowNull]
                public partial string {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameSet|}([NotNull] ref object? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_AllowNull_WithNullResilientGetter_Object_DoesNotWarn()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial string {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameGet|}([NotNull] ref object? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_AllowNull_WithNullResilientSetter_Warns()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:GeneratedDependencyProperty|}]
                [AllowNull]
                public partial string {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameSet|}([NotNull] ref string? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_AllowNull_WithNullResilientGetter_DoesNotWarn()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial string {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameGet|}([NotNull] ref string? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_AllowNull_WithNullResilientGetter_WithNoAttribute_Warns()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:{|WCTDP0024:GeneratedDependencyProperty|}|}]
                [AllowNull]
                public partial string {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameGet|}(ref string? propertyValue)
                {
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NotNullableType_AllowNull_Warns()
    {
        const string source = """
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0009:{|WCTDP0024:GeneratedDependencyProperty|}|}]
                [AllowNull]
                public partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithoutNotNull_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithNullResilientGetter_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [NotNull]
                public partial string? {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameGet|}([NotNull] ref string? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithDisallowNull_Required_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [NotNull]
                [DisallowNull]
                public required partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithNullResilientSetter_Required_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [NotNull]
                public required partial string? {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameSet|}([NotNull] ref string? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithDisallowNull_NonNullDefaultValue_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = "")]
                [NotNull]
                [DisallowNull]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithNullResilientSetter_NonNullDefaultValue_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = "")]
                [NotNull]
                public partial string? {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameSet|}([NotNull] ref string? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_InsideGeneric_NullableType_NotRequired_WithNotNull_NullResilientGetter_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public abstract partial class Animation<TValue, TKeyFrame> : DependencyObject
                where TKeyFrame : unmanaged
            {
                [GeneratedDependencyProperty]
                [NotNull]
                public partial KeyFrameCollection<TValue, TKeyFrame>? {|CS9248:KeyFrames|} { get; set; }

                partial void {|CS0759:OnKeyFramesGet|}([NotNull] ref KeyFrameCollection<TValue, TKeyFrame>? propertyValue)
                {
                    propertyValue = new();
                }

                partial void {|CS0759:OnKeyFramesPropertyChanged|}(DependencyPropertyChangedEventArgs e)
                {
                }
            }

            public sealed partial class KeyFrameCollection<TValue, TKeyFrame> : DependencyObjectCollection
                where TKeyFrame : unmanaged;
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_InsideGeneric_NotNullableType_NotRequired_WithAllowNull_NullResilientGetter_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public abstract partial class Animation<TValue, TKeyFrame> : DependencyObject
                where TKeyFrame : unmanaged
            {
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial KeyFrameCollection<TValue, TKeyFrame> {|CS9248:KeyFrames|} { get; set; }

                partial void {|CS0759:OnKeyFramesGet|}([NotNull] ref KeyFrameCollection<TValue, TKeyFrame>? propertyValue)
                {
                    propertyValue = new();
                }

                partial void {|CS0759:OnKeyFramesPropertyChanged|}(DependencyPropertyChangedEventArgs e)
                {
                }
            }

            public sealed partial class KeyFrameCollection<TValue, TKeyFrame> : DependencyObjectCollection
                where TKeyFrame : unmanaged;
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_Warns()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0025:GeneratedDependencyProperty|}]
                [NotNull]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_NoAttributeOnGetter_Warns()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0025:GeneratedDependencyProperty|}]
                [NotNull]
                public partial string? {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameSet|}(ref string? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithDisallowNull_NotRequired_Warns()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0025:GeneratedDependencyProperty|}]
                [NotNull]
                [DisallowNull]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_NullableType_WithNullResilientSetter_NotRequired_Warns()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0025:GeneratedDependencyProperty|}]
                [NotNull]
                public partial string? {|CS9248:Name|} { get; set; }

                partial void {|CS0759:OnNameSet|}([NotNull] ref string? propertyValue)
                {
                    propertyValue ??= "Bob";
                }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_InsideGeneric_NullableType_NotRequired_WithNotNull_Warns()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public abstract partial class Animation<TValue, TKeyFrame> : DependencyObject
                where TKeyFrame : unmanaged
            {
                [{|WCTDP0025:GeneratedDependencyProperty|}]
                [NotNull]
                public partial KeyFrameCollection<TValue, TKeyFrame>? {|CS9248:KeyFrames|} { get; set; }

                partial void {|CS0759:OnKeyFramesGet|}(ref KeyFrameCollection<TValue, TKeyFrame>? propertyValue)
                {
                }

                partial void {|CS0759:OnKeyFramesPropertyChanged|}(DependencyPropertyChangedEventArgs e)
                {
                }
            }

            public sealed partial class KeyFrameCollection<TValue, TKeyFrame> : DependencyObjectCollection
                where TKeyFrame : unmanaged;
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_InsideGeneric_NotNullableType_NotRequired_WithAllowNull_Warns()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public abstract partial class Animation<TValue, TKeyFrame> : DependencyObject
                where TKeyFrame : unmanaged
            {
                [{|WCTDP0009:{|WCTDP0024:GeneratedDependencyProperty|}|}]
                [AllowNull]
                public partial KeyFrameCollection<TValue, TKeyFrame> {|CS9248:KeyFrames|} { get; set; }

                partial void {|CS0759:OnKeyFramesGet|}(ref KeyFrameCollection<TValue, TKeyFrame>? propertyValue)
                {
                }

                partial void {|CS0759:OnKeyFramesPropertyChanged|}(DependencyPropertyChangedEventArgs e)
                {
                }
            }

            public sealed partial class KeyFrameCollection<TValue, TKeyFrame> : DependencyObjectCollection
                where TKeyFrame : unmanaged;
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_InsideGeneric_NotNullableType_rEQUIRED_WithAllowNull_Warns()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public abstract partial class Animation<TValue, TKeyFrame> : DependencyObject
                where TKeyFrame : unmanaged
            {
                [{|WCTDP0024:GeneratedDependencyProperty|}]
                [AllowNull]
                public required partial KeyFrameCollection<TValue, TKeyFrame> {|CS9248:KeyFrames|} { get; set; }
            }

            public sealed partial class KeyFrameCollection<TValue, TKeyFrame> : DependencyObjectCollection
                where TKeyFrame : unmanaged;
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public class MyControl : Control
            {
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_NoDefaultValue_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string?")]
    [DataRow("int")]
    [DataRow("int?")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_UnsetValue_DoesNotWarn(string propertyType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = GeneratedDependencyProperty.UnsetValue)]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string")]
    [DataRow("int?")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_NullValue_Nullable_DoesNotWarn(string propertyType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = null)]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string", "\"test\"")]
    [DataRow("int", "42")]
    [DataRow("double", "3.14")]
    [DataRow("int?", "42")]
    [DataRow("double?", "3.14")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_CompatibleType_DoesNotWarn(string propertyType, string defaultValueType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = {{defaultValueType}})]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("T1")]
    [DataRow("T1?")]
    [DataRow("T4?")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_TypeParameter_ExplicitNull_DoesNotWarn(string propertyType)
    {
        string source = $$"""
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable
            
            namespace MyApp;

            public partial class MyObject<T1, T2, T3, T4, T5> : DependencyObject
                where T1 : class
                where T3 : T2, new()
                where T4 : unmanaged
                where T5 : IDisposable
            {
                [GeneratedDependencyProperty(DefaultValue = null)]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_NullValue_NonNullable_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0010:GeneratedDependencyProperty(DefaultValue = null)|}]
                public partial int {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("T2")]
    [DataRow("T2?")]
    [DataRow("T3")]
    [DataRow("T3?")]
    [DataRow("T4")]
    [DataRow("T5")]
    [DataRow("T5?")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_TypeParameter_ExplicitNull_Warns(string propertyType)
    {
        string source = $$"""
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable
            
            namespace MyApp;

            public partial class MyObject<T1, T2, T3, T4, T5> : DependencyObject
                where T1 : class
                where T3 : T2, new()
                where T4 : unmanaged
                where T5 : IDisposable
            {
                [{|WCTDP0010:GeneratedDependencyProperty(DefaultValue = null)|}]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string", "42")]
    [DataRow("string", "3.14")]
    [DataRow("int", "\"test\"")]
    [DataRow("int?", "\"test\"")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_IncompatibleType_Warns(string propertyType, string defaultValueType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0011:GeneratedDependencyProperty(DefaultValue = {{defaultValueType}})|}]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_NoDefaultValueCallback1_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_NoDefaultValueCallback2_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = "Bob")]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_NullDefaultValueCallback_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValueCallback = null)]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string", "string")]
    [DataRow("string", "string?")]
    [DataRow("string", "object")]
    [DataRow("string", "object?")]
    [DataRow("string?", "string")]
    [DataRow("string?", "string?")]
    [DataRow("int", "int")]
    [DataRow("int", "object")]
    [DataRow("int", "object?")]
    [DataRow("int?", "int")]
    [DataRow("int?", "int?")]
    [DataRow("int?", "object")]
    [DataRow("int?", "object?")]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_ValidDefaultValueCallback_DoesNotWarn(string propertyType, string returnType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetDefaultValue))]
                public partial {{propertyType}} {|CS9248:Value|} { get; set; }

                private static {{returnType}} GetDefaultValue() => default!;
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_BothDefaultValuePropertiesSet_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0013:GeneratedDependencyProperty(DefaultValue = "Bob", DefaultValueCallback = nameof(GetDefaultName))|}]
                public partial string? {|CS9248:Name|} { get; set; }

                private static string? GetDefaultName() => "Bob";
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_MethodNotFound_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0014:GeneratedDependencyProperty(DefaultValueCallback = "MissingMethod")|}]
                public partial string? {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_InvalidMethod_ExplicitlyImplemented_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control, IGetDefaultValue
            {
                [{|WCTDP0014:GeneratedDependencyProperty(DefaultValueCallback = "GetDefaultValue")|}]
                public partial string? {|CS9248:Name|} { get; set; }

                static string? IGetDefaultValue.GetDefaultValue() => "Bob";
            }

            public interface IGetDefaultValue
            {
                static abstract string? GetDefaultValue();
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("private string? GetDefaultName()")]
    [DataRow("private static string? GetDefaultName(int x)")]
    [DataRow("private static int GetDefaultName()")]
    [DataRow("private static int GetDefaultName(int x)")]
    public async Task InvalidPropertyDefaultValueCallbackTypeAnalyzer_InvalidMethod_Warns(string methodSignature)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0015:GeneratedDependencyProperty(DefaultValueCallback = "GetDefaultName")|}]
                public partial string? {|CS9248:Name|} { get; set; }

                {{methodSignature}} => default!;
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueCallbackTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("Name")]
    [DataRow("TestProperty")]
    public async Task PropertyDeclarationWithPropertyNameSuffixAnalyzer_NoAttribute_DoesNotWarn(string propertyName)
    {
        string source = $$"""
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public partial string? {|CS9248:{{propertyName}}|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<PropertyDeclarationWithPropertyNameSuffixAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("Name")]
    [DataRow("PropertyGroup")]
    public async Task PropertyDeclarationWithPropertyNameSuffixAnalyzer_ValidName_DoesNotWarn(string propertyName)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public partial string? {|CS9248:{{propertyName}}|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<PropertyDeclarationWithPropertyNameSuffixAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task PropertyDeclarationWithPropertyNameSuffixAnalyzer_InvalidName_Warns()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [{|WCTDP0016:GeneratedDependencyProperty|}]
                public partial string? {|CS9248:TestProperty|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<PropertyDeclarationWithPropertyNameSuffixAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_FieldNotInitialized_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty;

                public string? Name
                {
                    get => (string?)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_FieldWithDifferentName_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty OtherNameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? Name
                {
                    get => (string?)GetValue(OtherNameProperty);
                    set => SetValue(OtherNameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("null", "typeof(string)", "typeof(MyControl)", "null")]
    [DataRow("\"NameProperty\"", "typeof(string)", "typeof(MyControl)", "null")]
    [DataRow("\"OtherName\"", "typeof(string)", "typeof(MyControl)", "null")]
    [DataRow("\"Name\"", "typeof(int)", "typeof(MyControl)", "null")]
    [DataRow("\"Name\"", "typeof(MyControl)", "typeof(MyControl)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(string)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(Control)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(DependencyObject)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(MyControl)", "new PropertyMetadata(null, (d, e) => { })")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_DoesNotWarn(
        string name,
        string propertyType,
        string ownerType,
        string typeMetadata)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: {{name}},
                    propertyType: {{propertyType}},
                    ownerType: {{ownerType}},
                    typeMetadata: {{typeMetadata}});

                public string? Name
                {
                    get => (string?)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_MissingGetter_DoesNotWarn()
    {
        const string source = """
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
                    typeMetadata: null);

                public string? Name
                {
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_MissingSetter_DoesNotWarn()
    {
        const string source = """
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
                    typeMetadata: null);

                public string? Name
                {
                    get => (string?)GetValue(NameProperty);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("global::System.TimeSpan", "global::System.TimeSpan", "global::System.TimeSpan.FromSeconds(1)")]
    [DataRow("global::System.TimeSpan?", "global::System.TimeSpan?", "global::System.TimeSpan.FromSeconds(1)")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_ExplicitDefaultValue_DoesNotWarn(
        string dependencyPropertyType,
        string propertyType,
        string defaultValueExpression)
    {
        string source = $$"""
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

                public {{propertyType}} Name
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public struct MyStruct { public string X { get; set; } }
            public enum MyEnum { A, B, C }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_WithInvalidAttribute_DoesNotWarn()
    {
        const string source = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                [property: Test]
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? Name
                {
                    get => (string?)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public class TestAttribute : Attribute;
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string", "string")]
    [DataRow("string", "string?")]
    [DataRow("object", "object")]
    [DataRow("object", "object?")]
    [DataRow("int", "int")]
    [DataRow("int?", "int?")]
    [DataRow("global::System.TimeSpan", "global::System.TimeSpan")]
    [DataRow("global::System.TimeSpan?", "global::System.TimeSpan?")]
    [DataRow("global::System.DateTimeOffset", "global::System.DateTimeOffset")]
    [DataRow("global::System.DateTimeOffset?", "global::System.DateTimeOffset?")]
    [DataRow("global::System.Guid?", "global::System.Guid?")]
    [DataRow("global::System.Collections.Generic.KeyValuePair<int, float>?", "global::System.Collections.Generic.KeyValuePair<int, float>?")]
    [DataRow("global::System.Collections.Generic.KeyValuePair<int, float>?", "global::System.Collections.Generic.KeyValuePair<int, float>?" )]
    [DataRow("global::MyApp.MyStruct", "global::MyApp.MyStruct")]
    [DataRow("global::MyApp.MyStruct?", "global::MyApp.MyStruct?")]
    [DataRow("global::MyApp.MyStruct?", "global::MyApp.MyStruct?")]
    [DataRow("global::MyApp.MyEnum", "global::MyApp.MyEnum")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?")]
    [DataRow("global::MyApp.MyClass", "global::MyApp.MyClass")]
    [DataRow("global::MyApp.MyClass", "global::MyApp.MyClass")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_Warns(
        string dependencyPropertyType,
        string propertyType)
    {
        string source = $$"""
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
                    typeMetadata: null);

                public {{propertyType}} {|WCTDP0017:Name|}
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public struct MyStruct { public string X { get; set; } }
            public enum MyEnum { A, B, C }
            public class MyClass { }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("[Test]")]
    [DataRow("[field: Test]")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_WithAttributeOnField_Warns(string attributeDeclaration)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                {{attributeDeclaration}}
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? {|WCTDP0017:Name|}
                {
                    get => (string?)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public class TestAttribute : Attribute;
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string", "string", "null")]
    [DataRow("string", "string", "default(string)")]
    [DataRow("string", "string", "(string)null")]
    [DataRow("string", "string", "\"\"")]
    [DataRow("string", "string", "\"Hello\"")]
    [DataRow("string", "string?", "null")]
    [DataRow("object", "object", "null")]
    [DataRow("object", "object?", "null")]
    [DataRow("int", "int", "0")]
    [DataRow("int", "int", "42")]
    [DataRow("int", "int", "default(int)")]
    [DataRow("int?", "int?", "null")]
    [DataRow("int?", "int?", "0")]
    [DataRow("int?", "int?", "42")]
    [DataRow("int?", "int?", "default(int?)")]
    [DataRow("int?", "int?", "null")]
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
    [DataRow("global::Windows.UI.Xaml.Visibility", "global::Windows.UI.Xaml.Visibility", "global::Windows.UI.Xaml.Visibility.Collapsed")]
    [DataRow("global::System.TimeSpan", "global::System.TimeSpan", "default(System.TimeSpan)")]
    [DataRow("global::System.DateTimeOffset", "global::System.DateTimeOffset", "default(global::System.DateTimeOffset)")]
    [DataRow("global::System.DateTimeOffset?", "global::System.DateTimeOffset?", "null")]
    [DataRow("global::System.DateTimeOffset?", "global::System.DateTimeOffset?", "default(global::System.DateTimeOffset?)")]
    [DataRow("global::System.TimeSpan?", "global::System.TimeSpan?", "default(global::System.TimeSpan?)")]
    [DataRow("global::System.Guid?", "global::System.Guid?", "default(global::System.Guid?)")]
    [DataRow("global::System.Collections.Generic.KeyValuePair<int, float>?", "global::System.Collections.Generic.KeyValuePair<int, float>?", "default(global::System.Collections.Generic.KeyValuePair<int, float>?)")]
    [DataRow("global::System.Collections.Generic.KeyValuePair<int, float>?", "global::System.Collections.Generic.KeyValuePair<int, float>?", "null")]
    [DataRow("global::MyApp.MyStruct", "global::MyApp.MyStruct", "default(global::MyApp.MyStruct)")]
    [DataRow("global::MyApp.MyStruct?", "global::MyApp.MyStruct?", "null")]
    [DataRow("global::MyApp.MyStruct?", "global::MyApp.MyStruct?", "default(global::MyApp.MyStruct?)")]
    [DataRow("global::MyApp.MyEnum", "global::MyApp.MyEnum", "default(global::MyApp.MyEnum)")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?", "null")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?", "default(global::MyApp.MyEnum?)")]
    [DataRow("global::MyApp.MyClass", "global::MyApp.MyClass", "null")]
    [DataRow("global::MyApp.MyClass", "global::MyApp.MyClass", "default(global::MyApp.MyClass)")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_ExplicitDefaultValue_Warns(
        string dependencyPropertyType,
        string propertyType,
        string defaultValueExpression)
    {
        string source = $$"""
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

                public {{propertyType}} {|WCTDP0017:Name|}
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public struct MyStruct { public string X { get; set; } }
            public enum MyEnum { A, B, C }
            public class MyClass { }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string?", "object")]
    [DataRow("MyControl", "DependencyObject")]
    [DataRow("double?", "object")]
    [DataRow("double?", "double")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_ExplicitMetadataType_Warns(string declaredType, string propertyType)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{propertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public {{declaredType}} {|WCTDP0017:Name|}
                {
                    get => ({{declaredType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_NoDependencyPropertyAttribute_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public class MyControl : Control
            {
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_NoForwardedAttribute_DoesNotWarn()
    {
        const string source = """            
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_ValidForwardedAttribute_DoesNotWarn()
    {
        const string source = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [static: Test]
                public string? Name { get; set; }
            }

            public class TestAttribute : Attribute;
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_TypoInAttributeName_NotTargetingStatic_DoesNotWarn()
    {
        const string source = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            public class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [Testt]
                public string? Name { get; set; }
            }

            public class TestAttribute : Attribute;
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13,
        [
            // /0/Test0.cs(9,6): error CS0246: The type or namespace name 'Testt' could not be found (are you missing a using directive or an assembly reference?)
            DiagnosticResult.CompilerError("CS0246").WithSpan(9, 6, 9, 11).WithArguments("Testt"),

            // /0/Test0.cs(9,6): error CS0246: The type or namespace name 'TesttAttribute' could not be found (are you missing a using directive or an assembly reference?)
            DiagnosticResult.CompilerError("CS0246").WithSpan(9, 6, 9, 11).WithArguments("TesttAttribute")
        ]);
    }

    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_MissingUsingDirective_Warns()
    {
        const string source = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp
            {
                public class MyControl : Control
                {
                    [GeneratedDependencyProperty]
                    [static: {|WCTDP0018:Test|}]
                    public string? Name { get; set; }
                }
            }

            namespace MyAttributes
            {
                public class TestAttribute : Attribute;
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_TypoInAttributeName_Warns()
    {
        const string source = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            public class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [static: {|WCTDP0018:Testt|}]
                public string? Name { get; set; }
            }

            public class TestAttribute : Attribute;
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // See https://github.com/CommunityToolkit/dotnet/issues/683
    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_InvalidExpressionOnFieldAttribute_Warns()
    {
        const string source = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            public class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [static: {|WCTDP0019:Test(TestAttribute.M)|}]
                public string? Name { get; set; }
            }

            public class TestAttribute : Attribute
            {
                public static string M => "";
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyForwardedAttributeDeclarationAnalyzer_InvalidExpressionOnFieldAttribute_WithExistingParameter_Warns()
    {
        const string source = """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            public class MyControl : Control
            {
                [GeneratedDependencyProperty]
                [static: {|WCTDP0019:Test(TestAttribute.M)|}]
                public string? Name { get; set; }
            }

            public class TestAttribute(string P) : Attribute
            {
                public static string M => "";
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyForwardedAttributeDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationCorrectlyAnalyzer_NotDependencyProperty_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                private static string TestProperty = "Blah";
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationCorrectlyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationCorrectlyAnalyzer_ValidField_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty TestProperty = DependencyProperty.Register("Test", typeof(string), typeof(MyObject), null);
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationCorrectlyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("private static readonly DependencyProperty")]
    [DataRow("public static DependencyProperty")]
    [DataRow("public static volatile DependencyProperty")]
    [DataRow("public static readonly DependencyProperty?")]
    public async Task UseFieldDeclarationCorrectlyAnalyzer_Warns(string fieldDeclaration)
    {
        string source = $$"""
            using Windows.UI.Xaml;

            #nullable enable
            
            public class MyObject : DependencyObject
            {
                {{fieldDeclaration}} {|WCTDP0020:TestProperty|};
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationCorrectlyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationAnalyzer_NotDependencyProperty_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                public static string TestProperty => "Blah";
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationAnalyzer_ExplicitInterfaceImplementation_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject, IMyObject
            {
                static DependencyProperty IMyObject.TestProperty => DependencyProperty.Register("Test", typeof(string), typeof(MyObject), null);
            }

            public interface IMyObject
            {
                static abstract DependencyProperty TestProperty { get; }
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationAnalyzer_ImplicitInterfaceImplementation_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject, IMyObject
            {
                public static DependencyProperty TestProperty => DependencyProperty.Register("Test", typeof(string), typeof(MyObject), null);
            }

            public interface IMyObject
            {
                static abstract DependencyProperty TestProperty { get; }
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationAnalyzer_WinRTComponent_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                public static DependencyProperty TestProperty => DependencyProperty.Register("Test", typeof(string), typeof(MyObject), null);
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13, editorconfig: [("CsWinRTComponent", true)]);
    }

    [TestMethod]
    public async Task UseFieldDeclarationAnalyzer_UnsupportedModifier_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public abstract class MyObject : MyBase
            {
                public DependencyProperty Test1Property => DependencyProperty.Register("Test1", typeof(string), typeof(MyObject), null);
                public virtual DependencyProperty Test2Property => DependencyProperty.Register("Test2", typeof(string), typeof(MyObject), null);
                public abstract DependencyProperty Test3Property { get; }
                public override DependencyProperty BaseProperty => DependencyProperty.Register("Base", typeof(string), typeof(MyObject), null);
            }

            public abstract class MyBase : DependencyObject
            {
                public abstract DependencyProperty BaseProperty { get; }
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationAnalyzer_WithNoGetter_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                public static DependencyProperty TestProperty
                {
                    set { }
                }
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseFieldDeclarationAnalyzer_NormalProperty_Warns()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                public static DependencyProperty {|WCTDP0021:Test1Property|} => DependencyProperty.Register("Test1", typeof(string), typeof(MyObject), null);
                public static DependencyProperty {|WCTDP0021:Test2Property|} { get; } = DependencyProperty.Register("Test2", typeof(string), typeof(MyObject), null);
                public static DependencyProperty {|WCTDP0021:Test3Property|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<UseFieldDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task ExplicitPropertyMetadataTypeAnalyzer_NoAttribute_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                public string? Name { get; set; }
            }
            """;

        await CSharpAnalyzerTest<ExplicitPropertyMetadataTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string", "object")]
    [DataRow("MyObject", "DependencyObject")]
    [DataRow("MyObject", "IMyInterface")]
    [DataRow("double?", "object")]
    [DataRow("double?", "double")]
    public async Task ExplicitPropertyMetadataTypeAnalyzer_ValidExplicitType_Warns(string declaredType, string propertyType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject, IMyInterface
            {
                [GeneratedDependencyProperty(PropertyType = typeof({{propertyType}}))]
                public {{declaredType}} Name { get; set; }
            }

            public interface IMyInterface;
            """;

        await CSharpAnalyzerTest<ExplicitPropertyMetadataTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("object")]
    [DataRow("string")]
    [DataRow("MyObject")]
    [DataRow("DependencyObject")]
    [DataRow("IMyInterface")]
    [DataRow("double?")]
    [DataRow("double")]
    public async Task ExplicitPropertyMetadataTypeAnalyzer_SameType_Warns(string type)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject, IMyInterface
            {
                [{|WCTDP0022:GeneratedDependencyProperty(PropertyType = typeof({{type}}))|}]
                public {{type}} Name { get; set; }
            }

            public interface IMyInterface;
            """;

        await CSharpAnalyzerTest<ExplicitPropertyMetadataTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("object", "string")]
    [DataRow("DependencyObject", "MyObject")]
    [DataRow("MyObject", "IMyInterface")]
    [DataRow("double", "double?")]
    [DataRow("double?", "IMyInterface")]
    [DataRow("double", "float")]
    [DataRow("float", "double")]
    public async Task ExplicitPropertyMetadataTypeAnalyzer_IncompatibleType_Warns(string declaredType, string propertyType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                [{|WCTDP0023:GeneratedDependencyProperty(PropertyType = typeof({{propertyType}}))|}]
                public {{declaredType}} Name { get; set; }
            }

            public interface IMyInterface;
            """;

        await CSharpAnalyzerTest<ExplicitPropertyMetadataTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }
}
