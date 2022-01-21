using System;

namespace CommunityToolkit.Labs.Core.Attributes;

/// <summary>
/// Registers a control as a toolkit sample using the provided data.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class ToolkitSampleAttribute : Attribute
{
    /// <summary>
    /// Creates a new instance of <see cref="ToolkitSampleAttribute"/>.
    /// </summary>
    public ToolkitSampleAttribute(string id, string displayName, ToolkitSampleCategory category, ToolkitSampleSubcategory subcategory, string description)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
        Category = category;
        Subcategory = subcategory;
    }

    /// <summary>
    /// A unique identifier for this sample, used by the sample system.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// The display name for this sample page.
    /// </summary>
    public string DisplayName { get; }

    /// <summary>
    /// The category that this sample belongs to.
    /// </summary>
    public ToolkitSampleCategory Category { get; }

    /// <summary>
    /// A more specific category within the provided <see cref="Category"/>.
    /// </summary>
    public ToolkitSampleSubcategory Subcategory { get; }

    /// <summary>
    /// The description for this sample page.
    /// </summary>
    public string Description { get; }
}
