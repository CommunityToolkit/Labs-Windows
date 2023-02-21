// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.App.Shared;

#if WINAPPSDK
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace CommunityToolkit.App.Wasm;

public class Program
{
	private static CommunityToolkit.App.Shared.App? _app;

	static int Main(string[] args)
	{
		Application.Start(_ => _app = new CommunityToolkit.App.Shared.App());

		return 0;
	}
}
