// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Metadata;

namespace CommunityToolkit.Tooling.SampleGen.Attributes;

/// <summary>
/// Holds data for a multiple choice option.
/// Primarily used by <see cref="ToolkitSampleMultiChoiceOptionMetadataViewModel"/>.
/// </summary>
/// <param name="Label">A label shown to the user for this option.</param>
/// <param name="Value">The value passed to XAML when this option is selected.</param>
public record MultiChoiceOption(string Label, string Value)
{
    /// <remarks>
    /// The string has been overriden to display the label only,
    /// especially so the data can be easily displayed in XAML without a custom template, converter or code behind.
    /// </remarks>
    public override string ToString()
    {
        return Label;
    }
}
