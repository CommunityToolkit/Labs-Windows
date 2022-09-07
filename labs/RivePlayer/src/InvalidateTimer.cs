// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINDOWS_WINAPPSDK
#define HAS_PERIODIC_TIMER
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

#if HAS_PERIODIC_TIMER
using System.Threading;
#endif

namespace CommunityToolkit.Labs.WinUI.Rive;

// Continuously calls Invalidate() on the given RivePlayer at a set fps.
internal class InvalidateTimer : IDisposable
{
    private readonly RivePlayer rivePlayer;
    private readonly TimeSpan timeSpanPerFrame;

    public InvalidateTimer(RivePlayer rivePlayer, double fps)
    {
        this.rivePlayer = rivePlayer;
        timeSpanPerFrame = TimeSpan.FromSeconds(1.0 / fps);
    }

    private bool _running = false;
#if HAS_PERIODIC_TIMER
    private PeriodicTimer? _timer;
#else
    // Incremented when the "InvalLoopAsync" should terminate.
    int _invalLoopContinuationToken = 0;
#endif

    public void Dispose()
    {
        Stop();
    }

    public void Start()
    {
        if (_running)
        {
            return;
        }
#if HAS_PERIODIC_TIMER
        _timer = new PeriodicTimer(timeSpanPerFrame);
        InvalLoopAsync(_timer);
#else
        InvalLoopAsync(_invalLoopContinuationToken);
#endif
        _running = true;
    }

    public void Stop()
    {
        if (!_running)
        {
            return;
        }
#if HAS_PERIODIC_TIMER
        _timer!.Dispose(); // Stop the current inval loop.
        _timer = null;
#else
        ++_invalLoopContinuationToken;  // Terminate the existing inval loop (if any).
#endif
        _running = false;
    }

#if HAS_PERIODIC_TIMER
    private async void InvalLoopAsync(PeriodicTimer timer)
#else
    private async void InvalLoopAsync(int continuationToken)
#endif
    {
#if HAS_PERIODIC_TIMER
        while (await timer.WaitForNextTickAsync())
#else
        while (continuationToken == _invalLoopContinuationToken)
#endif
        {
            if (!rivePlayer.IsLoaded)
            {
                break;
            }
            rivePlayer.Invalidate();
#if !HAS_PERIODIC_TIMER
            await Task.Delay(timeSpanPerFrame);
#endif
        }
    }
}
