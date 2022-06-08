// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Labs.UnitTests;

/// <summary>
/// Attribute to add to a <see cref="TestMethod"/> implementation in order to load a XAML based page to use within that test. Class with containing method needs to inherit from <see cref="VisualUITestBase"/> for functionality to work. Requires <see cref="PageType"/> to be set to function.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class TestPageAttribute : Attribute
{
    public TestPageAttribute(Type pageType)
    {
        if (pageType == null)
        {
            throw new ArgumentException($"'{nameof(pageType)}' cannot be null", nameof(pageType));
        }

        PageType = pageType;
    }

    public Type PageType { get; private set; }
}
