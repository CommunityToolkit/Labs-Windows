// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

[TestClass]
public class Test_DependencyPropertyGenerator_Incrementality
{
    [TestMethod]
    public void ModifiedOptions_ModifiesOutput()
    {
        const string source = """"
            using Windows.UI.Xaml;
            using CommunityToolkit.WinUI;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty]
                public partial int Number { get; set; }
            }
            """";

        const string updatedSource = """"
            using Windows.UI.Xaml;
            using CommunityToolkit.WinUI;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty(DefaultValue = 42)]
                public partial int Number { get; set; }
            }
            """";

        CSharpGeneratorTest<DependencyPropertyGenerator>.VerifyIncrementalSteps(
            source,
            updatedSource,
            executeReason: IncrementalStepRunReason.Modified,
            diagnosticsReason: null,
            outputReason: IncrementalStepRunReason.Modified,
            diagnosticsSourceReason: null,
            sourceReason: IncrementalStepRunReason.Modified);
    }

    [TestMethod]
    public void AddedLeadingTrivia_DoesNotModifyOutput()
    {
        const string source = """"
            using Windows.UI.Xaml;
            using CommunityToolkit.WinUI;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty]
                public partial int Number { get; set; }
            }
            """";

        const string updatedSource = """"
            using Windows.UI.Xaml;
            using CommunityToolkit.WinUI;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                /// <summary>
                /// This is some property.
                /// </summary>
                [GeneratedDependencyProperty]
                public partial int Number { get; set; }
            }
            """";

        CSharpGeneratorTest<DependencyPropertyGenerator>.VerifyIncrementalSteps(
            source,
            updatedSource,
            executeReason: IncrementalStepRunReason.Unchanged,
            diagnosticsReason: null,
            outputReason: IncrementalStepRunReason.Cached,
            diagnosticsSourceReason: null,
            sourceReason: IncrementalStepRunReason.Cached);
    }

    [TestMethod]
    public void AddedOtherMember_DoesNotModifyOutput()
    {
        const string source = """"
            using Windows.UI.Xaml;
            using CommunityToolkit.WinUI;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty]
                public partial int Number { get; set; }
            }
            """";

        const string updatedSource = """"
            using Windows.UI.Xaml;
            using CommunityToolkit.WinUI;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                public void Foo()
                {
                }

                [GeneratedDependencyProperty]
                public partial int Number { get; set; }
            }
            """";

        CSharpGeneratorTest<DependencyPropertyGenerator>.VerifyIncrementalSteps(
            source,
            updatedSource,
            executeReason: IncrementalStepRunReason.Unchanged,
            diagnosticsReason: null,
            outputReason: IncrementalStepRunReason.Cached,
            diagnosticsSourceReason: null,
            sourceReason: IncrementalStepRunReason.Cached);
    }
}
