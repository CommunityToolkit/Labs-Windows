// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using GLib;
using Uno.UI.Runtime.Skia;

namespace CommunityToolkit.Labs.Skia.Gtk;

class Program
{
	static void Main(string[] args)
	{
		ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs)
		{
			Console.WriteLine("GLIB UNHANDLED EXCEPTION" + expArgs.ExceptionObject.ToString());
			expArgs.ExitApplication = true;
		};

		var host = new GtkHost(() => new Shared.App(), args);

		host.Run();
	}
}
