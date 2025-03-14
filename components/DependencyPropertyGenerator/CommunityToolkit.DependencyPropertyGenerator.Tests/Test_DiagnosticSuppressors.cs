// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

[TestClass]
public class Test_DiagnosticSuppressors
{
    private static readonly DiagnosticResult CS0658 = DiagnosticResult.CompilerWarning("CS0658");

    [TestMethod]
    [DataRow("get")]
    [DataRow("with")]
    [DataRow("readonly")]
    [DataRow("propdp")]
    public async Task StaticAttributeListTargetOnGeneratedDependencyPropertyDeclarationSuppressor_OtherTarget_NotSuppressed(string target)
    {
        await new CSharpSuppressorTest<StaticAttributeListTargetOnGeneratedDependencyPropertyDeclarationSuppressor>(
            $$"""
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                [GeneratedDependencyProperty]
                [{{target}}: Test]
                public string? Name { get; set; }
            }

            public class TestAttribute : Attribute;
            """)
            .WithSpecificDiagnostics(CS0658)
            .RunAsync();
    }

    [TestMethod]
    public async Task StaticAttributeListTargetOnGeneratedDependencyPropertyDeclarationSuppressor_NoTriggerAttribute_NotSuppressed()
    {
        await new CSharpSuppressorTest<StaticAttributeListTargetOnGeneratedDependencyPropertyDeclarationSuppressor>(
            """
            using System;
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                [static: Test]
                public string? Name { get; set; }
            }

            public class TestAttribute : Attribute;
            """)
            .WithSpecificDiagnostics(CS0658)
            .RunAsync();
    }

    [TestMethod]
    public async Task StaticAttributeListTargetOnGeneratedDependencyPropertyDeclarationSuppressor_ValidUse_Suppressed()
    {
        await new CSharpSuppressorTest<StaticAttributeListTargetOnGeneratedDependencyPropertyDeclarationSuppressor>(
            """
            using System;
            using CommunityToolkit.WinUI;
            using Windows.UI.Xaml;

            public class MyObject : DependencyObject
            {
                [GeneratedDependencyProperty]
                [static: Test]
                public string? Name { get; set; }
            }

            public class TestAttribute : Attribute;
            """)
            .WithSpecificDiagnostics(CS0658)
            .RunAsync();
    }
}
