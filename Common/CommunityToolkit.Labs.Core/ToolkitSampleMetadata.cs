// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Labs.Core;

/// <summary>
/// Contains the metadata needed to identify and display a toolkit sample.
/// </summary>
/// <param name="Category">The category that this sample belongs to.</param>
/// <param name="Subcategory">A more specific category within the provided <paramref name="Category"/>.</param>
/// <param name="DisplayName">The display name for this sample page.</param>
/// <param name="Description">The description for this sample page.</param>
/// <param name="SampleControlType">A type that can be used to construct an instance of the sample control.</param>
/// <param name="SampleOptionsPaneType">
/// The control type for the sample page's options pane.
/// Constructor should have exactly one parameter that can be assigned to the control type (<see cref="SampleControlType"/>).
/// </param>
public sealed record ToolkitSampleMetadata(
        ToolkitSampleCategory Category,
        ToolkitSampleSubcategory Subcategory,
        string DisplayName,
        string Description,
        Type SampleControlType,
        Type? SampleOptionsPaneType = null);
