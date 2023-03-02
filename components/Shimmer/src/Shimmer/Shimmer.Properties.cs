// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunityToolkit.Labs.WinUI;
public partial class Shimmer : Control
{
    public static readonly DependencyProperty AnimationDurationInMillisecondsProperty = DependencyProperty.Register(
        nameof(AnimationDurationInMilliseconds), typeof(double), typeof(Shimmer), new PropertyMetadata(1600d, (s, e) =>
            {
                var self = (Shimmer)s;
                if (self.IsAnimating)
                {
                    self.TryStartAnimation();
                }
            }));

    public static readonly DependencyProperty IsAnimatingProperty = DependencyProperty.Register(
        nameof(IsAnimating), typeof(bool), typeof(Shimmer), new PropertyMetadata(true, (s, e) =>
            {
                var self = (Shimmer)s;
                var isAnimating = (bool)e.NewValue;

                if (isAnimating)
                {
                    self.TryStartAnimation();
                }
                else
                {
                    self.StopAnimation();
                }
            }));

    public double AnimationDurationInMilliseconds
    {
        get => (double)GetValue(AnimationDurationInMillisecondsProperty);
        set => SetValue(AnimationDurationInMillisecondsProperty, value);
    }

    public bool IsAnimating
    {
        get => (bool)GetValue(IsAnimatingProperty);
        set => SetValue(IsAnimatingProperty, value);
    }

}
