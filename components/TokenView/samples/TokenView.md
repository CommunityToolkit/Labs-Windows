---
title: TokenView
author: niels9001
description: The TokenView can be used to display and select tokens.
keywords: TokenView, Control, TokenItem, ChipsControl
dev_langs:
  - csharp
category: Controls
subcategory: Layout
discussion-id: 323
issue-id: 0
---

<!-- To know about all the available Markdown syntax, Check out https://docs.microsoft.com/contribute/markdown-reference -->
<!-- Ensure you remove all comments before submission, to ensure that there are no formatting issues when displaying this page.  -->
<!-- It is recommended to check how the Documentation will look in the sample app, before Merging a PR -->
<!-- **Note:** All links to other docs.microsoft.com pages should be relative without locale, i.e. for the one above would be /contribute/markdown-reference -->
<!-- Included images should be optimized for size and not include any Intellectual Property references. -->

<!-- Be sure to update the discussion/issue numbers above with your Labs discussion/issue id numbers in order for UI links to them from the sample app to work. -->

# TokenView

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
