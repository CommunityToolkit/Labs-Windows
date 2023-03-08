// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WinUI;
public partial class Shimmer : Control
{
    public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
        nameof(Duration), typeof(TimeSpan), typeof(Shimmer), new PropertyMetadata(TimeSpan.FromMilliseconds(1600), (s, e) =>
        {
            var self = (Shimmer)s;
            if (self.IsActive)
            {
                self.TryStartAnimation();
            }
        }));

    public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
        nameof(IsActive), typeof(bool), typeof(Shimmer), new PropertyMetadata(true, (s, e) =>
        {
            var self = (Shimmer)s;
            var isActive = (bool)e.NewValue;

            if (isActive)
            {
                self.TryStartAnimation();
            }
            else
            {
                self.StopAnimation();
            }
        }));

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }
}
