using CommunityToolkit.Labs.Shared;

namespace CanvasLayout.Wasm;

public class Program
{
	private static App? _app;

	static int Main(string[] args)
	{
		Application.Start(_ => _app = new App());

		return 0;
	}
}
