
#if !WINAPPSDK
#warning NOT using WinAppSDK
global using Microsoft.Toolkit.Uwp;
global using Microsoft.Toolkit.Uwp.UI;
global using Windows.UI.Xaml.Automation;
global using Windows.UI.Xaml.Automation.Peers;
global using Windows.UI.Xaml.Controls;
global using MUXC = Microsoft.UI.Xaml.Controls;
#else
#warning using WinAppSDK
global using CommunityToolkit.WinUI;
global using CommunityToolkit.WinUI.UI;
global using Microsoft.UI.Xaml.Automation;
global using Microsoft.UI.Xaml.Automation.Peers;
global using Microsoft.UI.Xaml.Controls;
global using MUXC = Microsoft.UI.Xaml.Controls;
#endif

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
