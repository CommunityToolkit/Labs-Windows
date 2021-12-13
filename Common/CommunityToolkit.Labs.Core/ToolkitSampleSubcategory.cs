// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.Core
{
    /// <summary>
    /// All the different subcategories used by samples.
    /// </summary>
    
    // Subcategory is a flat enum so we can use a subcategory in multiple categories,
    // and so we can freely move samples or whole sections in the future.
    public enum ToolkitSampleSubcategory : byte
    {
        /// <summary>
        /// No subcategory specified.
        /// </summary>
        None,

        /// <summary>
        /// A sample that focuses on control layout.
        /// </summary>
        Layout,
    }
}
