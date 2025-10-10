// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

internal static class ColorExtensions
{
    internal static float FindColorfulness(this Color color)
    {
        var vectorColor = color.ToVector3();
        var rg = vectorColor.X - vectorColor.Y;
        var yb = ((vectorColor.X + vectorColor.Y) / 2) - vectorColor.Z;
        return 0.3f * new Vector2(rg, yb).Length();
    }

    internal static float FindColorfulness(this Color[] colors)
    {
        var vectorColors = colors.Select(ToVector3);

        // Isolate rg and yb
        var rg = vectorColors.Select(x => Math.Abs(x.X - x.Y));
        var yb = vectorColors.Select(x => Math.Abs(0.5f * (x.X + x.Y) - x.Z));

        // Evaluate rg and yb mean and std
        var rg_std = FindStandardDeviation(rg, out var rg_mean);
        var yb_std = FindStandardDeviation(yb, out var yb_mean);

        // Combine means and standard deviations
        var std = new Vector2(rg_mean, yb_mean).Length();
        var mean = new Vector2(rg_std, yb_std).Length();

        // Return colorfulness
        return std + (0.3f * mean);
    }

    internal static Color ToColor(this Vector3 color)
    {
        color *= 255;
        return Color.FromArgb(255, (byte)(color.X), (byte)(color.Y), (byte)(color.Z));
    }

    internal static Vector3 ToVector3(this Color color)
    {
        var vector = new Vector3(color.R, color.G, color.B);
        return vector / 255;
    }

    private static float FindStandardDeviation(IEnumerable<float> data, out float avg)
    {
        var average = data.Average();
        avg = average;
        var sumOfSquares = data.Select(x => (x - average) * (x - average)).Sum();
        return (float)Math.Sqrt(sumOfSquares / data.Count());
    }
}
