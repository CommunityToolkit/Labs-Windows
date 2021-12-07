using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CounterButton.Library
{
    [TemplatePart(Name = CounterButtonName, Type = typeof(Button))]
    [TemplatePart(Name = CounterButtonTextBlockName, Type = typeof(TextBlock))]
    public partial class CounterButton : ButtonBase
    {
        private const string CounterButtonName = "PART_CounterButton";
        private const string CounterButtonTextBlockName = "PART_CounterButtonTextBlock";

        private Button _counterButton;

        public static DependencyProperty CountProperty = DependencyProperty.Register(nameof(Count), typeof(int), typeof(CounterButton), new PropertyMetadata(0));

        public new RoutedEventHandler Click;

        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public CounterButton()
        {
            DefaultStyleKey = typeof(CounterButton);
            DataContext = this;
        }

        public void Increment()
        {
            Count += 1;
        }

        protected override void OnApplyTemplate()
        {
            if(_counterButton != null)
            {
                _counterButton.Click -= CounterButton_Click;
            }

            _counterButton = GetTemplateChild(CounterButtonName) as Button;

            if (_counterButton != null)
            {
                _counterButton.Click += CounterButton_Click;
            }

            base.OnApplyTemplate();
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            Increment();
            Click.Invoke(this, e);
        }
    }
}
