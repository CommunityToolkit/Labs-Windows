// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

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
        /// <returns>Whether the current target is a UWP application.</returns>
        public static bool IsUwpTarget(Compilation compilation)
        {
            return compilation.Options.OutputKind == OutputKind.WindowsRuntimeApplication;
        }
    }
}
