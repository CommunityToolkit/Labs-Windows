using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

/// <summary>
/// An option used in <see cref="ToolkitSampleMultiChoiceOptionAttribute"/> and <see cref="ToolkitSampleMultiChoiceOptionMetadataViewModel"/>.
/// </summary>
/// <param name="Label">A label shown to the user for this option.</param>
/// <param name="Value">The value passed to XAML when this option is selected.</param>
public record MultiChoiceOption(string Label, string Value)
{
    public override string ToString()
    {
        return Label;
    }
}
