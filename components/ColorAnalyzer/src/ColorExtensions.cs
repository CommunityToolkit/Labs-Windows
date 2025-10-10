// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Windows.UI;

namespace CommunityToolkit.WinUI.Helpers;

internal static class ColorExtensions
{
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

    /// <summary>
    /// Get WCAG contrast ratio between two colors.
    /// </summary>
    internal static double ContrastRatio(this Color color1, Color color2)
    {
        // Using the formula for contrast ratio
        // Source WCAG guidelines: https://www.w3.org/TR/WCAG20/#contrast-ratiodef

        // Calculate perceived luminance for both colors
        double luminance1 = color1.PerceivedLuminance();
        double luminance2 = color2.PerceivedLuminance();

        // Determine lighter and darker luminance
        double lighter = Math.Max(luminance1, luminance2);
        double darker = Math.Min(luminance1, luminance2);

        // Calculate contrast ratio
        return (lighter + 0.05f) / (darker + 0.05f);
    }

    internal static double PerceivedLuminance(this Color color)
    {
        // Color theory is a massive iceberg. Here's a peek at the tippy top:

        // There's two (main) standards for calculating luminance from RGB values.

        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +
        // | Standard      | Formula                              | Ref. Section       | Ref. Link                                                                       |
        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +
        // | ITU Rec. 709  | Y = 0.2126 R + 0.7152 G + 0.0722 B   | Page 4/Item 3.2    | https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf  |
        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +
        // | ITU Rec. 601  | Y = 0.299 R + 0.587 G + 0.114 B      | Page 2/Item 2.5.1  | https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.601-7-201103-I!!PDF-E.pdf  |
        // + ------------- + ------------------------------------ + ------------------ + ------------------------------------------------------------------------------- +

        // They're based on the standard ability of the human eye to perceive brightness,
        // from different colors, as well as the average monitor's ability to produce them.
        // Both standards produce similar results, but Rec. 709 is more accurate for modern displays.

        // NOTE: If we for whatever reason we ever need to optimize this code,
        // we can make approximations using integer math instead of floating point math.
        // The precise values are not critical, as long as the relative luminance is accurate.
        // Like so: return (2 * color.R + 7 * color.G + color.B);

        // TLDR: We use ITU Rec. 709 standard formula for perceived luminance.
        return (0.2126f * color.R + 0.7152f * color.G + 0.0722 * color.B) / 255;
    }

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

    private static float FindStandardDeviation(IEnumerable<float> data, out float avg)
    {
        var average = data.Average();
        avg = average;
        var sumOfSquares = data.Select(x => (x - average) * (x - average)).Sum();
        return (float)Math.Sqrt(sumOfSquares / data.Count());
    }
}
