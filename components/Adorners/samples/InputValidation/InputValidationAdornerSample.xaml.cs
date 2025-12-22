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
