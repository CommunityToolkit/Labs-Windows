// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Threading.Tasks;
using UnitTests.Extensions.Helpers;

#if !WINAPPSDK
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#else
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#endif

namespace NullableBoolMarkup.Tests
{
    [TestClass]
    public class Test_NullableBoolMarkupExtension

#if WINAPPSDK
        : VisualUITestBase
#endif
    {
        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_MarkupExtension_ProvidesTrue()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <CheckBox x:Name=""Check"" IsChecked=""{ui:NullableBool Value=True}""/>
</Page>") as FrameworkElement;

            var toggle = treeroot?.FindChild("Check") as CheckBox;

            Assert.IsNotNull(toggle, "Could not find checkbox control in tree.");

            Assert.AreEqual(true, toggle.IsChecked, "Expected checkbox value to be true.");
        }

        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_MarkupExtension_ProvidesFalse()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <CheckBox x:Name=""Check"" IsChecked=""{ui:NullableBool Value=False}""/>
</Page>") as FrameworkElement;

            var toggle = treeroot?.FindChild("Check") as CheckBox;

            Assert.IsNotNull(toggle, "Could not find checkbox control in tree.");

            Assert.AreEqual(false, toggle.IsChecked, "Expected checkbox value to be false.");
        }

        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_MarkupExtension_ProvidesNull()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <CheckBox x:Name=""Check"" IsChecked=""{ui:NullableBool IsNull=True}""/>
</Page>") as FrameworkElement;

            var toggle = treeroot?.FindChild("Check") as CheckBox;

            Assert.IsNotNull(toggle, "Could not find checkbox control in tree.");

            Assert.AreEqual(null, toggle.IsChecked, "Expected checkbox value to be null.");
        }

        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_MarkupExtension_ProvidesNullStillWithTrueValue()
        {
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <CheckBox x:Name=""Check"" IsChecked=""{ui:NullableBool IsNull=True, Value=True}""/>
</Page>") as FrameworkElement;

            var toggle = treeroot?.FindChild("Check") as CheckBox;

            Assert.IsNotNull(toggle, "Could not find checkbox control in tree.");

            Assert.AreEqual(null, toggle.IsChecked, "Expected checkbox value to be null.");
        }

        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_MarkupExtension_ProvidesNullStillWithFalseValue()
        {
            // Should be no-op as Value is false by default.
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI"">
        <CheckBox x:Name=""Check"" IsChecked=""{ui:NullableBool IsNull=True, Value=False}""/>
</Page>") as FrameworkElement;

            var toggle = treeroot?.FindChild("Check") as CheckBox;

            Assert.IsNotNull(toggle, "Could not find checkbox control in tree.");

            Assert.AreEqual(null, toggle.IsChecked, "Expected checkbox value to be null.");
        }

        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_Test_TestObject()
        {
            // Just test that we can properly parse our object as-is into our test harness.
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:helpers=""using:UnitTests.Extensions.Helpers"">
    <Page.Resources>
        <helpers:ObjectWithNullableBoolProperty x:Key=""OurObject""/>
    </Page.Resources>
</Page>") as FrameworkElement;

            var obj = treeroot?.Resources["OurObject"] as ObjectWithNullableBoolProperty;

            Assert.IsNotNull(obj, "Could not find object in resources.");

            Assert.AreEqual(null, obj.NullableBool, "Expected obj value to be null.");
        }

#region System-based Unit Tests, See Issue #3198
        [Ignore] // This test has trouble running on CI in release mode for some reason, we should re-enable when we test WinUI 3 Issue #3106
        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_DependencyProperty_SystemTrue()
        {
            // This is the failure case in the OS currently which causes us to need
            // this markup extension.
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:helpers=""using:UnitTests.Extensions.Helpers"">
    <Page.Resources>
        <helpers:ObjectWithNullableBoolProperty x:Key=""OurObject"" NullableBool=""True""/>
    </Page.Resources>
</Page>") as FrameworkElement;

            var obj = treeroot?.Resources["OurObject"] as ObjectWithNullableBoolProperty;

            Assert.IsNotNull(obj, "Could not find object in resources.");

            Assert.AreEqual(true, obj.NullableBool, "Expected obj value to be true.");
        }

        [Ignore] // This test has trouble running on CI in release mode for some reason, we should re-enable when we test WinUI 3 Issue #3106
        [TestCategory("NullableBoolMarkupExtension")]
        [UITestMethod]
        public void Test_NullableBool_DependencyProperty_SystemFalse()
        {
            // This is the failure case in the OS currently which causes us to need
            // this markup extension.
            var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:helpers=""using:UnitTests.Extensions.Helpers"">
    <Page.Resources>
        <helpers:ObjectWithNullableBoolProperty x:Key=""OurObject"" NullableBool=""False""/>
    </Page.Resources>
</Page>") as FrameworkElement;

            var obj = treeroot?.Resources["OurObject"] as ObjectWithNullableBoolProperty;

            Assert.IsNotNull(obj, "Could not find object in resources.");

            Assert.AreEqual(false, obj.NullableBool, "Expected obj value to be true.");
        }

        [TestCategory("NullableBoolMarkupExtension")]
        [TestMethod]
        public async Task Test_NullableBool_DependencyProperty_SystemNull()
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
            var ownbp = new ObjectWithNullableBoolProperty();

            Assert.IsNotNull(ownbp);

                // This is the failure case in the OS currently which causes us to need
                // this markup extension.
                var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:helpers=""using:UnitTests.Extensions.Helpers"">
    <Page.Resources>
        <helpers:ObjectWithNullableBoolProperty x:Key=""OurObject"" NullableBool=""{x:Null}""/>
    </Page.Resources>
</Page>") as FrameworkElement;

                Assert.IsTrue(treeroot?.Resources.Keys.Contains("OurObject"), "Expected resource could not be found."); // Check this as it gives a meaningful error message, while the following line is more likely to give a COMException than assign a null that will be caught in subsequent Asserts

                var obj = treeroot?.Resources["OurObject"] as ObjectWithNullableBoolProperty;

                Assert.IsNotNull(obj, "Could not find object in resources.");

                Assert.IsNull(obj.NullableBool, "Expected obj value to be null.");
            });
        }
#endregion

        [TestCategory("NullableBoolMarkupExtension")]
        [TestMethod]
        public async Task Test_NullableBool_DependencyProperty_TrueValue()
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                var treeroot = XamlReader.Load(@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:ui=""using:Microsoft.Toolkit.Uwp.UI""
    xmlns:helpers=""using:UnitTests.Extensions.Helpers"">
    <Page.Resources>
        <helpers:ObjectWithNullableBoolProperty x:Key=""OurObject"" NullableBool=""{ui:NullableBool Value=True}""/>
    </Page.Resources>
</Page>") as FrameworkElement;

            var obj = treeroot?.Resources["OurObject"] as ObjectWithNullableBoolProperty;

            Assert.IsNotNull(obj, "Could not find object in resources.");

            Assert.AreEqual(true, obj.NullableBool, "Expected obj value to be true.");
            });
        }
    }
}

namespace UnitTests.Extensions.Helpers
{
#if WINAPPSDK
    [Windows.UI.Xaml.Data.Bindable]
#endif
    public class ObjectWithNullableBoolProperty : DependencyObject
    {
        public bool? NullableBool
        {
            get { return (bool?)GetValue(NullableBoolProperty); }
            set { SetValue(NullableBoolProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NullableBool.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NullableBoolProperty =
            DependencyProperty.Register(nameof(NullableBool), typeof(bool?), typeof(ObjectWithNullableBoolProperty), new PropertyMetadata(null));
    }
}
