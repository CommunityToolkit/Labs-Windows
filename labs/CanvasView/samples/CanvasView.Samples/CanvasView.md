---
title: CanvasView
author: michael-hawker
description: CanvasView is an ItemsControl which uses a Canvas for layout of items.
keywords: CanvasView, ItemsControl, Canvas, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
---

# CanvasView

For more information about this experiment see:

- Discussion: https://github.com/CommunityToolkit/WindowsCommunityToolkit/discussions/3716
- Issue: https://github.com/CommunityToolkit/Labs-Windows/issues/212

CanvasView is an ItemsControl which uses a Canvas for the layout of its items.

It which provides built-in support for presenting a collection of items bound to specific coordinates and drag-and-drop support of those items.

**Note:** This control isn't working on Web Assembly at the moment.

The following example shows how the CanvasView can be used to display and drag a collection of rectangles:

> [!SAMPLE CanvasViewDragSample]
