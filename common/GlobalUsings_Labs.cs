
// To learn more about this file see https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/using-directive#global-modifier

global using System.Runtime.InteropServices.WindowsRuntime;
global using CommunityToolkit.Labs.WinUI;

global using Microsoft.UI.Xaml.Controls;

global using Windows.Foundation;
global using Windows.Foundation.Collections;

#if !WINAPPSDK
global using Windows.ApplicationModel;
global using Windows.ApplicationModel.Activation;

global using Windows.UI.Xaml;
global using Windows.UI.Xaml.Controls;
global using Windows.UI.Xaml.Controls.Primitives;
global using Windows.UI.Xaml.Data;
global using Windows.UI.Xaml.Input;
global using Windows.UI.Xaml.Media;
global using Windows.UI.Xaml.Navigation;

#else
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls.Primitives;
global using Microsoft.UI.Xaml.Data;
global using Microsoft.UI.Xaml.Input;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Navigation;
#endif
