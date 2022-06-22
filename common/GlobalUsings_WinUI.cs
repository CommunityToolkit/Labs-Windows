// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains directives available to projects that are compiled for multiple frameworks.
// Learn more global using directives at https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/using-directive#global-modifier

global using System.Runtime.InteropServices.WindowsRuntime;

global using CommunityToolkit.Labs.WinUI;

global using Windows.Foundation;
global using Windows.Foundation.Collections;

#if !WINAPPSDK
global using Windows.ApplicationModel;
global using Windows.ApplicationModel.Activation;

global using Windows.UI.Xaml.Automation;
global using Windows.UI.Xaml.Automation.Peers;

global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
global using Windows.UI.Xaml.Controls.Primitives;
global using Windows.UI.Xaml.Data;
global using Windows.UI.Xaml.Input;
global using Windows.UI.Xaml.Markup;
global using Windows.UI.Xaml.Media;
global using Windows.UI.Xaml.Navigation;

#else

global using Microsoft.UI.Xaml.Automation;
global using Microsoft.UI.Xaml.Automation.Peers;

global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Controls.Primitives;
global using Microsoft.UI.Xaml.Data;
global using Microsoft.UI.Xaml.Input;
global using Microsoft.UI.Xaml.Markup;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Navigation;
#endif

global using MUXC = Microsoft.UI.Xaml.Controls;
