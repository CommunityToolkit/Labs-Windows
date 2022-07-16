---
title: ContentSizer
author: mhawker
description: The ContentSizer is a control which can be used to resize any element, usually its parent.
keywords: ContentSizer, SizerBase, Control, Layout, Expander
dev_langs:
  - csharp
category: Controls
subcategory: Layout
---

<!-- To know about all the available Markdown syntax, Check out https://docs.microsoft.com/en-us/contribute/markdown-reference -->
<!-- Ensure you remove all comments before submission, to ensure that there are no formatting issues when displaying this page.  -->
<!-- It is recommended to check how the Documentation will look in the sample app, before Merging a PR -->
<!-- **Note:** All links to other docs.microsoft.com pages should be relative without locale, i.e. for the one above would be /contribute/markdown-reference -->
<!-- Included images should be optimized for size and not include any Intellectual Property references. -->

# ContentSizer

<!-- TODO: Link docs -->
The ContentSizer is a control which can be used to resize any element, usually its parent. If you are using a `Grid`, use [GridSplitter](GridSplitter.md) instead.

# Examples 

The main use-case for a ContentSizer is to create an expandable shelf for your application. This allows the `Expander` itself to remember its opening/closing sizes.

A GridSplitter would be insufficient as it would force the grid to remember the row size and maintain its position when the `Expander` collapses.

> [!SAMPLE ContentSizerTopShelfPage]

The following example shows how to use the ContentSizer to create a left-side shelf; however, this scenario can also be accomplished with a GridSplitter.

> [!SAMPLE ContentSizerLeftShelfPage]
