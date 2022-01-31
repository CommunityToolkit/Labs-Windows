// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Metadata
{
    /// <summary>
    /// Implementors of this class contain properties which were created by source generators, are bound to in XAML, and are manipulated from another source.
    /// </summary>
    public interface IToolkitSampleGeneratedOptionPropertyContainer
    {
        /// <summary>
        /// Holds a reference to the backing ViewModels for all generated properties.
        /// </summary>
        public IEnumerable<IToolkitSampleOptionViewModel>? GeneratedPropertyMetadata { get; set; }
    }
}
