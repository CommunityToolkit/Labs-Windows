// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.Labs.WPF.Host;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : System.Windows.Window
{
	public MainWindow()
	{
		InitializeComponent();

		root.Content = new global::Uno.UI.Skia.Platform.WpfHost(Dispatcher, () => new Shared.App());
	}
}
