// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for the <see cref="AnalyzerConfigOptions"/> type.
/// </summary>
internal static class AnalyzerConfigOptionsExtensions
{
    /// <summary>
    /// Gets the boolean value of a given MSBuild property from an input <see cref="AnalyzerConfigOptions"/> instance.
    /// </summary>
    /// <param name="options">The input <see cref="AnalyzerConfigOptions"/> instance.</param>
    /// <param name="propertyName">The name of the target MSBuild property.</param>
    /// <param name="defaultValue">The default value to return if the property is not found or cannot be parsed.</param>
    /// <returns>The value of the target MSBuild property.</returns>
    public static bool GetMSBuildBooleanPropertyValue(this AnalyzerConfigOptions options, string propertyName, bool defaultValue = false)
    {
        if (options.TryGetMSBuildStringPropertyValue(propertyName, out string? propertyValue))
        {
            if (bool.TryParse(propertyValue, out bool booleanPropertyValue))
            {
                return booleanPropertyValue;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Tries to get a <see cref="string"/> value of a given MSBuild property from an input <see cref="AnalyzerConfigOptions"/> instance.
    /// </summary>
    /// <param name="options">The input <see cref="AnalyzerConfigOptions"/> instance.</param>
    /// <param name="propertyName">The name of the target MSBuild property.</param>
    /// <param name="propertyValue">The resulting property value.</param>
    /// <returns>Whether the property value was retrieved..</returns>
    public static bool TryGetMSBuildStringPropertyValue(this AnalyzerConfigOptions options, string propertyName, [NotNullWhen(true)] out string? propertyValue)
    {
        return options.TryGetValue($"build_property.{propertyName}", out propertyValue);
    }
}
