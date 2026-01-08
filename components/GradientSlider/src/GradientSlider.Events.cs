// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

public partial class GradientSlider
{
    private void OnGradientStopOffsetChanged(DependencyObject d, DependencyProperty e)
    {
        if (d is not GradientStop stop)
            return;

        if (!_stopThumbs.TryGetValue(stop, out var thumb))
            return;

        Canvas.SetLeft(thumb, stop.Offset);
    }

    private void ContainerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        => SyncThumbs();
}
