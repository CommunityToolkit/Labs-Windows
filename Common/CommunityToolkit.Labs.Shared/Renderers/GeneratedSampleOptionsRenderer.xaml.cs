using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;

#if WINAPPSDK
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#endif

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
