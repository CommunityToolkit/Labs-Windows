// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.Tests;
using CommunityToolkit.Tooling.TestGen;

namespace TransitionHelperExperiment.Tests;

[TestClass]
public partial class ExampleTransitionHelperTestClass : VisualUITestBase
{
    // If you don't need access to UI objects directly or async code, use this pattern.
    [TestMethod]
    public void SimpleSynchronousExampleTest()
    {
        var assembly = typeof(TransitionHelper).Assembly;
        var type = assembly.GetType(typeof(TransitionHelper).FullName ?? string.Empty);

        Assert.IsNotNull(type, "Could not find TransitionHelper type.");
        Assert.AreEqual(typeof(TransitionHelper), type, "Type of TransitionHelper does not match expected type.");
    }

    // The UIThreadTestMethod automatically dispatches to the UI for us to work with UI objects.
    [UIThreadTestMethod]
    public void SimpleUIAttributeExampleTest()
    {
        var component = new TransitionHelper();
        Assert.IsNotNull(component);
    }
}
