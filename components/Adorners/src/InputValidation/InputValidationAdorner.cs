// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.DataAnnotations;

using INotifyDataErrorInfo = System.ComponentModel.INotifyDataErrorInfo;
using DataErrorsChangedEventArgs = System.ComponentModel.DataErrorsChangedEventArgs;

namespace CommunityToolkit.WinUI.Adorners;

/// <summary>
/// An Adorner that shows an error message if Data Validation fails.
/// Set the <see cref="NotifyDataErrorInfo"/> with the object that must implement <see cref="INotifyDataErrorInfo"/>. It assumes that the return of <see cref="INotifyDataErrorInfo.GetErrors(string?)"/> is a <see cref="ValidationResult"/> or string collection.
/// Adorner is shown automatically when the <see cref="INotifyDataErrorInfo.ErrorsChanged"/> event is raised and the <see cref="PropertyName"/> matches the invalid property of the event arguments.
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
        DependencyProperty.Register(nameof(PropertyName), typeof(string), typeof(InputValidationAdorner), new PropertyMetadata(null, (s, e) => (s as InputValidationAdorner)?.RefreshErrors()));

    /// <summary>
    /// Gets or sets the <see cref="INotifyDataErrorInfo"/> context object to use for validation.
    /// </summary>
    public INotifyDataErrorInfo NotifyDataErrorInfo
    {
        get { return (INotifyDataErrorInfo)GetValue(NotifyDataErrorInfoProperty); }
        set { SetValue(NotifyDataErrorInfoProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="NotifyDataErrorInfo"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NotifyDataErrorInfoProperty =
        DependencyProperty.Register(nameof(NotifyDataErrorInfo), typeof(INotifyDataErrorInfo), typeof(InputValidationAdorner), new PropertyMetadata(null, (s, e) => (s as InputValidationAdorner)?.RefreshErrors()));

    /// <summary>
    /// Gets or sets whether the validation adorners is displayed (handled automatically).
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
    /// Gets or sets the validation message for this failed property, set automatically by the adorner.
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

    // TODO: Do we consider an InfoBar style Severity property?

    /// <summary>
    /// Initializes a new instance of the <see cref="InputValidationAdorner"/> class.
    /// </summary>
    public InputValidationAdorner()
    {
        this.DefaultStyleKey = typeof(InputValidationAdorner);

        // Uno workaround
        DataContext = this;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
    }

    /// <inheritdoc/>
    protected override void OnAttached()
    {
        base.OnAttached();

        NotifyDataErrorInfo?.ErrorsChanged += this.INotifyDataErrorInfo_ErrorsChanged;
    }

    private void INotifyDataErrorInfo_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
    {
        RefreshErrors();
    }

    private void RefreshErrors()
    {
        // Check if we have any errors for our specified property
        if (NotifyDataErrorInfo is not null
            && PropertyName is not null
            && NotifyDataErrorInfo.GetErrors(PropertyName) is IEnumerable errors
            && errors.Cast<object>().Any())
        {
            // Build up error messages depending on error collection type.
            StringBuilder message = new();
            if (errors is IEnumerable<ValidationResult> validationResults)
            {
                foreach (ValidationResult result in validationResults)
                {
                    message.AppendLine(result.ErrorMessage);
                }
            }
            else if (errors is IEnumerable<string> stringErrors)
            {
                foreach (string result in stringErrors)
                {
                    message.AppendLine(result);
                }
            }
            else
            {
                // TODO: Not sure if should handle more types of collections here?
                throw new ArgumentException("The errors returned by INotifyDataErrorInfo.GetErrors must be of type IEnumerable<ValidationResult> or IEnumerable<string>.");
            }

            HasValidationFailed = true;
            ValidationMessage = message.ToString().Trim();
        }
        else
        {
            // Hide if we have no object or errors to validate against.
            HasValidationFailed = false;
            ValidationMessage = string.Empty;
        }
    }

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        NotifyDataErrorInfo?.ErrorsChanged -= this.INotifyDataErrorInfo_ErrorsChanged;

        base.OnDetaching();
    }
}
