// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.AppServices.SourceGenerators.Models;

/// <summary>
/// Indicates a type of parameter or return value that is supported for an app service method signature.
/// </summary>
/// <remarks>The allowed element types are taken from <see href="https://learn.microsoft.com/windows/win32/api/windows.foundation/ne-windows-foundation-propertytype"/>.</remarks>
[Flags]
internal enum ParameterOrReturnType
{
    /// <summary>
    /// A <see cref="byte"/> value.
    /// </summary>
    UInt8 = 0x1 << 0,

    /// <summary>
    /// A <see cref="short"/> value.
    /// </summary>
    Int16 = 0x1 << 1,

    /// <summary>
    /// An <see cref="ushort"/> value.
    /// </summary>
    UInt16 = 0x1 << 2,

    /// <summary>
    /// An <see cref="int"/> value.
    /// </summary>
    Int32 = 0x1 << 3,

    /// <summary>
    /// A <see cref="uint"/> value.
    /// </summary>
    UInt32 = 0x1 << 4,

    /// <summary>
    /// A <see cref="long"/> value.
    /// </summary>
    Int64 = 0x1 << 5,

    /// <summary>
    /// A <see cref="ulong"/> value.
    /// </summary>
    UInt64 = 0x1 << 6,

    /// <summary>
    /// A <see cref="float"/> value.
    /// </summary>
    Single = 0x1 << 7,

    /// <summary>
    /// A <see cref="double"/> value.
    /// </summary>
    Double = 0x1 << 8,

    /// <summary>
    /// A <see cref="char"/> value.
    /// </summary>
    Char16 = 0x1 << 9,

    /// <summary>
    /// A <see cref="bool"/> value.
    /// </summary>
    Boolean = 0x1 << 10,

    /// <summary>
    /// A <see cref="string"/> value.
    /// </summary>
    String = 0x1 << 11,

    /// <summary>
    /// A <see cref="DateTime"/> value.
    /// </summary>
    DateTime = 0x1 << 12,

    /// <summary>
    /// A <see cref="TimeSpan"/> value.
    /// </summary>
    TimeSpan = 0x1 << 13,

    /// <summary>
    /// A <see cref="Guid"/> value.
    /// </summary>
    Guid = 0x1 << 14,

    /// <summary>
    /// A <see href="https://learn.microsoft.com/uwp/api/windows.foundation.point"><c>Windows.Foundation.Point</c></see> value.
    /// </summary>
    Point = 0x1 << 15,

    /// <summary>
    /// A <see href="https://learn.microsoft.com/uwp/api/windows.foundation.size"><c>Windows.Foundation.Size</c></see> value.
    /// </summary>
    Size = 0x1 << 16,

    /// <summary>
    /// A <see href="https://learn.microsoft.com/uwp/api/windows.foundation.rect"><c>Windows.Foundation.Rect</c></see> value.
    /// </summary>
    Rect = 0x1 << 17,

    /// <summary>
    /// An <see cref="System.Enum"/> type.
    /// </summary>
    Enum = 0x1 << 18,

    /// <summary>
    /// Gets a mask for all the existing element types.
    /// </summary>
    ElementTypeMask = UInt8 | Int16 | UInt16 | Int32 | UInt32 | Int64 | UInt64 | Single | Double | Char16 | Boolean | String | DateTime | TimeSpan | Guid | Point | Size | Rect | Enum,

    /// <summary>
    /// An array type (when this flag is set, one of the flags above this must also be set).
    /// </summary>
    Array = 0x1 << 19,

    /// <summary>
    /// A <see cref="System.Threading.Tasks.Task"/> value (only valid for returns).
    /// </summary>
    Task = 0x1 << 20,

    /// <summary>
    /// A <see cref="System.Threading.Tasks.Task{TResult}"/> value (only valid for returns).
    /// </summary>
    TaskOfT = 0x1 << 21,

    /// <summary>
    /// An <see cref="IProgress{T}"/> value (only valid for parameters, and must have a value flag set as well).
    /// </summary>
    IProgressOfT = 0x1 << 22,

    /// <summary>
    /// A <see cref="System.Threading.CancellationToken"/> value (only valid for parameters).
    /// </summary>
    CancellationToken = 0x1 << 23,

    /// <summary>
    /// A type that relies on a custom serializer in order to be marshalled across processes.
    /// </summary>
    /// <remarks>
    /// This is applied as follows:
    /// <list type="bullet">
    ///   <item>If the type is a parameter of <see cref="IProgress{T}"/> type, the serializer applies to the inner progress values.</item>
    ///   <item>If the type is a parameter in all other cases, the serializer applies to the whole type (eg. if the type is an array, the serializer must be for <c>T[]</c> values, not <c>T</c> values.</item>
    ///   <item>If the type is in a return, the serializer applies to the type argument of the returned <see cref="System.Threading.Tasks.Task{T}"/> value.</item>
    /// </list>
    /// </remarks>
    CustomSerializerType = 0x1 << 24
}