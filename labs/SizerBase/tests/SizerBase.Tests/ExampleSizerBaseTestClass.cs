// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.TestGen;
using CommunityToolkit.Tests;
using CommunityToolkit.Labs.WinUI.Automation.Peers;

namespace SizerBaseExperiment.Tests;

[TestClass]
public partial class ExampleSizerBaseTestClass : VisualUITestBase
{
    [TestMethod]
    public async Task ShouldConfigureGridSplitterAutomationPeer()
    {
        await EnqueueAsync(() =>
        {
            const string automationName = "MyCustomAutomationName";
            const string name = "Sizer";

            var gridSplitter = new GridSplitter();
            var gridSplitterAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(gridSplitter) as SizerAutomationPeer;

            Assert.IsNotNull(gridSplitterAutomationPeer, "Verify that the AutomationPeer is SizerAutomationPeer.");

            gridSplitter.Name = name;
            Assert.IsTrue(gridSplitterAutomationPeer.GetName().Contains(name), "Verify that the UIA name contains the given Name of the GridSplitter (Sizer).");

            gridSplitter.SetValue(AutomationProperties.NameProperty, automationName);
            Assert.IsTrue(gridSplitterAutomationPeer.GetName().Contains(automationName), "Verify that the UIA name contains the customized AutomationProperties.Name of the GridSplitter.");
        });
    }

    [UIThreadTestMethod]
    public void PropertySizer_TestInitialBinding(PropertySizerTestInitialBinding testControl)
    {
        var propertySizer = testControl.FindDescendant<PropertySizer>();

        Assert.IsNotNull(propertySizer, "Could not find PropertySizer control.");

        // Set in XAML Page LINK: PropertySizerTestInitialBinding.xaml#L14
        Assert.AreEqual(300, propertySizer.Binding, "Property Sizer not at expected initial value.");
    }

    [UIThreadTestMethod]
    public void PropertySizer_TestChangeBinding(PropertySizerTestInitialBinding testControl)
    {
        var propertySizer = testControl.FindDescendant<PropertySizer>();
        var navigationView = testControl.FindDescendant<MUXC.NavigationView>();

        Assert.IsNotNull(propertySizer, "Could not find PropertySizer control.");
        Assert.IsNotNull(navigationView, "Could not find NavigationView control.");

        navigationView.OpenPaneLength = 200;

        // Set in XAML Page LINK: PropertySizerTestInitialBinding.xaml#L14
        Assert.AreEqual(200, propertySizer.Binding, "Property Sizer not at expected changed value.");
    }
}
