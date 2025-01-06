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
                [{|WCTDPG0001:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0001:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0001:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0001:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0001:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0002:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0002:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0003:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0003:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0004:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0012:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0012:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0005:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0006:GeneratedDependencyProperty|}]
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
                [{|WCTDPG0007:GeneratedDependencyProperty(IsLocalCacheEnabled = true)|}]
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
                [{|WCTDPG0008:GeneratedDependencyProperty|}]
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
                [GeneratedDependencyProperty]
                public partial string {|WCTDPG0009:{|CS9248:Name|}|} { get; set; }
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
                [GeneratedDependencyProperty(DefaultValue = null)]
                public partial string {|WCTDPG0009:{|CS9248:Name|}|} { get; set; }
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
                [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetDefaultName))]
                public partial string {|WCTDPG0009:{|CS9248:Name|}|} { get; set; }

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
                [GeneratedDependencyProperty(DefaultValueCallback = nameof(GetDefaultName))]
                public partial string {|WCTDPG0009:{|CS9248:Name|}|} { get; set; }

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
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial string {|WCTDPG0009:{|CS9248:Name|}|} { get; set; }

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
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial string {|WCTDPG0009:{|CS9248:Name|}|} { get; set; }

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
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial string {|WCTDPG0009:{|WCTDPG0024:{|CS9248:Name|}|}|} { get; set; }

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
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial string {|WCTDPG0009:{|WCTDPG0024:{|CS9248:Name|}|}|} { get; set; }
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
    public async Task InvalidPropertyNullableAnnotationAnalyzer_TypeParameter_NotNullableType_DoesNotWarn()
    {
        const string source = """            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject<T1, T2, T3, T4> : DependencyObject
                where T1 : class
                where T3 : T2, new()
                where T4 : unmanaged
            {
                [GeneratedDependencyProperty]
                public partial T4 {|CS9248:Value|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("T1?")]
    [DataRow("T2?")]
    [DataRow("T3?")]
    [DataRow("T4?")]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_TypeParameter_NullableType_DoesNotWarn(string declaredType)
    {
        string source = $$"""            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject<T1, T2, T3, T4> : DependencyObject
                where T1 : class
                where T3 : T2, new()
                where T4 : unmanaged
            {
                [GeneratedDependencyProperty]
                public partial {{declaredType}} {|CS9248:Value|} { get; set; }
            }
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
                [GeneratedDependencyProperty]
                [NotNull]
                public partial string? {|WCTDPG0025:{|CS9248:Name|}|} { get; set; }
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
                [GeneratedDependencyProperty]
                [NotNull]
                public partial string? {|WCTDPG0025:{|CS9248:Name|}|} { get; set; }

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
                [GeneratedDependencyProperty]
                [NotNull]
                [DisallowNull]
                public partial string? {|WCTDPG0025:{|CS9248:Name|}|} { get; set; }
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
                [GeneratedDependencyProperty]
                [NotNull]
                public partial string? {|WCTDPG0025:{|CS9248:Name|}|} { get; set; }

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
                [GeneratedDependencyProperty]
                [NotNull]
                public partial KeyFrameCollection<TValue, TKeyFrame>? {|WCTDPG0025:{|CS9248:KeyFrames|}|} { get; set; }

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
                [GeneratedDependencyProperty]
                [AllowNull]
                public partial KeyFrameCollection<TValue, TKeyFrame> {|WCTDPG0009:{|WCTDPG0024:{|CS9248:KeyFrames|}|}|} { get; set; }

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
    public async Task InvalidPropertyNullableAnnotationAnalyzer_InsideGeneric_NotNullableType_Required_WithAllowNull_Warns()
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
                public required partial KeyFrameCollection<TValue, TKeyFrame> {|WCTDPG0024:{|CS9248:KeyFrames|}|} { get; set; }
            }

            public sealed partial class KeyFrameCollection<TValue, TKeyFrame> : DependencyObjectCollection
                where TKeyFrame : unmanaged;
            """;

        await CSharpAnalyzerTest<InvalidPropertyNullableAnnotationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("T1")]
    [DataRow("T2")]
    [DataRow("T3")]
    public async Task InvalidPropertyNullableAnnotationAnalyzer_TypeParameter_NotNullableType_Warns(string declaredType)
    {
        string source = $$"""            
            using System.Diagnostics.CodeAnalysis;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable

            namespace MyApp;

            public partial class MyObject<T1, T2, T3, T4> : DependencyObject
                where T1 : class
                where T3 : T2, new()
                where T4 : unmanaged
            {
                [GeneratedDependencyProperty]
                public partial {{declaredType}} {|WCTDPG0009:{|CS9248:Value|}|} { get; set; }
            }
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
                [GeneratedDependencyProperty({|WCTDPG0010:DefaultValue = null|})]
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
                [GeneratedDependencyProperty({|WCTDPG0010:DefaultValue = null|})]
                public partial {{propertyType}} {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("where T : class", "null")]
    [DataRow("where T : class", "default(T)")]
    [DataRow("where T : TOther where TOther : class", "null")]
    [DataRow("where T : class where TOther : class", "default(TOther)")]
    [DataRow("where T : Delegate", "null")]
    [DataRow("where T : Enum", "null")]
    [DataRow("where T : DependencyObject", "null")]
    [DataRow("where T : DependencyObject", "default(T)")]
    [DataRow("where T : TOther where TOther : Delegate", "null")]
    [DataRow("where T : TOther where TOther : Enum", "null")]
    [DataRow("where T : TOther where TOther : DependencyObject", "null")]
    [DataRow("where T : DependencyObject where TOther : class", "default(TOther)")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_TypeParameter_ConstrainedExplicitNull_DoesNotWarn(
        string typeConstraints,
        string defaultValue)
    {
        string source = $$"""
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable
            
            namespace MyApp;

            public partial class MyObject<T, TOther> : DependencyObject {{typeConstraints}}
            {
                [GeneratedDependencyProperty(DefaultValue = {{defaultValue}})]
                public partial T {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyDefaultValueTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("where T : struct")]
    [DataRow("where T : unmanaged")]
    [DataRow("where T : struct, Enum")]
    [DataRow("where T : unmanaged, Enum")]
    public async Task InvalidPropertyDefaultValueTypeAnalyzer_TypeParameter_ConstrainedExplicitNull_Warns(string typeConstraints)
    {
        string source = $$"""
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            #nullable enable
            
            namespace MyApp;

            public partial class MyObject<T, TOther> : DependencyObject {{typeConstraints}}
            {
                [GeneratedDependencyProperty({|WCTDPG0010:DefaultValue = null|})]
                public partial T {|CS9248:Name|} { get; set; }
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
                [GeneratedDependencyProperty({|WCTDPG0011:DefaultValue = {{defaultValueType}}|})]
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
                [{|WCTDPG0013:GeneratedDependencyProperty(DefaultValue = "Bob", DefaultValueCallback = nameof(GetDefaultName))|}]
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
                [GeneratedDependencyProperty({|WCTDPG0014:DefaultValueCallback = "MissingMethod"|})]
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
                [GeneratedDependencyProperty({|WCTDPG0014:DefaultValueCallback = "GetDefaultValue"|})]
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
                [GeneratedDependencyProperty({|WCTDPG0015:DefaultValueCallback = "GetDefaultName"|})]
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
                [{|WCTDPG0016:GeneratedDependencyProperty|}]
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
                    {|WCTDPG0027:name: "Name"|},
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
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidFieldDeclaration_WCTDPG0026_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty {|WCTDPG0026:NameField|} = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public string? Name
                {
                    get => (string?)GetValue(NameField);
                    set => SetValue(NameField, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("null", "typeof(string)", "typeof(MyControl)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(MyControl)", "new PropertyMetadata(null, (d, e) => { })")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_NoAdditionalDiagnostic_DoesNotWarn(
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
    [DataRow("int", "float")]
    [DataRow("float", "double")]
    [DataRow("int", "object")]
    [DataRow("int?", "object")]
    [DataRow("string", "object")]
    [DataRow("MyControl", "IDisposable")]
    [DataRow("MyControl", "Control")]
    [DataRow("MyControl", "DependencyObject")]
    [DataRow("Control", "DependencyObject")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0030_DoesNotWarn(
        string dependencyPropertyType,
        string propertyType)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    {|WCTDPG0030:propertyType: typeof({{dependencyPropertyType}})|},
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public {{propertyType}} Name
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // Regression test for a case found in the Microsoft Store
    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0030_WithObjectInitialization_DoesNotWarn()
    {
       const string source = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty MarginProperty = DependencyProperty.Register(
                    nameof(Margin),
                    {|WCTDPG0030:typeof(double)|},
                    typeof(MyControl),
                    new PropertyMetadata({|WCTDPG0032:new Thickness(0)|}));

                private Thickness Margin
                {
                    get => (Thickness)GetValue(MarginProperty);
                    set => SetValue(MarginProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0030_WithExplicitDefaultValueNull_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty MarginProperty = DependencyProperty.Register(
                    nameof(Margin),
                    {|WCTDPG0030:typeof(double)|},
                    typeof(MyControl),
                    new PropertyMetadata({|WCTDPG0031:null|}));

                private Thickness Margin
                {
                    get => (Thickness)GetValue(MarginProperty);
                    set => SetValue(MarginProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // Regression test for a case found in https://github.com/jenius-apps/ambie
    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0030_WithInvalidPropertyName_DoesNotWarn()
    {
        string source = $$"""
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace AmbientSounds.Controls;

            public sealed partial class PlayerControl : UserControl
            {
                public static readonly DependencyProperty PlayVisibleProperty = DependencyProperty.Register(
                    {|WCTDPG0027:nameof(PlayButtonVisible)|},
                    {|WCTDPG0030:typeof(bool)|},
                    typeof(PlayerControl),
                    new PropertyMetadata({|WCTDPG0032:Visibility.Visible|}));

                public static readonly DependencyProperty VolumeVisibleProperty = DependencyProperty.Register(
                    nameof(VolumeVisible),
                    {|WCTDPG0030:typeof(bool)|},
                    typeof(PlayerControl),
                    new PropertyMetadata({|WCTDPG0032:Visibility.Visible|}));

                public Visibility PlayButtonVisible
                {
                    get => (Visibility)GetValue(PlayVisibleProperty);
                    set => SetValue(PlayVisibleProperty, value);
                }

                public Visibility VolumeVisible
                {
                    get => (Visibility)GetValue(VolumeVisibleProperty);
                    set => SetValue(VolumeVisibleProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // Regression test for a case found in the Microsoft Store
    [TestMethod]
    [DataRow("default(float)")]
    [DataRow("1.0F")]
    [DataRow("(float)1.0")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0030_WithMismatchedNullableUnderlyingType_DoesNotWarn(string defaultValue)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                public static readonly DependencyProperty Value1Property = DependencyProperty.Register(
                    nameof(Value1),
                    typeof(int?),
                    typeof(MyObject),
                    new PropertyMetadata({|WCTDPG0032:{{defaultValue}}|}));

                public static readonly DependencyProperty Value2Property = DependencyProperty.Register(
                    nameof(Value2),
                    {|WCTDPG0030:typeof(int?)|},
                    typeof(MyObject),
                    new PropertyMetadata({|WCTDPG0032:{{defaultValue}}|}));

                public static readonly DependencyProperty Value3Property = DependencyProperty.Register(
                    "Value3",
                    typeof(int?),
                    typeof(MyObject),
                    new PropertyMetadata({|WCTDPG0032:{{defaultValue}}|}));

                public int? {|WCTDPG0017:Value1|}
                {
                    get => (int?)GetValue(Value1Property);
                    set => SetValue(Value1Property, value);
                }

                public float Value2
                {
                    get => (float)GetValue(Value2Property);
                    set => SetValue(Value2Property, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // Same as above, but with property changed callbacks too
    [TestMethod]
    [DataRow("default(float)")]
    [DataRow("1.0F")]
    [DataRow("(float)1.0")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0030_WithMismatchedNullableUnderlyingType_WithCallbacks_DoesNotWarn(string defaultValue)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                public static readonly DependencyProperty Value1Property = DependencyProperty.Register(
                    nameof(Value1),
                    typeof(int?),
                    typeof(MyObject),
                    new PropertyMetadata({|WCTDPG0032:{{defaultValue}}|}, ItemSourcePropertyChanged));

                public static readonly DependencyProperty Value2Property = DependencyProperty.Register(
                    nameof(Value2),
                    {|WCTDPG0030:typeof(int?)|},
                    typeof(MyObject),
                    new PropertyMetadata({|WCTDPG0032:{{defaultValue}}|}, ItemSourcePropertyChanged));

                public static readonly DependencyProperty Value3Property = DependencyProperty.Register(
                    "Value3",
                    typeof(int?),
                    typeof(MyObject),
                    new PropertyMetadata({|WCTDPG0032:{{defaultValue}}|}, ItemSourcePropertyChanged));

                public int? Value1
                {
                    get => (int?)GetValue(Value1Property);
                    set => SetValue(Value1Property, value);
                }

                public float Value2
                {
                    get => (float)GetValue(Value2Property);
                    set => SetValue(Value2Property, value);
                }

                private static void ItemSourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
                {
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("\"Name\"", "typeof(string)", "typeof(string)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(Control)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(DependencyObject)", "null")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0029_DoesNotWarn(
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
                    {|WCTDPG0029:ownerType: {{ownerType}}|},
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
    [DataRow("\"NameProperty\"", "typeof(string)", "typeof(MyControl)", "null")]
    [DataRow("\"OtherName\"", "typeof(string)", "typeof(MyControl)", "null")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_WCTDPG0027_WCTDPG0028_DoesNotWarn(
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
                    {|WCTDPG0027:{|WCTDPG0028:name: {{name}}|}|},
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
    [DataRow("global::System.Numerics.Matrix3x2?", "global::System.Numerics.Matrix3x2?", "default(global::System.Numerics.Matrix3x2)")]
    [DataRow("global::System.Numerics.Matrix4x4?", "global::System.Numerics.Matrix4x4?", "default(global::System.Numerics.Matrix4x4)")]
    [DataRow("global::System.Numerics.Plane?", "global::System.Numerics.Plane?", "default(global::System.Numerics.Plane)")]
    [DataRow("global::System.Numerics.Quaternion?", "global::System.Numerics.Quaternion?", "default(global::System.Numerics.Quaternion)")]
    [DataRow("global::System.Numerics.Vector2?", "global::System.Numerics.Vector2?", "default(global::System.Numerics.Vector2)")]
    [DataRow("global::System.Numerics.Vector3?", "global::System.Numerics.Vector3?", "default(global::System.Numerics.Vector3)")]
    [DataRow("global::System.Numerics.Vector4?", "global::System.Numerics.Vector4?", "default(global::System.Numerics.Vector4)")]
    [DataRow("global::Windows.Foundation.Point?", "global::Windows.Foundation.Point?", "default(global::Windows.Foundation.Point)")]
    [DataRow("global::Windows.Foundation.Rect?", "global::Windows.Foundation.Rect?", "default(global::Windows.Foundation.Rect)")]
    [DataRow("global::Windows.Foundation.Size?", "global::Windows.Foundation.Size?", "default(global::Windows.Foundation.Size)")]
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

    // Using 'default(T)' is not a constant (therefore it's not allowed as attribute argument), even if constrained to an enum type.
    // Some of these combinations (eg. 'object' property with 'T1?' backing type) are also just flat out invalid and would error out.
    [TestMethod]
    [DataRow("T1?", "T1?", "new PropertyMetadata(default(T1))")]
    [DataRow("object", "T1?", "new PropertyMetadata(default(T1))")]
    [DataRow("T2?", "T2?", "new PropertyMetadata(default(T2))")]
    [DataRow("object", "T2?", "new PropertyMetadata(default(T2))")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_ExplicitDefaultValue_ConstrainedGeneric_DoesNotWarn(
        string dependencyPropertyType,
        string propertyType,
        string propertyMetadataExpression)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl<T1, T2> : Control
                where T1 : struct, Enum
                where T2 : unmanaged, Enum
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{dependencyPropertyType}}),
                    ownerType: typeof(MyControl<T1, T2>),
                    typeMetadata: {{propertyMetadataExpression}});

                public {{propertyType}} Name
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // Same as above, but this one also produces some WCTDPG0030 warnings, so we need to split these cases out
    [TestMethod]
    [DataRow("T1", "object", "new PropertyMetadata(default(T1))")]
    [DataRow("T1?", "object", "new PropertyMetadata(default(T1))")]
    [DataRow("T2", "object", "new PropertyMetadata(default(T2))")]
    [DataRow("T2?", "object", "new PropertyMetadata(default(T2))")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_ExplicitDefaultValue_ConstrainedGeneric_WithMismatchedType_DoesNotWarn(
        string dependencyPropertyType,
        string propertyType,
        string propertyMetadataExpression)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl<T1, T2> : Control
                where T1 : struct, Enum
                where T2 : unmanaged, Enum
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    {|WCTDPG0030:propertyType: typeof({{dependencyPropertyType}})|},
                    ownerType: typeof(MyControl<T1, T2>),
                    typeMetadata: {{propertyMetadataExpression}});

                public {{propertyType}} Name
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
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

                public {{propertyType}} {|WCTDPG0017:Name|}
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

                public string? {|WCTDPG0017:Name|}
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
    [DataRow("int?", "int?", "default(int)")]
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
    [DataRow("global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility?", "default(global::Windows.UI.Xaml.Visibility)")]
    [DataRow("global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility?", "default(global::Windows.UI.Xaml.Visibility?)")]
    [DataRow("global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility.Visible")]
    [DataRow("global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility.Collapsed")]
    [DataRow("object", "global::Windows.UI.Xaml.Visibility?", "default(global::Windows.UI.Xaml.Visibility)")]
    [DataRow("object", "global::Windows.UI.Xaml.Visibility?", "default(global::Windows.UI.Xaml.Visibility?)")]
    [DataRow("object", "global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility.Visible")]
    [DataRow("object", "global::Windows.UI.Xaml.Visibility?", "global::Windows.UI.Xaml.Visibility.Collapsed")]
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
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?", "global::MyApp.MyEnum.A")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?", "global::MyApp.MyEnum.B")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?", "default(global::MyApp.MyEnum)")]
    [DataRow("global::MyApp.MyEnum?", "global::MyApp.MyEnum?", "default(global::MyApp.MyEnum?)")]
    [DataRow("object", "global::MyApp.MyEnum?", "global::MyApp.MyEnum.A")]
    [DataRow("object", "global::MyApp.MyEnum?", "global::MyApp.MyEnum.B")]
    [DataRow("object", "global::MyApp.MyEnum?", "default(global::MyApp.MyEnum)")]
    [DataRow("object", "global::MyApp.MyEnum?", "default(global::MyApp.MyEnum?)")]
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

                public {{propertyType}} {|WCTDPG0017:Name|}
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

    // Using the declared property type as first argument here for clarity when reading all combinations
    [TestMethod]
    [DataRow("T1?", "T1?", "new PropertyMetadata(default(T1?))")]
    [DataRow("T1?", "object", "new PropertyMetadata(default(T1?))")]
    [DataRow("T1?", "T1?", "new PropertyMetadata(null)")]
    [DataRow("T1?", "object", "new PropertyMetadata(null)")]
    [DataRow("T1?", "T1?", "null")]
    [DataRow("T1?", "object", "null")]
    [DataRow("T2?", "T2?", "new PropertyMetadata(default(T2?))")]
    [DataRow("T2?", "object", "new PropertyMetadata(default(T2?))")]
    [DataRow("T2?", "T2?", "new PropertyMetadata(null)")]
    [DataRow("T2?", "object", "new PropertyMetadata(null)")]
    [DataRow("T2?", "T2?", "null")]
    [DataRow("T2?", "object", "null")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_ExplicitDefaultValue_ConstrainedGeneric_Warns(
        string propertyType,
        string dependencyPropertyType,
        string propertyMetadataExpression)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;

            #nullable enable
            
            namespace MyApp;

            public partial class MyControl<T1, T2> : Control
                where T1 : struct, Enum
                where T2 : unmanaged, Enum
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{dependencyPropertyType}}),
                    ownerType: typeof(MyControl<T1, T2>),
                    typeMetadata: {{propertyMetadataExpression}});

                public {{propertyType}} {|WCTDPG0017:Name|}
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
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

                public {{declaredType}} {|WCTDPG0017:Name|}
                {
                    get => ({{declaredType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_OrphanedPropertyField_DoesNotWarn()
    {
        const string source = """
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyObject),
                    typeMetadata: null);
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_OrphanedPropertyField_NoPropertySuffixOnDependencyPropertyField_Warns()
    {
        const string source = """
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty {|WCTDPG0026:NameField|} = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyObject),
                    typeMetadata: null);
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_OrphanedPropertyField_InvalidPropertyNameOnDependencyPropertyField_Warns()
    {
        const string source = """
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    {|WCTDPG0027:name: "Text"|},
                    propertyType: typeof(string),
                    ownerType: typeof(MyObject),
                    typeMetadata: null);
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_OrphanedPropertyField_InvalidOwningTypeOnDependencyPropertyField_Warns()
    {
        const string source = """
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    {|WCTDPG0029:ownerType: typeof(MyOtherObject)|},
                    typeMetadata: null);
            }

            public class MyOtherObject : DependencyObject;
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("string", "null")]
    [DataRow("string", "\"Bob\"")]
    [DataRow("object", "null")]
    [DataRow("object", "\"Bob\"")]
    [DataRow("object", "42")]
    [DataRow("int?", "null")]
    [DataRow("Visibility?", "null")]
    [DataRow("string", "DependencyProperty.UnsetValue")]
    [DataRow("object", "DependencyProperty.UnsetValue")]
    [DataRow("int", "DependencyProperty.UnsetValue")]
    [DataRow("int?", "DependencyProperty.UnsetValue")]
    [DataRow("Visibility", "DependencyProperty.UnsetValue")]
    [DataRow("Visibility?", "DependencyProperty.UnsetValue")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_OrphanedPropertyField_WithExplicitDefaultValue_DoesNotWarn(
        string propertyType,
        string defaultValueExpression)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{propertyType}}),
                    typeof(MyObject),
                    typeMetadata: new PropertyMetadata({{defaultValueExpression}}));
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("int")]
    [DataRow("global::System.TimeSpan")]
    [DataRow("global::Windows.Foundation.Rect")]
    [DataRow("Visibility")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_OrphanedPropertyField_NullDefaultValue_Warns(string propertyType)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{propertyType}}),
                    typeof(MyObject),
                    typeMetadata: new PropertyMetadata({|WCTDPG0031:null|}));
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("int", "3.0")]
    [DataRow("int", "3.0F")]
    [DataRow("int", "3L")]
    [DataRow("int", "\"Bob\"")]
    [DataRow("int", "Visibility.Visible")]
    [DataRow("int", "default(Visibility)")]
    [DataRow("bool", "Visibility.Visible")]
    [DataRow("string", "42")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_OrphanedPropertyField_InvalidDefaultValue_Warns(string propertyType, string defaultValue)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public class MyObject : DependencyObject
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{propertyType}}),
                    typeof(MyObject),
                    typeMetadata: new PropertyMetadata({|WCTDPG0032:{{defaultValue}}|}));
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // This is an explicit test to ensure upcasts are correctly detected and allowed
    [TestMethod]
    [DataRow("IDisposable", "MyClass")]
    [DataRow("object", "string")]
    [DataRow("object", "int")]
    [DataRow("object", "int?")]
    [DataRow("Control", "MyControl")]
    [DataRow("DependencyObject", "MyControl")]
    [DataRow("DependencyObject", "Control")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_WithUpcast_Warns(
        string dependencyPropertyType,
        string propertyType)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;
            using Windows.UI.Xaml.Controls;
            
            namespace MyApp;

            public partial class MyControl : Control
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof({{dependencyPropertyType}}),
                    ownerType: typeof(MyControl),
                    typeMetadata: null);

                public {{propertyType}} {|WCTDPG0017:Name|}
                {
                    get => ({{propertyType}})GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }

            public class MyClass : IDisposable
            {
                public void Dispose()
                {
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    // Regression test for a case found in the Microsoft Store
    [TestMethod]
    [DataRow("where T : class", "null")]
    [DataRow("where T : class", "default(T)")]
    [DataRow("where T : TOther where TOther : class", "null")]
    [DataRow("where T : class where TOther : class", "default(TOther)")]
    [DataRow("where T : Delegate", "null")]
    [DataRow("where T : Enum", "null")]
    [DataRow("where T : DependencyObject", "null")]
    [DataRow("where T : DependencyObject", "default(T)")]
    [DataRow("where T : TOther where TOther : Delegate", "null")]
    [DataRow("where T : TOther where TOther : Enum", "null")]
    [DataRow("where T : TOther where TOther : DependencyObject", "null")]
    [DataRow("where T : DependencyObject where TOther : class", "default(TOther)")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_WithNullConstrainedGeneric_Warns(
        string typeConstraints,
        string defaultValue)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;

            #nullable enable
            
            namespace MyApp;

            public partial class MyObject<T, TOther> : DependencyObject {{typeConstraints}}
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(T),
                    ownerType: typeof(MyObject<T, TOther>),
                    typeMetadata: new PropertyMetadata({{defaultValue}}));

                public T {|WCTDPG0017:Name|}
                {
                    get => (T?)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("where T : struct")]
    [DataRow("where T : unmanaged")]
    [DataRow("where T : struct, Enum")]
    [DataRow("where T : unmanaged, Enum")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_WithNullConstrainedGeneric_WCTDPG0031_DoesNotWarn(string typeConstraints)
    {
        string source = $$"""
            using System;
            using Windows.UI.Xaml;

            #nullable enable
            
            namespace MyApp;

            public partial class MyObject<T, TOther> : DependencyObject {{typeConstraints}}
            {
                public static readonly DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(T),
                    ownerType: typeof(MyObject<T, TOther>),
                    typeMetadata: new PropertyMetadata({|WCTDPG0031:null|}));

                public T {|WCTDPG0017:Name|}
                {
                    get => (T)GetValue(NameProperty);
                    set => SetValue(NameProperty, value);
                }
            }
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("private static readonly")]
    [DataRow("public static")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidFieldDeclaration_DoesNotWarn(string fieldDeclaration)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                {{fieldDeclaration}} DependencyProperty NameProperty = DependencyProperty.Register(
                    name: "Name",
                    propertyType: typeof(string),
                    ownerType: typeof(MyObject),
                    typeMetadata: null);

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
    [DataRow("private static readonly")]
    [DataRow("public static")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidFieldDeclaration_EmitsAdditionalDiagnosticsToo_DoesNotWarn(string fieldDeclaration)
    {
        string source = $$"""
            using Windows.UI.Xaml;
            
            namespace MyApp;

            public partial class MyObject : DependencyObject
            {
                {{fieldDeclaration}} DependencyProperty NameProperty = DependencyProperty.Register(
                    {|WCTDPG0027:{|WCTDPG0028:name: "Name2"|}|},
                    {|WCTDPG0030:propertyType: typeof(int?)|},
                    ownerType: typeof(MyObject),
                    typeMetadata: null);

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
                    [static: {|WCTDPG0018:Test|}]
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
                [static: {|WCTDPG0018:Testt|}]
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
                [static: {|WCTDPG0019:Test(TestAttribute.M)|}]
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
                [static: {|WCTDPG0019:Test(TestAttribute.M)|}]
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
                {{fieldDeclaration}} {|WCTDPG0020:TestProperty|};
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
                public static DependencyProperty {|WCTDPG0021:Test1Property|} => DependencyProperty.Register("Test1", typeof(string), typeof(MyObject), null);
                public static DependencyProperty {|WCTDPG0021:Test2Property|} { get; } = DependencyProperty.Register("Test2", typeof(string), typeof(MyObject), null);
                public static DependencyProperty {|WCTDPG0021:Test3Property|} { get; set; }
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
                [GeneratedDependencyProperty({|WCTDPG0022:PropertyType = typeof({{type}})|})]
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
                [GeneratedDependencyProperty({|WCTDPG0023:PropertyType = typeof({{propertyType}})|})]
                public {{declaredType}} Name { get; set; }
            }

            public interface IMyInterface;
            """;

        await CSharpAnalyzerTest<ExplicitPropertyMetadataTypeAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }
}
