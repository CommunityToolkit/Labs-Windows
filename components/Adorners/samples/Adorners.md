---
title: Adorners
author: michael-hawker
description: Adorners let you overlay content on top of your XAML components in a separate layer on top of everything else.
keywords: Adorners, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
discussion-id: 278
issue-id: 0
icon: assets/icon.png
---

# Adorners

Adorners allow a developer to overlay any content on top of another UI element in a separate layer that resides on top of everything else.

## Background

Adorners originally existed in WPF as a main integration part as part of the framework. [You can read more about how they worked in WPF here.](https://learn.microsoft.com/dotnet/desktop/wpf/controls/adorners-overview)

UWP/WinUI unfortunately never ported this integration point into the new framework, this experiment hopes to fill that gap with a similar and modernized version of the API surface.

### Without Adorners

Imagine a scenario where you have a button or tab that checks a user's e-mail, and you'd like it to display the number of new e-mails that have arrived.

You could try and incorporate a [`InfoBadge`](https://learn.microsoft.com/windows/apps/design/controls/info-badge) into your Visual Tree in order to display this as  part of your icon, but that requires you to modify quite a bit of your content, as in this example:

> [!SAMPLE InfoBadgeWithoutAdorner]

It also by default gets confined to the perimeter of the button and clipped, as seen above.

### With Adorners

However, with an Adorner instead, you can abstract this behavior from the content of your control. You can even more easily place the notification outside the bounds of the original element, like so:

> [!SAMPLE AdornersInfoBadgeSample]

## Highlight Example

Adorners can be used in a variety of scenarios. For instance, if you wanted to highlight an element and show it's alignment to other elements in a creativity app.

> [!SAMPLE ElementHighlightAdornerSample]
