
// To learn more about this file see https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/using-directive#global-modifier

#if !WINAPPSDK
global using Microsoft.Toolkit.Uwp;
global using Microsoft.Toolkit.Uwp.UI;
global using Microsoft.Toolkit.Uwp.UI.Helpers;
global using Windows.Foundation;
global using Windows.Foundation.Collections;
global using Windows.UI;
global using Windows.UI.Core;
global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Automation;
global using Windows.UI.Xaml.Automation.Peers;
global using Windows.UI.Xaml.Controls;
global using Windows.UI.Xaml.Controls.Primitives;
global using Windows.UI.Xaml.Data;
global using Windows.UI.Xaml.Input;
global using Windows.UI.Xaml.Markup;
global using Windows.UI.Xaml.Media;
global using Windows.UI.Xaml.Navigation;
global using MUXC = Microsoft.UI.Xaml.Controls;
#else
global using CommunityToolkit.WinUI;
global using CommunityToolkit.WinUI.UI;
global using CommunityToolkit.WinUI.UI.Helpers;
global using Microsoft.UI;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Automation;
global using Microsoft.UI.Xaml.Automation.Peers;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Controls.Primitives;
global using Microsoft.UI.Xaml.Data;
global using Microsoft.UI.Xaml.Input;
global using Microsoft.UI.Xaml.Markup;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Navigation;
global using MUXC = Microsoft.UI.Xaml.Controls;
#endif

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
global using CommunityToolkit.Labs.WinUI;
