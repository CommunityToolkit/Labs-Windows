// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace CommunityToolkit.AppServices.Helpers;

/// <summary>
/// A base type for an app service host (sending requests to a component).
/// </summary>
internal static class ValueSetMarshaller
{
    /// <summary>
    /// Converts a <typeparamref name="T"/> value into an <see cref="object"/>.
    /// </summary>
    /// <typeparam name="T">The type of input value to convert.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value to convert.</param>
    /// <returns>An <see cref="object"/> conversion of <paramref name="value"/> compatible with <see cref="Windows.Foundation.Collections.ValueSet"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/> is not a valid <see cref="Enum"/> type.</exception>
    [return: NotNullIfNotNull(nameof(value))]
    public static object? ToObject<T>(T value)
    {
        if (typeof(T).IsEnum)
        {
            Type underlyingType = typeof(T).GetEnumUnderlyingType();

            if (underlyingType == typeof(byte))
            {
                return (byte)(object)value!;
            }

            if (underlyingType == typeof(sbyte))
            {
                return (sbyte)(object)value!;
            }

            if (underlyingType == typeof(short))
            {
                return (short)(object)value!;
            }

            if (underlyingType == typeof(ushort))
            {
                return (ushort)(object)value!;
            }

            if (underlyingType == typeof(int))
            {
                return (int)(object)value!;
            }

            if (underlyingType == typeof(uint))
            {
                return (uint)(object)value!;
            }

            if (underlyingType == typeof(long))
            {
                return (long)(object)value!;
            }

            if (underlyingType == typeof(ulong))
            {
                return (ulong)(object)value!;
            }

            ThrowArgumentExceptionForInvalidEnumType();
        }

        return value;
    }

    /// <summary>
    /// Converts an <see cref="object"/> instance into a <typeparamref name="T"/> value.
    /// </summary>
    /// <typeparam name="T">The type of value to convert.</typeparam>
    /// <param name="data">The input <see cref="object"/> instance to convert.</param>
    /// <returns>The resulting converted <typeparamref name="T"/> value.</returns>
    /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/> is not a valid type.</exception>
    public static T ToValue<T>(object data)
    {
        if (!TryGetValue(data, out T? result))
        {
            ThrowArgumentExceptionForInvalidDataType();
        }

        return result;
    }

    /// <summary>
    /// Tries to convert an <see cref="object"/> instance into a <typeparamref name="T"/> value.
    /// </summary>
    /// <typeparam name="T">The type of value to convert.</typeparam>
    /// <param name="data">The input <see cref="object"/> instance to convert.</param>
    /// <param name="value">The resulting <typeparamref name="T"/> value, if the conversion was successful.</param>
    /// <returns>Whether the conversion was successful..</returns>
    /// <exception cref="ArgumentException">Thrown if <typeparamref name="T"/> is not a valid <see cref="Enum"/> type.</exception>
    public static bool TryGetValue<T>(object data, [NotNullWhen(true)] out T? value)
    {
        if (typeof(T).IsEnum)
        {
            Type underlyingType = typeof(T).GetEnumUnderlyingType();

            if (underlyingType == typeof(byte))
            {
                value = (T)(object)(byte)data;

                return true;
            }

            if (underlyingType == typeof(sbyte))
            {
                value = (T)(object)(sbyte)data;

                return true;
            }

            if (underlyingType == typeof(short))
            {
                value = (T)(object)(short)data;

                return true;
            }

            if (underlyingType == typeof(ushort))
            {
                value = (T)(object)(ushort)data;

                return true;
            }

            if (underlyingType == typeof(int))
            {
                value = (T)(object)(int)data;

                return true;
            }

            if (underlyingType == typeof(uint))
            {
                value = (T)(object)(uint)data;

                return true;
            }

            if (underlyingType == typeof(long))
            {
                value = (T)(object)(long)data;

                return true;
            }

            if (underlyingType == typeof(ulong))
            {
                value = (T)(object)(ulong)data;

                return true;
            }

            ThrowArgumentExceptionForInvalidEnumType();
        }

        if (data is T matchingValue)
        {
            value = matchingValue;

            return true;
        }

        value = default;

        return false;
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> for an enum with an invalid type.
    /// </summary>
    [DoesNotReturn]
    private static void ThrowArgumentExceptionForInvalidEnumType()
    {
        throw new ArgumentException("The input enum type is not valid.");
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> for input data with an invalid value.
    /// </summary>
    [DoesNotReturn]
    private static void ThrowArgumentExceptionForInvalidDataType()
    {
        throw new ArgumentException("The input data is not valid.");
    }
}