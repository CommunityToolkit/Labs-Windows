// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !WINAPPSDK
using Windows.UI.Xaml;
#else
using Microsoft.UI.Xaml;
#endif

namespace CommunityToolkit.Labs.WinUI.ProjectTemplateRns;

/// <summary>
/// Backing code for this resource dictionary.
/// </summary>
public sealed partial class TemplatedControlStyle : ResourceDictionary
{
    // NOTICE
    // This file only exists to enable x:Bind in the resource dictionary.
    // Do not add code here.
    // Instead, add code-behind to your templated control.
    public TemplatedControlStyle()
    {
        this.InitializeComponent();
    }
}
