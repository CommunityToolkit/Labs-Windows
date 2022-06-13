// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace CommunityToolkit.Labs.Core.SourceGenerators.UIControlTestMethod.Diagnostics
{
    /// <summary>
    /// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
    /// </summary>
    public static class DiagnosticDescriptors
    {
        /// <summary>
        /// Gets a <see cref="DiagnosticDescriptor"/> indicating the provided <see cref="UIControlTestMethodAttribute.Type"/> isn't a valid FrameworkElement.
        /// <para>
        /// Format: <c>"Cannot generate test with type {0} as it does not inherit from FrameworkElement"</c>.
        /// </para>
        /// </summary>
        public static readonly DiagnosticDescriptor TypeDoesNotInheritFrameworkElement = new(
            id: "UICTRLTM0001",
            title: $"Invalid UI test control type",
            messageFormat: $"Cannot generate test with type {{0}} as it does not inherit from FrameworkElement",
            category: typeof(UIControlTestMethodGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Cannot generate test method.");

        /// <summary>
        /// Gets a <see cref="DiagnosticDescriptor"/> indicating the provided <see cref="UIControlTestMethodAttribute.Type"/> is a type has non-parameterless constructor.
        /// <para>
        /// Format: <c>"Cannot generate test with type {0} as it has a constructor with parameters."</c>.
        /// </para>
        /// </summary>
        public static readonly DiagnosticDescriptor TestControlHasConstructorWithParameters = new(
            id: "UICTRLTM0002",
            title: $"Provided test control must not have a constructor with parameters.",
            messageFormat: $"Cannot generate test with type {{0}} as it has a constructor with parameters.",
            category: typeof(UIControlTestMethodGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Cannot generate test method.");

        /// <summary>
        /// Gets a <see cref="DiagnosticDescriptor"/> indicating the attached test method is not parameterless.
        /// <para>
        /// Format: <c>"Cannot generate test as the attached method is not parameterless."</c>.
        /// </para>
        /// </summary>
        public static readonly DiagnosticDescriptor TestMethodIsNotParameterless = new(
            id: "UICTRLTM0003",
            title: $"Attached test method must not take any parameters.",
            messageFormat: $"Cannot generate test as the attached method is not parameterless.",
            category: typeof(UIControlTestMethodGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"Cannot generate test method.");
    }
}
