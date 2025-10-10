---
title: ContrastHelper
author: Avid29
description: A helper for adjusting colors to ensure sufficient contrast.
keywords: Color, Accesibility
dev_langs:
  - csharp
category: Helpers
subcategory: Miscellaneous
discussion-id: 745
issue-id: 736
icon: assets/icon.png
---

# ContrastHelper

### Using on TextBlocks or Controls

The contrast helper can be applied to a `TextBlock` or `Control` to
apply updates on its `Foreground` property.

When checking the original contrast ratio, the `ContrastHelper` will
attempt to grab the `Foreground` as a `SolidColorBrush`. If the original
`Foreground` is not a `SolidColorBrush` it will default to `Colors.Transparent`,
and always apply a raised contrast color.

> [!Sample TextBlockContrastSample]

If you are not using a `TextBlock` or `Control`,
you can directly apply the `ContrastHelper` to a `SolidColorBrush`.

Here for example, we adjust the stroke of a `Shape` by applying
the helper on the `SolidColorBrush` inside the `Shape.Stroke` property.`

### Using on SolidColorBrush

> [!Sample SolidColorBrushContrastSample]
