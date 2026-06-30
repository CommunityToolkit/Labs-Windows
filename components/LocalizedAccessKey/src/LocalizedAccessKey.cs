// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using Windows.System;
using static Windows.Win32.PInvoke;

namespace CommunityToolkit.WinUI.Controls;

/// <summary>
/// A markup extension to localize the access key of a control based on the current user keyboard layout.
/// By default, WinUI requires the user to type the requested access key sequence. Not all characters can be typed
/// on all keyboard layouts. This extension allows us to specify a sequence of <see cref="VirtualKey"/>s that will
/// be /// converted to the current keyboard layout characters. This allows us to specify the access key only once
/// and be sure that they will work on all keyboard layouts.
/// </summary>
public sealed partial class LocalizedAccessKey : MarkupExtension
{
    private static readonly byte[] _baseKeyState;
    private static readonly byte[] _shiftKeyState;

    static LocalizedAccessKey()
    {
        _baseKeyState = new byte[256];
        _shiftKeyState = new byte[256];
        _shiftKeyState[(int)VirtualKey.Shift] = 0x80;
    }

    /// <summary>
    /// The virtual keys to use for the access key.
    /// The provided virtual keys will be converted to the current keyboard layout.
    /// </summary>
    public VirtualKeySequence? VirtualKeys { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue() => VirtualKeys is null
        ? string.Empty
        : string.Concat(VirtualKeys.Items.Select(GetLocalizedAccessKey));


#if WINAPPSDK
    /// <inheritdoc/>
    protected override object ProvideValue(IXamlServiceProvider serviceProvider) => ProvideValue();
#endif

    private static string GetLocalizedAccessKey(VirtualKey virtualKey)
    {
        var baseChar = GetVirtualKeyChars(virtualKey, shiftState: false);
        if (baseChar is null)
        {
            // Fallback for empty keys – use the character of the VK itself.
            return ((char)virtualKey).ToString();
        }

        if (baseChar == "ς")
        {
            // Special case for Greek letter sigma.
            return "<";
        }

        if (baseChar.Length == 1)
        {
            var shiftChar = GetVirtualKeyChars(virtualKey, shiftState: true);
            if (shiftChar?.Length == 1 && (char.ToUpper(baseChar[0]) == shiftChar[0] || char.IsDigit(shiftChar[0])))
            {
                // Choose number or uppercase version of letter if possible.
                return shiftChar;
            }
        }

        // If none of the above applies, choose the base character(s) for the access key.
        return baseChar;
    }

    private static string? GetVirtualKeyChars(VirtualKey virtualKey, bool shiftState)
    {
        string? result = null;
        var scanCode = MapVirtualKey((uint)virtualKey, 0);
        var buffer = ArrayPool<char>.Shared.Rent(4);

        var keyState = shiftState ? _shiftKeyState : _baseKeyState;
        var size = ToUnicode((uint)virtualKey, scanCode, keyState.AsSpan(), buffer.AsSpan(), wFlags: 4);
        if (size > 0)
        {
            result = string.Concat(buffer.Take(size));
        }

        ArrayPool<char>.Shared.Return(buffer);
        return result;
    }
}
