---
title: PropertySizer
author: mhawker
description: The PropertySizer is a control which can be used to manipulate the value of another double based property.
keywords: PropertySizer, SizerBase, Control, Layout, NavigationView, Splitter
dev_langs:
  - csharp
category: Controls
subcategory: Layout
labs-discussion: 96
labs-issue: 101
---

# PropertySizer

The PropertySizer is a control which can be used to manipulate the value of another <c>double</c> based property. For instance manipulating the <c>OpenPaneLength</c> of a <c>NavigationView</c> control. If you are using a <see cref="Grid"/>, use <see cref="GridSplitter"/> instead.

# Examples 

The main use-case is for PropertySizer to allow you to manipulate the `OpenPaneLength` property of a `NavigationView` control to create a user customizable size shelf. This is handy when using NavigationView with a tree of items that represents some project or folder structure for your application.

Both GridSplitter and ContentSizer are insufficient as they would force the NavigationView to a specific size and not allow it to remember its size when it expands or collapses.

> [!SAMPLE PropertySizerNavigationViewPage]
