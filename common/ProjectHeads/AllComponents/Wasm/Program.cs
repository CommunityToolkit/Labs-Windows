// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.App.Shared;

#if WINAPPSDK
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace CommunityToolkit.Wasm;

public class Program
{
	private static App? _app;

	static int Main(string[] args)
	{
		Application.Start(_ => _app = new App());

		return 0;
	}
}
