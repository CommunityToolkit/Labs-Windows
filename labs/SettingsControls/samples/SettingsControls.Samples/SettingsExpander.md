---
title: SettingsExpander
author: niels9001
description: An expander control that can be used to create Windows 11 style settings experiences.
keywords: SettingsCard, SettingsExpander, Expander, Control, Layout, Settings
dev_langs:
  - csharp
category: Controls
subcategory: Layout
labs-discussion: 129
labs-issue: 216
---

# SettingsExpander

The `SettingsExpander` can be used to group multiple `SettingsCard`s into a single collapsable group.

A `SettingsExpander` can have it's own content to display a setting on the right, just like a `SettingsCard`, but in addition can have any number of extra `Items` to include as additional settings. These items are `SettingsCard`s themselves, which means you can easily move a setting into or out of Expanders just by cutting and pasting their XAML!

> [!SAMPLE SettingsExpanderSample]

You can easily override certain properties to create custom experiences. For instance, you can customize the `ContentAlignment` of a `SettingsCard`, to align your content to the Right (default), Left (hiding the `HeaderIcon`, `Header` and `Description`) or Vertically (usually best paired with changing the `HorizontalContentAlignment` to `Stretch`).

`SettingsExpander` is also an `ItemsControl`, so its items can be driven by a collection and the `ItemsSource` property. You can use the `ItemTemplate` to define how your data object is represented as a `SettingsCard`, as shown here:

> [!SAMPLE SettingsExpanderItemsSourceSample]
