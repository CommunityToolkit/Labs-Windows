// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;

namespace CommunityToolkit.Labs.WinUI;

public partial class Segmented : ListViewBase
{
    /// <summary>
    /// The backing <see cref="DependencyProperty"/> for the <see cref="AutoSelection"/> property.
    /// </summary>
    public static readonly DependencyProperty AutoSelectionProperty = DependencyProperty.Register(
        nameof(AutoSelection),
        typeof(bool),
        typeof(Segmented),
        new PropertyMetadata(defaultValue: true));

    /// <summary>
    /// Gets or sets a value that indicates if the first item should be auto-selected on load.
    /// </summary>
    public bool AutoSelection
    {
        get => (bool)GetValue(AutoSelectionProperty);
        set => SetValue(AutoSelectionProperty, value);
    }
}
