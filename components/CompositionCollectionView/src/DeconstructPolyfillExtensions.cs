// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace CommunityToolkit.Labs.WinUI;
internal static class DeconstructPolyfillExtensions
{
    public static void Deconstruct<T1, T2>(
        this KeyValuePair<T1, T2> pair,
        out T1 key,
        out T2 value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}
