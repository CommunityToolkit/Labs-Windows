// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for the <see cref="Compilation"/> type.
/// </summary>
internal static class CompilationExtensions
{
    /// <summary>
    /// Checks whether a given compilation (assumed to be for C#) is using at least a given language version.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/> to consider for analysis.</param>
    /// <param name="languageVersion">The minimum language version to check.</param>
    /// <returns>Whether <paramref name="compilation"/> is using at least the specified language version.</returns>
    public static bool HasLanguageVersionAtLeastEqualTo(this Compilation compilation, LanguageVersion languageVersion)
    {
        return ((CSharpCompilation)compilation).LanguageVersion >= languageVersion;
    }

    /// <summary>
    /// Checks whether a given compilation (assumed to be for C#) is using the preview language version.
    /// </summary>
    /// <param name="compilation">The <see cref="Compilation"/> to consider for analysis.</param>
    /// <returns>Whether <paramref name="compilation"/> is using the preview language version.</returns>
    public static bool IsLanguageVersionPreview(this Compilation compilation)
    {
        return ((CSharpCompilation)compilation).LanguageVersion == LanguageVersion.Preview;
    }

    /// <summary>
    /// Gets whether the current target is a WinRT application (i.e. legacy UWP).
    /// </summary>
    /// <param name="compilation">The input <see cref="Compilation"/> instance to inspect.</param>
    /// <returns>Whether the current target is a WinRT application.</returns>
    public static bool IsWindowsRuntimeApplication(this Compilation compilation)
    {
        return compilation.Options.OutputKind == OutputKind.WindowsRuntimeApplication;
    }
}
