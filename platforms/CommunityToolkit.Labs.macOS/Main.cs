using AppKit;

namespace CommunityToolkit.Labs.macOS
{
	static class MainClass
	{
		static void Main(string[] args)
		{
			NSApplication.Init();
			NSApplication.SharedApplication.Delegate = new Shared.App();
			NSApplication.Main(args);  
		}
	}
}

