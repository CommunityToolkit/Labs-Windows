// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.GeneratedDependencyProperty.Constants;

/// <summary>
/// The well known names for types used by source generators and analyzers.
/// </summary>
internal static class WellKnownTypeNames
{
    /// <summary>
    /// The fully qualified type name for the <c>[GeneratedDependencyProperty]</c> type.
    /// </summary>
    public const string GeneratedDependencyPropertyAttribute = "CommunityToolkit.WinUI.GeneratedDependencyPropertyAttribute";

    /// <summary>
    /// The fully qualified type name for the <c>GeneratedDependencyProperty</c> type.
    /// </summary>
    public const string GeneratedDependencyProperty = "CommunityToolkit.WinUI.GeneratedDependencyProperty";

    /// <summary>
    /// The fully qualified type name for the <c>XAML</c> namespace.
    /// </summary>
    public const string XamlNamespace =
#if WINDOWS_UWP
        "Windows.UI.Xaml";

#else
        "Microsoft.UI.Xaml";
#endif

    /// <summary>
    /// The fully qualified type name for the <c>DependencyObject</c> type.
    /// </summary>
    public const string DependencyObject = $"{XamlNamespace}.{nameof(DependencyObject)}";

    /// <summary>
    /// The fully qualified type name for the <c>DependencyProperty</c> type.
    /// </summary>
    public const string DependencyProperty = $"{XamlNamespace}.{nameof(DependencyProperty)}";

    /// <summary>
    /// The fully qualified type name for the <c>DependencyPropertyChangedEventArgs</c> type.
    /// </summary>
    public const string DependencyPropertyChangedEventArgs = $"{XamlNamespace}.{nameof(DependencyPropertyChangedEventArgs)}";

    /// <summary>
    /// The fully qualified type name for the <c>PropertyMetadata</c> type.
    /// </summary>
    public const string PropertyMetadata = $"{XamlNamespace}.{nameof(PropertyMetadata)}";
}
