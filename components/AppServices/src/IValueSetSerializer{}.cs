// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation.Collections;

namespace CommunityToolkit.AppServices;

/// <summary>
/// An interface for a custom serializer for parameters marshalled through <see cref="ValueSet"/> objects.
/// </summary>
/// <typeparam name="T">The type of values being serialized.</typeparam>
public interface IValueSetSerializer<T>
{
    /// <summary>
    /// Serializes an input <typeparamref name="T"/> value into a <see cref="ValueSet"/> object.
    /// </summary>
    /// <param name="value">The input <typeparamref name="T"/> object to serialize.</param>
    /// <returns>The <see cref="ValueSet"/> object representing <paramref name="value"/>.</returns>
    /// <exception cref="Exception">Thrown if serializing <paramref name="value"/> failed.</exception>
    [return: NotNullIfNotNull(nameof(value))]
    ValueSet? Serialize(T? value);

    /// <summary>
    /// Deserializes an input <see cref="ValueSet"/> object into a <typeparamref name="T"/> value.
    /// </summary>
    /// <param name="valueSet">The input <see cref="ValueSet"/> object to deserialize.</param>
    /// <returns>The deserialized <typeparamref name="T"/> value.</returns>
    /// <exception cref="Exception">Thrown if deserializing <paramref name="valueSet"/> failed.</exception>
    [return: NotNullIfNotNull(nameof(valueSet))]
    T? Deserialize(ValueSet? valueSet);
}
