// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI;

namespace AdornersExperiment.Samples.InPlaceTextEditor;

[ToolkitSample(id: nameof(InPlaceTextEditorAdornerSample), "In place text editor Adorner", description: "A sample for showing how add a popup TextBox component via an Adorner of a TextBlock.")]
public sealed partial class InPlaceTextEditorAdornerSample : Page
{
    public InPlaceTextEditorAdornerSample()
    {
        this.InitializeComponent();
    }
}

public sealed partial class InPlaceTextEditorAdorner : Adorner<TextBlock>
{
    /// <summary>
    /// Gets or sets whether the popup is open.
    /// </summary>
    public bool IsPopupOpen
    {
        get { return (bool)GetValue(IsPopupOpenProperty); }
        set { SetValue(IsPopupOpenProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="IsPopupOpen"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsPopupOpenProperty =
        DependencyProperty.Register("IsPopupOpen", typeof(bool), typeof(InPlaceTextEditorAdorner), new PropertyMetadata(false));

    private string _originalText = string.Empty;

    public InPlaceTextEditorAdorner()
    {
        this.DefaultStyleKey = typeof(InPlaceTextEditorAdorner);

        // Uno workaround
        DataContext = this;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        AdornedElement?.Tapped += AdornedElement_Tapped;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        AdornedElement?.Tapped -= AdornedElement_Tapped;
    }

    private void AdornedElement_Tapped(object sender, TappedRoutedEventArgs e)
    {
        _originalText = AdornedElement?.Text ?? string.Empty;
        IsPopupOpen = true;
    }

    public void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        IsPopupOpen = false;
    }

    public void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        AdornedElement?.Text = _originalText;
        IsPopupOpen = false;
    }
}
