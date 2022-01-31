using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;

namespace CommunityToolkit.Labs.Shared.Renderers
{
    public sealed partial class GeneratedSampleOptionsRenderer : UserControl
    {
        public GeneratedSampleOptionsRenderer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for <see cref="SampleOptions"/>.
        /// </summary>
        public static readonly DependencyProperty SampleOptionsProperty =
            DependencyProperty.Register(nameof(SampleOptions), typeof(IEnumerable<IToolkitSampleOptionViewModel>), typeof(GeneratedSampleOptionsRenderer), new PropertyMetadata(null));
        private readonly IToolkitSampleGeneratedOptionPropertyContainer _propertyContainer;

        /// <summary>
        /// The sample options that are displayed to the user.
        /// </summary>
        public IEnumerable<IToolkitSampleOptionViewModel>? SampleOptions
        {
            get => (IEnumerable<IToolkitSampleOptionViewModel>?)GetValue(SampleOptionsProperty);
            set => SetValue(SampleOptionsProperty, value);
        }
    }
}
