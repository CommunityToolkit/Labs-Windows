// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.GeneratedDependencyProperty.Constants;

namespace CommunityToolkit.GeneratedDependencyProperty.Models;

/// <summary>
/// A model representing a default value for a dependency property.
/// </summary>
internal abstract partial record DependencyPropertyDefaultValue
{
    /// <summary>
    /// A <see cref="DependencyPropertyDefaultValue"/> type representing a <see langword="null"/> value.
    /// </summary>
    public sealed record Null : DependencyPropertyDefaultValue
    {
        /// <summary>
        /// The shared <see cref="Null"/> instance (the type is stateless).
        /// </summary>
        public static Null Instance { get; } = new();

        /// <inheritdoc/>
        public override string ToString()
        {
            return "null";
        }
    }

    /// <summary>
    /// A <see cref="DependencyPropertyDefaultValue"/> type representing default value for a specific type.
    /// </summary>
    /// <param name="TypeName">The input type name.</param>
    /// <param name="IsProjectedType">Indicates whether the type is projected, meaning WinRT can default initialize it automatically if needed.</param>
    public sealed record Default(string TypeName, bool IsProjectedType) : DependencyPropertyDefaultValue
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"default({TypeName})";
        }
    }

    /// <summary>
    /// A <see cref="DependencyPropertyDefaultValue"/> type representing the special unset value.
    /// </summary>
    /// <param name="UseWindowsUIXaml">Whether to use the UWP XAML or WinUI 3 XAML namespaces.</param>
    public sealed record UnsetValue(bool UseWindowsUIXaml) : DependencyPropertyDefaultValue
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return $"global::{WellKnownTypeNames.DependencyProperty(UseWindowsUIXaml)}.UnsetValue";
        }
    }

    /// <summary>
    /// A <see cref="DependencyPropertyDefaultValue"/> type representing a constant value.
    /// </summary>
    /// <param name="Value">The constant value.</param>
    public sealed record Constant(TypedConstantInfo Value) : DependencyPropertyDefaultValue
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    /// <summary>
    /// A <see cref="DependencyPropertyDefaultValue"/> type representing a callback.
    /// </summary>
    /// <param name="MethodName">The name of the callback method to invoke.</param>
    public sealed record Callback(string MethodName) : DependencyPropertyDefaultValue
    {
        /// <inheritdoc/>
        public override string ToString()
        {
            return MethodName;
        }
    }
}
