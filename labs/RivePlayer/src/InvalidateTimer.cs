// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace CommunityToolkit.Labs.WinUI.Rive;

// Continuously calls Invalidate() on the given RivePlayer at a set fps.
internal partial class InvalidateTimer
{
    private readonly RivePlayer rivePlayer;
    private readonly TimeSpan timeSpanPerFrame;

    public InvalidateTimer(RivePlayer rivePlayer, double fps)
    {
        this.rivePlayer = rivePlayer;
        timeSpanPerFrame = TimeSpan.FromSeconds(1.0 / fps);
    }

    private bool _running = false;
    // Incremented when the "InvalLoopAsync" should terminate.
    int _invalLoopContinuationToken = 0;

    public void Start()
    {
        if (_running)
        {
            return;
        }
        InvalLoopAsync(_invalLoopContinuationToken);
        _running = true;
    }

    public void Stop()
    {
        if (!_running)
        {
            return;
        }
        ++_invalLoopContinuationToken;  // Terminate the existing inval loop (if any).
        _running = false;
    }

    private async void InvalLoopAsync(int continuationToken)
    {
        while (continuationToken == _invalLoopContinuationToken)
        {
            if (!rivePlayer.IsLoaded)
            {
                break;
            }
            rivePlayer.Invalidate();
            await Task.Delay(timeSpanPerFrame);
        }
    }
}
