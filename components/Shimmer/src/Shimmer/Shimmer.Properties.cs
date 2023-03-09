// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;

public partial class Shimmer : Control
{
    /// <summary>
    /// Identifies the <see cref="Duration"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
        nameof(Duration), typeof(TimeSpan), typeof(Shimmer), new PropertyMetadata(TimeSpan.FromMilliseconds(1600), (s, e) =>
        {
            var self = (Shimmer)s;
            if (self.IsActive)
            {
                self.TryStartAnimation();
            }
        }));

    /// <summary>
    /// Identifies the <see cref="IsActive"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(Shimmer), new PropertyMetadata(true, (s, e) =>
        {
            var self = (Shimmer)s;
            var isActive = (bool)e.NewValue;

            if (isActive)
            {
                self.StopAnimation();
                self.TryStartAnimation();
            }
            else
            {
                self.StopAnimation();
            }
        }));

    /// <summary>
    /// Gets or sets the animation duration
    /// </summary>
    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    /// <summary>
    /// Gets or sets if the animation is playing
    /// </summary>
    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }
}
