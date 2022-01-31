// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Metadata
{
    /// <summary>
    /// A common view model for any toolkit sample option.
    /// </summary>
    public interface IToolkitSampleOptionViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The current value. Bound in XAML.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// A unique identifier name for this option.
        /// </summary>
        /// <remarks>
        /// Used by the sample system to match up <see cref="ToolkitSampleBoolOptionMetadataViewModel"/> to the original <see cref="ToolkitSampleBoolOptionAttribute"/> and the control that declared it.
        /// </remarks>
        public string Name { get; }
    }
}
