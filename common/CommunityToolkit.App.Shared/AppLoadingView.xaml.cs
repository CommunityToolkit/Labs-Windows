// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Metadata;
using CommunityToolkit.Tooling.SampleGen;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Collections;
using CommunityToolkit.App.Shared.Pages;

#if !WINAPPSDK
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
#else
using DispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif

namespace CommunityToolkit.App.Shared;

/// <summary>
/// Kicks off the loading process and determines whether to display a single-sample or multi-sample view.
/// </summary>
public sealed partial class AppLoadingView : Page
{
    /// <summary>
    /// Creates a new instance of <see cref="AppLoadingView"/>.
    /// </summary>
    public AppLoadingView()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Backing dependency property for <see cref="IsLoading"/>.
    /// </summary>
    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(AppLoadingView), new PropertyMetadata(false));

    /// <summary>
    /// Backing dependency property for <see cref="LoadingMessage"/>.
    /// </summary>
    public static readonly DependencyProperty LoadingMessageProperty =
        DependencyProperty.Register(nameof(LoadingMessage), typeof(string), typeof(AppLoadingView), new PropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets a value indicating whether loading operations are being performed.
    /// </summary>
    public bool IsLoading
    {
        get { return (bool)GetValue(IsLoadingProperty); }
        set { SetValue(IsLoadingProperty, value); }
    }

    /// <summary>
    /// Gets or sets the displayed loading message.
    /// </summary>
    public string LoadingMessage
    {
        get { return (string)GetValue(LoadingMessageProperty); }
        set { SetValue(LoadingMessageProperty, value); }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        IsLoading = true;
        LoadingMessage = "Loading...";

        var sampleDocs = FindReferencedDocumentPages().ToArray();

        if (sampleDocs.Length == 0)
        {
            IsLoading = false;
            LoadingMessage = "No sample pages were found :(";
            return;
        }

#if LABS_ALL_SAMPLES
        ScheduleNavigate(typeof(Shell), sampleDocs);
#else
        var samples = FindReferencedSamples().ToArray();

        (IEnumerable<ToolkitSampleMetadata> Samples, IEnumerable<ToolkitFrontMatter> Docs, bool AreDocsFirst) displayInfo = (samples, sampleDocs, false);
        ScheduleNavigate(typeof(TabbedPage), displayInfo);
#endif
    }

    // Needed because Frame.Navigate doesn't work inside of the OnNavigatedTo override.
    private void ScheduleNavigate(Type type, object? param = null)
    {
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
#if !NETFX_CORE
            Frame.Navigate(type, param);
#else
            Frame.NavigateToType(type, param, new FrameNavigationOptions { IsNavigationStackEnabled = false });
#endif
        });
    }

    private IEnumerable<ToolkitFrontMatter> FindReferencedDocumentPages()
    {
        return ToolkitDocumentRegistry.Execute();
    }

    private IEnumerable<ToolkitSampleMetadata> FindReferencedSamples()
    {
        return ToolkitSampleRegistry.Listing.Values;
    }
}
