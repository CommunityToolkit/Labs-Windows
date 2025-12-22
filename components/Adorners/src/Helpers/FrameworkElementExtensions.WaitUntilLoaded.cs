// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Future;

/// <summary>
/// Helper extensions for <see cref="FrameworkElement"/>.
/// </summary>
public static partial class FrameworkElementExtensions
{
    /// <summary>
    /// A <see cref="FrameworkElement"/> extension which can be used in asynchronous scenarios to
    /// wait until an element has loaded before proceeding using a <see cref="TaskCompletionSource"/>
    /// that listens to the <see cref="FrameworkElement.Loaded"/> event. In the event the element
    /// is already loaded (<see cref="FrameworkElement.IsLoaded"/>), the method will return immediately.
    /// </summary>
    /// <param name="element">The element to await loading.</param>
    /// <param name="options"><see cref="TaskCreationOptions"/></param>
    /// <returns>True if the element is loaded.</returns>
    public static Task<bool> WaitUntilLoadedAsync(this FrameworkElement element, TaskCreationOptions? options = null)
    {
        if (element.IsLoaded && element.Parent != null)
        {
            return Task.FromResult(true);
        }

        var taskCompletionSource = options.HasValue ? new TaskCompletionSource<bool>(options.Value)
                : new TaskCompletionSource<bool>();
        try
        {
            void LoadedCallback(object sender, RoutedEventArgs args)
            {
                element.Loaded -= LoadedCallback;
                taskCompletionSource.SetResult(true);
            }

            element.Loaded += LoadedCallback;
        }
        catch (Exception e)
        {
            taskCompletionSource.SetException(e);
        }

        return taskCompletionSource.Task;
    }
}
