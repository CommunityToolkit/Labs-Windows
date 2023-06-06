// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.AppServices;

/// <summary>
/// An attribute that can be used to annotate an interface to generate app service connection points.
/// </summary>
[AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public sealed class AppServiceAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="AppServiceAttribute"/> instance with the specified parameters.
    /// </summary>
    /// <param name="appServiceName">The name of the app service.</param>
    public AppServiceAttribute(string appServiceName)
    {
        AppServiceName = appServiceName;
    }

    /// <summary>
    /// Gets the name of the app service.
    /// </summary>
    public string AppServiceName { get; }
}
