// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINAPPSDK
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System.Profile;

namespace CommunityToolkit.WinUI.Controls;
internal static class InfoHelper
{
    public static Version AppVersion { get; } = new Version(
        Package.Current.Id.Version.Major,
        Package.Current.Id.Version.Minor,
        Package.Current.Id.Version.Build,
        Package.Current.Id.Version.Revision
        );

    public static Version SystemVersion { get; }

    public static SystemDataPaths SystemDataPath { get; } = SystemDataPaths.GetDefault();

    public static UserDataPaths UserDataPath { get; } = UserDataPaths.GetDefault();

    public static string AppInstalledLocation { get; } = Package.Current.InstalledLocation.Path;

    static InfoHelper()
    {
        string systemVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
        ulong version = ulong.Parse(systemVersion);
        SystemVersion = new Version(
            (int)((version & 0xFFFF000000000000L) >> 48),
            (int)((version & 0x0000FFFF00000000L) >> 32),
            (int)((version & 0x00000000FFFF0000L) >> 16),
            (int)(version & 0x000000000000FFFFL)
            );
    }
}
#endif
