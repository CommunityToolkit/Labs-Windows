---
title: DataTable
author: michael-hawker
description: DataTable provides a set of components which can transform an ItemsControl or ListView into a simple table like experience.
keywords: DataTable, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
discussion-id: 0
issue-id: 0
---

# DataTable

DataTable provides a set of components which can transform an ItemsControl or ListView into a simple table
like experience. It is an intermediary between a list and a full-blown `DataGrid` experience.

It provides resizable columns and allows for complete styling control out-of-the-box.

It is useful in scenarios where:

- A simplified/modern style is required as more of a table of data
- Cell-level selection isn't required
- Editing every piece of data in the table isn't required

## Traditional Method

To compare how the DataTable works, we'll start with the traditional example of how a `ListView` (or `ItemsControl`)
can be made to look like a table of data:

> [!Sample ListViewTableSample]

There are limitations here with having fixed column sizes that can be difficult to align. Their definitions are
also duplicated, and every item is recreating this layout and duplicating it within the Visual Tree.

## DataTable Setup

The `DataTable` setup provides an easier way to maintain this coordinated effort to layout items as a table within
a `ListView` or `ItemsControl`.

A "DataTable" is actually made of two components a `DataTableHeader` component in the `Header` of
an `ItemsControl` or `ListView` control, and the `DataTable` panel itself within the `DataTemplate`
of your `ItemTemplate` for the items control you use. For example:

> [!Sample DataTableBasicSample]

## Comparison to DataGrid

Benefits/Comparison over DataGrid:

- Easier to use/more intuitive to setup/customize as using base XAML building blocks without complex templating/styling
- Therefore Styling/Look-and-feel much more in developer's control, based off common controls
- Usable with `ItemsControl` (static) and `ListView` (for selection)
- Grouping provided by `ItemsControl` itself
- More light-weight of a component in-general for Visual Tree
- Support row virtualization via `ItemsStackPanel`
- Still provides column resizing via header

Limitations compared to DataGrid is that:

- No Row Headers
- No Built-in Cell Editing
- No Built-in Cell Selection
- No Built-in Sorting
- No Detail Template
- No Grid lines

However, most of these limitations can still be enabled by the developer to an extent with modifications to templates or
with additional code.

For instance, sorting can be provided like so:

TODO: Add Header based Sorting sample
