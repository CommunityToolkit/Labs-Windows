// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.Labs.Core
{
    public class ToolkitSampleMetadata
    {
        /// <summary>
        /// Creates a new instance of <see cref="ToolkitSampleAttribute"/>.
        /// </summary>
        public ToolkitSampleMetadata(string displayName, string description, Type type)
        {
            DisplayName = displayName;
            Description = description;
            Type = type;
        }

        /// <summary>
        /// The display name for this sample page.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The description for this sample page.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The sample page's type.
        /// </summary>
        public Type Type { get; set; }
    }
}
