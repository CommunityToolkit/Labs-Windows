// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace CommunityToolkit.Tests;

/// <summary>
/// Use to log different types of error messages for MS Test. (Originally polyfill for TAEF API for MSTest, but keeping as nice to abstract intent of message.)
/// </summary>
public static class Log
{
    public static void Comment(string format, params object[] args)
    {
        LogMessage(format, args);
    }

    public static void Warning(string format, params object[] args)
    {
        LogMessage("[Warning] " + format, args);
    }

    public static void Error(string format, params object[] args)
    {
        LogMessage("[Error] " + format, args);
    }

    private static void LogMessage(string format, object[] args)
    {
        // string.Format() complains if we pass it something with braces, even if we have no arguments.
        // To account for that, we'll escape braces if we have no arguments.
        if (args.Length == 0)
        {
            format = format.Replace("{", "{{").Replace("}", "}}");
        }

        Logger.LogMessage(format, args);
    }
}
