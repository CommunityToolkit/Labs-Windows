// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CommunityToolkit.Labs.Uwp
{
    [TemplatePart(Name = CountTextBlockName, Type = typeof(TextBlock))]
    public partial class CounterButton : Button
    {
        private enum CommonStates
        {
            Normal,
            PointerOver,
            Pressed,
            Disabled
        }

        private const string CountTextBlockName = "PART_CountTextBlock";
        private const int DefaultCount = 0;
        private const int DefaultStep = 1;

        public static DependencyProperty CountProperty = DependencyProperty.Register(nameof(Count), typeof(int), typeof(CounterButton), new PropertyMetadata(DefaultCount, OnCountChanged));
        public static DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(int), typeof(CounterButton), new PropertyMetadata(DefaultStep));

        private static void OnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CounterButton cb)
            {
                cb.UpdateUI();
                cb.CountChanged?.Invoke(cb, new EventArgs());
            }
        }

        private TextBlock _countTextBlock = null;

        public event EventHandler CountChanged;

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

            Click += (s, e) => Increment();
            IsEnabledChanged += (s, e) => UpdateVisualState(CommonStates.Normal);
            PointerEntered += (s, e) => UpdateVisualState(CommonStates.PointerOver);
            PointerExited += (s, e) => UpdateVisualState(CommonStates.Normal);
            PointerPressed += (s, e) => UpdateVisualState(CommonStates.Pressed);
            PointerReleased += (s, e) => UpdateVisualState(CommonStates.PointerOver);
            KeyUp += this.OnKeyUp;

            AutomationProperties.SetAutomationId(this, nameof(CounterButton));
        }

        public void Increment()
        {
            Count += Step;
        }

        protected override void OnApplyTemplate()
        {
            _countTextBlock = GetTemplateChild(CountTextBlockName) as TextBlock;
            UpdateUI();
        }

        private void OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Enter:
                case VirtualKey.Space:
                    Increment();
                    break;
            }
        }

        private void UpdateUI()
        {
            if (_countTextBlock != null)
            {
                _countTextBlock.Text = Count.ToString();
            }
        }

        private void UpdateVisualState(CommonStates state)
        {
            if (!IsEnabled)
            {
                state = CommonStates.Disabled;
            }

            VisualStateManager.GoToState(this, state.ToString(), true);
        }
    }
}
