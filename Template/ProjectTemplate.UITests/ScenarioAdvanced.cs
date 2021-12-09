// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CommunityToolkit.Labs.Uwp.ProjectTemplate.UITests
{
    [TestClass]
    public class ScenarioAdvanced : UITestSession
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            TearDown();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Navigate to advanced sample tab
            session.FindElementByAccessibilityId("AdvancedSamplePivotItem").Click();
        }

        [TestMethod]
        public void ButtonClick()
        {
            var counterButton = session.FindElementByAccessibilityId("CounterButton");
            var countTextBlock = counterButton.FindElementByAccessibilityId("PART_CountTextBlock");

            // Check the initial Count and Step values
            var initialCount = Convert.ToInt32(countTextBlock.Text);

            // Click the button
            counterButton.Click();

            // Check that the Count value has incremented.
            var newCount = Convert.ToInt32(countTextBlock.Text);
            Assert.IsTrue(initialCount < newCount);

            // Check that the step is greater than 1.
            Assert.IsTrue(newCount - initialCount > 1);
        }
    }
}
