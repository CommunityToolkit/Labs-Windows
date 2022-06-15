// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CanvasLayout.Sample.SampleTwo;

[ToolkitSampleOptionsPane(sampleId: nameof(SamplePage2))]
public sealed partial class SamplePageOptions : UserControl
{
    private readonly SamplePage2 _samplePage;
    private SamplePage2.XamlNamedPropertyRelay _xamlProperties;

    public SamplePageOptions(SamplePage2 samplePage)
    {
        Loaded += SamplePageOptions_Loaded;

        _samplePage = samplePage;
        _xamlProperties = new SamplePage2.XamlNamedPropertyRelay(_samplePage);

        this.InitializeComponent();
    }

    private void SamplePageOptions_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= SamplePageOptions_Loaded;

        CustomText.Text = _xamlProperties.PrimaryText.Text;
        FontSizeSlider.Value = _xamlProperties.PrimaryText.FontSize;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _xamlProperties.PrimaryText.Text = ((TextBox)sender).Text;
    }

    private void OnRadioButtonSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is RadioButtons radioButtons)
        {
            if (radioButtons.SelectedItem is null)
                return;

            var selectedColor = (string)radioButtons.SelectedItem;

            _xamlProperties.PrimaryText.Foreground = (SolidColorBrush)XamlBindingHelper.ConvertValue(typeof(SolidColorBrush), selectedColor);
        }
    }

    private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (_xamlProperties.PrimaryText is not null && IsLoaded && _samplePage.IsLoaded)
            _xamlProperties.PrimaryText.FontSize = ((Slider)sender).Value;
    }
}
