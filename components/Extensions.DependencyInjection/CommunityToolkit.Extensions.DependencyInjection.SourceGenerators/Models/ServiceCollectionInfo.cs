// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Helpers;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Models;

/// <summary>
/// A model for a service collection method.
/// </summary>
/// <param name="Method">The <see cref="ServiceProviderMethodInfo"/> instance for the method.</param>
/// <param name="Services">The sequence of <see cref="RegisteredServiceInfo"/> instances for services to register.</param>
internal sealed record ServiceCollectionInfo(ServiceProviderMethodInfo Method, EquatableArray<RegisteredServiceInfo> Services);
