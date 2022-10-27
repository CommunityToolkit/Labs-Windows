---
title: SettingsControls
author: niels9001
description: A collection of controls that can be used to create Windows 11 style settings experiences.
keywords: SettingsCard, Control, Layout, Settings
dev_langs:
  - csharp
category: Controls
subcategory: Layout
---

# SettingsControls

For more information about this experiment see:
- Discussion: https://github.com/CommunityToolkit/Labs-Windows/discussions/129
- Issue: https://github.com/CommunityToolkit/Labs-Windows/issues/216


SettingsCard is a control that can be used to display settings in your experience. It uses the default styling found in Windows 11 and is easy to use, meets all accesibility standards and will make your settings page look great!
You can set the Header, Description, HeaderIcon and Content.

> [!SAMPLE SettingsCardSample]

SettingsCard can also be turned into a button, by setting the IsClickEnabled property. This can be useful whenever you want your settings component to e.g. navigate to a detail page or open an external link.

> [!SAMPLE ClickableSettingsCardSample]

The SettingsExpander can be used to group multiple SettingsCards into a single object. You can customize the ContentAlignment of a SettingsCard, to align your content to the Right (default), Left (hiding the HeaderIcon, Header and Description) and Vertically.

> [!SAMPLE SettingsExpanderSample]

You can easily override certain properties to create custom experiences.
