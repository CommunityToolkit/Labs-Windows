// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains directives available in test projects.
// Learn more global using directives at https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/using-directive#global-modifier

#if !WINAPPSDK
global using Microsoft.Toolkit.Uwp;
global using Microsoft.Toolkit.Uwp.UI;
global using Microsoft.Toolkit.Uwp.UI.Helpers;
global using Windows.UI;
global using Windows.UI.Core;
#else
global using CommunityToolkit.WinUI;
global using CommunityToolkit.WinUI.UI;
global using CommunityToolkit.WinUI.UI.Helpers;
global using Microsoft.UI;
#endif

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
