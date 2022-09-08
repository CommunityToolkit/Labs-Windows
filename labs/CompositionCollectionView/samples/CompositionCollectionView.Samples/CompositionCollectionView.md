---
title: CompositionCollectionView
author: arcadiogarcia
description: A composition-driven collection view with fully customizable look and behavior
keywords: CompositionCollectionView, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
---

<!-- To know about all the available Markdown syntax, Check out https://docs.microsoft.com/contribute/markdown-reference -->
<!-- Ensure you remove all comments before submission, to ensure that there are no formatting issues when displaying this page.  -->
<!-- It is recommended to check how the Documentation will look in the sample app, before Merging a PR -->
<!-- **Note:** All links to other docs.microsoft.com pages should be relative without locale, i.e. for the one above would be /contribute/markdown-reference -->
<!-- Included images should be optimized for size and not include any Intellectual Property references. -->

# CompositionCollectionView

For more information about this experiment see:
- Discussion: https://github.com/CommunityToolkit/WindowsCommunityToolkit/discussions/4565
- Issue: https://github.com/CommunityToolkit/Labs-Windows/issues/135

CompositionCollectionView is a control that can host and position a collection of elements using developer-defined layouts based on the composition animation APIs
This facilitates the task of creating complex, performant layouts that are integrated with touch gestures, allowing to reuse logic across them.

A layout can be as simple as this one, which calculates an static position for each element according to its properties:

> [!SAMPLE CompositionCollectionViewFirstSamplePage]

While a CompositionCollectionView is only using one layout at a time, it can transition to any other layout on demand, implicitly animating the elements if desired.

This sample shows how easy it is to transition between two unrelated layouts:

> [!SAMPLE SwitchLayoutsSample]

Behaviors can be used to define interactions, gestures or any other custom logic that might be shared across layouts. The built in InteractionTrackerGesture simplifies the task of integrating touch into a layout and defining layout-independent touch gestures.

This sample shows a scrollable list of elements that follow a custom path:

> [!SAMPLE InteractionTrackerSample]

Layouts can be as complicated as you want, your creativity is the limit!

This sample is inspired by the classic Maze 3d screensaver. The user can move around the maze with touch gestures (collisions with the inner walls are ommited) and will transition to an overhead view when they touch the smiley face.

> [!SAMPLE MazeSample]
