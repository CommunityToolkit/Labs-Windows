// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Models;

/// <summary>
/// Indicates the kind of service registration being used.
/// </summary>
internal enum ServiceRegistrationKind
{
    /// <summary>
    /// A singleton service.
    /// </summary>
    Singleton,

    /// <summary>
    /// A transient service.
    /// </summary>
    Transient
}
