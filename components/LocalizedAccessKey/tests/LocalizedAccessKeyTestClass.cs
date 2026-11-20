// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tests;
using CommunityToolkit.Tooling.TestGen;

namespace LocalizedAccessKeyTests;

[TestClass]
public partial class LocalizedAccessKeyTestClass : VisualUITestBase
{
    [TestCategory("LocalizedAccessKey")]
    [UIThreadTestMethod]
    public void LocalizedAccessKey_SetAccessKey(LocalizedAccessKeyTestPage page)
    {
        var button = page.FindDescendant<Button>();

        Assert.IsNotNull(button, "Could not find button.");
        Assert.AreEqual("A", button.AccessKey, "AccessKey of button does not match expected value.");
    }
}
