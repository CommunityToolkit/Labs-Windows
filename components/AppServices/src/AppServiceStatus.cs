// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.AppServices;

/// <summary>
/// Indicates the status of an app service operation.
/// </summary>
public enum AppServiceStatus
{
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Ok,

    /// <summary>
    /// The operation failed with an error.
    /// </summary>
    Error,

    /// <summary>
    /// The entry point for the operation was not found.
    /// </summary>
    ActionNotFound,

    /// <summary>
    /// No response was received for the operation.
    /// </summary>
    NoResponse,

    /// <summary>
    /// A request was received, but it was invalid.
    /// </summary>
    InvalidRequest,

    /// <summary>
    /// A response was received, but it was invalid.
    /// </summary>
    InvalidResponse,

    /// <summary>
    /// A response result was received, but it was of a mismatched type.
    /// </summary>
    MismatchedResponseType,

    /// <summary>
    /// An error occurred while trying to deserialize the response with a custom serializer.
    /// </summary>
    SerializationError,

    /// <summary>
    /// There was an error and it wasn't possible to send a request.
    /// </summary>
    CantSend,

    /// <summary>
    /// The app service is not compatible.
    /// </summary>
    AppServiceNotCompatible,

    /// <summary>
    /// The operation timed out.
    /// </summary>
    Timeout,

    /// <summary>
    /// The operation was canceled.
    /// </summary>
    Canceled,

    /// <summary>
    /// The operation failed to start.
    /// </summary>
    CantStart
}
