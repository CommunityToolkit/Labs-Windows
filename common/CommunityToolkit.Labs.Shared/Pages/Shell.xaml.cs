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
        NavigationFrame.Navigated += NavigationFrameOnNavigated;

        samplePages = e.Parameter as IEnumerable<ToolkitFrontMatter>;
        
        if (samplePages is not null)
        {
            var categories = GenerateSampleNavItemTree(samplePages);
            NavView.MenuItems.Add(new NavigationViewItem() { Content = "Get started", Icon = new SymbolIcon() { Symbol = Symbol.Home }, Tag = "GettingStarted" });
            NavView.MenuItems.Add(new NavigationViewItemSeparator());

            foreach (var item in categories)
                NavView.MenuItems.Add(item);

            NavView.SelectedItem = NavView.MenuItems[0];
        }
       
        base.OnNavigatedTo(e);
    }

    private void TitleBar_BackButtonClick(object sender, RoutedEventArgs e)
    {
        if (NavigationFrame.CanGoBack)
        {
            NavigationFrame.GoBack();
        }
    }

    private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs e)
    {
        var selected = (NavigationViewItem)e.SelectedItem;
        var selectedMetadata = selected.Tag as ToolkitFrontMatter;
        if (e.IsSettingsSelected)
        {
            if (NavigationFrame.CurrentSourcePageType != typeof(SettingsPage))
            {
                NavigationFrame.Navigate(typeof(SettingsPage));
            }
        }
        // Check if Getting Started page
        else if (selected.Tag != null && selected.Tag.GetType() == typeof(string))
        {
            NavigationFrame.Navigate(typeof(GettingStartedPage), samplePages);
        }
        else
        {
            if (selectedMetadata is null)
                return;
            NavigateToSample(selectedMetadata);
        }
    }

    public void NavigateToSample(ToolkitFrontMatter? sample)
    {
        if (sample == null)
        {
            NavigationFrame.Navigate(typeof(GettingStartedPage), samplePages);
        }
        else
        {
            NavigationFrame.Navigate(typeof(ToolkitDocumentationRenderer), sample);
        }
    }

    private void NavigationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
    {
        titleBar.IsBackButtonVisible = NavigationFrame.CanGoBack;
    }

    private IEnumerable<NavigationViewItem> GenerateSampleNavItemTree(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        // Make categories
        var categoryData = GenerateCategoryNavItems(sampleMetadata);

        foreach (var navData in categoryData)
        {
            // Make subcategories
            var subcategoryData = GenerateSubcategoryNavItems(navData.SampleMetadata ?? Enumerable.Empty<ToolkitFrontMatter>());

            foreach (var subcategoryItemData in subcategoryData)
            {
                // Make samples
                var sampleNavigationItems = GenerateSampleNavItems(subcategoryItemData.SampleMetadata ?? Enumerable.Empty<ToolkitFrontMatter>());

                foreach (var item in sampleNavigationItems)
                {
                    // Add sample to subcategory
                    subcategoryItemData.NavItem.MenuItems.Add(item);
                }

                // Add subcategory to category
                navData.NavItem.MenuItems.Add(subcategoryItemData.NavItem);
            }

            // Return category
            yield return navData.NavItem;
        }
    }

    private IEnumerable<NavigationViewItem> GenerateSampleNavItems(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        foreach (var metadata in sampleMetadata)
        {
            yield return new NavigationViewItem
            {
                Content = metadata.Title,
                Icon = new BitmapIcon() { ShowAsMonochrome = false, UriSource = new Uri("ms-appx:///Assets/Images/ConnectedAnimation.png") }, // TO DO: This is probably a property we need to add to ToolkitFrontMatter?
                Tag = metadata,
            };
        }
    }

    private IEnumerable<GroupNavigationItemData> GenerateSubcategoryNavItems(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        var samplesBySubcategory = sampleMetadata.GroupBy(x => x.Subcategory);

        foreach (var subcategoryGroup in samplesBySubcategory)
        {
            yield return new GroupNavigationItemData(new NavigationViewItem
            {
                Content = subcategoryGroup.Key,
                SelectsOnInvoked = false,
            }, subcategoryGroup.ToArray());
        }
    }

    private IEnumerable<GroupNavigationItemData> GenerateCategoryNavItems(IEnumerable<ToolkitFrontMatter> sampleMetadata)
    {
        var samplesByCategory = sampleMetadata.GroupBy(x => x.Category);

        foreach (var categoryGroup in samplesByCategory)
        {
            yield return new GroupNavigationItemData(new NavigationViewItem
            {
                Content = categoryGroup.Key,
                Icon = new SymbolIcon() { Symbol = Symbol.Keyboard }, // TO DO: Helper that checks what icon belongs to what Category enum
                SelectsOnInvoked = false,
            }, categoryGroup.ToArray());
        }
    }

    /// <param name="NavItem">A navigation item to contain items in this group.</param>
    /// <param name="SampleMetadata">The samples that belong under <see cref="NavItem"/>.</param>
    private record GroupNavigationItemData(NavigationViewItem NavItem, IEnumerable<ToolkitFrontMatter> SampleMetadata);
}
