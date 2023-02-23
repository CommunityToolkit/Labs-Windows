// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Labs.WinUI.Rive;

namespace RivePlayerExperiment.Samples;

/// <summary>
/// An example sample page of a custom control inheriting from Panel.
/// </summary>
[ToolkitSample(id: nameof(RivePlayerCustomSample), "Rive Player Sample", description: $"A sample for showing how to create and use a {nameof(RivePlayer)} custom control.")]
public sealed partial class RivePlayerCustomSample : Page
{
    public RivePlayerCustomSample()
    {
        this.InitializeComponent();
#if HAS_UNO
        // The Uno WASM platform seems to have trouble with our "ms-appx://" URI. Give it an http.
        this.RivePlayer.Source = "https://public.rive.app/community/runtime-files/2244-4463-animated-login-screen.riv";
#endif
    }
}
