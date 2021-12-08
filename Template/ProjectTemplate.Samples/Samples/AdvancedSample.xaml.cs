// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CommunityToolkit.Labs.Uwp.ProjectTemplate.Samples
{
    public sealed partial class AdvancedSample : Page
    {
        public AdvancedSample()
        {
            InitializeComponent();
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CounterButton cb && cb.Count > 9000)
            {
                cb.Background = new SolidColorBrush(Colors.LightSalmon);
                cb.Foreground = new SolidColorBrush(Colors.Crimson);
                cb.BorderBrush = new SolidColorBrush(Colors.Red);
                cb.BorderThickness = new Thickness(4);
            }
        }
    }
}
