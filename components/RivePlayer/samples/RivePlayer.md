---
title: RivePlayer
author: rive-app
description: Rive state machine animation player
keywords: RivePlayer, State Machine, Animation, Vector
dev_langs:
  - csharp
category: Animations
subcategory: Media
experimental: true
discussion-id: 309
issue-id: 0
icon: Assets/RivePlayer.png
---

![Rive hero image](https://rive-app.notion.site/image/https%3A%2F%2Fs3-us-west-2.amazonaws.com%2Fsecure.notion-static.com%2Fff44ed5f-1eea-4154-81ef-84547e61c3fd%2Frive_notion.png?table=block&id=f198cab2-c0bc-4ce8-970c-42220379bcf3&spaceId=9c949665-9ad9-445f-b9c4-5ee204f8b60c&width=512&userId=&cache=v2)

A high-level runtime for the [Windows Community Toolkit](https://docs.microsoft.com/windows/communitytoolkit/) to use [Rive](https://rive.app) in Universal Windows Platform (UWP) applications.

This library allows for control over Rive files with a high-level API for driving [state machines](https://help.rive.app/editor/state-machine).

> [!Sample RivePlayerCustomSample]

## Getting Started

To add the Rive API to your applications, include the namespace in your XAML:

```xml
<Page 
    ...
    xmlns:rive="using:CommunityToolkit.Labs.WinUI.Rive"
>
```

Note: This package is built on the [RiveSharp](https://github.com/rive-app/rive-sharp) library; a low-level API for Rive with C# that is used to build the render loop. The RiveSharp [nuget package](https://www.nuget.org/packages/Rive.RiveSharp/) is currently unlisted and still in alpha.

## Classes

### `<rive:RivePlayer>`

High-level UI control for rendering Rive content. It is declared in XAML files, and controlled via code or data-binding.

- Declared in XAML files
- Controlled via code or data binding

### Props

- `Width` - (double) Width of the canvas
- `Height` - (double) Height of the canvas
- `Source` - (string) URI to the `.riv` file to load into the app. Supported schemes are `http`, `https`, and `ms-appx`
- `Artboard` - (string) Name of the Rive artboard to instantiate. If empty, the default artboard from the Rive file is loaded.
- `StateMachine` - (string) Name of the Rive state machine to instantiate from the artboard. If empty, the the default state machine is instantiated. If a state machine with the given name does not exist in the artboard, the runtime attempts to load a (deprecated) Rive animation of the same name.
- `DrawInBackground` - (bool) Rive rendering executes in a different thread than the UI

### `<rive:BoolInput>`

High-level class for a state machine instance of a `boolean` input type. This should be nested under the `Rive:RivePlayer` element in XAML.

### Props

- `Target` - (string) Name of the correlated input in the state machine
- `Value` - (DependencyProperty) Binding to a boolean value

### Usage

One example using Rive `BoolInput` classes is binding it to checkbox UI Elements elsewhere in XAML. See below for a code snippet of this:

```xml
<rive:RivePlayer Width="600"
                 Height="600"
                 DrawInBackground="True"
                 Source="https://public.rive.app/community/runtime-files/2244-4463-animated-login-screen.riv">
		<rive:BoolInput Target="isChecking"
		                Value="{x:Bind CheckboxExample.IsChecked, Mode=OneWay}" />
</rive:RivePlayer>
<CheckBox x:Name="CheckboxExample"
          Content="Example" />
```

In the example above, we nest the `rive:BoolInput` strictly under the `rive:RivePlayer` markup.

To create a reference to the boolean input named `isChecking` in the state machine of this Rive file, we set the `Target` property to that input name. Additionally, the `Value` property uses the `x:Bind` markup extension to bind a checkbox's `IsChecked` property to the boolean value of the state machine input.

When the checkbox is toggled, the appropriate "checked" status will be set on the value of the `isChecking` state machine input and the state machine will respond accordingly.

### `<rive:NumberInput>`

High-level class for a state machine instance of a `double` input type. This should be nested under the `Rive:RivePlayer` element in XAML

### Props

- `Target` - (string) name of the correlated input in the state machine
- `Value` - (DependencyProperty) Binding to a number (double) value

### Usage

One example using Rive `NumberInput` classes is binding it to slider range UI Elements elsewhere in XAML. See below for a code snippet of this:

```xml
<rive:RivePlayer Width="600"
                 Height="600"
                 Source="https://public.rive.app/community/runtime-files/2244-4463-animated-login-screen.riv">
    <rive:NumberInput Target="numLook"
                      Value="{x:Bind SliderRangeExample.Value, Mode=OneWay}" />
</rive:RivePlayer>
<Slider x:Name="SliderRangeExample"
        Maximum="100"
        Minimum="0"
        Value="0" />
```

In the example above, we nest the `rive:NumberInput` strictly under the `rive:RivePlayer` markup.

To create a reference to the number input named `numLook` in the state machine of this Rive file, we set the `Target` property to that input name. Additionally, the `Value` property uses the `x:Bind` markup extension to bind the slider's `Value` property to the number value of the state machine input.

When the slider range value is changed, the new number value will be set on the value of the `numLook` state machine input and the state machine will respond accordingly.

### `<rive:TriggerInput>`

High-level class for a state machine instance of a trigger input type. This should be nested under the `Rive:RivePlayer` element in XAML

### Props

- `Target` - (string) name of the correlated input in the state machine
- `x:Name` - (string) name for this trigger reference

### Usage

One example using Rive `TriggerInput` classes is binding it to click events of button UI Elements elsewhere in XAML. See below for a code snippet of this:

```xml
<!-- Example.xaml -->  
<rive:RivePlayer Width="600"
                 Height="600"
                 Source="https://public.rive.app/community/runtime-files/2244-4463-animated-login-screen.riv">
    <rive:TriggerInput Target="trigSuccess"
                       x:Name="SuccessTriggerInput" />
</rive:RivePlayer>
<Button Content="Success">
    <i:Interaction.Behaviors>
        <ic:EventTriggerBehavior EventName="Click">
            <ic:CallMethodAction MethodName="Fire"
                                 TargetObject="{x:Bind SuccessTrigger}" />
        </ic:EventTriggerBehavior>
    </i:Interaction.Behaviors>
</Button>
```

In the example above, we nest the `rive:TriggerInput` strictly under the `rive:RivePlayer` markup.

To create a reference to the trigger input named `trigSuccess` in the state machine of this Rive file, we set the `Target` property to that input name. Additionally, the `x:Name` property defines the name for the class so that the button knows what to bind the click event to to "fire" the trigger.

When the button is clicked, it will call into the `SuccessTriggerInput` input class and invoke the `Fire`  method on the `trigSuccess` Rive trigger input where the state machine will respond accordingly.

Additionally in this example, we use the [EventTriggerBehavior](https://github.com/Microsoft/XamlBehaviors/wiki/EventTriggerBehavior) and [CallMethodAction](https://github.com/Microsoft/XamlBehaviors/wiki/CallMethodAction) APIs from the [XAML Behaviors](https://github.com/Microsoft/XamlBehaviors/wiki) package. You can reference this package as follows:

```xml
<!-- WinUI 2 / UWP -->
<ItemGroup Condition="'$(IsUwp)' == 'true'">
    <PackageReference Include="Microsoft.Toolkit.Uwp.UI.Behaviors"
                      Version="7.1.2"/>
</ItemGroup>

<!-- WinUI 2 / Uno -->
<ItemGroup Condition="'$(IsUno)' == 'true' AND '$(WinUIMajorVersion)' == '2'">
    <PackageReference Include="Uno.Microsoft.Toolkit.Uwp.UI.Behaviors"
                      Version="7.1.11"/>
</ItemGroup>

<!-- WinUI 3 / WinAppSdk -->
<ItemGroup Condition="'$(IsWinAppSdk)' == 'true'">
    <PackageReference Include="CommunityToolkit.WinUI.UI.Behaviors" Version="7.1.2"/>
</ItemGroup>

<!-- WinUI 3 / Uno -->
<ItemGroup Condition="'$(IsUno)' == 'true' AND '$(WinUIMajorVersion)' == '3'">
    <PackageReference Include="Uno.CommunityToolkit.WinUI.UI.Behaviors"
                      Version="7.1.100-dev.15.g12261e2626"/>
</ItemGroup>
```

And to include the `Interactions` and `Interactivity` namespace and Behaviors APIs, add the following in your XAML:
```xml
<Page
    ...
    xmlns:i="using:Microsoft.Xaml.Interactivity"
    xmlns:ic="using:Microsoft.Xaml.Interactions.Core"
>
```

## Other Notes

This runtime is built on SkiaSharp, which is still being optimized for all platforms. While you may notice slower frame rates drawing Rives, particularly on WinAppSdk, we are actively working to improve the rendering performance.
