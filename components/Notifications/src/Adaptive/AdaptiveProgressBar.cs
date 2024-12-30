// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1121 // UseBuiltInTypeAlias

using System;
using CommunityToolkit.Notifications.Adaptive.Elements;
using BindableProgressBarValue = CommunityToolkit.Notifications.BindableProgressBarValue;
using BindableString = CommunityToolkit.Notifications.BindableString;

namespace CommunityToolkit.Notifications;

/// <summary>
/// New in Creators Update: A progress bar. Only supported on toasts on Desktop, build 15007 or newer.
/// </summary>
public sealed class AdaptiveProgressBar : IToastBindingGenericChild
{
    /// <summary>
    /// Gets or sets an optional title string. Supports data binding.
    /// </summary>
    public BindableString Title { get; set; }

    /// <summary>
    /// Gets or sets the value of the progress bar. Supports data binding. Defaults to 0.
    /// </summary>
    public BindableProgressBarValue Value { get; set; } = AdaptiveProgressBarValue.FromValue(0);

    /// <summary>
    /// Gets or sets an optional string to be displayed instead of the default percentage string. If this isn't provided, something like "70%" will be displayed.
    /// </summary>
    public BindableString ValueStringOverride { get; set; }

    /// <summary>
    /// Gets or sets a status string (Required), which is displayed underneath the progress bar. This string should reflect the status of the operation, like "Downloading..." or "Installing..."
    /// </summary>
    public BindableString Status { get; set; }

    internal Element_AdaptiveProgressBar ConvertToElement()
    {
        // If Value not provided, we use 0
        var val = Value;
        if (val == null)
        {
            val = AdaptiveProgressBarValue.FromValue(0);
        }

        var answer = new Element_AdaptiveProgressBar();

        answer.Title = Title?.ToXmlString();
        answer.Value = val.ToXmlString();
        answer.ValueStringOverride = ValueStringOverride?.ToXmlString();
        answer.Status = Status?.ToXmlString();

        if (answer.Status == null)
        {
            throw new NullReferenceException("Status property is required.");
        }

        return answer;
    }
}
