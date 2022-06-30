// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ProjectTemplate.Tests;

[TestClass]
public class ExampleProjectTemplateTestClass
{
#if (!usenetstd)
    // TODO: https://github.com/CommunityToolkit/Labs-Windows/issues/160
    [TestMethod]
    public async Task SimpleExampleTest()
    {
        await CommunityToolkit.Labs.UnitTests.App.DispatcherQueue.EnqueueAsync(() =>
        {
            var systemUnderTest = new ProjectTemplate_ClassicBinding();
            Assert.IsNotNull(systemUnderTest);
        });
    }
#endif
#if (usenetstd)
    [TestMethod]
    public void SimpleExampleTest()
    {
        var systemUnderTest = new CommunityToolkit.Labs.WinUI.ProjectTemplate();
        Assert.IsNotNull(systemUnderTest);
    }
#endif
}
