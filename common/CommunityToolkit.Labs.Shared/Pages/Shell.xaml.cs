// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.Core;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation.Collections;

#if !WINAPPSDK
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
#endif

using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItemSeparator = Microsoft.UI.Xaml.Controls.NavigationViewItemSeparator;
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;
using CommunityToolkit.Labs.Shared.Renderers;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;
using CommunityToolkit.Labs.Shared.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.Labs.Shared.Pages;

/// <summary>
/// Used to display all Community Toolkit Labs sample projects in one place.
/// </summary>
public sealed partial class Shell : Page
{
    public static IEnumerable<ToolkitFrontMatter>? samplePages { get; private set; }
    public static Shell? Current { get; private set; }

    public Shell()
    {
        this.InitializeComponent();
        Current = this;
    }

    /// <summary>
    /// Gets the items used for navigating.
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        samplePages = e.Parameter as IEnumerable<ToolkitFrontMatter>;
        SetBackground();
        SetupNavigationMenu();
        base.OnNavigatedTo(e);
    }


    private void SetupNavigationMenu()
    {
        if (samplePages is not null)
        {
            NavView.MenuItems.Add(new NavigationViewItem() { Content = "Get started", Icon = new SymbolIcon() { Symbol = Symbol.Home }, Tag = "GettingStarted" });
            NavView.MenuItems.Add(new NavigationViewItemSeparator());

            // Populate menu with categories, subcategories and samples
            foreach (var item in NavigationViewHelper.GenerateNavItemTree(samplePages))
                NavView.MenuItems.Add(item);

            NavView.SelectedItem = NavView.MenuItems[0];
            NavigationFrame.Navigate(typeof(GettingStartedPage), samplePages);
        }
    }

    private void NavView_ItemInvoked(NavigationView sender, MUXC.NavigationViewItemInvokedEventArgs args)
    {
        var selectedItem = ((NavigationViewItem)args.InvokedItemContainer);

        if (args.IsSettingsInvoked)
        {
            if (NavigationFrame.CurrentSourcePageType != typeof(SettingsPage))
            {
                NavigationFrame.Navigate(typeof(SettingsPage));
            }
        }
        // Check if Getting Started page
        else if (selectedItem.Tag != null && selectedItem.Tag.GetType() == typeof(string))
        {
            NavigationFrame.Navigate(typeof(GettingStartedPage), samplePages);
        }
        else
        {
            var selectedMetadata = selectedItem.Tag as ToolkitFrontMatter;
            if (selectedMetadata is null)
                return;
            NavigationFrame.Navigate(typeof(ToolkitDocumentationRenderer), selectedMetadata);
        }
    }

    private void TitleBar_BackButtonClick(object sender, RoutedEventArgs e)
    {
        if (NavigationFrame.CanGoBack)
        {
            NavigationFrame.GoBack();
        }
    }

    public void NavigateToSample(ToolkitFrontMatter? sample)
    {
        if (sample is null)
        {
            NavigationFrame.Navigate(typeof(GettingStartedPage), samplePages);
        }
        else
        {
            NavigationFrame.Navigate(typeof(ToolkitDocumentationRenderer), sample);
        }

        EnsureNavigationSelection(sample?.FilePath);
    }

    private void NavigationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
    {
        NavView.IsBackEnabled = NavigationFrame.CanGoBack;
        appTitleBar.IsBackButtonVisible = NavigationFrame.CanGoBack;

        // Update the NavigationViewControl selection indicator
        if (navigationEventArgs.NavigationMode == NavigationMode.Back)
        {
            if (navigationEventArgs.SourcePageType == typeof(GettingStartedPage))
            {
                NavView.SelectedItem = NavView.MenuItems[0];
            }
            else if (navigationEventArgs.SourcePageType == typeof(SettingsPage))
            {
                NavView.SelectedItem = NavView.SettingsItem;
            }
            else if (navigationEventArgs.Parameter.GetType() == typeof(ToolkitFrontMatter))
            {

                EnsureNavigationSelection(((ToolkitFrontMatter)navigationEventArgs.Parameter).FilePath);
            }
        }
    }

    public void EnsureNavigationSelection(string? FilePath)
    {
        foreach (object rawCategory in this.NavView.MenuItems)
        {
            if (rawCategory is NavigationViewItem category)
            {
                foreach (object rawSubcategory in category.MenuItems)
                {
                    if (rawSubcategory is NavigationViewItem subcategory)
                    {
                        foreach (object rawSample in subcategory.MenuItems)
                        {
                            if (rawSample is NavigationViewItem sample)
                            {
                                if (sample.Tag != null)
                                {
                                    if (((ToolkitFrontMatter)sample.Tag).FilePath == FilePath) // TO DO: file path is unique and works for now, but do we need a SampleID of some sorts?
                                    {
                                        category.IsExpanded = true;
                                        subcategory.IsExpanded = true;
                                        NavView.SelectedItem = sample;
                                        sample.IsSelected = true;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetBackground()
    {
#if !WINAPPSDK
        BackdropMaterial.SetApplyToRootOrPageBackground(this, true);
#else
        // TO DO: SET MICA THE WINAPPSDK WAY, FALLING BACK TO DEFAULT BACKGROUND FOR NOW
        this.Background =  (SolidColorBrush)Resources["BackgroundColorBrush"];
#endif
    }
}
