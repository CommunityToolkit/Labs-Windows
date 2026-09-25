// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// Represents a sequence of <see cref="VirtualKey"/> elements.
/// </summary>
/// <remarks>This is the type used by <see cref="LocalizedAccessKey"/>.</remarks>
[Windows.Foundation.Metadata.CreateFromString(MethodName = "CommunityToolkit.WinUI.Controls.VirtualKeySequence.ConvertToVirtualKeySequence")]
public sealed class VirtualKeySequence
{
    private VirtualKeySequence(params VirtualKey[] virtualKeys) => Items = virtualKeys;

    /// <summary>
    /// The collection of <see cref="VirtualKey"/> representing the sequence.
    /// </summary>
    public IReadOnlyCollection<VirtualKey> Items { get; }

    /// <summary>
    /// Creates a <see cref="VirtualKeySequence"/> instance.
    /// </summary>
    /// <param name="virtualKeys">A sequence of <see cref="VirtualKey"/> elements.</param>
    /// <returns>A new <see cref="VirtualKeySequence"/> instance or null if <paramref name="virtualKeys"/> is null or empty.</returns>
    public static VirtualKeySequence? Create(params VirtualKey[] virtualKeys)
        => virtualKeys is null || virtualKeys.Length == 0
            ? null
            : new VirtualKeySequence(virtualKeys);

    /// <summary>
    /// The converter used to convert a string to a <see cref="VirtualKeySequence"/> instance.
    /// </summary>
    /// <param name="rawString">The string to convert.</param>
    /// <returns>A <see cref="VirtualKeySequence"/> instance or null in case of parsing error.</returns>
    public static VirtualKeySequence? ConvertToVirtualKeySequence(string rawString)
    {
        if (string.IsNullOrEmpty(rawString))
        {
            return null;
        }

        ReadOnlySpan<char> chars = rawString.AsSpan();
        var virtualKeys = new VirtualKey[chars.Length];
        for (var i = 0; i < chars.Length; i++)
        {
            var character = chars.Slice(i, 1);
            if (char.IsDigit(character[0]))
            {
#if WINUI2
                virtualKeys[i] = VirtualKey.Number0 + int.Parse(character.ToString());
#else
                virtualKeys[i] = VirtualKey.Number0 + int.Parse(character);
#endif
            }
            else if (char.IsLetter(character[0]) &&
#if WINUI2
                Enum.TryParse<VirtualKey>(chars.Slice(i, 1).ToString(), out var vKey) &&
#else
                Enum.TryParse<VirtualKey>(chars.Slice(i, 1), out var vKey) &&
#endif
                vKey != VirtualKey.None)
            {
                virtualKeys[i] = vKey;
            }
            else
            {
                return null;
            }
        }

        return Create(virtualKeys);
    }
}
