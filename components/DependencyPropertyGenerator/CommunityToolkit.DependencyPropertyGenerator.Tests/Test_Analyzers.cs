// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.SourceGenerators.UnitTests.Helpers;
using Microsoft.CodeAnalysis.CSharp;
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
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NoAttribute_DoesNotWarn()
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

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    [DataRow("int")]
    [DataRow("int?")]
    [DataRow("string?")]
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NullableOrNotApplicableType_DoesNotWarn(string propertyType)
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

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NotNullableType_WithMaybeNullAttribute_DoesNotWarn()
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

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NotNullableType_Required_DoesNotWarn()
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

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NotNullableType_NullableDisabled_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty]
                public required partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NotNullableType_WithNonNullDefaultValue_DoesNotWarn()
    {
        const string source = """
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml.Controls;

            #nullable enable

            namespace MyApp;

            public partial class MyControl : Control
            {
                [GeneratedDependencyProperty(DefaultValue = "Bob")]
                public required partial string {|CS9248:Name|} { get; set; }
            }
            """;

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NotNullableType_Warns()
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

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }

    [TestMethod]
    public async Task InvalidPropertyNonNullableDeclarationAnalyzer_NotNullableType_WithNullDefaultValue_Warns()
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

        await CSharpAnalyzerTest<InvalidPropertyNonNullableDeclarationAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
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
            using CommunityToolkit.WinUI;
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
            using CommunityToolkit.WinUI;
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
    [DataRow("\"Name\"", "typeof(object)", "typeof(MyControl)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(string)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(Control)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(DependencyObject)", "null")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(MyControl)", "new PropertyMetadata(42)")]
    [DataRow("\"Name\"", "typeof(string)", "typeof(MyControl)", "new PropertyMetadata(null, (d, e) => { })")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_InvalidRegisterArguments_DoesNotWarn(
        string name,
        string propertyType,
        string ownerType,
        string typeMetadata)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
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
            using CommunityToolkit.WinUI;
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
            using CommunityToolkit.WinUI;
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
    [DataRow("string", "string")]
    [DataRow("string", "string?")]
    [DataRow("object", "object")]
    [DataRow("object", "object?")]
    [DataRow("int", "int")]
    [DataRow("int?", "int?")]
    public async Task UseGeneratedDependencyPropertyOnManualPropertyAnalyzer_ValidProperty_Warns(
        string dependencyPropertyType,
        string propertyType)
    {
        string source = $$"""
            using CommunityToolkit.WinUI;
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
            """;

        await CSharpAnalyzerTest<UseGeneratedDependencyPropertyOnManualPropertyAnalyzer>.VerifyAnalyzerAsync(source, LanguageVersion.CSharp13);
    }
}
