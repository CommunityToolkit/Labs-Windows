// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

/// <summary>
/// Indicates the method of changing the visibility of UI elements.
/// </summary>
public enum VisualStateToggleMethod
{
    /// <summary>
    /// Change the visibility of UI elements by modifying the Visibility property.
    /// </summary>
    ByVisibility,

    /// <summary>
    /// Change the visibility of UI elements by modifying the IsVisible property of it's Visual.
    /// </summary>
    ByIsVisible
}
