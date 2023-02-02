---
title: SegmentedControl
author: niels9001
description: A common UI control to configure a view or setting. 
keywords: SegmentedControl, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
discussion-id: 314
issue-id: 0
---

<!-- To know about all the available Markdown syntax, Check out https://docs.microsoft.com/contribute/markdown-reference -->
<!-- Ensure you remove all comments before submission, to ensure that there are no formatting issues when displaying this page.  -->
<!-- It is recommended to check how the Documentation will look in the sample app, before Merging a PR -->
<!-- **Note:** All links to other docs.microsoft.com pages should be relative without locale, i.e. for the one above would be /contribute/markdown-reference -->
<!-- Included images should be optimized for size and not include any Intellectual Property references. -->

<!-- Be sure to update the discussion/issue numbers above with your Labs discussion/issue id numbers in order for UI links to them from the sample app to work. -->

# Segmented control

The `Segmented` control is a control to be used for configuring a view or setting. It's based off `ListViewBase` and support single and multi-selection.


## The basics

The `Segmented` control is best used with 2-5 items and does not support overflow. The `Icon` and `Content` property can be set on the `SegmentedItems`.

> [!Sample SegmentedControlBasicSample]

## Other styles

The `Segmented` control contains various additional styles, to match the look and feel of your application. The `PillSegmentedStyle` matches a modern `Pivot` and `NavigationView` style.

> [!SAMPLE SegmentedControlStylesSample]
