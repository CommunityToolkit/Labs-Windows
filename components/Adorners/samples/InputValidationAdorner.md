---
title: InputValidationAdorner
author: michael-hawker
description: An InputValidationAdorner provides input validation to any element implementing INotifyDataErrorInfo to provide feedback to a user.
keywords: Adorners, Input Validation, INotifyDataErrorInfo, MVVM, CommunityToolkit.Mvvm
dev_langs:
  - csharp
category: Controls
subcategory: Layout
discussion-id: 278
issue-id: 0
icon: assets/icon.png
---

# InputValidationAdorner

The `InputValidationAdorner` provides input validation to any element implementing `INotifyDataErrorInfo` to provide feedback to a user.

## Background

Input Validation existed in WPF and was available in a couple of ways. See the Migrating from WPF section below for more details on the differences between WPF and WinUI Input Validation.

## Input Validation Example

The `InputValidationAdorner` can be attached to any element and triggered to be shown automatically based on validation provided by the `INotifyDataErrorInfo` interface set on the `NotifyDataErrorInfo` property of the adorner.

The custom adorner will automatically display the validation message for the specified `PropertyName` is marked as invalid by the `INotifyDataErrorInfo` implementation.

For the example below, we use the `ObservableValidator` class from the `CommunityToolkit.Mvvm` package to provide automatic validation of the rules within our view model properties.
When the user submits invalid input, the adorner displays a red border around the text box and shows a tooltip with the validation error message:

> [!SAMPLE InputValidationAdornerSample]

## Migrating from WPF

Input Validation within WinUI is handled as a mix of both of WPF's [Binding Validation](https://learn.microsoft.com/dotnet/desktop/wpf/data/how-to-implement-binding-validation) and [Custom Object Validation](https://learn.microsoft.com/dotnet/desktop/wpf/data/how-to-implement-validation-logic-on-custom-objects).

> [!WARNING]
> That the WinUI Adorner uses the `INotifyDataErrorInfo` interface for validation feedback, whereas WPF's Custom Object Validation uses the `IDataErrorInfo` interface. You will need to adapt your validation logic accordingly when migrating from WPF to WinUI.

> [!NOTE]
> The `ValidationRule` Binding concept from WPF is not supported in WinUI. You will need to implement validation logic within your view model or data model using the `INotifyDataErrorInfo` interface instead.
> You can still specify a custom error template by styling the `InputValidationAdorner` control.

When paired with the validation provided by the `CommunityToolkit.Mvvm` package, you can achieve similar functionality to WPF's Input Validation with less boilerplate code.
