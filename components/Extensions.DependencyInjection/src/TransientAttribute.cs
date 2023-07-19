// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
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
    /// <param name="implementationType">The implementation type for the service.</param>
    /// <param name="serviceTypes">The service types to register for the provided implementation.</param>
    public TransientAttribute(Type implementationType, params Type[] serviceTypes)
    {
        ImplementationType = implementationType;
        ServiceTypes = serviceTypes;
    }

    /// <summary>
    /// Gets the implementation type for the service to register.
    /// </summary>
    public Type ImplementationType { get; }

    /// <summary>
    /// Gets the supported service types for the implementation being registered.
    /// </summary>
    public Type[] ServiceTypes { get; }
}
