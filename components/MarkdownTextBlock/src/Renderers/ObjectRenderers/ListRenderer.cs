// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using CommunityToolkit.WinUI.Controls.TextElements;
using Markdig.Syntax;
using RomanNumerals;

namespace CommunityToolkit.WinUI.Controls.Renderers.ObjectRenderers;

internal class ListRenderer : UWPObjectRenderer<ListBlock>
{
    public const string UnorderedListDot = "• ";

    protected override void Write(WinUIRenderer renderer, ListBlock listBlock)
    {
        int index = 1;
        bool isOrdered = false;
        BulletType bulletType = BulletType.Circle;
        if (listBlock.IsOrdered)
        {
            isOrdered = true;
            bulletType = ToOrderedBulletType(listBlock.BulletType);

            if (listBlock.OrderedStart != null && listBlock.DefaultOrderedStart != listBlock.OrderedStart)
            {
                int.TryParse(listBlock.OrderedStart, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out index);
            }
        }

        foreach (var listItem in listBlock)
        {
            renderer.PushListBullet(GetBulletString(isOrdered, bulletType, index));
            renderer.Write(listItem);
            renderer.PopListBullet();
            index++;
        }
    }

    internal static BulletType ToOrderedBulletType(char bullet)
    {
        return bullet switch
        {
            '1' => BulletType.Number,
            'a' => BulletType.LowerAlpha,
            'A' => BulletType.UpperAlpha,
            'i' => BulletType.LowerRoman,
            'I' => BulletType.UpperRoman,
            _ => BulletType.Number,
        };
    }

    private static string GetBulletString(bool isOrdered, BulletType bulletType, int index)
    {
        if (isOrdered)
        {
            return bulletType switch
            {
                BulletType.Number => $"{index}. ",
                BulletType.LowerAlpha => $"{index.ToAlphabetical()}. ",
                BulletType.UpperAlpha => $"{index.ToAlphabetical().ToUpper(CultureInfo.CurrentCulture)}. ",
                BulletType.LowerRoman => $"{index.ToRomanNumerals().ToLower(CultureInfo.CurrentCulture)} ",
                BulletType.UpperRoman => $"{index.ToRomanNumerals().ToUpper(CultureInfo.CurrentCulture)} ",
                BulletType.Circle => UnorderedListDot,
                _ => $"{index}. "
            };
        }
        else
        {
            return UnorderedListDot;
        }
    }
}
