// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Nostra13.Universalimageloader.Core;

#if WINAPPSDK
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace CommunityToolkit.Labs.Droid;

[global::Android.App.ApplicationAttribute(
	Label = "@string/ApplicationName",
	Icon = "@mipmap/icon",
	LargeHeap = true,
	HardwareAccelerated = true,
	Theme = "@style/AppTheme"
)]
public class Application : NativeApplication
{
	public Application(IntPtr javaReference, JniHandleOwnership transfer)
		: base(() => new CommunityToolkit.Labs.Shared.App(), javaReference, transfer)
	{
		ConfigureUniversalImageLoader();
	}

	private void ConfigureUniversalImageLoader()
	{
		// Create global configuration and initialize ImageLoader with this config
		ImageLoaderConfiguration config = new ImageLoaderConfiguration
			.Builder(Context)
			.Build();

		ImageLoader.Instance.Init(config);

		ImageSource.DefaultImageLoader = ImageLoader.Instance.LoadImageAsync;
	}
}
