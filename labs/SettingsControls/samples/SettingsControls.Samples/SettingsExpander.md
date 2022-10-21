---
title: SettingsExpander
author: niels9001
description: An expander control that can be used to create Windows 11 style settings experiences.
keywords: SettingsCard, SettingsExpander, Expander, Control, Layout, Settings
dev_langs:
  - csharp
category: Controls
subcategory: Layout
---

# SettingsExpander

For more information about this experiment see:

- Discussion: https://github.com/CommunityToolkit/Labs-Windows/discussions/129
- Issue: https://github.com/CommunityToolkit/Labs-Windows/issues/216

The `SettingsExpander` can be used to group multiple `SettingsCard`s into a single collapsable group.

A `SettingsExpander` can have it's own content to display a setting on the right, just like a `SettingsCard`, but in addition can have any number of extra `Items` to include as additional settings. These items are `SettingsCard`s themselves, which means you can easily move a setting into or out of Expanders just by cutting and pasting their XAML!

> [!SAMPLE SettingsExpanderSample]

You can easily override certain properties to create custom experiences. For instance, you can customize the `ContentAlignment` of a `SettingsCard`, to align your content to the Right (default), Left (hiding the `HeaderIcon`, `Header` and `Description`) or Vertically (usually best paired with changing the `HorizontalContentAlignment` to `Stretch`).
