// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    protected Task EnqueueAsync<T>(Func<Task<T>> function)
    {
        var taskCompletionSource = new TaskCompletionSource<T>();

        var addedToQueue = App.DispatcherQueue.TryEnqueue(async () =>
        {
            var res = await function();
            taskCompletionSource?.TrySetResult(res);
        });

        Assert.IsTrue(addedToQueue);

        return taskCompletionSource.Task;
    }

    protected Task EnqueueAsync(Func<Task> function)
    {
        var taskCompletionSource = new TaskCompletionSource<object?>();

        var addedToQueue = App.DispatcherQueue.TryEnqueue(async () =>
        {
            await function();
            taskCompletionSource?.TrySetResult(null);
        });

        Assert.IsTrue(addedToQueue);

        return taskCompletionSource.Task;
    }

    protected Task EnqueueAsync(Action function)
    {
        var taskCompletionSource = new TaskCompletionSource<object?>();

        var addedToQueue = App.DispatcherQueue.TryEnqueue(() =>
        {
            function();
            taskCompletionSource?.TrySetResult(null);
        });

        Assert.IsTrue(addedToQueue);

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Sets the content of the test app to a simple <see cref="FrameworkElement"/> to load into the visual tree.
    /// Waits for that element to be loaded and rendered before returning.
    /// </summary>
    /// <param name="content">Content to set in test app.</param>
    /// <returns>When UI is loaded.</returns>
    protected async Task LoadTestContentAsync(FrameworkElement content)
    {
        var taskCompletionSource = new TaskCompletionSource<object?>();

        content.Loaded += OnLoaded;
        App.ContentRoot = content;

        await taskCompletionSource.Task;
        Assert.IsTrue(content.IsLoaded);

        async void OnLoaded(object sender, RoutedEventArgs args)
        {
            content.Loaded -= OnLoaded;

            // Wait for first Render pass
            await CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() => { });

            taskCompletionSource.SetResult(null);
        }
    }

    public async Task UnloadTestContentAsync(FrameworkElement element)
    {
        var taskCompletionSource = new TaskCompletionSource<object?>();

        element.Unloaded += OnUnloaded;

        App.ContentRoot = null;

        await taskCompletionSource.Task;
        Assert.IsFalse(element.IsLoaded);

        void OnUnloaded(object sender, RoutedEventArgs args)
        {
            element.Unloaded -= OnUnloaded;
            taskCompletionSource.SetResult(null);
        }
    }
}
