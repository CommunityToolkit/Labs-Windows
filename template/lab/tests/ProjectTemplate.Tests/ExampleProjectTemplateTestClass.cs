// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace ProjectTemplate.Tests
{
    [TestClass]
    public class ExampleProjectTemplateTestClass
    {
        [TestMethod]
        public void SimpleExampleTest()
        {
            var systemUnderTest = new CommunityToolkit.Labs.WinUI.ProjectTemplate();
            Assert.IsNotNull(systemUnderTest);
        }
    }
}
