// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace CommunityToolkit.Extensions.DependencyInjection;

/// <summary>
/// <para>
/// An attribute that can be used to instruct the generator to add a singleton service to the target <see cref="IServiceCollection"/> instance.
/// </para>
/// <para>
/// This attribute should be added to a <see langword="partial"/> method receiving an <see cref="IServiceCollection"/>
/// instance, and the generator will register all requested services (optionally also returning the input object).
/// </para>
/// <para>
/// That is, given a declaration as follows:
/// <code>
/// [Singleton(typeof(MyServiceA), typeof(IMyServiceA))]
/// [Singleton(typeof(MyServiceB), typeof(IMyServiceB))]
/// [Singleton(typeof(MyServiceC), typeof(IMyServiceC))]
/// private static partial void ConfigureServices(IServiceCollection services);
/// </code>
/// The generator will produce code as follows:
/// <code>
/// private static partial void ConfigureServices(IServiceCollection services)
/// {
///     services.AddSingleton(typeof(IMyServiceA), static services => new MyServiceA());
///     services.AddSingleton(typeof(IMyServiceB), static services => new MyServiceB(
///         services.GetRequiredServices&lt;IMyServiceA&gt;()));
///     services.AddSingleton(typeof(IMyServiceC), static services => new MyServiceC(
///         services.GetRequiredServices&lt;IMyServiceA&gt;(),
///         services.GetRequiredServices&lt;IMyServiceB&gt;()));
/// }
/// </code>
/// </para>
/// </summary>
/// <remarks>
/// This attribute is conditional for two reasons:
/// <list type="bullet">
///   <item>
///     Since the attributes are only used for source generation and there can be a large number of them, this
///     reduces the metadata impact on the final assemblies. If needed, the directive can be manually defined.
///   </item>
///   <item>
///     The attributes have a constructor parameter of an array type, which is not allowed in WinRT assemblies.
///     Making the attributes conditional makes Roslyn skip emitting them, which avoids WinMDExp generating an
///     invalid PE file and then causing projects referencing it to fail to build. For more info on the WinMDExp
///     issue, see <see href="https://developercommunity.visualstudio.com/t/MSBuild:-OutOfMemoryException:-Task-Gen/10270567?"/>.
///   </item>
/// </list>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
[Conditional("SERVICES_CONFIGURATION_METADATA")]
public sealed class SingletonAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="SingletonAttribute"/> instance with the specified parameters.
    /// </summary>
    /// <param name="implementationType">The implementation type for the service.</param>
    /// <param name="serviceTypes">The service types to register for the provided implementation.</param>
    public SingletonAttribute(Type implementationType, params Type[] serviceTypes)
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
