---
title: Adorners
author: michael-hawker
description: Adorners let you overlay content on top of your XAML components in a separate layer on top of everything else.
keywords: Adorners, Control, Layout, InfoBadge, AdornerLayer, AdornerDecorator, Adorner, Input Validation, Resize, Highlighting
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

Adorners originally existed in WPF as an extension part of the framework. [You can read more about how they worked in WPF here.](https://learn.microsoft.com/dotnet/desktop/wpf/controls/adorners-overview) See more about the commonalities and differences to WinUI adorners in the migration section below.

### Without Adorners

Imagine a scenario where you have a button or tab that checks a user's e-mail, and you'd like it to display the number of new e-mails that have arrived.

You could try and incorporate a [`InfoBadge`](https://learn.microsoft.com/windows/apps/design/controls/info-badge) into your Visual Tree in order to display this as  part of your icon, but that requires you to modify quite a bit of your content, as in this example:

> [!SAMPLE InfoBadgeWithoutAdorner]

It also, by default, gets confined to the perimeter of the button and clipped, as seen above.

### With Adorners

However, with an Adorner instead, you can abstract this behavior from the content of your control. You can even more easily place the notification outside the bounds of the original element, like so:

> [!SAMPLE AdornersInfoBadgeSample]

You can see how Adorners react to more dynamic content with this more complete example here:

> [!SAMPLE AdornersTabBadgeSample]

The above example shows how to leverage XAML animations and data binding alongside the XAML-based Adorner with a `TabViewItem` which can also move or disappear.

## Highlight Example

Adorners can be used in a variety of scenarios. For instance, if you wanted to highlight an element and show it's alignment to other elements in a creativity app:

> [!SAMPLE ElementHighlightAdornerSample]

The above examples highlights how adorners are sized and positioned directly atop the adorned element. This allows for relative positioning of elements within the context of the Adorner's visuals in relation to the Adorned Element itself.

## Custom Adorner Example

Adorners can be subclassed in order to encapsulate specific logic and/or styling for your scenario.
For instance, you may want to create a custom Adorner that allows a user to click and edit a piece of text in place.
The following example uses `IEditableObject` to control the editing lifecycle coordinated with a typical MVVM pattern binding:

> [!SAMPLE InPlaceTextEditorAdornerSample]

Adorners are template-based controls, but you can use a class-backed resource dictionary to better enable usage of x:Bind for easier creation and binding to the `AdornedElement`, as seen here.

You can see other example of custom adorners with the other Adorner help topics for the built-in adorners provided in this package, such as the `InputValidationAdorner` and `ResizeElementAdorner`.

## Migrating from WPF

The WinUI Adorner API surface adapts many similar names and concepts as WPF Adorners; however, WinUI Adorners are XAML based and make use of the attached properties to make using Adorners much simpler, like Behaviors. Where as defining Adorners in WPF required custom drawing routines. It's possible to replicate many similar scenarios with this new API surface and make better use of XAML features like data binding and styling; however, it will mean rewriting any existing WPF code.

### Concepts

The `AdornerLayer` is still an element of the visual tree which resides atop other content within your app and is the parent of all adorners. In WPF, this is usually already automatically a component of your app or `ScrollViewer`. Like WPF, adorners parent's in the visual tree will be the `AdornerLayer` and not the adorned element. The WinUI-based `AdornerLayer` will automatically be inserted in many common scenarios, otherwise, an `AdornerDecorator` may still be used to direct the placement of the `AdornerLayer` within the Visual Tree.

The `AdornerDecorator` provides a similar purpose to that of its WPF counterpart, it will host an `AdornerLayer`. The main difference with the WinUI API is that the `AdornerDecorator` will wrap your contained content vs. in WPF it sat as a sibling to your content. We feel this makes it easier to use and ensure your adorned elements reside atop your adorned content, it also makes it easier to find within the Visual Tree for performance reasons.

The `Adorner` class in WinUI is now a XAML-based element that can contain any content you wish to overlay atop your adorned element. In WPF, this was a non-visual class that required custom drawing logic to render the adorner's content. This change allows for easier creation of adorners using XAML, data binding, and styling. Many similar concepts and properties still exist between the two, like a reference to the `AdornedElement`. Any loose XAML attached via the `AdornerLayer.Xaml` attached property is automatically wrapped within a basic `Adorner` container. You can either restyle or subclass the `Adorner` class in order to better encapsulate logic of a custom `Adorner` for your specific scenario, like a behavior, as shown above.
