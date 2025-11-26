// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;

using System.ComponentModel.DataAnnotations;

using INotifyDataErrorInfo = System.ComponentModel.INotifyDataErrorInfo;
using DataErrorsChangedEventArgs = System.ComponentModel.DataErrorsChangedEventArgs;

namespace AdornersExperiment.Samples.InputValidation;

[ToolkitSample(id: nameof(InputValidationAdornerSample), "Input Validation Adorner", description: "A sample for showing how to use an Adorner for any Input Validation with INotifyDataErrorInfo.")]
public sealed partial class InputValidationAdornerSample : Page
{
    public ValidationFormWidgetViewModel ViewModel { get; } = new();

    public InputValidationAdornerSample()
    {
        this.InitializeComponent();
    }
}

/// <summary>
/// ViewModel that shows using <see cref="INotifyDataErrorInfo"/> in conjunction with an Adorner.
/// Via the <see cref="ObservableValidator"/> base class from the MVVM Toolkit.
/// Example modified from the MVVM Toolkit Sample App.
/// </summary>
public partial class ValidationFormWidgetViewModel : ObservableValidator
{
    public event EventHandler? FormSubmissionCompleted;
    public event EventHandler? FormSubmissionFailed;

    [ObservableProperty]
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public partial string? FirstName { get; set; }

    [ObservableProperty]
    [Required]
    [MinLength(2)]
    [MaxLength(100)]
    public partial string? LastName { get; set; }

    [ObservableProperty]
    [Required]
    [EmailAddress]
    public partial string? Email { get; set; }

    [ObservableProperty]
    [Required]
    [Phone]
    public partial string? PhoneNumber { get; set; }

    [ObservableProperty]
    [Required]
    [Range(13, 120)]
    public partial int Age { get; set; }

    [RelayCommand]
    private void Submit()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            FormSubmissionFailed?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            FormSubmissionCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}

/// <summary>
/// An Adorner that shows an error message if Data Validation fails.
/// The adorned control's <see cref="FrameworkElement.DataContext"/> must implement <see cref="INotifyDataErrorInfo"/>. It assumes that the return of <see cref="INotifyDataErrorInfo.GetErrors(string?)"/> is a <see cref="ValidationResult"/> collection.
/// </summary>
public sealed partial class InputValidationAdorner : Adorner<FrameworkElement>
{
    /// <summary>
    /// Gets or sets the name of the property this adorner should look for errors on.
    /// </summary>
    public string PropertyName
    {
        get { return (string)GetValue(PropertyNameProperty); }
        set { SetValue(PropertyNameProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="PropertyName"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PropertyNameProperty =
        DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(InputValidationAdorner), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets whether the popup is open.
    /// </summary>
    public bool HasValidationFailed
    {
        get { return (bool)GetValue(HasValidationFailedProperty); }
        set { SetValue(HasValidationFailedProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="HasValidationFailed"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty HasValidationFailedProperty =
        DependencyProperty.Register(nameof(HasValidationFailed), typeof(bool), typeof(InputValidationAdorner), new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets the validation message for this failed property.
    /// </summary>
    public string ValidationMessage
    {
        get { return (string)GetValue(ValidationMessageProperty); }
        set { SetValue(ValidationMessageProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="ValidationMessage"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ValidationMessageProperty =
        DependencyProperty.Register(nameof(ValidationMessage), typeof(string), typeof(InputValidationAdorner), new PropertyMetadata(null));

    private INotifyDataErrorInfo? _dataErrorInfo;

    public InputValidationAdorner()
    {
        this.DefaultStyleKey = typeof(InputValidationAdorner);

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

        if (AdornedElement?.DataContext is INotifyDataErrorInfo iError)
        {
            _dataErrorInfo = iError;
            _dataErrorInfo.ErrorsChanged += this.INotifyDataErrorInfo_ErrorsChanged;
        }
    }

    private void INotifyDataErrorInfo_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        if (_dataErrorInfo is not null)
        {
            // Reset state
            if (!_dataErrorInfo.HasErrors)
            {
                HasValidationFailed = false;
                ValidationMessage = string.Empty;
                return;
            }

            if (e.PropertyName == PropertyName)
            {
                HasValidationFailed = true;

                StringBuilder message = new();
                foreach (ValidationResult result in _dataErrorInfo.GetErrors(e.PropertyName))
                {
                    message.AppendLine(result.ErrorMessage);
                }

                ValidationMessage = message.ToString().Trim();
            }
        }
    }

    protected override void OnDetaching()
    {
        if (_dataErrorInfo is not null)
        {
            _dataErrorInfo.ErrorsChanged -= this.INotifyDataErrorInfo_ErrorsChanged;
            _dataErrorInfo = null;
        }

        base.OnDetaching();
    }
}
