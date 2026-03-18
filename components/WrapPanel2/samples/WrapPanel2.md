---
title: WrapPanel2
author: Avid29
description: A labs-component candidate for a new WrapPanel implementation.
keywords: WrapPanel, Control, Layout
dev_langs:
  - csharp
category: Layouts
subcategory: Panel
discussion-id: 762
issue-id: 763
icon: assets/icon.png
---

# WrapPanel2

The WrapPanel2 is an advanced layout control that uses `GridLength` definitions to manage item sizing within a wrapping flow. It provides granular control over how items occupy space, particularly when using proportional (Star) sizing.

## Proportional Sizing Logic

The behavior of items with **Star** 'LayoutLength' values depends on the panel's justification:

**When Stretched/Distributed:** If `ItemsJustification` is set to a distribution mode (like `SpaceBetween`) or if the panel is stretched along the orientation axis, Star-sized items proportionally occupy the available remaining space.

**When Aligned (Start, Center, End):** Star-sized child elements will collapse to the smallest size possible while maintaining their relative proportions and ensuring all child elements are fully visible.

[!Sample WrapPanel2BasicSample]

## Properties

### Items Justification

The `ItemsJustification` property determines how items are aligned and distributed along a line.

| Value     | Description | Star-Size Behavior |
| --------- | ----------- | ------------------ |
| Automatic | Arranges items according to the control's alignment. | Varies by alignment. |
| Start / Center / End | Aligns items to the beginning, middle, or end of the line. | Collapse to minimum proportional size. |
| SpaceAround | Equal internal padding with half-sized padding at margins. | Proportional expansion. |
| SpaceBetween | Equal spacing between items; no margin padding. | Proportional expansion. |
| SpaceEvenly | Equal spacing between all items and margins. | Proportional expansion. |

### Items Stretch

The `ItemsStretch` property defines how the panel fills space on lines that do not contain Star-sized definitions, or when forced to fill a fixed row length.

| Value        | Description |
| ------------ | ----------- |
| None         | No additional stretching is applied to non-star items. Note that Star-sized items will still expand if the `ItemsJustification` mode triggers a stretch. |
| First        | The first item in the line is stretched to occupy all remaining space. |
| Last         | The last item in the line is stretched to occupy all remaining space. |
| Equal        | Every item in the line is stretched to a uniform size to fill the row, regardless of their individual content size. |
| Proportional | Every item in the line is stretched proportionally based on their desired size to fill the remaining space. |
 
## Additional Samples

### Adjusted Sizings Sample

Demonstrates a mix of Auto, Pixel, and Star lengths within a wrapping layout.

> [!Sample WrapPanel2MegaSample]

### Proportional Sizing

Demonstrates how Star-sized items maintain ratios even when the panel is not set to stretch.

> [!Sample WrapPanel2ProportionalSample]

