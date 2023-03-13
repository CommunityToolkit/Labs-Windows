// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Tooling.SampleGen.Metadata;

//// We can't use record for WinUI 3 yet due to https://github.com/microsoft/microsoft-ui-xaml/issues/5315

/// <summary>
/// Contains the metadata needed to identify and display a toolkit sample.
/// </summary>
public sealed class ToolkitSampleMetadata
{
    /// <summary>
    /// Gets or sets a unique identifier for the sample, across all samples.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the display name for this sample page.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the description for this sample page.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a type that can be used to construct an instance of the sample control.
    /// </summary>
    public Type SampleControlType { get; set; }

    /// <summary>
    /// Gets or sets a factory method that returns a new instance of the control.
    /// </summary>
    public Func<object> SampleControlFactory { get; set; }

    /// <summary>
    /// Gets or sets the (optional) control type for the sample page's options pane.
    /// Constructor should have exactly one parameter that can be assigned to the control type (<see cref="SampleControlType"/>).
    /// </summary>
    public Type? SampleOptionsPaneType { get; set; }

    /// <summary>
    /// Gets or sets a factory method that returns a new instance of the sample options control.
    /// </summary>
    public Func<object, object>? SampleOptionsPaneFactory { get; set; }

    /// <summary>
    /// Gets or sets the generated sample options that were declared alongside this sample, if any.
    /// </summary>
    public IEnumerable<IGeneratedToolkitSampleOptionViewModel>? GeneratedSampleOptions { get; set; }

    /// <summary>
    /// Contains the metadata needed to identify and display a toolkit sample.
    /// </summary>
    /// <param name="id">A unique identifier for the sample, across all samples.</param>
    /// <param name="displayName">The display name for this sample page.</param>
    /// <param name="description">The description for this sample page.</param>
    /// <param name="sampleControlType">A type that can be used to construct an instance of the sample control.</param>
    /// <param name="sampleControlFactory">A factory method that returns a new instance of the control.</param>
    /// <param name="sampleOptionsPaneType">
    /// The (optional) control type for the sample page's options pane.
    /// Constructor should have exactly one parameter that can be assigned to the control type (<see cref="SampleControlType"/>).
    /// </param>
    /// <param name="sampleOptionsPaneFactory">A factory method that returns a new instance of the sample options control.</param>
    /// <param name="generatedSampleOptions">The generated sample options that were declared alongside this sample, if any.</param>
    public ToolkitSampleMetadata(
        string id,
        string displayName,
        string description,
        Type sampleControlType,
        Func<object> sampleControlFactory,
        Type? sampleOptionsPaneType = null,
        Func<object, object>? sampleOptionsPaneFactory = null,
        IEnumerable<IGeneratedToolkitSampleOptionViewModel>? generatedSampleOptions = null)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
        SampleControlType = sampleControlType;
        SampleControlFactory = sampleControlFactory;
        SampleOptionsPaneType = sampleOptionsPaneType;
        SampleOptionsPaneFactory = sampleOptionsPaneFactory;
        GeneratedSampleOptions = generatedSampleOptions;
    }
}
