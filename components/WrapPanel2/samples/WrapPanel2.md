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

The WrapPanel2 is an experiment for a new WrapPanel API using GridLength definitions to define the item's desired sizings.

When stretched along the main axis, the child elements with star-sized GridLength values will proportionally occupy the available space.

When not stretched along the main axis, star-sized child elements will be the smallest size possible while maintaining proportional sizing relative to each other and ensuring that all child elements are fully visible.


> [!Sample WrapPanel2BasicSample]

## Properties

### Fixed Row Length

When `FixedRowLengths` is enabled, all rows/columns will to stretch to the size of the largest row/column in the panel. When this is not enabled, rows/columns will size to their content individually.

### Stretching Children

The `StretchChildren` property allows you to specify how the panel should handle stretching in rows without star-sized definitions.

#### StarSizedOnly

When set to `StarSizedOnly`, this panel will never stretch rows/columns that do not have star-sized definitions. When the alignment is set to stretch, and even when fixed row lengths is enabled, the rows/columns without star-sized definitions will size to their content.

#### First

When set the `First`, this panel will stretch the first item in the row/column to occupy the remaining space when needed if `FixedRowLengths` is enabled.

#### Last

When set to `Last`, this panel will stretch the last item in the row/column to occupy the remaining space when needed if `FixedRowLengths` is enabled.

#### Equal

When set to `Equal`, this panel will stretch all items in the row/column to occupy the equal space throughout the row when needed if `FixedRowLengths` is enabled.

#### Proportional

When set to `Proportional`, this panel will stretch all items in the row/column proportionally to their defined size to occupy the remaining space when needed if `FixedRowLengths` is enabled.

## Additional Samples

### Adjusted Sizings Sample

> [!Sample WrapPanel2MegaSample]

### Proportional Sizing

Encoding diagram example

> [!Sample WrapPanel2ProportionalSample]

