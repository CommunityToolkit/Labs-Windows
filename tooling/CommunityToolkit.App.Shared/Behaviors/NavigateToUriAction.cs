// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Xaml.Interactivity;

namespace CommunityToolkit.App.Shared.Behaviors;

//// TODO: Check with https://github.com/CommunityToolkit/Labs-Windows/issues/174
#pragma warning disable CA1001 // Type 'NavigateToUriAction' owns disposable field(s) '__storeBackingField' but is not disposable. From Uno - Gtk, Skia/WPF, WASM
public sealed partial class NavigateToUriAction : DependencyObject, IAction
#pragma warning restore CA1001 // Type 'NavigateToUriAction' owns disposable field(s) '__storeBackingField' but is not disposable. From Uno - Gtk, Skia/WPF, WASM
{
    /// <summary>
    /// Gets or sets the linked <see cref="NavigateUri"/> instance to invoke.
    /// </summary>
    public Uri NavigateUri
    {
        get => (Uri)GetValue(NavigateUriProperty);
        set => SetValue(NavigateUriProperty, value);
    }

    /// <summary>
    /// Identifies the <seealso cref="NavigateUri"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty NavigateUriProperty = DependencyProperty.Register(
        nameof(NavigateUri),
        typeof(Uri),
        typeof(NavigateToUriAction),
        new PropertyMetadata(null));

    /// <inheritdoc/>
    public object Execute(object sender, object parameter)
    {
        if (NavigateUri != null)
        {
            _ = Windows.System.Launcher.LaunchUriAsync(NavigateUri);
        }
        else
        {
            throw new ArgumentNullException(nameof(NavigateUri));
        }

        return true;
    }
}
