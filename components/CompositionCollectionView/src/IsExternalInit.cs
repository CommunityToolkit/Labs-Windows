// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This class is required to make Record types work in .NET Framework and .NET Core 3.1.
    /// It's included in .NET 5. 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IsExternalInit
    {
    }
}
