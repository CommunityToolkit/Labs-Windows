// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Labs.UnitTests;
using CommunityToolkit.Labs.WinUI;
using CommunityToolkit.Labs.WinUI.Automation.Peers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Threading.Tasks;
using CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

#if !WINAPPSDK
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using MUXC = Microsoft.UI.Xaml.Controls;
#else
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using MUXC = Microsoft.UI.Xaml.Controls;
#endif

namespace SizerBase.Tests;

[TestClass]
public partial class ExampleSizerBaseTestClass : VisualUITestBase
{

    [TestMethod]
    public void Just_an_example_test()
    {
        Assert.AreEqual(1, 1);
    }

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

    [UIControlTestMethod(typeof(PropertySizerTestInitialBinding))]
    public void PropertySizer_TestInitialBinding()
    {
        // TestPage shouldn't be null here, but we'll do the safer ?. to be sure.
        var propertySizer = TestPage?.FindDescendant<PropertySizer>();

        Assert.IsNotNull(propertySizer, "Could not find PropertySizer control.");

        // Set in XAML Page LINK: PropertySizerTestInitialBinding.xaml#L14
        Assert.AreEqual(300, propertySizer.Binding, "Property Sizer not at expected initial value.");
    }

    [UIControlTestMethod(typeof(PropertySizerTestInitialBinding))]
    public void PropertySizer_TestChangeBinding()
    {
        // TestPage shouldn't be null here, but we'll do the safer ?. to be sure.
        var propertySizer = TestPage?.FindDescendant<PropertySizer>();
        var navigationView = TestPage?.FindDescendant<MUXC.NavigationView>();

        Assert.IsNotNull(propertySizer, "Could not find PropertySizer control.");
        Assert.IsNotNull(navigationView, "Could not find NavigationView control.");

        navigationView.OpenPaneLength = 200;

        // Set in XAML Page LINK: PropertySizerTestInitialBinding.xaml#L14
        Assert.AreEqual(200, propertySizer.Binding, "Property Sizer not at expected changed value.");
    }
}
