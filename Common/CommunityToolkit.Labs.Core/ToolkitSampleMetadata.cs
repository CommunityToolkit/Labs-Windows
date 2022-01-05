// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Labs.Core
{
    /// <summary>
    /// Contains the metadata needed to identify and display a toolkit sample.
    /// </summary>
    public sealed class ToolkitSampleMetadata
    {
        /// <summary>
        /// Creates a new instance of <see cref="ToolkitSampleAttribute"/>.
        /// </summary>
        public ToolkitSampleMetadata(ToolkitSampleCategory category, ToolkitSampleSubcategory subcategory, string displayName, string description, Type sampleControlType)
        {
            DisplayName = displayName;
            Description = description;
            SampleControlType = sampleControlType;
            Category = category;
            Subcategory = subcategory;
        }

        /// <summary>
        /// The category that this sample belongs to.
        /// </summary>
        public ToolkitSampleCategory Category { get; }

        /// <summary>
        /// A more specific category within the provided <see cref="Category"/>.
        /// </summary>
        public ToolkitSampleSubcategory Subcategory { get; }

        /// <summary>
        /// The display name for this sample page.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The description for this sample page.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// A type that can be used to construct an instance of the sample control.
        /// </summary>
        public Type SampleControlType { get; }
    }
}
