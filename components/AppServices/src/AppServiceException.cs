// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.AppServices;

/// <summary>
/// An <see cref="Exception"/> that is thrown by a host or component when an app service operation fails.
/// </summary>
public sealed class AppServiceException : Exception
{
    /// <summary>
    /// Creates a new <see cref="AppServiceException"/> with the specified parameters.
    /// </summary>
    /// <param name="status">The <see cref="AppServiceStatus"/> for the exception.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="hresult">The error code for the exception.</param>
    internal AppServiceException(AppServiceStatus status, string message, int hresult)
        : base(message)
    {
        Status = status;
        HResult = hresult;
    }

    /// <summary>
    /// Creates a new <see cref="AppServiceException"/> with the specified parameters.
    /// </summary>
    /// <param name="status">The <see cref="AppServiceStatus"/> for the exception.</param>
    internal AppServiceException(AppServiceStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Gets the status associated with the current exception.
    /// </summary>
    public AppServiceStatus Status { get; }
}