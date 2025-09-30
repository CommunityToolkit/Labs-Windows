// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tests;
using CommunityToolkit.WinUI.Helpers;

namespace AccentAnalyzerTests;

[TestClass]
public partial class ExampleAccentAnalyzerTestClass : VisualUITestBase
{
    // If you don't need access to UI objects directly or async code, use this pattern.
    [TestMethod]
    public void SimpleSynchronousExampleTest()
    {
        var assembly = typeof(AccentAnalyzer).Assembly;
        var type = assembly.GetType(typeof(AccentAnalyzer).FullName ?? string.Empty);

        Assert.IsNotNull(type, "Could not find AccentAnalyzer type.");
        Assert.AreEqual(typeof(AccentAnalyzer), type, "Type of AccentAnalyzer does not match expected type.");
    }
}
