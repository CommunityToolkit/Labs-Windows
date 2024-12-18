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
    /// The fully qualified name for the <c>GeneratedDependencyProperty</c> type.
    /// </summary>
    public const string GeneratedDependencyProperty = "CommunityToolkit.WinUI.GeneratedDependencyProperty";

    /// <summary>
    /// The fully qualified name for the <c>Windows.UI.Xaml</c> namespace.
    /// </summary>
    public const string WindowsUIXamlNamespace = "Windows.UI.Xaml";

    /// <summary>
    /// The fully qualified name for the <c>Microsoft.UI.Xaml</c> namespace.
    /// </summary>
    public const string MicrosoftUIXamlNamespace = "Microsoft.UI.Xaml";

    /// <summary>
    /// Gets the fully qualified name for theXAML namespace.
    /// </summary>
    /// <param name="useWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
    public static string XamlNamespace(bool useWindowsUIXaml)
    {
        return useWindowsUIXaml
            ? WindowsUIXamlNamespace
            : MicrosoftUIXamlNamespace;
    }

    /// <summary>
    /// Gets the fully qualified type name for the <c>DependencyObject</c> type for a given XAML mode.
    /// </summary>
    /// <param name="useWindowsUIXaml"><inheritdoc cref="XamlNamespace(bool)" path="/param[@name='useWindowsUIXaml']/text()"/></param>
    public static string DependencyObject(bool useWindowsUIXaml)
    {
        return useWindowsUIXaml
            ? $"{WindowsUIXamlNamespace}.{nameof(DependencyObject)}"
            : $"{MicrosoftUIXamlNamespace}.{nameof(DependencyObject)}";
    }

    /// <summary>
    /// Gets the fully qualified type name for the <c>DependencyProperty</c> type.
    /// </summary>
    /// <param name="useWindowsUIXaml"><inheritdoc cref="XamlNamespace(bool)" path="/param[@name='useWindowsUIXaml']/text()"/></param>
    public static string DependencyProperty(bool useWindowsUIXaml)
    {
        return useWindowsUIXaml
            ? $"{WindowsUIXamlNamespace}.{nameof(DependencyProperty)}"
            : $"{MicrosoftUIXamlNamespace}.{nameof(DependencyProperty)}";
    }

    /// <summary>
    /// Gets the fully qualified type name for the <c>DependencyPropertyChangedEventArgs</c> type.
    /// </summary>
    /// <param name="useWindowsUIXaml"><inheritdoc cref="XamlNamespace(bool)" path="/param[@name='useWindowsUIXaml']/text()"/></param>
    public static string DependencyPropertyChangedEventArgs(bool useWindowsUIXaml)
    {
        return useWindowsUIXaml
            ? $"{WindowsUIXamlNamespace}.{nameof(DependencyPropertyChangedEventArgs)}"
            : $"{MicrosoftUIXamlNamespace}.{nameof(DependencyPropertyChangedEventArgs)}";
    }

    /// <summary>
    /// Gets the fully qualified type name for the <c>PropertyMetadata</c> type.
    /// </summary>
    /// <param name="useWindowsUIXaml"><inheritdoc cref="XamlNamespace(bool)" path="/param[@name='useWindowsUIXaml']/text()"/></param>
    public static string PropertyMetadata(bool useWindowsUIXaml)
    {
        return useWindowsUIXaml
            ? $"{WindowsUIXamlNamespace}.{nameof(PropertyMetadata)}"
            : $"{MicrosoftUIXamlNamespace}.{nameof(PropertyMetadata)}";
    }

    /// <summary>
    /// Gets the fully qualified type name for the <c>CreateDefaultValueCallback </c> type.
    /// </summary>
    /// <param name="useWindowsUIXaml"><inheritdoc cref="XamlNamespace(bool)" path="/param[@name='useWindowsUIXaml']/text()"/></param>
    public static string CreateDefaultValueCallback(bool useWindowsUIXaml)
    {
        return useWindowsUIXaml
            ? $"{WindowsUIXamlNamespace}.{nameof(CreateDefaultValueCallback)}"
            : $"{MicrosoftUIXamlNamespace}.{nameof(CreateDefaultValueCallback)}";
    }
}
