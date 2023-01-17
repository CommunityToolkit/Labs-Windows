// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AppKit;

namespace CommunityToolkit.Labs.macOS;

static class MainClass
{
	static void Main(string[] args)
	{
		NSApplication.Init();
		NSApplication.SharedApplication.Delegate = new Shared.App();
		NSApplication.Main(args);  
	}
}
