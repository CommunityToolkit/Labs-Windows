---
title: DataTable
author: michael-hawker
description: DataTable provides a set of components which can transform an ItemsControl or ListView into a simple table like experience.
keywords: DataTable, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
experimental: true
discussion-id: 415
issue-id: 0
icon: Assets/DataTable.png
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

To compare how the DataTable works, we'll start with the traditional example of how a `ListView` (or `HeaderedItemsControl`)
can be made to look like a table of data:

> [!Sample ListViewTableSample]

There are limitations here with having fixed column sizes that can be difficult to align. Their definitions are
also duplicated, and every item is recreating this layout and duplicating it within the Visual Tree.

## DataRow Hybrid Setup

As a first step, moving to **DataTable** is easy, just replace the `Grid` in your `ItemsTemplate` with the `DataRow` panel
and remove the Column attributes from your controls. `DataRow` automatically will lay each subsequent control in the next column
for you automatically:

> [!Sample DataTableHybridSample]

## DataTable Setup

The `DataTable` setup provides an easier way to define and manage your columns within your header for this coordinated effort
to layout items as a table within a `ListView` or `ItemsControl`.

A _"DataTable"_ is actually made of three components a `DataTable` component in the `Header` of an `HeaderedItemsControl` or
`ListView` control. There you can define `DataColumn`s within the table and configure their size and other settings.
Finally, as you saw above, the `DataRow` panel itself is within the `DataTemplate` of your `ItemTemplate` for the items control
you use. For example:

> [!Sample DataTableSample]

### Simple Table

If you don't need headers and want to show a simple table of data, just don't provide any content to the `DataColumn` headers:

> [!Sample DataTableBlankHeaderSample]

### Virtualization

Since `DataTable` is just built on top of `ListView` it can handle many data rows just the same as a ListView can.

> [!Sample DataTableVirtualizationSample]

## DataTable + TreeView (Test)

The `DataTable` setup works with other types of views as well, like `TreeView`. This enables a "TreeGrid" like experience, if required.

An example here with the `HeaderedTreeView` is for testing this scenario:

> [!Sample TreeTableSample]

## Comparison of DataTable to DataGrid

Benefits/Similarities:

- Easier to use/more intuitive to setup/customize as using base XAML building blocks without complex templating/styling
- Therefore Styling/Look-and-feel much more in developer's control, based off common controls
- Usable with `ItemsControl` (static) and `ListView` (for selection)
- Grouping provided by `ItemsControl` itself
- More light-weight of a component in-general for Visual Tree
- Support row virtualization via `ItemsStackPanel`
- Still provides column resizing via header

Limitations:

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
