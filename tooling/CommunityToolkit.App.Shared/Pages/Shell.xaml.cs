// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.App.Shared.Renderers;
using CommunityToolkit.Tooling.SampleGen.Metadata;
using CommunityToolkit.App.Shared.Helpers;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.App.Shared.Pages;

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
        BackgroundHelper.SetBackground(this);
        SetupNavigationMenu();
        base.OnNavigatedTo(e); 
    }


    private void SetupNavigationMenu()
    {
        if (samplePages is not null)
        {
            NavView.MenuItems.Add(new MUXC.NavigationViewItem() { Content = "Get started", Icon = new SymbolIcon() { Symbol = Symbol.Home }, Tag = "GettingStarted" });
            NavView.MenuItems.Add(new MUXC.NavigationViewItemSeparator());

            // Populate menu with categories, subcategories and samples
            foreach (var item in NavigationViewHelper.GenerateNavItemTree(samplePages))
                NavView.MenuItems.Add(item);

            NavView.SelectedItem = NavView.MenuItems[0];
            NavigationFrame.Navigate(typeof(GettingStartedPage), samplePages);
        }
    }

    private void NavView_ItemInvoked(MUXC.NavigationView sender, MUXC.NavigationViewItemInvokedEventArgs args)
    {
        var selectedItem = (MUXC.NavigationViewItem)args.InvokedItemContainer;

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
            if (rawCategory is MUXC.NavigationViewItem category)
            {
                foreach (object rawSubcategory in category.MenuItems)
                {
                    if (rawSubcategory is MUXC.NavigationViewItem subcategory)
                    {
                        foreach (object rawSample in subcategory.MenuItems)
                        {
                            if (rawSample is MUXC.NavigationViewItem sample)
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

//// See AutoSuggestBox issue for WASM https://github.com/unoplatform/uno/issues/7778
#if !HAS_UNO
    private void searchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.ItemsSource = null;
                return;
            }
            else
            {
                var query = searchBox.Text;
                searchBox.ItemsSource = samplePages?.Where(s => s!.Title!.ToLower().Contains(query) || s!.Keywords!.ToLower().Contains(query) || s!.Category!.ToString().ToLower().Contains(query) || s!.Subcategory!.ToString().ToLower().Contains(query)).ToArray(); ;
                return;
            }
        }
    }

    private void searchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null && args.ChosenSuggestion is ToolkitFrontMatter)
        {
            var selectedSample = args.ChosenSuggestion as ToolkitFrontMatter;
            NavigateToSample(selectedSample);
            searchBox.Text = string.Empty;
        }
        else
        {
            return;
        }
    }
#endif
}
