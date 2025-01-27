// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using CommunityToolkit.GeneratedDependencyProperty.Tests.Helpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.GeneratedDependencyProperty.Tests;

partial class Test_DependencyPropertyGenerator
{
    [TestMethod]
    [DataRow("GeneratedDependencyProperty")]
    [DataRow("GeneratedDependencyPropertyAttribute")]
    public void SingleProperty_String_WithNoCaching_PostInitializationSources(string typeName)
    {
        const string source = """
            using Windows.UI.Xaml;
            using CommunityToolkit.WinUI;

            namespace MyNamespace;

            public partial class MyControl : DependencyObject
            {
                [GeneratedDependencyProperty]
                public partial string? Name { get; set; }
            }
            """;

        string fileName = $"{typeName}.g.cs";
        string sourceText;

        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName)!)
        using (StreamReader reader = new(stream))
        {
            sourceText = reader.ReadToEnd();
        }

        string updatedSourceText = sourceText
            .Replace("<GENERATOR_NAME>", "CommunityToolkit.WinUI.DependencyPropertyGenerator")
            .Replace("<ASSEMBLY_VERSION>", typeof(DependencyPropertyGenerator).Assembly.GetName().Version!.ToString());

        CSharpGeneratorTest<DependencyPropertyGenerator>.VerifySources(source, (fileName, updatedSourceText), languageVersion: LanguageVersion.CSharp13);
    }
}
