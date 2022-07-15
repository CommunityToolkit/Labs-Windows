// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

// These `InternalsVisibleTo` calls are intended to make it easier for
// for any internal code to be testable in all the different test projects
// used with the Labs infrastructure.
[assembly: InternalsVisibleTo("SettingsCard.UnitTests.Uwp")]
[assembly: InternalsVisibleTo("SettingsCard.UnitTests.WinAppSdk")]
[assembly: InternalsVisibleTo("CommunityToolkit.Labs.UnitTests.Uwp")]
[assembly: InternalsVisibleTo("CommunityToolkit.Labs.UnitTests.WinAppSdk")]
