// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Diagnostics;

/// <summary>
/// A container for all <see cref="SuppressionDescriptors"/> instances for suppressed diagnostics by analyzers in this project.
/// </summary>
internal static class SuppressionDescriptors
{
    /// <summary>
    /// Gets a <see cref="SuppressionDescriptor"/> for a property using [GeneratedDependencyProperty] with an attribute list targeting the 'static' keyword.
    /// </summary>
    public static readonly SuppressionDescriptor StaticPropertyAttributeListForGeneratedDependencyPropertyDeclaration = new(
        id: "WCTDPSPR0001",
        suppressedDiagnosticId: "CS0658",
        justification: "Properties using [GeneratedDependencyProperty] can use [static:] attribute lists to forward attributes to the generated dependency property fields.");
}
