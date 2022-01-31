// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using System.ComponentModel;

namespace CommunityToolkit.Labs.Core.SourceGenerators.Metadata
{
    /// <summary>
    /// A metadata container for data defined in <see cref="ToolkitSampleBoolOptionAttribute"/> with INPC support.
    /// </summary>
    public class ToolkitSampleBoolOptionMetadataViewModel : IToolkitSampleOptionViewModel
    {
        private string _label;
        private object _value;

        /// <summary>
        /// Creates a new instance of <see cref="ToolkitSampleBoolOptionAttribute"/>.
        /// </summary>
        public ToolkitSampleBoolOptionMetadataViewModel(string id, string label, bool defaultState)
        {
            Name = id;
            _label = label;
            _value = defaultState;
        }

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// A unique identifier for this option.
        /// </summary>
        /// <remarks>
        /// Used by the sample system to match up <see cref="ToolkitSampleBoolOptionMetadataViewModel"/> to the original <see cref="ToolkitSampleBoolOptionAttribute"/> and the control that declared it.
        /// </remarks>
        public string Name { get; }

        /// <summary>
        /// The current boolean value.
        /// </summary>
        public bool BoolValue
        {
            get => (bool)_value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
            }
        }

        /// <summary>
        /// The current boolean value.
        /// </summary>
        public object Value
        {
            get => BoolValue;
            set
            {
                BoolValue = (bool)value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
            }
        }

        /// <summary>
        /// A label to display along the boolean option.
        /// </summary>
        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Label)));
            }
        }
    }
}
