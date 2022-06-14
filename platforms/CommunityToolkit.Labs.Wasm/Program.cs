using CommunityToolkit.Labs.Shared;
using System;

#if WINAPPSDK
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace CommunityToolkit.Labs.Wasm;

	public class Program
	{
		private static App? _app;

		static int Main(string[] args)
		{
			Application.Start(_ => _app = new App());

			return 0;
		}
	}
