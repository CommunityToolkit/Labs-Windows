// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

/// <summary>
/// A struct containing accent color info.
/// </summary>
public readonly struct AccentColorInfo
{
    internal AccentColorInfo(Vector3 rgb, float prominence)
    {
        Colorfulness = AccentAnalyzer.FindColorfulness(rgb);

        rgb *= byte.MaxValue;
        Color = Color.FromArgb(byte.MaxValue, (byte)rgb.X, (byte)rgb.Y, (byte)rgb.Z);
        Prominence = prominence;
    }

    /// <summary>
    /// Gets the <see cref="Windows.UI.Color"/> of the accent color.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Gets the colorfulness index of the accent color.
    /// </summary>
    /// <remarks>
    /// The exact definition of colorfulness is defined by David Hasler and Sabine Susstrunk's paper on measuring colorfulness
    /// <seealso href="https://infoscience.epfl.ch/server/api/core/bitstreams/77f5adab-e825-4995-92db-c9ff4cd8bf5a/content"/>.
    ///
    /// Colors of high saturation and value will have a high colorfulness (around 1),
    /// while colors that are mostly gray or white will have a low colorfulness (around 0).
    /// </remarks>
    public float Colorfulness { get; }

    /// <summary>
    /// Gets the prominence of the accent color in the sampled image.
    /// </summary>
    public float Prominence { get; }
}
