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

#if !WINAPPSDK
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
#else
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
#endif

namespace SizerBase.Tests;

[TestClass]
public class ExampleSizerBaseTestClass : VisualUITestBase
{
    [TestMethod]
    public void Just_an_example_test()
    {
        Assert.AreEqual(1, 1);
    }

    [TestMethod]
    public async Task ShouldConfigureGridSplitterAutomationPeer()
    {
        await App.DispatcherQueue.EnqueueAsync(() =>
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
}
