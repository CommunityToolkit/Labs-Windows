// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// How the Marquee moves.
/// </summary>
public enum MarqueeBehavior
{
    /// <summary>
    /// The text flows across the screen from start to finish.
    /// </summary>
    Ticker,

    /// <summary>
    /// As the text flows across the screen a duplicate follows.
    /// </summary>
    /// <remarks>
    /// Looping text won't move if all the text already fits on the screen.
    /// </remarks>
    Looping,

    // Waiting on AutoReverse implementation for Uno storyboards
#if !HAS_UNO
    /// <summary>
    /// The text bounces back and forth across the screen.
    /// </summary>
    Bouncing,
#endif
}
