// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.AppServices.SourceGenerators.Helpers;

namespace CommunityToolkit.AppServices.SourceGenerators.Models;

/// <summary>
/// A model with gathered info on a given app service (either host or component).
/// </summary>
/// <param name="Methods">The methods in this app service.</param>
/// <param name="AppServiceName">The name of the app service.</param>
/// <param name="InterfaceFullyQualifiedName">The fully qualified name of the AppService interface.</param>
internal sealed record AppServiceInfo(EquatableArray<MethodInfo> Methods, string AppServiceName, string InterfaceFullyQualifiedName);