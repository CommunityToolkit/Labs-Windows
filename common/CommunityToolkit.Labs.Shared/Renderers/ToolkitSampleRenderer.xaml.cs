using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

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
    /// <summary>
    /// Handles the display of a single toolkit sample, its source code, and the options that control it.
    /// </summary>
    public sealed partial class ToolkitSampleRenderer : Page
    {
        /// <summary>
        /// Creates a new instance of <see cref="ToolkitSampleRenderer"/>.
        /// </summary>
        public ToolkitSampleRenderer()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="Metadata"/> property.
        /// </summary>
        public static readonly DependencyProperty MetadataProperty =
            DependencyProperty.Register(nameof(Metadata), typeof(ToolkitSampleMetadata), typeof(ToolkitSampleRenderer), new PropertyMetadata(null, OnMetadataPropertyChanged));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="SampleControlInstance"/> property.
        /// </summary>
        public static readonly DependencyProperty SampleControlInstanceProperty =
            DependencyProperty.Register(nameof(SampleControlInstance), typeof(UIElement), typeof(ToolkitSampleRenderer), new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="SampleOptionsPaneInstance"/> property.
        /// </summary>
        public static readonly DependencyProperty SampleOptionsPaneInstanceProperty =
            DependencyProperty.Register(nameof(SampleOptionsPaneInstance), typeof(UIElement), typeof(ToolkitSampleRenderer), new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="XamlCode"/> property.
        /// </summary>
        public static readonly DependencyProperty XamlCodeProperty =
            DependencyProperty.Register(nameof(XamlCode), typeof(string), typeof(ToolkitSampleRenderer), new PropertyMetadata(null));

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="CSharpCode"/> property.
        /// </summary>
        public static readonly DependencyProperty CSharpCodeProperty =
            DependencyProperty.Register(nameof(CSharpCode), typeof(string), typeof(ToolkitSampleRenderer), new PropertyMetadata(null));

        public ToolkitSampleMetadata? Metadata
        {
            get { return (ToolkitSampleMetadata?)GetValue(MetadataProperty); }
            set { SetValue(MetadataProperty, value); }
        }

        /// <summary>
        /// The sample control instance being displayed.
        /// </summary>
        public UIElement? SampleControlInstance
        {
            get => (UIElement?)GetValue(SampleControlInstanceProperty);
            set => SetValue(SampleControlInstanceProperty, value);
        }


        /// <summary>
        /// The options pane for the sample being displayed.
        /// </summary>
        public UIElement? SampleOptionsPaneInstance
        {
            get => (UIElement?)GetValue(SampleOptionsPaneInstanceProperty);
            set => SetValue(SampleOptionsPaneInstanceProperty, value);
        }

        /// <summary>
        /// The XAML code being rendered.
        /// </summary>
        public string? XamlCode
        {
            get => (string?)GetValue(XamlCodeProperty);
            set => SetValue(XamlCodeProperty, value);
        }

        /// <summary>
        /// The backing C# for the <see cref="XamlCode"/> being rendered. 
        /// </summary>
        public string? CSharpCode
        {
            get => (string?)GetValue(CSharpCodeProperty);
            set => SetValue(CSharpCodeProperty, value);
        }

        private static async void OnMetadataPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is ToolkitSampleRenderer renderer &&
                renderer.Metadata != null &&
                args.OldValue != args.NewValue)
            {
                await renderer.LoadData();
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Metadata = (ToolkitSampleMetadata)e.Parameter;
        }

        private async Task LoadData()
        {
            if (Metadata is null)
            {
                return;
            }

            XamlCode = await GetMetadataFileContents(Metadata, "xaml");
            CSharpCode = await GetMetadataFileContents(Metadata, "xaml.cs");

            var sampleControlInstance = (UIElement)Metadata.SampleControlFactory();

            // Custom control-based sample options.
            if (Metadata.SampleOptionsPaneType is not null && Metadata.SampleOptionsPaneFactory is not null)
            {
                SampleOptionsPaneInstance = (UIElement)Metadata.SampleOptionsPaneFactory(sampleControlInstance);
            }

            // Source generater-based sample options
            else if (sampleControlInstance is IToolkitSampleGeneratedOptionPropertyContainer propertyContainer)
            {
                // Pass the generated sample options to the displayed Control instance.
                // Generated properties reference these in getters and setters.
                propertyContainer.GeneratedPropertyMetadata = Metadata.GeneratedSampleOptions;

                SampleOptionsPaneInstance = new GeneratedSampleOptionsRenderer
                {
                    SampleOptions = propertyContainer.GeneratedPropertyMetadata
                };
            }

            // Generated options must be assigned before attempting to render the control,
            // else some platforms will nullref from XAML but not properly ignore the exception when binding to generated properties.
            SampleControlInstance = sampleControlInstance;
        }

        public static async Task<string?> GetMetadataFileContents(ToolkitSampleMetadata metadata, string fileExtension)
        {
            var filePath = GetPathToFileWithoutExtension(metadata.SampleControlType);

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"{filePath}.{fileExtension.Trim('.')}"));
                var textContents = await FileIO.ReadTextAsync(file);

                // Remove toolkit attributes
                textContents = Regex.Replace(textContents, @$"\s+?\[{nameof(ToolkitSampleAttribute).Replace("Attribute", "")}.+\]", "");
                textContents = Regex.Replace(textContents, @$"\s+?\[{nameof(ToolkitSampleOptionsPaneAttribute).Replace("Attribute", "")}.+\]", "");

                return textContents;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Compute path to a code file bundled in the app using type information.
        /// Assumes file path in project matches namespace.
        /// </summary>
        public static string GetPathToFileWithoutExtension(Type type)
        {
            var simpleAssemblyName = type.Assembly.GetName().Name;
            var typeNamespace = type.Namespace;

            if (string.IsNullOrWhiteSpace(simpleAssemblyName))
                throw new ArgumentException($"Unable to find assembly name for provided type {type}.", nameof(simpleAssemblyName));

            if (string.IsNullOrWhiteSpace(typeNamespace))
                throw new ArgumentException($"Unable to find namespace for provided type {type}.", nameof(typeNamespace));

            var folderPath = typeNamespace.Replace(simpleAssemblyName, "").Trim('.').Replace('.', '/');

            return $"ms-appx:///{simpleAssemblyName}/{folderPath}/{type.Name}";
        }
    }
}
