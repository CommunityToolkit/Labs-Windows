// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;
using CommunityToolkit.Labs.UnitTests;

namespace ProjectTemplate.Tests;

[TestClass]
public partial class ExampleProjectTemplateTestClass : VisualUITestBase
{
    // If you don't need access to UI objects directly or async code, use this pattern.
    [TestMethod]
    public void SimpleSynchronousExampleTest()
    {
        var assembly = typeof(ProjectTemplate_ClassicBinding).Assembly;
        var type = assembly.GetType(typeof(ProjectTemplate_ClassicBinding).FullName ?? string.Empty);

        Assert.IsNotNull(type, "Could not find ProjectTemplate_ClassicBinding type.");
        Assert.AreEqual(typeof(ProjectTemplate_ClassicBinding), type, "Type of ProjectTemplate_ClassicBinding does not match expected type.");
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

    // The LabsUITestMethod automatically dispatches to the UI for us to work with UI objects.
    [LabsUITestMethod]
    public void SimpleUIAttributeExampleTest()
    {
        var component = new ProjectTemplate_ClassicBinding();
        Assert.IsNotNull(component);
    }

    // The LabsUITestMethod can also easily grab a XAML Page for us by passing its type as a parameter.
    // This lets us actually test a control as it would behave within an actual application.
    // The page will already be loaded by the time your test is called.
    [LabsUITestMethod]
    public void SimpleUIExamplePageTest(ExampleProjectTemplateTestPage page)
    {
        // You can use the Toolkit Visual Tree helpers here to find the component by type or name:
        var component = page.FindDescendant<ProjectTemplate_ClassicBinding>();

        Assert.IsNotNull(component);

        var componentByName = page.FindDescendant("ProjectTemplateControl");

        Assert.IsNotNull(componentByName);
    }

    // You can still do async work with a LabsUITestMethod as well.
    [LabsUITestMethod]
    public async Task SimpleAsyncUIExamplePageTest(ExampleProjectTemplateTestPage page)
    {
        // This helper can be used to wait for a rendering pass to complete.
        await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

        var component = page.FindDescendant<ProjectTemplate_ClassicBinding>();

        Assert.IsNotNull(component);
    }

    // If you need to use DataRow, you can use this pattern with the UI dispatch still.
    // Otherwise, checkout the LabsUITestMethod attribute above.
    // See https://github.com/CommunityToolkit/Labs-Windows/issues/186
    [TestMethod]
    public async Task ComplexAsyncUIExampleTest()
    {
        await App.DispatcherQueue.EnqueueAsync(() =>
        {
            var component = new ProjectTemplate_ClassicBinding();
            Assert.IsNotNull(component);
        });
    }
}
