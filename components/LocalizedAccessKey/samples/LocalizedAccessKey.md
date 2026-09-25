---
title: LocalizedAccessKey
author: vgromfeld
description: Add keyboard localization support to access keys
keywords: LocalizedAccessKey, Extensions, Markup, Localization
dev_langs:
  - csharp
category: Extensions
subcategory: Markup
experimental: true
discussion-id: 811
issue-id: 812
icon: assets/icon.png
---

# LocalizedAccessKey
`LocalizedAccessKey` is a markup extension to localize the access key of a control based on the current user keyboard layout.

By default, WinUI requires the user to type exactly the requested access key sequence to trigger an access key.

Not all characters can be typed on all keyboard layouts. This extension allows us to specify a sequence of `VirtualKey`s that will
be converted to the current keyboard layout characters. This allows us to specify the access key only once
and be sure that they will work on all keyboard layouts.

### Example
The latin character "V" is not available on the Greek keyboard layout. It is replaced by "Ω".

The access key defined like in the following XAML fragment:

```xml
<Button AccessKey="V" Content="Click me" />
```

Won't be reachable using the Greek keyboard layout. The user will have to switch to the English keyboard layout to be able to type "V".

When using the `LocalizedAccessKey` markup extension, the access key will be automatically localized to the current keyboard layout:

```xml
<Button AccessKey="{toolkit:LocalizedAccessKey V}" Content="Click me" />
```

### Notes
The extension does not support dynamic reloading when the user changes the keyboard layout.

The access key will be set to the current keyboard layout when the control is loaded.
If the user changes the keyboard layout afterwards, the access key will not be updated.
To see the changes, the control will have to be reloaded (for example by navigating away and back to the page).

## Sample
You can add new keyboard layouts from the Windows settings. Go to "Clock and languages" > "Language" > "Add a language" and select the desired language. You can then switch between keyboard layouts using the language bar in the taskbar.

> [!Sample LocalizedAccessKeyCustomSample]
