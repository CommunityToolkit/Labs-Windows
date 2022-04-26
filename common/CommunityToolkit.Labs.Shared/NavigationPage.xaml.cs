using CommunityToolkit.Labs.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
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
using NavigationViewSelectionChangedEventArgs = Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs;
using CommunityToolkit.Labs.Shared.Renderers;
using CommunityToolkit.Labs.Core.SourceGenerators.Metadata;

namespace CommunityToolkit.Labs.Shared
{
    /// <summary>
    /// Used to display all Community Toolkit Labs sample projects in one place.
    /// </summary>
    public sealed partial class NavigationPage : Page
    {
        public NavigationPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the items used for navigating.
        /// </summary>
        public ObservableCollection<NavigationViewItem> NavigationViewItems { get; } = new ObservableCollection<NavigationViewItem>();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var samplePages = e.Parameter as IEnumerable<ToolkitFrontMatter>;

            if (samplePages is not null)
            {
                var categories = GenerateSampleNavItemTree(samplePages);

                foreach (var item in categories)
                    NavigationViewItems.Add(item);
            }

            base.OnNavigatedTo(e);
        }

        private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs e)
        {
            var selected = (NavigationViewItem)e.SelectedItem;
            var selectedMetadata = selected.Tag as ToolkitFrontMatter;

            if (selectedMetadata is null)
                return;

            NavFrame.Navigate(typeof(ToolkitDocumentationRenderer), selectedMetadata);
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
                }, categoryGroup.ToArray());
            }
        }

        /// <param name="NavItem">A navigation item to contain items in this group.</param>
        /// <param name="SampleMetadata">The samples that belong under <see cref="NavItem"/>.</param>
        private record GroupNavigationItemData(NavigationViewItem NavItem, IEnumerable<ToolkitFrontMatter> SampleMetadata);
    }
}
