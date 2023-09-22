---
title: TokenView
author: niels9001
description: The TokenView can be used to display and select tokens.
keywords: TokenView, Control, TokenItem, ChipsControl
dev_langs:
  - csharp
category: Controls
subcategory: Layout
experimental: true
discussion-id: 323
issue-id: 0
icon: Assets/TokenView.png
---

The TokenView is used to display `Tokens` in a consistent way that is inline with the Windows 11 design language. This control can be easily used to to display e.g. filters, contacts or other snippets of information.

Tokens allow for setting the `Icon` and `Content` while they can be removed as well. 

## IsWrapped

By default, `TokenItems` are laid out horizontally and scrollbuttons will automatically appear upon control resizing. When `IsWrapped` is set, all items will be vertically stacked if the width of the items is larger than the available space.
> [!Sample TokenViewBasicSample]

## Removing TokenItems

TokenItems can be removed from the collection by setting the `IsRemoveable` property on the TokenItem- or by setting the `CanRemoveTokens` property on the TokenView itself.

> [!Sample TokenViewRemoveSample]

## Binding

Binding is supported. Collections can be bound by setting a collection of items. You can use the `ItemTemplate` to define how your data object is represented.

> [!Sample TokenViewItemsSourceSample]
