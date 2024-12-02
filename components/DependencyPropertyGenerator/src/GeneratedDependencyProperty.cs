// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_UWP
using DependencyProperty = Windows.UI.Xaml.DependencyProperty;
#else
using DependencyProperty = Microsoft.UI.Xaml.DependencyProperty;
#endif

namespace CommunityToolkit.WinUI;

/// <summary>
/// Provides constant values that can be used as default values for <see cref="GeneratedDependencyPropertyAttribute"/>.
/// </summary>
public sealed class GeneratedDependencyProperty
{
    /// <summary><inheritdoc cref="DependencyProperty.UnsetValue"/></summary>
    /// <remarks>
    /// This constant is only meant to be used in assignments to <see cref="DefaultValue"/> (because <see cref="DependencyProperty.UnsetValue"/>
    /// cannot be used in that context, as it is not a constant, but rather a static field). Using this constant in other scenarios is undefined behavior.
    /// </remarks>
    public const object UnsetValue = null!;
}
