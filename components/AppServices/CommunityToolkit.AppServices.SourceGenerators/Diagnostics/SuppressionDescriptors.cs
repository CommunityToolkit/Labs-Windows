// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.AppServices.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="SuppressionDescriptors"/> instances for suppressed diagnostics by analyzers in this project.
/// </summary>
internal static class SuppressionDescriptors
{
    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a synchronous AppService method using the async modifier.
    /// </summary>
    public static readonly SuppressionDescriptor SynchronousAppServiceMethod = new(
        id: "APPSRVSPR0001",
        suppressedDiagnosticId: "CS1998",
        justification: "All AppService methods must return a Task, but components implementing them might not need them to be asynchronous (but making them so simplifies the code and normalizes exceptions)");
}
