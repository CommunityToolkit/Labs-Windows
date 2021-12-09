using System;

namespace CommunityToolkit.Labs.Core.Attributes
{
    /// <summary>
    /// Contains the registratino data for a toolkit sample project.
    /// </summary>
    public class ToolkitSampleAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="ToolkitSampleAttribute"/>.
        /// </summary>

        public ToolkitSampleAttribute(Type sample)
        {
            Sample = sample;
        }

        /// <summary>
        /// A control type containing the sample.
        /// </summary>
        public Type Sample { get; set; }
    }
}
