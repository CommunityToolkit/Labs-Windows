// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Extensions.DependencyInjection;

/// <summary>
/// <para>
/// An attribute that can be used to instruct the generator to add a transient service to the input <see cref="IServiceCollection"/> instance.
/// </para>
/// <para>
/// This attribute can be used in the same way as <see cref="SingletonAttribute"/>, the only difference being that it will register transient services.
/// A method can be annotated with any combination of <see cref="SingletonAttribute"/> and <see cref="TransientAttribute"/>.
/// </para>
/// </summary>
/// <remarks>For more info, see <seealso cref="SingletonAttribute"/>.</remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
[Conditional("SERVICES_CONFIGURATION_METADATA")]
public sealed class TransientAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="TransientAttribute"/> instance with the specified parameters.
    /// </summary>
    /// <param name="serviceType">The service type to register (must be a concrete service type).</param>
    public TransientAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }

    /// <summary>
    /// Creates a new <see cref="TransientAttribute"/> instance with the specified parameters.
    /// </summary>
    /// <param name="serviceType">The service type to register for the provided implementation.</param>
    /// <param name="implementationType">The implementation type for the service.</param>
    public TransientAttribute(Type serviceType, Type implementationType)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
    }

    /// <summary>
    /// Gets the service type for the current service registration.
    /// </summary>
    public Type ServiceType { get; }

    /// <summary>
    /// Gets the optional implementation type for the service to register (if it's not the same as <see cref="ServiceType"/>).
    /// </summary>
    public Type? ImplementationType { get; }

    /// <summary>
    /// Gets the additional supported service types for the implementation or service being registered.
    /// </summary>
    [DisallowNull]
    public Type[]? AdditionalServiceTypes { get; init; }
}
