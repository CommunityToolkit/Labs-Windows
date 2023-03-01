// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public static class DisposableExtensions
{
    // this offers a quick way to assign a disposable to a container 
    // you can do "myClass.DisposeWith(container)" 
    public static T DisposeWith<T>(this T disposable, CompositeDisposable container) where T : IDisposable
    {
        if (container != null && disposable != null)
        {
            container.Add(disposable);
        }

#pragma warning disable CS8603 // Possible null reference return.
        return disposable;
#pragma warning restore CS8603 // Possible null reference return.
    }

    // this offers a quick way to chain a collection of disposables 
    // you can do "container.Include(obj1).Include(obj2)..." 
    public static CompositeDisposable Include<T>(this CompositeDisposable container, T disposable) where T : IDisposable
    {
        if (container != null && disposable != null)
        {
            container.Add(disposable);
        }

#pragma warning disable CS8603 // Possible null reference return.
        return container;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
