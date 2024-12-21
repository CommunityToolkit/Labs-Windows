// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.AppServices.SourceGenerators;

/// <inheritdoc/>
partial class AppServiceGenerator : IIncrementalGenerator
{
    /// <summary>
    /// Shader generators logic for app service hosts and components.
    /// </summary>
    private static class Helpers
    {
        /// <summary>
        /// Gets whether the current target is a UWP application.
        /// </summary>
        /// <param name="compilation">The input <see cref="Compilation"/> instance to inspect.</param>
        /// <param name="analyzerOptions">The analyzer options to use to get info on the target application.</param>
        /// <returns>Whether the current target is a UWP application.</returns>
        public static bool IsUwpTarget(Compilation compilation, AnalyzerConfigOptions analyzerOptions)
        {
            // If the application type is a Windows Runtime application, then it's for sure a UWP app
            if (compilation.Options.OutputKind == OutputKind.WindowsRuntimeApplication)
            {
                return true;
            }

            // Otherwise, the application is UWP if "UseUwpTools" is set
            if (analyzerOptions.TryGetValue("build_property.UseUwpTools", out string? propertyValue))
            {
                if (bool.TryParse(propertyValue, out bool useUwpTools))
                {
                    return true;
                }
            }

            // The app is definitely not a UWP app
            return false;
        }
    }
}
