---
title: Ribbon
author: vgromfeld
description: An office like ribbon.
keywords: Ribbon, Control
dev_langs:
  - csharp
category: Controls
subcategory: Layout
experimental: true
discussion-id: 544
issue-id: 545
icon: Assets/Ribbon.png
---

# Ribbon

An Office like Ribbon control which displays groups of commands. If there is not enough space to display all the groups,
some of them can be collapsed based on a priority order.

> [!Sample RibbonCustomSample]

## RibbonGroup

A basic group displayed in a Ribbon.
It mostly adds a *label* to some content and will not collapse if there is not enough space available.


## RibbonCollapsibleGroup

A `RibbonGroup` which can be collapsed if its content does not fit in the available Ribbon's space.
When collapsed, the group is displayed as a single icon button. Clicking on this button opens
a flyout containing the group's content.

### IconSource
The icon to display when the group is collapsed.

### AutoCloseFlyout
Set to true to automatically close the overflow flyout if one interactive element is clicked.
Note that the logic to detect the click is very basic. It will request the flyout to close
for all the handled pointer released events. It assumes that if the pointer has been handled
something reacted to it. It works well for buttons or check boxes but does not work for text
or combo boxes.

### Priority
The priority of the group.
The group with the lower priority will be the first one to be collapsed.

### CollapsedAccessKey
The access key to access the collapsed button and open the flyout when the group is collapsed.

### RequestedWidths

The list of requested widths for the group.
If null or empty, the group will automatically use the size of its content.
If set, the group will use the smallest provided width fitting in the ribbon.
This is useful if the group contains a variable size control which can adjust
its width (like a GridView with several items).

### State
The state of the group (collapsed or visible). This property is used by the `RibbonPanel`.

## RibbonPanel

The inner panel of the Ribbon control. It displays the groups inside the `Ribbon` and
automatically collapse the `CollapsibleGroup` elements based on their priority order if
there is not enough space available.
