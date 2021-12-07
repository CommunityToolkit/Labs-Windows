using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CounterButton.Samples
{
    public sealed partial class AdvancedSample : Page
    {
        public AdvancedSample()
        {
            InitializeComponent();
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Library.CounterButton cb && cb.Count > 9000)
            {
                cb.Background = new SolidColorBrush(Colors.LightSalmon);
                cb.Foreground = new SolidColorBrush(Colors.Crimson);
                cb.BorderBrush = new SolidColorBrush(Colors.Red);
                cb.BorderThickness = new Thickness(4);
            }
        }
    }
}
