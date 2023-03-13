---
title: SegmentedControl
author: niels9001
description: A common UI control to configure a view or setting. 
keywords: SegmentedControl, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Input
discussion-id: 314
issue-id: 392
---

# Segmented control

The `Segmented` control is a control to be used for configuring a view or setting.

## The basics

The `Segmented` control is best used with 2-5 items and does not support overflow. The `Icon` and `Content` property can be set on the `SegmentedItems`.

> [!Sample SegmentedControlBasicSample]

## Selection
`Segmented` supports single and multi-selection. When `SelectionMode` is set to `Single` the first item will be selected by default. This can be overriden by settings `AutoSelection` to `false`. 

## Other styles

The `Segmented` control contains various additional styles, to match the look and feel of your application. The `PivotSegmentedStyle` matches a modern `Pivot` style while the `ButtonSegmentedStyle` represents buttons.

> [!SAMPLE SegmentedControlStylesSample]
