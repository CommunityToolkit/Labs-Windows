// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tests;
using CommunityToolkit.Tooling.TestGen;
using CommunityToolkit.WinUI.Controls;

namespace RibbonTests;

[TestClass]
public partial class RibbonTestClass : VisualUITestBase
{
    [TestMethod]
    public void SimpleSynchronousExampleTest()
    {
        var assembly = typeof(Ribbon).Assembly;
        var type = assembly.GetType(typeof(Ribbon).FullName ?? string.Empty);

        Assert.IsNotNull(type, "Could not find Ribbon type.");
        Assert.AreEqual(typeof(Ribbon), type, "Type of Ribbon does not match expected type.");
    }

    // The UIThreadTestMethod automatically dispatches to the UI for us to work with UI objects.
    [UIThreadTestMethod]
    public void SimpleUIAttributeExampleTest()
    {
        var component = new Ribbon();
        Assert.IsNotNull(component);
    }

    // The UIThreadTestMethod can also easily grab a XAML Page for us by passing its type as a parameter.
    // This lets us actually test a control as it would behave within an actual application.
    // The page will already be loaded by the time your test is called.
    [UIThreadTestMethod]
    public void SimpleUIExamplePageTest(RibbonTestPage page)
    {
        // You can use the Toolkit Visual Tree helpers here to find the component by type or name:
        var component = page.FindDescendant<Ribbon>();

        Assert.IsNotNull(component);
    }
}
