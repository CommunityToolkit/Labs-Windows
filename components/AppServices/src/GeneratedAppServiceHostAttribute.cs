// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.AppServices;

/// <summary>
/// An attribute that can be used to request the generator to emit a host implementation of a given app service.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class GeneratedAppServiceHostAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="GeneratedAppServiceHostAttribute"/> instance with the specified parameters.
    /// </summary>
    /// <param name="appServiceType">The type of the app service.</param>
    public GeneratedAppServiceHostAttribute(Type appServiceType)
    {
        AppServiceType = appServiceType;
    }

    /// <summary>
    /// Gets the type of the app service.
    /// </summary>
    public Type AppServiceType { get; }
}
