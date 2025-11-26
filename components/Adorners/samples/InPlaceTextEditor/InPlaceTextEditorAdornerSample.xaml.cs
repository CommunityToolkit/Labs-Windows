// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;

namespace AdornersExperiment.Samples.InPlaceTextEditor;

[ToolkitSample(id: nameof(InPlaceTextEditorAdornerSample), "In place text editor Adorner", description: "A sample for showing how add a popup TextBox component via an Adorner of a TextBlock.")]
public sealed partial class InPlaceTextEditorAdornerSample : Page
{
    public MyViewModel ViewModel { get; } = new();

    public InPlaceTextEditorAdornerSample()
    {
        this.InitializeComponent();
    }
}

/// <summary>
/// ViewModel that shows using <see cref="IEditableObject"/> in conjunction with an Adorner.
/// </summary>
public partial class MyViewModel : ObservableObject, IEditableObject
{
    [ObservableProperty]
    public partial string MyText { get; set; } = "Hello, World!";

    bool _isEditing = false;
    private string _backupText = string.Empty;

    public void BeginEdit()
    {
        if (!_isEditing)
        {
            _backupText = MyText;
            _isEditing = true;
        }
    }

    public void CancelEdit()
    {
        if (_isEditing)
        {
            MyText = _backupText;
            _isEditing = false;
        }
    }

    public void EndEdit()
    {
        if (_isEditing)
        {
            _backupText = MyText;
            _isEditing = false;
        }
    }
}

/// <summary>
/// An Adorner that shows a popup TextBox for editing a TextBlock's text.
/// If that TextBlock's DataContext implements <see cref="IEditableObject"/>,
/// it will be used to manage the editing session.
/// </summary>
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
        DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(InPlaceTextEditorAdorner), new PropertyMetadata(false));

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
        if (AdornedElement?.DataContext is IEditableObject editableObject)
        {
            editableObject.BeginEdit();
        }
        IsPopupOpen = true;
    }

    public void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        if (AdornedElement?.DataContext is IEditableObject editableObject)
        {
            editableObject.EndEdit();
        }
        IsPopupOpen = false;
    }

    public void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (AdornedElement?.DataContext is IEditableObject editableObject)
        {
            editableObject.CancelEdit();
        }
        IsPopupOpen = false;
    }
}
