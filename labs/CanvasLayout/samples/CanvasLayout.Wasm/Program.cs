using CommunityToolkit.Labs.Shared;

#if WINAPPSDK
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace CanvasLayout.Wasm
{
	public class Program
	{
		private static App? _app;

		static int Main(string[] args)
		{
			Application.Start(_ => _app = new App());

			return 0;
		}
	}
}
