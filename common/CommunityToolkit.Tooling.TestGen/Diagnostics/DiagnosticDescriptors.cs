// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Tooling.TestGen.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
public static class DiagnosticDescriptors
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating that a test method decorated with <see cref="UIThreadTestMethodAttribute"/> asks for a control instance with a non-parameterless constructor.
    /// <para>
    /// Format: <c>"Cannot generate test with type {0} as it has a constructor with parameters."</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor TestControlHasConstructorWithParameters = new(
        id: "LUITM0001",
        title: $"Provided control must not have a constructor with parameters.",
        messageFormat: $"Cannot generate test with control {{0}} as it has a constructor with parameters.",
        category: typeof(UIThreadTestMethodGenerator).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: $"Cannot generate test method with provided control.");
}
