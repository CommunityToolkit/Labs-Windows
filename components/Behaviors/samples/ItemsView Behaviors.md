---
title: ItemsView Behaviors
author: githubaccount
description: A set of behaviors for the ItemsView control.
keywords: Behaviors, Control, Behavior
dev_langs:
  - csharp
category: Xaml
subcategory: Behaviors
discussion-id: 532
issue-id: 0
icon: assets/icon.png
---
The new control `ItemsView` which can replace `ListView`&`GridView`, does not support `ISupportIncrementalLoading`.

Here are some Behaviors to help you make `ItemsView` support `ISupportIncrementalLoading`.

## NeedMoreItemTriggerBehaviorSample

This trigger behavior can excute actions when the `ItemsView` scrolling to bottom.
You can customize the loading behavior through actions, but note that you may need to debounce manually.

> [!SAMPLE NeedMoreItemTriggerBehaviorSample]

## LoadMoreItemBehaviorSample

If you don't have complex loading requirements, you can use this behavior.
It automatically calls `ISupportIncrementalLoading.LoadMoreItemsAsync` when the `ItemsSource` changes, in addition to when `ItemsView` scrolls to the bottom.
Besides, this behavior has a built-in debounce function.

> [!SAMPLE LoadMoreItemBehaviorSample]
