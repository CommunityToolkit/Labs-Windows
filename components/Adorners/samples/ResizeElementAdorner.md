---
title: ResizeElementAdorner
author: michael-hawker
description: A ResizeElementAdorner provides resizing functionality to FrameworkElement.
keywords: Adorners, Resize, FrameworkElement, Layout, Controls
dev_langs:
  - csharp
category: Controls
subcategory: Layout
discussion-id: 278
issue-id: 0
icon: assets/icon.png
---

# ResizeElementAdorner

The `ResizeElementAdorner` provides resizing functionality to any `FrameworkElement`.

## Usage Example

The `ResizeElementAdorner` can be attached to any element and displayed to allow a user to resize the adorned element by dragging the resize handles.

> [!SAMPLE ResizeElementAdornerCanvasSample]

This can be done above within a `Canvas` layout control, or with general layout using Margins as well:

// TODO: Add Margin-based example here

## Complete Example

Using the `CanvasView` control allows for advanced manipulation with dragging elements as well, this can be combined to create a design-type surface:

> [!SAMPLE ResizeElementAdornerWithDragSample]
