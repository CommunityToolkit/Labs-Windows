// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Storage;

namespace CommunityToolkit.Labs.Shared.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

         
            int? selectedTheme = ApplicationData.Current.LocalSettings.Values["Theme"] as int?;

            if (selectedTheme == null)
            {
                ThemeSelectionBox.SelectedIndex = 0;
            }
            else
            {
                ThemeSelectionBox.SelectedIndex = (int)selectedTheme;
            }
            ThemeSelectionBox.SelectionChanged += ThemeSelectionBox_SelectionChanged;

        }

        private void ThemeSelectionBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["Theme"] = ThemeSelectionBox.SelectedIndex;
        }
    }
}
