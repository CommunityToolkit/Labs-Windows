// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

#pragma warning disable IDE0090 // Use 'new(...)' for field initializers, suppressed as it breaks a Roslyn analyzer

namespace CommunityToolkit.AppServices.SourceGenerators.Diagnostics;

/// <summary>
/// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
/// </summary>
internal static class DiagnosticDescriptors
{
    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an app services interface declares a member of an invalid type.
    /// <para>
    /// Format: <c>"Cannot declare member "{0}" in interface {1}, as it is not a valid member type for an [AppServices] interface type (only non-generic instance methods or static non-virtual DIMs are allowed)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAppServicesMemberType = new DiagnosticDescriptor(
        id: "APPSRVSPR0001",
        title: "Invalid [AppServices] interface member declaration",
        messageFormat: "Cannot declare member \"{0}\" in interface {1}, as it is not a valid member type for an [AppServices] interface type (only non-generic instance methods or static non-virtual DIMs are allowed)",
        category: typeof(InvalidAppServicesMemberAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Only non-generic instance methods or static non-virtual DIMs are allowed as members for interfaces annotated with the [AppServices] attribute.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an app services interface declares a method with an invalid return type.
    /// <para>
    /// Format: <c>"Method "{0}" in interface {1} has an invalid return type ({2}) for an [AppServices] method (only Task and Task&lt;T&gt; types, where T is a supported primitive or enum type, foundation type or SZ array with a supported element type, or has a custom serializer specified, are allowed)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAppServicesMethodReturnType = new DiagnosticDescriptor(
        id: "APPSRVSPR0002",
        title: "Invalid return type in [AppServices] interface method",
        messageFormat: "Method \"{0}\" in interface {1} has an invalid return type ({2}) for an [AppServices] method (only Task and Task<T> types, where T is a supported primitive or enum type, foundation type or SZ array with a supported element type, or has a custom serializer specified, are allowed)",
        category: typeof(InvalidAppServicesMemberAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Only Task and Task<T> with valid result types are allowed as return types for [AppServices] methods.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an app services interface declares a method with an invalid parameter type.
    /// <para>
    /// Format: <c>"Parameter "{0}" in method "{1}" in interface {2} has an invalid type ({3}) for an [AppServices] method (only supported primitive or enum types, foundation types and SZ arrays with a supported element type, IProgress&lt;T&gt;, CancellationToken or types with a custom serializer specified, are allowed)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidAppServicesMethodParameterType = new DiagnosticDescriptor(
        id: "APPSRVSPR0003",
        title: "Invalid parameter type in [AppServices] interface method",
        messageFormat: "Parameter \"{0}\" in method \"{1}\" in interface {2} has an invalid type ({3}) for an [AppServices] method (only supported primitive or enum types, foundation types and SZ arrays with a supported element type, IProgress<T>, CancellationToken or types with a custom serializer specified, are allowed)",
        category: typeof(InvalidAppServicesMemberAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Only supported primitive types, foundation types and SZ arrays with these types as element types, IProgress<T>, CancellationToken or types with a custom serializer specified, are allowed as parameter types for [AppServices] methods.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an app services interface declares a method that takes more than an IProgress&lt;T&gt; parameter.
    /// <para>
    /// Format: <c>"Parameter "{0}" in method "{1}" in interface {2} has an invalid type ({3}) for an [AppServices] method, as its containing method already has an IProgress&lt;T&gt; parameter (only one is allowed)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidRepeatedAppServicesMethodIProgressParameter = new DiagnosticDescriptor(
        id: "APPSRVSPR0004",
        title: "Invalid parameter type in [AppServices] interface method",
        messageFormat: "Parameter \"{0}\" in method \"{1}\" in interface {2} has an invalid type ({3}) for an [AppServices] method, as its containing method already has an IProgress<T> parameter (only one is allowed)",
        category: typeof(InvalidAppServicesMemberAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Only a single IProgress<T> parameter can be used in an [AppServices] method.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when an app services interface declares a method that takes more than a CancellationToken parameter.
    /// <para>
    /// Format: <c>"Parameter "{0}" in method "{1}" in interface {2} has an invalid type ({3}) for an [AppServices] method, as its containing method already has a CancellationToken parameter (only one is allowed)"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidRepeatedAppServicesMethodCancellationTokenParameter = new DiagnosticDescriptor(
        id: "APPSRVSPR0005",
        title: "Invalid parameter type in [AppServices] interface method",
        messageFormat: "Parameter \"{0}\" in method \"{1}\" in interface {2} has an invalid type ({3}) for an [AppServices] method, as its containing method already has a CancellationToken parameter (only one is allowed)",
        category: typeof(InvalidAppServicesMemberAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Only a single CancellationToken parameter can be used in an [AppServices] method.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a value set serializer type is invalid.
    /// <para>
    /// Format: <c>"Type {0} cannot be used as a custom ValueSet serializer type, as it lacks a public parameterless constructor"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidValueSetSerializerType = new DiagnosticDescriptor(
        id: "APPSRVSPR0006",
        title: "Invalid ValueSet serializer type",
        messageFormat: "Type {0} cannot be used as a custom serializer type, as it lacks a public parameterless constructor",
        category: typeof(InvalidValueSetSerializerUseAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Only types with a public parameterless constructor can be used as custom ValueSet serializer types.");

    /// <summary>
    /// Gets a <see cref="DiagnosticDescriptor"/> indicating when a value set serializer use is invalid.
    /// <para>
    /// Format: <c>"Method or parameter named "{0}" cannot request a custom ValueSet serializer, as this is only enabled for methods and parameters of methods in an [AppServices] interface"</c>.
    /// </para>
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidValueSetSerializerLocation = new DiagnosticDescriptor(
        id: "APPSRVSPR0007",
        title: "Invalid ValueSet serializer use",
        messageFormat: "Method or parameter named \"{0}\" cannot request a custom ValueSet serializer, as this is only enabled for methods and parameters of methods in an [AppServices] interface",
        category: typeof(InvalidValueSetSerializerUseAnalyzer).FullName,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Only methods and method parameters of methods in an [AppServices] interface should request a custom ValueSet serializer.");
}
