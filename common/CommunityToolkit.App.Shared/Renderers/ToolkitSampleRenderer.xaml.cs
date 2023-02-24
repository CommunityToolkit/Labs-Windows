// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Attributes;
using CommunityToolkit.Tooling.SampleGen.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace CommunityToolkit.App.Shared.Renderers;

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

    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="IsTabbedMode"/> property.
    /// </summary>
    public static readonly DependencyProperty IsTabbedModeProperty =
        DependencyProperty.Register(nameof(IsTabbedMode), typeof(bool), typeof(ToolkitSampleRenderer), new PropertyMetadata(false));


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

    /// <summary>
    /// The mode of which the control renders. 
    /// </summary>
    public bool IsTabbedMode
    {
        get => (bool)GetValue(IsTabbedModeProperty);
        set => SetValue(IsTabbedModeProperty, value);
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

    protected override void OnNavigatedTo(NavigationEventArgs e)
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
        else
        {
            OptionsScrollViewer.Visibility = Visibility.Collapsed;
        }

        // Generated options must be assigned before attempting to render the control,
        // else some platforms will nullref from XAML but not properly ignore the exception when binding to generated properties.
        SampleControlInstance = sampleControlInstance;
    }

    public static async Task<string?> GetMetadataFileContents(ToolkitSampleMetadata metadata, string fileExtension)
    {
        var filePath = GetRelativePathToFileWithoutExtension(metadata.SampleControlType);

        try
        {
            // Workaround for https://github.com/unoplatform/uno/issues/8649
            if (fileExtension.Contains(".cs"))
            {
                fileExtension = fileExtension.Replace(".cs", ".cs.dat");
            }

            var finalPath = $"ms-appx:///{filePath}.{fileExtension.Trim('.')}";

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{filePath}.{fileExtension.Trim('.')}"));
            var textContents = await FileIO.ReadTextAsync(file);

            return textContents;
        }
        catch (Exception e)
        {
            return $"Exception Encountered Loading file '{filePath}':\n{e.Message}\n{e.StackTrace}";
        }
    }

    /// <summary>
    /// Compute path to a code file bundled in the app using type information.
    /// Assumes path to file within the included assembly folder matches the namespace.
    /// </summary>
    private static string GetRelativePathToFileWithoutExtension(Type type)
    {
        // TODO: https://github.com/CommunityToolkit/Labs-Windows/issues/142
        // MSBuild uses wildcard to find the files, and the wildcards decide where they end up
        // Single experiments use relative paths, the allExperiment head uses absolute paths that grab from all experiments
        // The wildcard captures decide the paths. This discrepency is accounted for manually.
        // Logic here is the exact same that MSBuild uses to find and include the files we need.
        var assemblyName = typeof(ToolkitSampleRenderer).Assembly.GetName().Name;
        if (string.IsNullOrWhiteSpace(assemblyName))
            throw new InvalidOperationException();

        var isAllExperimentHead = assemblyName.StartsWith("CommunityToolkit.", StringComparison.OrdinalIgnoreCase);
        var isProjectTemplateHead = assemblyName.StartsWith("ProjectTemplate");
        var isSingleExperimentHead = !isAllExperimentHead && !isProjectTemplateHead;

        var simpleAssemblyName = type.Assembly.GetName().Name;
        var typeNamespace = type.Namespace;

        if (string.IsNullOrWhiteSpace(simpleAssemblyName))
        {
            throw new ArgumentException($"Unable to find assembly name for provided type {type}.", nameof(simpleAssemblyName));
        }

        if (string.IsNullOrWhiteSpace(typeNamespace))
        {
            throw new ArgumentException($"Unable to find namespace for provided type {type}.", nameof(typeNamespace));
        }

        var folderPath = typeNamespace.Replace(simpleAssemblyName, "").Trim('.').Replace('.', '/');
        if (folderPath.Length != 0)
            folderPath += "/";

        // Component assembly names are formatted as 'ProjectTemplateExperiment.Samples'
        // but the content folder is formatted as 'ProjectTemplate.Samples'
        simpleAssemblyName = simpleAssemblyName.Replace("Experiment", "");

        if (isSingleExperimentHead || isProjectTemplateHead)
        {
            return $"SourceAssets/{folderPath}{type.Name}";
        }

        if (isAllExperimentHead)
        {
            var sampleName = simpleAssemblyName.Replace(".Samples", "");
            return $"SourceAssets/{sampleName}/samples/{folderPath}{type.Name}";
        }

        throw new InvalidOperationException("Unable to determine if running in a single or all experiment solution.");
    }

    private void ToolkitSampleRenderer_Loaded(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(this, IsTabbedMode ? "Tabbed" : "Normal", true);
    }

    private void ThemeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        if (ContentPageHolder.ActualTheme == ElementTheme.Dark)
        {
            ContentPageHolder.RequestedTheme = ElementTheme.Light;
        }
        else
        {
            ContentPageHolder.RequestedTheme = ElementTheme.Dark;
        }

        if (this.ActualTheme != ContentPageHolder.RequestedTheme)
        {
            ThemeBG.Visibility = Visibility.Visible;
        }
        else
        {
            ThemeBG.Visibility = Visibility.Collapsed;
        }

    }

    private void FlowDirectionBtn_OnClick(object sender, RoutedEventArgs e)
    {
#if !HAS_UNO
        if (PageControl.FlowDirection == FlowDirection.LeftToRight)
        {
            PageControl.FlowDirection = FlowDirection.RightToLeft;
        }
        else
        {
            PageControl.FlowDirection = FlowDirection.LeftToRight;
        }
#endif
    }

    private void CodeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SourcecodeExpander.IsExpanded = !SourcecodeExpander.IsExpanded;
    }
}
