// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.AppServices;

/// <summary>
/// An attribute that can be used to annotate an interface to generate app service connection points.
/// </summary>
[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
public sealed class ValueSetSerializerAttribute : Attribute
{
    /// <summary>
    /// Creates a new <see cref="ValueSetSerializerAttribute"/> instance with the specified parameters.
    /// </summary>
    /// <param name="valueSetSerializerType">The type of <see cref="IValueSetSerializer{T}"/> to use.</param>
    public ValueSetSerializerAttribute(Type valueSetSerializerType)
    {
        ValueSetSerializerType = valueSetSerializerType;
    }

    /// <summary>
    /// Gets the type of <see cref="IValueSetSerializer{T}"/> serializer to use.
    /// </summary>
    public Type ValueSetSerializerType { get; }
}
