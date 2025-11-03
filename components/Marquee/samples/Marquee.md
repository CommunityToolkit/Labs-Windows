---
title: Marquee
author: Avid29
description: A control for scrolling content in a marquee fashion.
keywords: Marquee, Control
dev_langs:
  - csharp
category: Controls
subcategory: Layout
experimental: true
discussion-id: 231 
issue-id: 426
icon: Assets/Marquee.png
---

The Marquee control allows text or other content to scroll in a marquee fashion. The control is heavily templated and many changes can be made by modifying the style. The control can also be adjusted using the Speed, Behavior, RepeatBehavior, and Direction properties.

> [!Sample MarqueeTextSample]

## Speed

The speed property determines how quickly the content moves in pixels per second. The speed can be adjusted mid-animation and handled continously.

## Behavior

The Marquee control has 3 behaviors

Default: `Ticker`

### Ticker

Ticker mode starts with all content off the screen then scrolls the content across across the screen. When the animation finishes in the mode no content will be on screen.

### Looping

Looping mode will begin with the start of the content fully in frame then scroll towards the end. When the end is reached it will loop back to the start of the content. Nothing will happen if the content fits in frame.

### Bouncing

Looping mode will begin with the start of the content fully in frame then scroll towards the end. When the end is reached it will bounce and scroll backwards to the start of the content. Nothing will happen if the content fits in frame.

## RepeatBehavior

The repeat behavior determines how many times the marquee will loop before the animation finishes. It can be a number of iteration, a duration, or forever.

Default: `1x`

## Direction

The default direction is left, meaning the content will move leftwards, but this can be changed to right, up, or down. Direction changed between left and right or up and down are handled continously, meaning that the animation will resume from its current position if changed between these directions.

Default: `Left`

## Speed

The speed property determines how quickly the content moves in pixels-per-second. The speed can be adjusted mid-animation and handled continously. Because the speed is in pixels per second the actual time it takes to scroll will depend on the size of the content. The content can also change size dynamically and the speed will remain constant.

Default: `32`

## AutoPlay

The AutoPlay property determines if the marquee will start animating when it is loaded, upon on size changes, or when the content changes. If set to false the marquee can be started manually by calling the `StartMarquee` method.

Default: `false`

## Non-Text Content

It is possible to use non-text content in the Marquee control. However templating must be used because the control will need to be duplicated for the looping animation.

> [!Sample MarqueeSample]

# Level Up with Behaviors

Use behaviors to triggers the Marquee on events

> [!Sample MarqueeBehaviorSample]
