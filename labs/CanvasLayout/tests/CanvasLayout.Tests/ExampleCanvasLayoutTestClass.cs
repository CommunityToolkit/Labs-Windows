// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.TestGen;
using CommunityToolkit.Tests;

namespace CanvasLayoutExperiment.Tests;

[TestClass]
public partial class ExampleCanvasLayoutTestClass : VisualUITestBase
{
    // If you don't need access to UI objects directly or async code, use this pattern.
    [TestMethod]
    public void SimpleSynchronousExampleTest()
    {
        var assembly = typeof(CanvasLayout).Assembly;
        var type = assembly.GetType(typeof(CanvasLayout).FullName ?? string.Empty);

        Assert.IsNotNull(type, "Could not find CanvasLayout type.");
        Assert.AreEqual(typeof(CanvasLayout), type, "Type of CanvasLayout does not match expected type.");
    }

    // If you don't need access to UI objects directly, use this pattern.
    [TestMethod]
    public async Task SimpleAsyncExampleTest()
    {
        await Task.Delay(250);

        Assert.IsTrue(true);
    }

    // Example that shows how to check for exception throwing.
    [TestMethod]
    public void SimpleExceptionCheckTest()
    {
        // If you need to check exceptions occur for invalid inputs, etc...
        // Use Assert.ThrowsException to limit the scope to where you expect the error to occur.
        // Otherwise, using the ExpectedException attribute could swallow or
        // catch other issues in setup code.
        Assert.ThrowsException<NotImplementedException>(() => throw new NotImplementedException());
    }

    // The UIThreadTestMethod automatically dispatches to the UI for us to work with UI objects.
    [UIThreadTestMethod]
    public void SimpleUIAttributeExampleTest()
    {
        var component = new CanvasLayout();
        Assert.IsNotNull(component);
    }

    // The UIThreadTestMethod can also easily grab a XAML Page for us by passing its type as a parameter.
    // This lets us actually test a control as it would behave within an actual application.
    // The page will already be loaded by the time your test is called.
    [UIThreadTestMethod]
    public void SimpleUIExamplePageTest(ExampleCanvasLayoutTestPage page)
    {
        // You can use the Toolkit Visual Tree helpers here to find the component by type or name:
        var component = page.FindDescendant<MUXC.ItemsRepeater>();

        Assert.IsNotNull(component);
        Assert.IsNotNull(component.Layout);
        Assert.AreEqual(typeof(CanvasLayout), component.Layout.GetType());
    }

    // You can still do async work with a UIThreadTestMethod as well.
    [UIThreadTestMethod]
    public async Task SimpleAsyncUIExamplePageTest(ExampleCanvasLayoutTestPage page)
    {
        // This helper can be used to wait for a rendering pass to complete.
        await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

        var component = page.FindDescendant<MUXC.ItemsRepeater>();

        Assert.IsNotNull(component);
        Assert.IsNotNull(component.Layout);
        Assert.AreEqual(typeof(CanvasLayout), component.Layout.GetType());
    }
}
