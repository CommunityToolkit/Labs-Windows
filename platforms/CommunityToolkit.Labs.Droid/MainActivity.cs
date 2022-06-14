using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;

#if WINAPPSDK
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace CommunityToolkit.Labs.Droid;

	[Activity(
			MainLauncher = true,
			ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : ApplicationActivity
	{
	}

