// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Helpers;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Models;

/// <summary>
/// A model for a method producing a service provider.
/// </summary>
/// <param name="Hierarchy">The <see cref="HierarchyInfo"/> instance for the containing type for the method.</param>
/// <param name="MethodName">The method name.</param>
/// <param name="ServiceCollectionParameterName">The name of the service collection parameter.</param>
/// <param name="ReturnsVoid">Whether the method returns <see cref="void"/>.</param>
/// <param name="Modifiers">The method modifiers.</param>
internal sealed record ServiceProviderMethodInfo(
    HierarchyInfo Hierarchy,
    string MethodName,
    string ServiceCollectionParameterName,
    bool ReturnsVoid,
    EquatableArray<ushort> Modifiers);
