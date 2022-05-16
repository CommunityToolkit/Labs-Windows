---
title: PropertySizer
author: mhawker
description: The PropertySizer is a control which can be used to manipulate the value of another double based property.
keywords: PropertySizer, SizerBase, Control, Layout, NavigationView
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

# PropertySizer

<!-- TODO: Link docs -->
The PropertySizer is a control which can be used to manipulate the value of another <c>double</c> based property. For instance manipulating the <c>OpenPaneLength</c> of a <c>NavigationView</c> control. If you are using a <see cref="Grid"/>, use <see cref="GridSplitter"/> instead.

# Examples 

The main use-case is for PropertySizer to allow you to manipulate the `OpenPaneLength` property of a `NavigationView` control to create a user customizable size shelf. This is handy when using NavigationView with a tree of items that represents some project or folder structure for your application.

Both GridSplitter and ContentSizer are insufficient as they would force the NavigationView to a specific size and not allow it to remember its size when it expands or collapses.

> [!SAMPLE PropertySizerNavigationViewPage]
