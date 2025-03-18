// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Helpers;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Models;

/// <summary>
/// A model for a singleton service registration.
/// </summary>
/// <param name="RegistrationKind">The registration kind for the service.</param>
/// <param name="ImplementationTypeName">The type name of the implementation type.</param>
/// <param name="ImplementationFullyQualifiedTypeName">The fully qualified type name of the implementation type.</param>
/// <param name="RequiredServiceFullyQualifiedTypeNames">The fully qualified type names of dependent services for <paramref name="ImplementationFullyQualifiedTypeName"/>.</param>
/// <param name="ServiceFullyQualifiedTypeNames">The fully qualified type names for the services to register for <paramref name="ImplementationFullyQualifiedTypeName"/>.</param>
internal sealed record RegisteredServiceInfo(
    ServiceRegistrationKind RegistrationKind,
    string ImplementationTypeName,
    string ImplementationFullyQualifiedTypeName,
    EquatableArray<string> RequiredServiceFullyQualifiedTypeNames,
    EquatableArray<string> ServiceFullyQualifiedTypeNames);
