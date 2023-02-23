// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Tooling.SampleGen.Metadata;
using CommunityToolkit.Tooling.SampleGen;

namespace CommunityToolkit.App.Shared.Pages
{
    public sealed partial class GettingStartedPage : Page
    {
        public GettingStartedPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the items used for navigating.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            controlsGridView.ItemsSource = e.Parameter as IEnumerable<ToolkitFrontMatter>;
            base.OnNavigatedTo(e);
        }

        private void controlsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedSample = e.ClickedItem as ToolkitFrontMatter;

            Shell.Current?.NavigateToSample(selectedSample);

        }
    }
}
