// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.LabsUITestMethod;
using CommunityToolkit.Labs.Tests;
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

    [LabsUITestMethod]
    public void PropertySizer_TestInitialBinding(PropertySizerTestInitialBinding testControl)
    {
        var propertySizer = testControl.FindDescendant<PropertySizer>();

        Assert.IsNotNull(propertySizer, "Could not find PropertySizer control.");

        // Set in XAML Page LINK: PropertySizerTestInitialBinding.xaml#L14
        Assert.AreEqual(300, propertySizer.Binding, "Property Sizer not at expected initial value.");
    }

    [LabsUITestMethod]
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

    [LabsUITestMethod]
    public async Task InputInjection_TestClickButton(TouchInjectionTest testControl)
    {
        var button = testControl.FindDescendant<Button>();

        Assert.IsNotNull(button, "Could not find button control.");
        Assert.IsFalse(testControl.WasButtonClicked, "Initial state unexpected. Button shouldn't be clicked yet.");

        // Get location to button.
        var location = App.ContentRoot.CoordinatesTo(button); // TODO: Write a `CoordinatesToCenter` helper?

        SimulateInput.StartTouch();
        // Offset location slightly to ensure we're inside the button.
        var pointerId = SimulateInput.TouchDown(new Point(location.X + 25, location.Y + 25));
        await Task.Delay(50);
        SimulateInput.TouchUp(pointerId);

        // Ensure UI event is processed by our button
        await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

        // Optional delay for us to be able to see button content change before test shuts down.
        await Task.Delay(250);

        Assert.IsTrue(testControl.WasButtonClicked, "Button wasn't clicked.");

        SimulateInput.StopTouch();
    }
}
