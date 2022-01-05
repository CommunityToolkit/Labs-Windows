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
        public ToolkitSampleMetadata(ToolkitSampleCategory category, ToolkitSampleSubcategory subcategory, string displayName, string description, string assemblyQualifiedName)
        {
            DisplayName = displayName;
            Description = description;
            AssemblyQualifiedName = assemblyQualifiedName;
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
        /// A type name that can be used to locate the sample page's type.
        /// </summary>
        public string AssemblyQualifiedName { get; }
    }
}
