// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Metadata;

/// <summary>
/// Contains the metadata needed to identify and display a toolkit sample.
/// </summary>
/// <param name="DisplayName">The display name for this sample page.</param>
/// <param name="Description">The description for this sample page.</param>
/// <param name="SampleControlType">A type that can be used to construct an instance of the sample control.</param>
/// <param name="SampleControlFactory">A factory method that returns a new instance of the control.</param>
/// <param name="SampleOptionsPaneType">
/// The control type for the sample page's options pane.
/// Constructor should have exactly one parameter that can be assigned to the control type (<see cref="SampleControlType"/>).
/// </param>
/// <param name="GeneratedSampleOptions">The generated sample options that were declared alongside this sample, if any.</param>
public sealed record ToolkitSampleMetadata(
        string DisplayName,
        string Description,
        Type SampleControlType,
        Func<object> SampleControlFactory,
        Type? SampleOptionsPaneType = null,
        Func<object, object>? SampleOptionsPaneFactory = null,
        IEnumerable<IGeneratedToolkitSampleOptionViewModel>? GeneratedSampleOptions = null);
