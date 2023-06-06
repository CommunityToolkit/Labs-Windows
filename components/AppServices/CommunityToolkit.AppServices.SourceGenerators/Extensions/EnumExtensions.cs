// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.AppServices.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for <see cref="Enum"/> types.
/// </summary>
internal static class EnumExtensions
{
    /// <summary>
    /// Checks whether a given enum has a flag, without boxing.
    /// </summary>
    /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value.</param>
    /// <param name="flag">The <typeparamref name="T"/> flag.</param>
    /// <remarks>
    /// This is needed over <see cref="Enum.HasFlag(Enum)"/> because source generators run on a .NET Framework host,
    /// and that API is not a JIT intrinsic there, meaning it will box the input value every single time it is called.
    /// </remarks>
    public static unsafe bool HasFlag<T>(this T value, T flag)
        where T : unmanaged, Enum
    {
        if (sizeof(T) == sizeof(byte))
        {
            byte value8 = *(byte*)&value;
            byte flag8 = *(byte*)&flag;

            return (value8 & flag8) == flag8;
        }

        if (sizeof(T) == sizeof(ushort))
        {
            ushort value16 = *(ushort*)&value;
            ushort flag16 = *(ushort*)&flag;

            return (value16 & flag16) == flag16;
        }

        if (sizeof(T) == sizeof(uint))
        {
            uint value32 = *(uint*)&value;
            uint flag32 = *(uint*)&flag;

            return (value32 & flag32) == flag32;
        }

        if (sizeof(T) == sizeof(ulong))
        {
            ulong value64 = *(ulong*)&value;
            ulong flag64 = *(ulong*)&flag;

            return (value64 & flag64) == flag64;
        }

        throw new ArgumentException("Invalid enum type.");
    }

    /// <summary>
    /// Checks whether a given enum has any of the input flags, without boxing.
    /// </summary>
    /// <typeparam name="T">The <see cref="Enum"/> type.</typeparam>
    /// <param name="value">The input <typeparamref name="T"/> value.</param>
    /// <param name="flags">The <typeparamref name="T"/> flags.</param>
    public static unsafe bool HasAnyFlags<T>(this T value, T flags)
        where T : unmanaged, Enum
    {
        if (sizeof(T) == sizeof(byte))
        {
            byte value8 = *(byte*)&value;
            byte flags8 = *(byte*)&flags;

            return (value8 & flags8) != 0;
        }

        if (sizeof(T) == sizeof(ushort))
        {
            ushort value16 = *(ushort*)&value;
            ushort flags16 = *(ushort*)&flags;

            return (value16 & flags16) != 0;
        }

        if (sizeof(T) == sizeof(uint))
        {
            uint value32 = *(uint*)&value;
            uint flags32 = *(uint*)&flags;

            return (value32 & flags32) != 0;
        }

        if (sizeof(T) == sizeof(ulong))
        {
            ulong value64 = *(ulong*)&value;
            ulong flags64 = *(ulong*)&flags;

            return (value64 & flags64) != 0;
        }

        throw new ArgumentException("Invalid enum type.");
    }
}