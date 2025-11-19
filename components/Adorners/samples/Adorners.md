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

Adorners originally existed in WPF as a main integration part as part of the framework. [You can read more about how they worked in WPF here.](https://learn.microsoft.com/dotnet/desktop/wpf/controls/adorners-overview) See more about the commonalities and differences to WinUI adorners in the migration section below.

### Without Adorners

Imagine a scenario where you have a button or tab that checks a user's e-mail, and you'd like it to display the number of new e-mails that have arrived.

You could try and incorporate a [`InfoBadge`](https://learn.microsoft.com/windows/apps/design/controls/info-badge) into your Visual Tree in order to display this as  part of your icon, but that requires you to modify quite a bit of your content, as in this example:

> [!SAMPLE InfoBadgeWithoutAdorner]

It also, by default, gets confined to the perimeter of the button and clipped, as seen above.

### With Adorners

However, with an Adorner instead, you can abstract this behavior from the content of your control. You can even more easily place the notification outside the bounds of the original element, like so:

> [!SAMPLE AdornersInfoBadgeSample]

## Highlight Example

Adorners can be used in a variety of scenarios. For instance, if you wanted to highlight an element and show it's alignment to other elements in a creativity app:

> [!SAMPLE ElementHighlightAdornerSample]

## TODO: Resize Example

Another common use case for adorners is to allow a user to resize a visual element.

// TODO: Make a simple example here for this soon...

## Migrating from WPF

The WinUI Adorner API surface adapts many similar names and concepts as WPF Adorners; however, WinUI Adorners are XAML based and make use of the attached properties to make using Adorners much simpler, like Behaviors. Where as defining Adorners in WPF required custom drawing routines. It's possible to replicate many similar scenarios with this new API surface and make better use of XAML features like data binding; however, it will mean rewriting any existing WPF code.

### Concepts

The `AdornerLayer` is still an element of the visual tree which resides atop other content within your app and is the parent of all adorners. In WPF, this is usually already automatically a component of your app or `ScrollViewer`. Like WPF, adorners parent's in the visual tree will be the `AdornerLayer` and not the adorned element.

The `AdornerDecorator` provides a similar purpose to that of its WPF counterpart, it will host an `AdornerLayer`. The main difference with the WinUI API is that the `AdornerDecorator` will wrap your contained content vs. in WPF it sat as a sibling to your content. We feel this makes it easier to use and ensure your adorned elements reside atop your adorned content, it also makes it easier to find within the Visual Tree for performance reasons.

TODO: Adorner class info...
