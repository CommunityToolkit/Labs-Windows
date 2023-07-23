---
title: MarkdownTextBlock
author: nerocui
description: A control for displaying markdown natively.
keywords: MarkdownTextBlock, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
discussion-id: 0
issue-id: 0
icon: Assets/MarkdownTextBlock.png
---

<!-- To know about all the available Markdown syntax, Check out https://docs.microsoft.com/contribute/markdown-reference -->
<!-- Ensure you remove all comments before submission, to ensure that there are no formatting issues when displaying this page.  -->
<!-- It is recommended to check how the Documentation will look in the sample app, before Merging a PR -->
<!-- **Note:** All links to other docs.microsoft.com pages should be relative without locale, i.e. for the one above would be /contribute/markdown-reference -->
<!-- Included images should be optimized for size and not include any Intellectual Property references. -->

<!-- Be sure to update the discussion/issue numbers above with your Labs discussion/issue id numbers in order for UI links to them from the sample app to work. -->

# MarkdownTextBlock

MarkdownTextBlock is a evolution of the existing MarkdownTextBlock in the community toolkit. This new implementation uses the popular [Markdig](https://github.com/xoofx/markdig) library for parsing. This solves some long standing bugs and feature gaps in our existing implementation.

## Templated Controls

The Toolkit is built with templated controls. This provides developers a flexible way to restyle components
easily while still inheriting the general functionality a control provides. The examples below show
how a component can use a default style and then get overridden by the end developer.
