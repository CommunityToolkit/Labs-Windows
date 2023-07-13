// Licensnsed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

#pragma warning disable IDE0090 // Use 'new(...)' for field initializers, suppressed as it breaks a Roslyn analyzer

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
internal static class DiagnosticDescriptors
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a registered service is using an invalid implementation type.
    /// <para>
    /// Format: <c>"Cannot register a service of implementation type {0}, as the type has to be a non static, non abstract class with a public constructor"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidRegistrationImplementationType = new DiagnosticDescriptor(
        id: "TKEXDI0001",
        title: "Invalid registration implementation type",
        messageFormat: "Cannot register a service of implementation type {0}, as the type has to be a non static, non abstract class with a public constructor",
        category: typeof(InvalidServiceRegistrationAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Registered service implementation types must be non static, non abstract classes with a public constructor.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a registered service is using an invalid service type.
    /// <para>
    /// Format: <c>"Cannot register a service of implementation type {0} with the type {1}, as there is no implicit type conversion between the two"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidRegistrationServiceType = new DiagnosticDescriptor(
        id: "TKEXDI0002",
        title: "Invalid registration service type",
        messageFormat: "Cannot register a service of implementation type {0} with the type {1}, as there is no implicit type conversion between the two",
        category: typeof(InvalidServiceRegistrationAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Registered service types must be implicitly convertible from their implementation type.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an implementation type is registered twice.
    /// <para>
    /// Format: <c>"The implementation type {0} has already been registered on the target service collection"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateImplementationTypeRegistration = new DiagnosticDescriptor(
        id: "TKEXDI0003",
        title: "Duplicate implementation type registration",
        messageFormat: "The implementation type {0} has already been registered on the target service collection",
        category: typeof(InvalidServiceRegistrationAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Each implementation type can only be registered once in a target service collection.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a service type is registered twice.
    /// <para>
    /// Format: <c>"The service type {0} has already been registered on the target service collection"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateServiceTypeRegistration = new DiagnosticDescriptor(
        id: "TKEXDI0004",
        title: "Duplicate service type registration",
        messageFormat: "The service type {0} has already been registered on the target service collection",
        category: typeof(InvalidServiceRegistrationAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Each service type should only be registered once in a target service collection.");
}
