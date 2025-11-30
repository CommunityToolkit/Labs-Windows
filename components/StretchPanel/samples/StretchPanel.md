---
title: StretchPanel
author: Avid29
description: A highly flexible panel that arranges its child elements according to GridLength definitions, allowing for proportional sizing and stretching behavior.
keywords: StretchPanel, Control, Layout
dev_langs:
  - csharp
category: Layouts
subcategory: Panel
discussion-id: 0
issue-id: 0
icon: assets/icon.png
---

# StretchPanel

The StretchPanel is a panel that arranges its child elements according to GridLength definitions.

When stretched along the main axis, the child elements with star-sized GridLength values will proportionally occupy the available space.

When not stretched along the main axis, star-sized child elements will be the smallest size possible while maintaining proportional sizing relative to each other and ensuring that all child elements are fully visible.

> [!Sample StretchPanelBasicSample]

## Properties

### Fixed Row Length

When `FixedRowLengths` is enabled, all rows/columns will to stretch to the size of the largest row/column in the panel. When this is not enabled, rows/columns will size to their content individually.

### Forced Stretch Method

The `ForcedStretchMethod` property allows you to specify how the StretchPanel should handle stretching in rows without star-sized definitions.

#### None

When set to `None`, the StretchPanel will not stretch rows/columns that do not have star-sized definitions. When the alignment is set to stretch, and even when fixed row lengths is enabled, the rows/columns without star-sized definitions will size to their content.

#### First

When set the `First`, the StretchPanel will stretch the first item in the row/column to occupy the remaining space when needed to comply with stretch alignment.

#### Last

When set to `Last`, the StretchPanel will stretch the last item in the row/column to occupy the remaining space when needed to comply with stretch alignment.

#### Equal

When set to `Equal`, the StretchPanel will stretch all items in the row/column to occupy the equal space throughout the row when needed to comply with stretch alignment.

#### Proportional

When set to `Proportional`, the StretchPanel will stretch all items in the row/column proportionally to their defined size to occupy the remaining space when needed to comply with stretch alignment.
