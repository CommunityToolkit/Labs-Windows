// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Basic.Reference.Assemblies;
using CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;
using CommunityToolkit.WinUI;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

partial class Test_DependencyPropertyGenerator
{
    [TestMethod]
    [DynamicData(nameof(GetReferenceSetterData), DynamicDataSourceType.Method)]
    public void XamlBindingHelper_ReferenceValues(
        string propertyType,
        bool isLocalCacheEnabled,
        bool useWindowsUIXaml)
    {
        Compilation compilation = CreateXamlBindingHelperCompilation(
            CreateReferenceSetterSource(propertyType, isLocalCacheEnabled, useWindowsUIXaml),
            useWindowsUIXaml);

        RunGeneratedSetter(compilation, type =>
        {
            object control = Activator.CreateInstance(type)!;
            PropertyInfo property = type.GetProperty("Value")!;
            List<string> events = (List<string>)type.GetProperty("Events")!.GetValue(control)!;
            object value = propertyType.StartsWith("System.Uri", StringComparison.Ordinal)
                ? new Uri("https://example.com/first")
                : "First";
            string setMethod = propertyType.TrimEnd('?') switch
            {
                "System.Uri" => "SetPropertyFromUri",
                "string" => "SetPropertyFromString",
                _ => "SetValue"
            };

            property.SetValue(control, null);

            CollectionAssert.AreEqual(
                isLocalCacheEnabled ? new[] { "Set" } : new[] { "Set", "SetValue", "Changed" },
                events);
            Assert.IsNull(property.GetValue(control));

            events.Clear();
            property.SetValue(control, value);

            Assert.AreEqual(value, property.GetValue(control));
            Assert.AreEqual(value, type.GetProperty("LocalValue")!.GetValue(control));
            CollectionAssert.AreEqual(GetSetterEvents(isLocalCacheEnabled, setMethod), events);

            events.Clear();
            property.SetValue(control, null);

            Assert.IsNull(property.GetValue(control));
            Assert.IsNull(type.GetProperty("LocalValue")!.GetValue(control));
            Assert.AreEqual(value, type.GetProperty("OldValue")!.GetValue(control));
            Assert.IsNull(type.GetProperty("NewValue")!.GetValue(control));
            CollectionAssert.AreEqual(GetSetterEvents(isLocalCacheEnabled, "SetValue"), events);

            property.SetValue(control, value);
            type.GetProperty("SetToNull")!.SetValue(control, true);
            events.Clear();
            property.SetValue(control, value);

            Assert.IsNull(property.GetValue(control));
            Assert.IsNull(type.GetProperty("LocalValue")!.GetValue(control));
            CollectionAssert.AreEqual(GetSetterEvents(isLocalCacheEnabled, "SetValue"), events);

            if (propertyType.StartsWith("string", StringComparison.Ordinal))
            {
                type.GetProperty("SetToNull")!.SetValue(control, false);
                events.Clear();
                property.SetValue(control, "");

                Assert.AreEqual("", property.GetValue(control));
                Assert.AreEqual("", type.GetProperty("LocalValue")!.GetValue(control));
                CollectionAssert.AreEqual(GetSetterEvents(isLocalCacheEnabled, "SetValue"), events);
            }
        });
    }

    [TestMethod]
    [DynamicData(nameof(GetBoxedSetHookData), DynamicDataSourceType.Method)]
    public void XamlBindingHelper_BoxedSetHook(
        string propertyType,
        bool isLocalCacheEnabled,
        bool useWindowsUIXaml)
    {
        string source = CreateReferenceSetterSource(propertyType, isLocalCacheEnabled, useWindowsUIXaml, hasBoxedSetHook: true);
        Compilation compilation = CreateXamlBindingHelperCompilation(source, useWindowsUIXaml);

        Assert.IsFalse(GetGeneratedSetter(compilation).Contains("XamlBindingHelper", StringComparison.Ordinal));

        RunGeneratedSetter(compilation, type =>
        {
            object control = Activator.CreateInstance(type)!;
            PropertyInfo property = type.GetProperty("Value")!;
            List<string> events = (List<string>)type.GetProperty("Events")!.GetValue(control)!;
            object value = propertyType.StartsWith("System.Uri", StringComparison.Ordinal)
                ? new Uri("https://example.com/first")
                : "First";

            property.SetValue(control, value);

            Assert.IsNull(type.GetProperty("LocalValue")!.GetValue(control));
            CollectionAssert.AreEqual(
                isLocalCacheEnabled
                    ? new[] { "Set", "Changing", "ChangingWithOldValue", "BoxedSet", "SetValue", "Changed", "ChangedWithOldValue" }
                    : new[] { "Set", "BoxedSet", "SetValue", "Changed" },
                events);
        });
    }

    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public void XamlBindingHelper_Uri_Setter(bool isLocalCacheEnabled, bool useWindowsUIXaml)
    {
        string xamlNamespace = useWindowsUIXaml ? "Windows.UI.Xaml" : "Microsoft.UI.Xaml";
        string cachingPrefix = isLocalCacheEnabled ? """

                if (global::System.Collections.Generic.EqualityComparer<global::System.Uri?>.Default.Equals(field, value))
                {
                    return;
                }

                global::System.Uri? __oldValue = field;

                OnValueChanging(value);
                OnValueChanging(__oldValue, value);

                field = value;
            """ : "";
        string cachingSuffix = isLocalCacheEnabled ? "OnValueChanged(__oldValue, value);" : "";
        string expected = $$"""
            {
                OnValueSet(ref value);
                {{cachingPrefix}}

                if (value is null)
                {
                    SetValue(ValueProperty, value);
                }
                else
                {
                    global::{{xamlNamespace}}.Markup.XamlBindingHelper.SetPropertyFromUri(this, ValueProperty, value);
                }

                OnValueChanged(value);
                {{cachingSuffix}}
            }
            """;

        foreach (string propertyType in new[] { "System.Uri", "System.Uri?" })
        {
            Compilation compilation = CreateXamlBindingHelperCompilation(
                CreateReferenceSetterSource(propertyType, isLocalCacheEnabled, useWindowsUIXaml),
                useWindowsUIXaml);

            Assert.AreEqual(SyntaxFactory.ParseStatement(expected).NormalizeWhitespace().ToFullString(), GetGeneratedSetter(compilation));
        }
    }

    [TestMethod]
    [DynamicData(nameof(GetValueSetterData), DynamicDataSourceType.Method)]
    public void XamlBindingHelper_ValueTypes(
        string propertyType,
        string? setMethod,
        bool isLocalCacheEnabled,
        bool useWindowsUIXaml,
        bool isNullable)
    {
        string xamlNamespace = useWindowsUIXaml ? "Windows.UI.Xaml" : "Microsoft.UI.Xaml";
        string source = $$"""
            using CommunityToolkit.WinUI;
            using {{xamlNamespace}};

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty(IsLocalCacheEnabled = {{(isLocalCacheEnabled ? "true" : "false")}})]
                public partial {{propertyType}}{{(isNullable ? "?" : "")}} Value { get; set; }
            }
            """;
        Compilation compilation = CreateXamlBindingHelperCompilation(source, useWindowsUIXaml);
        string setter = GetGeneratedSetter(compilation);

        if (isNullable || setMethod is null)
        {
            StringAssert.Contains(setter, "SetValue(ValueProperty, __boxedValue);");
            Assert.IsFalse(setter.Contains("XamlBindingHelper", StringComparison.Ordinal));
        }
        else
        {
            StringAssert.Contains(setter, $"global::{xamlNamespace}.Markup.XamlBindingHelper.{setMethod}(this, ValueProperty, value);");
            Assert.IsFalse(setter.Contains("value is null", StringComparison.Ordinal));
        }
    }

    public static IEnumerable<object[]> GetReferenceSetterData()
    {
        foreach (string propertyType in new[] { "System.Uri", "System.Uri?", "string", "string?", "object", "object?" })
        {
            foreach (bool isLocalCacheEnabled in new[] { false, true })
            {
                foreach (bool useWindowsUIXaml in new[] { false, true })
                {
                    yield return [propertyType, isLocalCacheEnabled, useWindowsUIXaml];
                }
            }
        }
    }

    public static IEnumerable<object[]> GetBoxedSetHookData()
    {
        return GetReferenceSetterData().Where(static data => !((string)data[0]).StartsWith("object", StringComparison.Ordinal));
    }

    public static IEnumerable<object?[]> GetValueSetterData()
    {
        foreach (bool useWindowsUIXaml in new[] { false, true })
        {
            foreach (var (propertyType, setMethod) in GetValueSetterTypes(useWindowsUIXaml))
            {
                foreach (bool isLocalCacheEnabled in new[] { false, true })
                {
                    foreach (bool isNullable in new[] { false, true })
                    {
                        bool isOptionalHelper = setMethod is "SetPropertyFromColor" or "SetPropertyFromCornerRadius" or "SetPropertyFromThickness";

                        yield return [propertyType, useWindowsUIXaml && isOptionalHelper ? null : setMethod, isLocalCacheEnabled, useWindowsUIXaml, isNullable];
                    }
                }
            }
        }
    }

    private static (string Type, string Method)[] GetValueSetterTypes(bool useWindowsUIXaml)
    {
        string xamlNamespace = useWindowsUIXaml ? "Windows.UI.Xaml" : "Microsoft.UI.Xaml";

        return
        [
            ("bool", "SetPropertyFromBoolean"),
            ("byte", "SetPropertyFromByte"),
            ("char", "SetPropertyFromChar16"),
            ("double", "SetPropertyFromDouble"),
            ("int", "SetPropertyFromInt32"),
            ("long", "SetPropertyFromInt64"),
            ("float", "SetPropertyFromSingle"),
            ("uint", "SetPropertyFromUInt32"),
            ("ulong", "SetPropertyFromUInt64"),
            ("System.DateTimeOffset", "SetPropertyFromDateTime"),
            ("System.TimeSpan", "SetPropertyFromTimeSpan"),
            ("Windows.Foundation.Point", "SetPropertyFromPoint"),
            ("Windows.Foundation.Rect", "SetPropertyFromRect"),
            ("Windows.Foundation.Size", "SetPropertyFromSize"),
            ("Windows.UI.Color", "SetPropertyFromColor"),
            ($"{xamlNamespace}.CornerRadius", "SetPropertyFromCornerRadius"),
            ($"{xamlNamespace}.Thickness", "SetPropertyFromThickness")
        ];
    }

    private static string[] GetSetterEvents(bool isLocalCacheEnabled, string setMethod)
    {
        return isLocalCacheEnabled
            ? ["Set", "Changing", "ChangingWithOldValue", setMethod, "PropertyChanged", "SharedPropertyChanged", "Changed", "ChangedWithOldValue"]
            : ["Set", setMethod, "PropertyChanged", "SharedPropertyChanged", "Changed"];
    }

    private static string CreateReferenceSetterSource(
        string propertyType,
        bool isLocalCacheEnabled,
        bool useWindowsUIXaml,
        bool hasBoxedSetHook = false)
    {
        string xamlNamespace = useWindowsUIXaml ? "Windows.UI.Xaml" : "Microsoft.UI.Xaml";
        string oldValueType = $"{propertyType.TrimEnd('?')}?";
        string cachingHooks = isLocalCacheEnabled ? $$"""
                partial void OnValueChanging({{propertyType}} newValue) => Events.Add("Changing");
                partial void OnValueChanging({{oldValueType}} oldValue, {{propertyType}} newValue) => Events.Add("ChangingWithOldValue");
                partial void OnValueChanged({{oldValueType}} oldValue, {{propertyType}} newValue) => Events.Add("ChangedWithOldValue");
            """ : "";
        string boxedSetHook = hasBoxedSetHook ? $$"""
                partial void OnValueSet(ref {{(propertyType.EndsWith("?", StringComparison.Ordinal) ? "object?" : "object")}} propertyValue)
                {
                    Events.Add("BoxedSet");
                    propertyValue = null!;
                }
            """ : "";

        return $$"""
            using CommunityToolkit.WinUI;
            using {{xamlNamespace}};

            #nullable enable

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty(IsLocalCacheEnabled = {{(isLocalCacheEnabled ? "true" : "false")}})]
                public partial {{propertyType}} Value { get; set; }

                public bool SetToNull { get; set; }
                public object? LocalValue => ReadLocalValue(ValueProperty);
                public object? OldValue { get; private set; }
                public object? NewValue { get; private set; }

                partial void OnValueSet(ref {{propertyType}} propertyValue)
                {
                    Events.Add("Set");

                    if (SetToNull)
                    {
                        propertyValue = null!;
                    }
                }

                {{boxedSetHook}}
                {{cachingHooks}}
                partial void OnValueChanged({{propertyType}} newValue) => Events.Add("Changed");

                partial void OnValuePropertyChanged(DependencyPropertyChangedEventArgs e)
                {
                    Events.Add("PropertyChanged");
                    OldValue = e.OldValue;
                    NewValue = e.NewValue;
                }

                partial void OnPropertyChanged(DependencyPropertyChangedEventArgs e) => Events.Add("SharedPropertyChanged");
            }
            """;
    }

    private static string GetGeneratedSetter(Compilation compilation)
    {
        return compilation.SyntaxTrees
            .Single(tree => Path.GetFileName(tree.FilePath) == "MyNamespace.MyControl.g.cs")
            .GetRoot()
            .DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .Single()
            .AccessorList!.Accessors.Single(accessor => accessor.IsKind(SyntaxKind.SetAccessorDeclaration))
            .Body!.NormalizeWhitespace().ToFullString();
    }

    private static void RunGeneratedSetter(Compilation compilation, Action<Type> test)
    {
        using MemoryStream stream = new();

        Assert.IsTrue(compilation.Emit(stream).Success);

        stream.Position = 0;

        AssemblyLoadContext context = new(name: null, isCollectible: true);

        try
        {
            Assembly assembly = context.LoadFromStream(stream);

            test(assembly.GetType("MyNamespace.MyControl", throwOnError: true)!);
        }
        finally
        {
            context.Unload();
        }
    }

    private static Compilation CreateXamlBindingHelperCompilation(string source, bool useWindowsUIXaml)
    {
        string xamlNamespace = useWindowsUIXaml ? "Windows.UI.Xaml" : "Microsoft.UI.Xaml";
        string valueSetters = string.Join(Environment.NewLine, GetValueSetterTypes(useWindowsUIXaml).Select(static item => $$"""
                    public static void {{item.Method}}(object dependencyObject, DependencyProperty propertyToSet, {{item.Type}} value)
                        => ((DependencyObject)dependencyObject).SetValueCore(propertyToSet, value, "{{item.Method}}");
            """));

        // Managed XAML doubles exercise generated setters without requiring a XAML application
        string xamlTypes = $$"""
            using System;
            using System.Collections.Generic;

            #nullable enable

            namespace Windows.Foundation
            {
                public struct Point;
                public struct Rect;
                public struct Size;
            }

            namespace Windows.UI
            {
                public struct Color;
            }

            namespace {{xamlNamespace}}
            {
                public struct CornerRadius;
                public struct Thickness;
                public delegate void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e);

                public sealed class DependencyPropertyChangedEventArgs(object? oldValue, object? newValue)
                {
                    public object? OldValue { get; } = oldValue;
                    public object? NewValue { get; } = newValue;
                }

                public sealed class PropertyMetadata(object? defaultValue, PropertyChangedCallback? propertyChangedCallback = null)
                {
                    public object? DefaultValue { get; } = defaultValue;
                    public PropertyChangedCallback? Callback { get; } = propertyChangedCallback;
                }

                public sealed class DependencyProperty
                {
                    public static object UnsetValue { get; } = new();
                    public PropertyMetadata? Metadata { get; private init; }

                    public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata? typeMetadata)
                        => new() { Metadata = typeMetadata };
                }

                public class DependencyObject
                {
                    private readonly Dictionary<DependencyProperty, object?> values = new();

                    public List<string> Events { get; } = new();

                    public object? GetValue(DependencyProperty property)
                        => values.TryGetValue(property, out object? value) ? value : property.Metadata?.DefaultValue;

                    public object? ReadLocalValue(DependencyProperty property)
                        => values.TryGetValue(property, out object? value) ? value : DependencyProperty.UnsetValue;

                    public void SetValue(DependencyProperty property, object? value) => SetValueCore(property, value, "SetValue");

                    public void SetValueCore(DependencyProperty property, object? value, string method)
                    {
                        object? oldValue = GetValue(property);
                        values[property] = value;
                        Events.Add(method);

                        if (!Equals(oldValue, value))
                        {
                            property.Metadata?.Callback?.Invoke(this, new(oldValue, value));
                        }
                    }
                }
            }

            namespace {{xamlNamespace}}.Markup
            {
                public static class XamlBindingHelper
                {
                    public static void SetPropertyFromUri(object dependencyObject, DependencyProperty propertyToSet, Uri value)
                    {
                        ArgumentNullException.ThrowIfNull(value);
                        ((DependencyObject)dependencyObject).SetValueCore(propertyToSet, value, "SetPropertyFromUri");
                    }

                    public static void SetPropertyFromString(object dependencyObject, DependencyProperty propertyToSet, string value)
                    {
                        ArgumentException.ThrowIfNullOrEmpty(value);
                        ((DependencyObject)dependencyObject).SetValueCore(propertyToSet, value, "SetPropertyFromString");
                    }

                    public static void SetPropertyFromObject(object dependencyObject, DependencyProperty propertyToSet, object value)
                    {
                        ArgumentNullException.ThrowIfNull(value);
                        ((DependencyObject)dependencyObject).SetValueCore(propertyToSet, value, "SetPropertyFromObject");
                    }

                    {{valueSetters}}
                }
            }
            """;
        CSharpParseOptions parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
        CSharpCompilation compilation = CSharpCompilation.Create(
            "XamlBindingHelperTests",
            [CSharpSyntaxTree.ParseText(source, parseOptions), CSharpSyntaxTree.ParseText(xamlTypes, parseOptions)],
            [.. Net80.References.All, MetadataReference.CreateFromFile(typeof(GeneratedDependencyPropertyAttribute).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));

        return CSharpGeneratorTest<DependencyPropertyGenerator>.VerifyCompiles(
            compilation,
            new DependencyPropertyGeneratorAnalyzerConfigOptionsProvider(useWindowsUIXaml));
    }
}
