// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A base class for a color data source in the <see cref="ColorPaletteSampler"/>.
/// </summary>
public abstract partial class ColorSource : DependencyObject
{
    /// <summary>
    /// An event invoked when the source pixels changed.
    /// </summary>
    public event EventHandler? SourceUpdated;

    /// <summary>
    /// Retreives the pixels from the source as a stream
    /// </summary>
    /// <param name="requestedSamples">The number of samples requested by the <see cref="ColorPaletteSampler"/>.</param>
    /// <returns>A stream of pixels in rgba format.</returns>
    public abstract Task<Stream?> GetPixelDataAsync(int requestedSamples);

    /// <summary>
    /// Invokes the <see cref="SourceUpdated"/> event.
    /// </summary>
    protected void InvokeSourceUpdated() => SourceUpdated?.Invoke(this, EventArgs.Empty);
}
