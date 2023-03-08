// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Tooling.TestGen;

/// <summary>
/// Generates a test method that auto-dispatches method contents to the UI thread,
/// and provides an instance of a control as a parameter if present in the method signature.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class UIThreadTestMethodAttribute : Attribute
{
}
