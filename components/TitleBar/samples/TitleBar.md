---
title: TitleBar
author: niels9001
description: A control that provides a modern TitleBar
keywords: TitleBar, Control, Layout
dev_langs:
  - csharp
category: Controls
subcategory: Layout
experimental: true
discussion-id: 0
issue-id: 0
icon: assets/icon.png
---

> [!NOTE]
> If you're using the Windows App SDK with WinUI, you can leverage the `TitleBar` control that [shipped in the 1.7 release](https://learn.microsoft.com/windows/apps/windows-app-sdk/stable-channel#new-titlebar-control) over this one.

The `TitleBar` provides an easy way to display a modern titlebar experience. The control handles all the required APIs to extend content into the titlebar area and set custom drag regions. The control is set up in a way that it handles the correct design guidelines while being flexible in what content to show.

> [!Sample TitleBarConfigSample]

> [!Sample TitleBarFullSample]

## Using TitleBar in WASDK apps
If `AutoConfigureCustomTitleBar` is enabled, the `TitleBar` will make the required code-behind changes to make your content extend into the titlebar area and setting the correct caption background brushes. However, launching the app might briefly show the standard titlebar. To overcome this, make sure you set the following code in the `OnLaunched` method (in `App.xaml.cs`) or wherever you create your window:

```CS
currentWindow.AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
currentWindow.AppWindow.TitleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
`

