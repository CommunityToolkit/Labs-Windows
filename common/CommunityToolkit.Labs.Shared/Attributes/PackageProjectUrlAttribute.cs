// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Attributes;

[AttributeUsage(AttributeTargets.Assembly)]
public class PackageProjectUrlAttribute : Attribute
{
    public string PackageProjectUrl { get; set; }

    public PackageProjectUrlAttribute(string url)
    {
        PackageProjectUrl = url;
    }
}
