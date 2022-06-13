// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod;

/// <summary>
/// Generates a test method that provides an instance of the given FrameworkElement type
/// and auto-dispatches method contents to the UI thread.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class UIControlTestMethodAttribute : Attribute
{
    public UIControlTestMethodAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}
