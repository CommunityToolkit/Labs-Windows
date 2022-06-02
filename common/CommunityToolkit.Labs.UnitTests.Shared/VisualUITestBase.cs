// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Reflection;

#if !WINAPPSDK
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Windows.UI.Xaml;
#else
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Helpers;
using Microsoft.UI.Xaml;
#endif

namespace CommunityToolkit.Labs.UnitTests;

/// <summary>
/// Base class to be used in API tests which require UI layout or rendering to occur first.
/// For more E2E scenarios or testing components for user interation, see integration test suite instead.
/// Use this class when an API needs direct access to test functions of the UI itself in more simplistic scenarios (i.e. visual tree helpers).
/// </summary>
public class VisualUITestBase
{
    public TestContext? TestContext { get; set; }

    public FrameworkElement? TestPage { get; private set; }

    [TestInitialize]
    public async Task TestInitialize()
    {
        if (TestContext != null)
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                TestPage = GetPageForTest(TestContext);

                if (TestPage != null)
                {
                    Task result = SetTestContentAsync(TestPage);

                    await result;

                    if (!result.IsCompletedSuccessfully)
                    {
                        throw new Exception($"Failed to load page for {TestContext.TestName} with Exception: {result.Exception?.Message}", result.Exception);
                    }
                }
            });
        }
    }

    /// <summary>
    /// Sets the content of the test app to a simple <see cref="FrameworkElement"/> to load into the visual tree.
    /// Waits for that element to be loaded and rendered before returning.
    /// </summary>
    /// <param name="content">Content to set in test app.</param>
    /// <returns>When UI is loaded.</returns>
    protected Task SetTestContentAsync(FrameworkElement content)
    {
        return App.DispatcherQueue.EnqueueAsync(() =>
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            async void Callback(object sender, RoutedEventArgs args)
            {
                content.Loaded -= Callback;

                // Wait for first Render pass
                await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

                taskCompletionSource.SetResult(true);
            }

            // Going to wait for our original content to unload
            content.Loaded += Callback;

            // Trigger that now
            try
            {
                App.ContentRoot = content;
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }

            return taskCompletionSource.Task;
        });
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        await App.DispatcherQueue.EnqueueAsync(() =>
        {
            // If we didn't set our content we don't have to do anything but complete here.
            if (App.ContentRoot is null)
            {
                taskCompletionSource.SetResult(true);
                return;
            }

            // Going to wait for our original content to unload
            App.ContentRoot.Unloaded += (_, _) => taskCompletionSource.SetResult(true);

            TestPage = null;

            // Trigger that now
            App.ContentRoot = null;
        });

        await taskCompletionSource.Task;
    }

    private static FrameworkElement? GetPageForTest(TestContext testContext)
    {
        var testName = testContext.TestName;
        var theClassName = testContext.FullyQualifiedTestClassName;

        var testClassString = $"test class \"{theClassName}\"";
        if (Type.GetType(theClassName) is not Type type)
        {
            throw new Exception($"Could not find {testClassString}.");
        }

        Log.Comment($"Found {testClassString}.");

        var testMethodString = $"test method \"{testName}\" in {testClassString}";
        if (type.GetMethod(testName) is not MethodInfo method)
        {
            throw new Exception($"Could not find {testMethodString}.");
        }

        Log.Comment($"Found {testMethodString}.");

        var testpageAttributeString = $"\"{typeof(TestPageAttribute)}\" on {testMethodString}";
        if (method.GetCustomAttribute(typeof(TestPageAttribute), true) is not TestPageAttribute attribute)
        {
            // If we don't have an attribute, we'll return null here to indicate such.
            // Otherwise, we'll be throwing an exception on failure anyway below.
            return null;
        }

        if (attribute.PageType is null)
        {
            throw new Exception($"{testpageAttributeString} requires `PageType` to be set.");
        }

        var obj = Activator.CreateInstance(attribute.PageType);

        if (obj is null)
        {
            throw new Exception($"Could not instantiate page of type {attribute.PageType.FullName} for {testpageAttributeString}.");
        }

        if (obj is FrameworkElement element)
        {
            return element;
        }

        throw new Exception($"{attribute.PageType.FullName} is required to inherit from `FrameworkElement` for the {testpageAttributeString}.");
    }
}
