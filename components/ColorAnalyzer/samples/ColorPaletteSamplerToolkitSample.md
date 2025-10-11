---
title: ColorPaletteSampler
author: Avid29
description: A resource that generates a color palette from an image or any other UI element.
keywords: Color
dev_langs:
  - csharp
category: Helpers
subcategory: Miscellaneous
discussion-id: 254
issue-id: 736
icon: assets/icon.png
---

# ColorPaletteSampler

## Overview

The `ColorPaletteSampler` is a resource that generates a color palette from any `UIElement`,
but most notably from an `Image`. After the palette is generated, you can use a `PaletteSelector`
or collection of `PaletteSelector` items to select colors from the palette to bind to in the UI.

## AccentColorPaletteSelector

The `AccentColorPaletteSelector` can be used to extract accent colors from an image. An
accent color is a color that stands out, which we detect by looking for colors following
a "colorness" formula.

Here's an example where you can play around and see the results.

> [!Sample AccentColorSample]

## BaseColorPaletteSelector

The `BaseColorPaletteSelector` can be used to extract basic colors from an image.
Basic colors are colors that standand out less stands out, which we detect by using
the same "colorness" formula, as accent colors, but inverting the results.

> [!Sample BaseColorSample]

## ColorWeightPaletteSelector

The `ColorWeightPaletteSelector` can be used to determine which colors cover the most
area in an image.

> [!Sample ColorWeightSample]

## Using multiple `PaletteSelectors` items

Finally, multiple `PaletteSelector` items can be used together in a single `ColorPaletteSampler`
to extract any combination of color data from an image.

> [!Sample MultiplePaletteSelectorSample]
