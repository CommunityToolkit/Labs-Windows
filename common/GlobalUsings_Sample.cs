
// To learn more about this file see https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/using-directive#global-modifier

global using System.Runtime.InteropServices.WindowsRuntime;

global using CommunityToolkit.Labs.Core.SourceGenerators;
global using CommunityToolkit.Labs.Core.SourceGenerators.Attributes;

#if !WINAPPSDK
global using Windows.UI.Xaml.Automation;
global using Windows.UI.Xaml.Automation.Peers;
#else
global using Microsoft.UI.Xaml.Automation;
global using Microsoft.UI.Xaml.Automation.Peers;
#endif
