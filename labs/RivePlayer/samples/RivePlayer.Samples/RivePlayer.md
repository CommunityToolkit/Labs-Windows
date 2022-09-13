---
title: RivePlayer
author: rive-app
description: Rive state machine animation player
keywords: RivePlayer, State Machine, Animation, Vector
dev_langs:
  - csharp
category: Animations
subcategory: Input
---

<!-- To know about all the available Markdown syntax, Check out https://docs.microsoft.com/contribute/markdown-reference -->
<!-- Ensure you remove all comments before submission, to ensure that there are no formatting issues when displaying this page.  -->
<!-- It is recommended to check how the Documentation will look in the sample app, before Merging a PR -->
<!-- **Note:** All links to other docs.microsoft.com pages should be relative without locale, i.e. for the one above would be /contribute/markdown-reference -->
<!-- Included images should be optimized for size and not include any Intellectual Property references. -->

# RivePlayer

![Rive hero image](https://rive-app.notion.site/image/https%3A%2F%2Fs3-us-west-2.amazonaws.com%2Fsecure.notion-static.com%2Fff44ed5f-1eea-4154-81ef-84547e61c3fd%2Frive_notion.png?table=block&id=f198cab2-c0bc-4ce8-970c-42220379bcf3&spaceId=9c949665-9ad9-445f-b9c4-5ee204f8b60c&width=2000&userId=&cache=v2)

A high-level runtime for the [Windows Community Toolkit](https://docs.microsoft.com/windows/communitytoolkit/) to use [Rive](https://rive.app) in Universal Windows Platform (UWP) applications.

This library allows for control over Rive files with a high-level API for driving [state machines](https://help.rive.app/editor/state-machine) and animations.


## Getting Started

To add the Rive API to your applications, include the namespace in your XAML:

```xml
<Page 
    ...
    xmlns:Rive="using:CommunityToolkit.Labs.WinUI.Rive"
>
```

Note: This package is a wrapper around the [RiveSharp](https://github.com/rive-app/rive-sharp) library; a low-level API for Rive with C# that is used to build the render loop.

## Classes

### `<Rive:RivePlayer>`

High-level control class for a Rive instance to display on a canvas. It is declared in XAML files, and controlled via code or data-binding.

- Declared in XAML files
- Controlled via code or data binding

### Props

- `Width` - (double) Width of the canvas
- `Height` - (double) Height of the canvas
- `Source` - (string) URI to the `.riv` file to load into the app. Supported schemes are HTTP, HTTPS, and `ms-appx`
- `Artboard` - (string) Name of the Rive artboard to instantiate. If empty, the default artboard from the Rive file is loaded.
- `StateMachine` - (string) Name of the Rive state machine to instantiate from the artboard. If empty, the given `Animation` or default state machine is instantiated.
- `Animation` - (string) If `StateMachine` is empty, this is the name of the Rive animation to instantiate. If `Animation` is empty, and there is no `StateMachine` specified, nor a default state machine present in the Rive file, the default animation from the Rive file is instantiated.
- `DrawInBackground` - (bool) Rive rendering executes in a different thread than the UI

### `<Rive:BoolInput>`

High-level class for a state machine instance of a `boolean` input type. This should be nested under the `Rive:RivePlayer` element in XAML.

### Props

- `Target` - (string) Name of the correlated input in the state machine
- `Value` - (DependencyProperty) Binding to a boolean value

### Usage

One example using Rive `BoolInput` classes is binding it to checkbox UI Elements elsewhere in XAML. See below for a code snippet of this:

```xml
<Rive:RivePlayer Width="600"
                 Height="600"
                 DrawInBackground="True"
                 Source="ms-appx:///animated-login-screen.riv">
		<Rive:BoolInput Target="isChecking"
		                Value="{x:Bind CheckboxExample.IsChecked, Mode=OneWay}" />
</Rive:RivePlayer>
<CheckBox x:Name="CheckboxExample"
          Content="Example" />
```

In the example above, we nest the `Rive:BoolInput` strictly under the `Rive:RivePlayer` markup.

To create a reference to the boolean input named `isChecking` in the state machine of this Rive file, we set the `Target` property to that input name. Additionally, the `Value` property uses the `x:Bind` markup extension to bind a checkbox's `IsChecked` property to the boolean value of the state machine input.

When the checkbox is toggled, the appropriate "checked" status will be set on the value of the `isChecking` state machine input.

### `<Rive:NumberInput>`

High-level class for a state machine instance of a `double` input type. This should be nested under the `Rive:RivePlayer` element in XAML

### Props

- `Target` - (string) name of the correlated input in the state machine
- `Value` - (DependencyProperty) Binding to a number (double) value

### Usage

One example using Rive `NumberInput` classes is binding it to slider range UI Elements elsewhere in XAML. See below for a code snippet of this:

```xml
<Rive:RivePlayer Width="600"
				 Height="600"
				 Source="ms-appx:///animated-login-screen.riv">
		<Rive:NumberInput Target="numLook"
				          Value="{x:Bind SliderRangeExample.Value, Mode=OneWay}" />
</Rive:RivePlayer>
<Slider x:Name="SliderRangeExample"
        Maximum="100"
        Minimum="0"
        Value="0" />
```

In the example above, we nest the `Rive:NumberInput` strictly under the `Rive:RivePlayer` markup.

To create a reference to the number input named `numLook` in the state machine of this Rive file, we set the `Target` property to that input name. Additionally, the `Value` property uses the `x:Bind` markup extension to bind the slider's `Value` property to the number value of the state machine input.

When the slider range value is changed, the new number value will be set on the value of the `numLook` state machine input and the state machine will respond accordingly.

### `<Rive:TriggerInput>`

High-level class for a state machine instance of a trigger input type. This should be nested under the `Rive:RivePlayer` element in XAML

### Props

- `Target` - (string) name of the correlated input in the state machine
- `x:Name` - (string) name for this trigger reference

### Usage

One example using Rive `TriggerInput` classes is binding it to click events of button UI Elements elsewhere in XAML. See below for a code snippet of this:

```xml
<!-- Example.xaml -->  
<Rive:RivePlayer Width="600"
				 Height="600"
				 Source="ms-appx:///animated-login-screen.riv">
		<Rive:TriggerInput Target="trigSuccess"
						   x:Name="SuccessTriggerInput" />
</Rive:RivePlayer>
<Button Content="Success">
	  <Interactivity:Interaction.Behaviors>
			  <Interactions:EventTriggerBehavior EventName="Click">
						<Interactions:CallMethodAction MethodName="Fire"
												       TargetObject="{x:Bind SuccessTriggerInput}" />
			  </Interactions:EventTriggerBehavior>
      </Interactivity:Interaction.Behaviors>
</Button>
```

In the example above, we nest the `Rive:TriggerInput` strictly under the `Rive:RivePlayer` markup.

To create a reference to the trigger input named `trigSuccess` in the state machine of this Rive file, we set the `Target` property to that input name. Additionally, the `x:Name` property defines the name for the class so that the button knows what to bind the click event to to "fire" the trigger.

When the button is clicked, it will call into the `SuccessTriggerInput` input class and invoke the `Fire`  method on the `trigSuccess` Rive trigger input where the state machine will respond accordingly.

Additionally in this example, we use the [EventTriggerBehavior](https://github.com/Microsoft/XamlBehaviors/wiki/EventTriggerBehavior) and [CallMethodAction](https://github.com/Microsoft/XamlBehaviors/wiki/CallMethodAction) APIs from the [XAML Behaviors](https://github.com/Microsoft/XamlBehaviors/wiki) dependency. You can include the dependency in that example with the following snippet below in a `Dependencies.props` file:

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
    xmlns:Interactions="using:Microsoft.Xaml.Interactions.Core"
    xmlns:Interactivity="using:Microsoft.Xaml.Interactivity"
>
```

## Other Notes

Currently, this runtime uses [SKSwapChainPanel](https://docs.microsoft.com/en-us/dotnet/api/skiasharp.views.uwp.skswapchainpanel?view=skiasharp-views-2.88) from SkiaSharp to do GPU rendering. While you may notice slower frame rates drawing Rives, we are actively working on improving the rendering performance.
