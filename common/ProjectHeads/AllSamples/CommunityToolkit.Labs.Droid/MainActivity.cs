// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
