---
title: MarqueeText
author: Avid29
description: A control for scrolling text in a marquee fashion.
keywords: Marquee, Control, Text
dev_langs:
  - csharp
category: Controls
subcategory: StatusAndInfo
discussion-id: 231 
issue-id: 0
---

# MarqueeText

The MarqueeText control allows text to scroll in a marquee fashion. The control is heavily templated and many changes can be made by modifying the style. The control can also be adjusted using the Speed, Behavior, RepeatBehavior, and Direction properties.

##Speed

The speed property determines how quickly the text moves in pixels per second. The speed can be adjusted mid-animation and handled continously.

##Behavior

The MarqueeText control has 3 behaviors

###Ticker

Ticker mode starts with all text off the screen then scrolls the text across across the screen. When the animation finishes in the mode no text will be on screen.

###Looping

Looping mode will begin with the start of the text fully in frame then scroll towards the end. When the end is reached it will loop back to the start of the text. Nothing will happen if the text fits in frame.

###Bouncing

Looping mode will begin with the start of the text fully in frame then scroll towards the end. When the end is reached it will bounce and scroll backwards to the start of the text. Nothing will happen if the text fits in frame.

##RepeatBehavior

The repeat behavior determines how many times the marquee will loop before the animation finishes. It can be a number of iteration, a duration, or forever.

##Direction

The default direction is left, meaning the text will move leftwards, but this can be changed to right, up, or down. Direction changed between left and right or up and down are handled continously, meaning that the animation will resume from its current position if changed between these directions.

> [!Sample MarqueeTextSample]
