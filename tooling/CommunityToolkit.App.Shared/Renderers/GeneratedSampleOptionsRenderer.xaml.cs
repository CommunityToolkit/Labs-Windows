// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Metadata;

namespace CommunityToolkit.App.Shared.Renderers;

/// <summary>
/// Displays the provided <see cref="SampleOptions"/> for manipulation by the user.
/// </summary>
/// <remarks>
/// Sample pages implement <see cref="IToolkitSampleGeneratedOptionPropertyContainer"/> via source generators,
/// and are provided a reference to the same <see cref="SampleOptions"/> given to this control.
/// <para/>
/// When the user updates the <see cref="IGeneratedToolkitSampleOptionViewModel.Value"/>,
/// a PropertyChanged event with the <see cref="IGeneratedToolkitSampleOptionViewModel.Name"/> should be emitted.
/// <para/>
/// The sample page sees this property change event via the generated <see cref="IToolkitSampleGeneratedOptionPropertyContainer"/>,
/// causing it to re-get the proxied <see cref="IGeneratedToolkitSampleOptionViewModel.Value"/>.
/// </remarks>
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
        DependencyProperty.Register(nameof(SampleOptions), typeof(IEnumerable<IGeneratedToolkitSampleOptionViewModel>), typeof(GeneratedSampleOptionsRenderer), new PropertyMetadata(null));

    /// <summary>
    /// The generated sample options that should be displayed to the user.
    /// </summary>
    public IEnumerable<IGeneratedToolkitSampleOptionViewModel>? SampleOptions
    {
        get => (IEnumerable<IGeneratedToolkitSampleOptionViewModel>?)GetValue(SampleOptionsProperty);
        set => SetValue(SampleOptionsProperty, value);
    }

    public static Visibility NullOrWhiteSpaceToVisibility(string? str) => string.IsNullOrWhiteSpace(str) ? Visibility.Collapsed : Visibility.Visible;
}
