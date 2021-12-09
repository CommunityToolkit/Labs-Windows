// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CommunityToolkit.Labs.Uwp.ProjectTemplate
{
    [TemplatePart(Name = CounterButtonName, Type = typeof(Button))]
    public partial class CounterButton : Control
    {
        private const string CounterButtonName = "PART_CounterButton";
        private const int DefaultCount = 0;
        private const int DefaultStep = 1;

        public static DependencyProperty CountProperty = DependencyProperty.Register(nameof(Count), typeof(int), typeof(CounterButton), new PropertyMetadata(DefaultCount, OnCountChanged));
        public static DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(int), typeof(CounterButton), new PropertyMetadata(DefaultStep));

        private static void OnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CounterButton cb)
            {
                cb.UpdateUI();
            }
        }

        private Button _counterButton = null;

        public event RoutedEventHandler Click;

        public int Count
        {
            get => (int)GetValue(CountProperty);
            set => SetValue(CountProperty, value);
        }

        public int Step
        {
            get => (int)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        public CounterButton()
        {
            DefaultStyleKey = typeof(CounterButton);
        }

        public void Increment()
        {
            Count += Step;
        }

        public void Reset()
        {
            Count = DefaultCount;
        }

        protected override void OnApplyTemplate()
        {
            if (_counterButton != null)
            {
                _counterButton.Click -= CounterButton_Click;
            }

            _counterButton = GetTemplateChild(CounterButtonName) as Button;

            if (_counterButton != null)
            {
                _counterButton.Click += CounterButton_Click;
            }

            UpdateUI();
            base.OnApplyTemplate();
        }

        private void CounterButton_Click(object sender, RoutedEventArgs e)
        {
            Increment();
            Click?.Invoke(this, e);
        }

        private void UpdateUI()
        {
            if (_counterButton != null)
            {
                _counterButton.Content = Count;
            }
        }
    }
}
