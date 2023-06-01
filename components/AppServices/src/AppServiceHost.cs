// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using CommunityToolkit.AppServices.Helpers;

#pragma warning disable CA1068

namespace CommunityToolkit.AppServices;

/// <summary>
/// A base type for an app service host (sending requests to a component).
/// </summary>
public abstract class AppServiceHost
{
    internal const string CommandKey = "__endpoint";
    internal const string ArgsKey = "__args";
    internal const string StatusKey = "__status";
    internal const string ReasonKey = "__reason";
    internal const string ValueKey = "__value";
    internal const string HResultKey = "__HRESULT";
    internal const string ProgressKey = "__progressKey";
    internal const string ProgressValue = "__progressValue";
    internal const string CancellationKey = "__cancellationKey";

    /// <summary>
    /// The name of the app service.
    /// </summary>
    private readonly string _appServiceName;
    private readonly SemaphoreSlim _semaphoreConnection = new(0, 1);
    private readonly SemaphoreSlim _lockConnection = new(1, 1);

    /// <summary>
    /// <para>
    /// The mapping of progress keys to <see cref="IProgress{T}"/> instances. This is used to associate
    /// incoming progress messages with the <see cref="IProgress{T}"/> instance passed by callers, so
    /// the current progress amount can be forwarded to the right one.
    /// </para>
    /// <para>
    /// This mapping is populated when a request is started, and cleared right after it completes.
    /// Each individual request is responsible for adding and removing its own progress pair.
    /// </para>
    /// </summary>
    private readonly ConcurrentDictionary<Guid, IProgress<object>> _progressTrackers = new();
    private BackgroundTaskDeferral? _appServiceDeferral;
    private AppServiceConnection? _appServiceConnection;

    /// <summary>
    /// Creates a new <see cref="AppServiceHost"/> instance with the specified parameters.
    /// </summary>
    /// <param name="appServiceName">The name of the app service.</param>
    protected AppServiceHost(string appServiceName)
    {
        _appServiceName = appServiceName;
    }

    /// <summary>
    /// Gets a value indicating whether the app service functionality can be used.
    /// </summary>
    private static bool CanUseAppServiceFunctionality { get; } = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop" && ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0);

    /// <summary>
    /// Handles the app service activation when <see cref="Windows.UI.Xaml.Application.OnBackgroundActivated(BackgroundActivatedEventArgs)"/> is invoked.
    /// </summary>
    /// <param name="args">The args for the background activation.</param>
    /// <returns>Whether this activation was an app service connection that could be handled by this host.</returns>
    /// <remarks>
    /// <para>
    /// When this method returns <see langword="true"/>, no further work should be done by the caller.
    /// </para>
    /// <para>
    /// This method should be used as follows (from <c>App.xaml.cs</c>):
    /// <code>
    /// protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
    /// {
    ///     base.OnBackgroundActivated(args);
    ///
    ///     if (DesktopExtension.OnBackgroundActivated(args))
    ///     {
    ///         return;
    ///     }
    ///
    ///     // Any other work, if needed
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public bool OnBackgroundActivated(BackgroundActivatedEventArgs args)
    {
        IBackgroundTaskInstance backgroundTaskInstance = args.TaskInstance;

        // Check that the activation is an app service connection response
        if (backgroundTaskInstance.TriggerDetails is not AppServiceTriggerDetails appServiceTriggerDetails)
        {
            return false;
        }

        // Check if the connection is from the same package
        if (!appServiceTriggerDetails.CallerPackageFamilyName.Equals(Package.Current.Id.FamilyName, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        // Check that the app service name matches the one for this host instance
        if (!appServiceTriggerDetails.AppServiceConnection.AppServiceName.Equals(_appServiceName, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        BackgroundTaskDeferral? previousAppServiceDeferral = _appServiceDeferral;

        _appServiceDeferral = backgroundTaskInstance.GetDeferral();

        bool hadNoConnection = _appServiceConnection is null;

        _appServiceConnection = appServiceTriggerDetails.AppServiceConnection;

        appServiceTriggerDetails.AppServiceConnection.ServiceClosed += AppServiceConnection_ServiceClosed;
        appServiceTriggerDetails.AppServiceConnection.RequestReceived += AppServiceConnection_RequestReceived;

        if (hadNoConnection)
        {
            _semaphoreConnection.Release();
        }

        previousAppServiceDeferral?.Complete();

        return true;
    }

    /// <summary>
    /// Creates a new <see cref="AppServiceRequest"/> for a given operation.
    /// </summary>
    /// <param name="requestName">The name of the request to prepare.</param>
    /// <returns>An <see cref="AppServiceRequest"/> instance to construct a request to send.</returns>
    protected AppServiceRequest CreateAppServiceRequest([CallerMemberName] string? requestName = null)
    {
        return new(this, requestName!);
    }

    /// <summary>
    /// Closes a connection when a service is closed.
    /// </summary>
    /// <param name="sender">The <see cref="AppServiceConnection"/> that was closed.</param>
    /// <param name="args">The arguments for the operation.</param>
    private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
    {
        CloseConnection(sender);
    }

    /// <summary>
    /// Closes a target <see cref="AppServiceConnection"/> instance.
    /// </summary>
    /// <param name="appServiceConnection">The <see cref="AppServiceConnection"/> instance to close.</param>
    private void CloseConnection(AppServiceConnection appServiceConnection)
    {
        if (appServiceConnection is null)
        {
            return;
        }

        appServiceConnection.ServiceClosed -= AppServiceConnection_ServiceClosed;
        appServiceConnection.RequestReceived -= AppServiceConnection_RequestReceived;

        if (_appServiceConnection == appServiceConnection)
        {
            _appServiceConnection = null;

            try
            {
                _appServiceDeferral?.Complete();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    /// Handles an incoming app service request.
    /// </summary>
    /// <param name="sender">The <see cref="AppServiceConnection"/> instance that received the request.</param>
    /// <param name="args">The request arguments.</param>
    private void AppServiceConnection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
        // If this is a request to report progress, try to retrieve the progress tracker and invoke it.
        // If the progress is not present, it means its associated request has already completed, so it
        // is safe to ignore it. Unhandled exceptions in the progress handler will bubble up normally.
        if (args.Request.Message.TryGetValue(ProgressKey, out object? progressKey) &&
            args.Request.Message.TryGetValue(ProgressValue, out object? progressValue) &&
            progressKey is Guid id &&
            _progressTrackers.TryGetValue(id, out IProgress<object> progress))
        {
            progress.Report(progressValue);
        }
    }

    /// <summary>
    /// Tries to get an <see cref="AppServiceConnection"/> instance to send request to.
    /// </summary>
    /// <param name="timeout">The connection timeout.</param>
    /// <returns>A <see cref="Task{TResult}"/> producing the connection, if successful.</returns>
    /// <exception cref="AppServiceException">Thrown if trying to create or open a connection failed.</exception>
    /// <exception cref="Exception">Thrown if the full trust process fails to launch.</exception>
    private async Task<AppServiceConnection?> GetConnectionAsync(TimeSpan timeout)
    {
        if (_appServiceConnection is null)
        {
            await _lockConnection.WaitAsync();

            try
            {
                if (_appServiceConnection is null)
                {
                    if (await StartAppServiceAsync())
                    {
                        if (_appServiceConnection is null && !await _semaphoreConnection.WaitAsync(timeout))
                        {
                            if (_appServiceConnection is null)
                            {
                                throw new AppServiceException(AppServiceStatus.Timeout);
                            }
                        }
                    }
                    else
                    {
                        throw new AppServiceException(AppServiceStatus.CantStart);
                    }
                }
            }
            finally
            {
                _lockConnection.Release();
            }
        }

        return _appServiceConnection;
    }

    /// <summary>
    /// Starts the app service connection.
    /// </summary>
    /// <returns>Whether starting the connection was successful.</returns>
    private static async Task<bool> StartAppServiceAsync()
    {
        if (!CanUseAppServiceFunctionality)
        {
            return false;
        }

        try
        {
            await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Sends an app service connection message.
    /// </summary>
    /// <param name="request">The request parameters.</param>
    /// <param name="cancellationKey">The id to notify cancellation, if available.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to use.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <param name="canRetry">Whether the operation can be attempted again in case of failure.</param>
    /// <returns>The response for the request.</returns>
    private async Task<AppServiceResponse?> SendMessageAsync(
        ValueSet request,
        Guid cancellationKey,
        CancellationToken cancellationToken,
        TimeSpan timeout,
        bool canRetry = true)
    {
        if (await GetConnectionAsync(timeout) is not { } connection)
        {
            return null;
        }

        AppServiceResponse response;
        CancellationTokenRegistration registration;

        // If there is a valid cancellation token, handle the possible cases.
        // Otherwise, just use a dummy registration token that does nothing.
        if (cancellationKey == Guid.Empty)
        {
            registration = default;
        }
        else
        {
            // If the token has already been canceled, just fail immediately
            if (cancellationToken.IsCancellationRequested)
            {
                throw new AppServiceException(AppServiceStatus.Canceled);
            }

            // Otherwise, register the token cancellation to notify the component
            registration = cancellationToken.Register(async () =>
            {
                try
                {
                    await connection.SendMessageAsync(new ValueSet { [CancellationKey] = cancellationKey });
                }
                catch
                {
                    // If a cancellation request is lost, just ignore it. This shouldn't really fail anyway
                    // given that at this point the connection has already been established with the component.
                    // If that wasn't the case, it'd mean that there would not be a request to cancel at all.
                }
            });
        }

        // Ensure the cancellation registration is unregistered once the request completes
        using (registration)
        {
            response = await connection.SendMessageAsync(request);
        }

        if (response?.Status == AppServiceResponseStatus.Failure && canRetry)
        {
            // When the application process is paused by the OS, it also stops the full trust (extension) process
            // without raising AppServiceConnection.ServiceClosed. To mitigate the scenario, we create a new
            // appServiceConnection (including launching the full trust process) and try one more time.
            CloseConnection(connection);

            return await SendMessageAsync(request, cancellationKey, cancellationToken, timeout, false);
        }

        return response;
    }

    /// <summary>
    /// Adds a new progress tracker to the set managed by this host.
    /// </summary>
    /// <param name="id">The progress tracker id.</param>
    /// <param name="progress">The <see cref="IProgress{T}"/> instance.</param>
    private void AddProgressTracker(Guid id, IProgress<object> progress)
    {
        _ = _progressTrackers.TryAdd(id, progress);
    }

    /// <summary>
    /// Removes a progress tracker from the set managed by this host.
    /// </summary>
    /// <param name="id">The progress tracker id.</param>
    private void RemoveProgressTracker(Guid id)
    {
        _ = _progressTrackers.TryRemove(id, out _);
    }

    /// <summary>
    /// Sends an app service request.
    /// </summary>
    /// <typeparam name="T">The type of response to expect.</typeparam>
    /// <param name="commandName">The name of the endpoint to invoke.</param>
    /// <param name="args">The request arguments.</param>
    /// <param name="cancellationKey">The id to notify cancellation, if available.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance to use.</param>
    /// <param name="serializer">The optional serializer for the return value.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>The result for the request.</returns>
    /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
    private async Task<T?> SendAppServiceRequestAsync<T>(
        string commandName,
        ValueSet args,
        Guid cancellationKey,
        CancellationToken cancellationToken,
        IValueSetSerializer<T>? serializer,
        TimeSpan timeout)
    {
        if (!CanUseAppServiceFunctionality)
        {
            throw new AppServiceException(AppServiceStatus.AppServiceNotCompatible);
        }

        ValueSet request = new() { [CommandKey] = commandName, [ArgsKey] = args };
        AppServiceResponse? answer = await SendMessageAsync(request, cancellationKey, cancellationToken, timeout);

        if (answer?.Status == AppServiceResponseStatus.Success)
        {
            if (answer.Message.TryGetValue(StatusKey, out object? statusValue))
            {
                if (statusValue is not int rawStatus)
                {
                    throw new AppServiceException(AppServiceStatus.InvalidResponse);
                }

                AppServiceStatus status = (AppServiceStatus)rawStatus;

                if (status == AppServiceStatus.Ok)
                {
                    // The response must contain the known return value
                    if (!answer.Message.TryGetValue(ValueKey, out object? value))
                    {
                        throw new AppServiceException(AppServiceStatus.InvalidResponse);
                    }

                    T? result = default;

                    // The response must match the expected return value:
                    //   - If a serializer is available, it must be null or a ValueSet
                    //   - If there is no serializer, it must be a valid T instance
                    if ((serializer is not null && value is not (null or ValueSet)) ||
                        (serializer is null && (value is null || !ValueSetMarshaller.TryGetValue(value, out result))))
                    {
                        throw new AppServiceException(AppServiceStatus.MismatchedResponseType);
                    }

                    // If there is a serializer, invoke it to produce the return value
                    if (serializer is not null)
                    {
                        try
                        {
                            return serializer.Deserialize((ValueSet?)value);
                        }
                        catch
                        {
                            throw new AppServiceException(AppServiceStatus.SerializationError);
                        }
                    }

                    // Otherwise return the value directly
                    return result;
                }
                else if (status is AppServiceStatus.InvalidRequest or AppServiceStatus.Canceled)
                {
                    // If the operation is canceled or an invalid request was sent, no other exception info is needed
                    throw new AppServiceException(status);
                }

                // Report an error on the component side
                throw new AppServiceException(
                    status,
                    (string)answer.Message[ReasonKey],
                    answer.Message.TryGetValue(HResultKey, out object? hr) ? (int)hr : 0);
            }
            else
            {
                throw new AppServiceException(AppServiceStatus.NoResponse);
            }
        }
        else
        {
            throw new AppServiceException(AppServiceStatus.CantSend);
        }
    }

    /// <summary>
    /// An object that can be used to build app service requests.
    /// </summary>
    protected sealed class AppServiceRequest
    {
        /// <summary>
        /// The source <see cref="AppServiceHost"/> instance.
        /// </summary>
        private readonly AppServiceHost _host;

        /// <summary>
        /// The request name.
        /// </summary>
        private readonly string _requestName;

        /// <summary>
        /// The <see cref="ValueSet"/> instance with the arguments for the current request.
        /// </summary>
        private readonly ValueSet _valueSet;

        /// <summary>
        /// The <see cref="IProgress{T}"/> instance to use, if available;
        /// </summary>
        private IProgress<object>? _progress;

        /// <summary>
        /// The id of the <see cref="IProgress{T}"/> instance to use, if available.
        /// </summary>
        private Guid _progressId;

        /// <summary>
        /// The <see cref="CancellationToken"/> instance to use, if available.
        /// </summary>
        private CancellationToken _cancellationToken;

        /// <summary>
        /// The id of the <see cref="CancellationToken"/> instance to use, if available.
        /// </summary>
        private Guid _cancellationTokenId;

        /// <summary>
        /// Creates a new <see cref="AppServiceRequest"/> instance with the specified parameters.
        /// </summary>
        /// <param name="host">The source <see cref="AppServiceHost"/> instance.</param>
        /// <param name="requestName">The request name.</param>
        public AppServiceRequest(AppServiceHost host, string requestName)
        {
            _host = host;
            _requestName = requestName;
            _valueSet = new ValueSet();
        }

        /// <summary>
        /// Adds a new parameter to the request.
        /// </summary>
        /// <typeparam name="T">The type of the parameter.</typeparam>
        /// <param name="parameter">The parameter to add to the request.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>The current <see cref="AppServiceRequest"/> instance.</returns>
        public AppServiceRequest WithParameter<T>(T parameter, [CallerArgumentExpression("parameter")] string? parameterName = null)
        {
            _valueSet.Add(parameterName!, ValueSetMarshaller.ToObject(parameter));

            return this;
        }

        /// <summary>
        /// Adds a new parameter to the request.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
        /// <typeparam name="TParameter">The type of parameter to retrieve.</typeparam>
        /// <param name="serializer">The serializer to use to load <paramref name="parameter"/>.</param>
        /// <param name="parameter">The parameter to add to the request.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>The current <see cref="AppServiceRequest"/> instance.</returns>
        public AppServiceRequest WithParameter<TSerializer, TParameter>(TSerializer serializer, TParameter? parameter, [CallerArgumentExpression("parameter")] string? parameterName = null)
            where TSerializer : IValueSetSerializer<TParameter>
        {
            _valueSet.Add(parameterName!, serializer.Serialize(parameter));

            return this;
        }

        /// <summary>
        /// Adds an <see cref="IProgress{T}"/> parameter to the request.
        /// </summary>
        /// <typeparam name="T">The progress value to use.</typeparam>
        /// <param name="progress">The <see cref="IProgress{T}"/> instance for the request.</param>
        /// <returns>The current <see cref="AppServiceRequest"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progress"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an <see cref="IProgress{T}"/> instance has already been registered on this request.</exception>
        /// <remarks>A request can only have a single <see cref="IProgress{T}"/> object associated with it.</remarks>
        [MemberNotNull(nameof(_progress))]
        public AppServiceRequest WithProgress<T>(IProgress<T> progress)
        {
            if (progress is null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            if (_progressId != Guid.Empty)
            {
                throw new InvalidOperationException("Only one IProgress<T> instance can be used per AppService request.");
            }

            _progress = new Progress<object>(value => progress.Report(ValueSetMarshaller.ToValue<T>(value)));
            _progressId = Guid.NewGuid();

            return this;
        }

        /// <summary>
        /// Adds an <see cref="IProgress{T}"/> parameter to the request.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
        /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
        /// <param name="serializer">The serializer to use to serialize the return value.</param>
        /// <param name="progress">The <see cref="IProgress{T}"/> instance for the request.</param>
        /// <returns>The current <see cref="AppServiceRequest"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="progress"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown if an <see cref="IProgress{T}"/> instance has already been registered on this request.</exception>
        /// <remarks>A request can only have a single <see cref="IProgress{T}"/> object associated with it.</remarks>
        [MemberNotNull(nameof(_progress))]
        public AppServiceRequest WithProgress<TSerializer, TResult>(TSerializer serializer, IProgress<TResult?> progress)
             where TSerializer : IValueSetSerializer<TResult>
        {
            if (progress is null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            if (_progressId != Guid.Empty)
            {
                throw new InvalidOperationException("Only one IProgress<T> instance can be used per AppService request.");
            }

            _progress = new Progress<object>(value => progress.Report(serializer.Deserialize((ValueSet?)value)));
            _progressId = Guid.NewGuid();

            return this;
        }

        /// <summary>
        /// Adds a <see cref="CancellationToken"/> parameter to the request.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance for the request.</param>
        /// <returns>The current <see cref="AppServiceRequest"/> instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="WithCancellationToken"/> has already been called on this request.</exception>
        /// <remarks>The <see cref="WithCancellationToken"/> can only be called once per request.</remarks>
        public AppServiceRequest WithCancellationToken(CancellationToken cancellationToken)
        {
            if (_cancellationTokenId != Guid.Empty)
            {
                throw new InvalidOperationException("Only one CancellationToken instance can be used per AppService request.");
            }

            _cancellationToken = cancellationToken;
            _cancellationTokenId = Guid.NewGuid();

            return this;
        }

        /// <summary>
        /// Sends the request, waits for a response and tries to convert the result to a specified type.
        /// </summary>
        /// <returns>A <see cref="Task"/> with the expected result, if successful.</returns>
        /// <remarks>This overload will use a default timeout value of 5 seconds.</remarks>
        /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
        public Task SendAndWaitForResultAsync()
        {
            return SendAndWaitForResultAsync<int>(serializer: null, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Sends the request, waits for a response and tries to convert the result to a specified type.
        /// </summary>
        /// <typeparam name="T">The type of result to expect.</typeparam>
        /// <returns>A <see cref="Task{TResult}"/> with the expected result, if successful.</returns>
        /// <remarks>This overload will use a default timeout value of 5 seconds.</remarks>
        /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
        public Task<T> SendAndWaitForResultAsync<T>()
        {
            return SendAndWaitForResultAsync<T>(serializer: null, TimeSpan.FromSeconds(5))!;
        }

        /// <summary>
        /// Sends the request, waits for a response and tries to convert the result to a specified type.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
        /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
        /// <param name="serializer">The serializer to use to serialize the return value.</param>
        /// <returns>A <see cref="Task{TResult}"/> with the expected result, if successful.</returns>
        /// <remarks>This overload will use a default timeout value of 5 seconds.</remarks>
        /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
        public Task<TResult?> SendAndWaitForResultAsync<TSerializer, TResult>(TSerializer serializer)
            where TSerializer : IValueSetSerializer<TResult>
        {
            return SendAndWaitForResultAsync(serializer, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Sends the request, waits for a response and tries to convert the result to a specified type.
        /// </summary>
        /// <param name="timeout">The timeout to start and wait for a service connection.</param>
        /// <returns>A <see cref="Task"/> with the expected result, if successful.</returns>
        /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
        public Task SendAndWaitForResultAsync(TimeSpan timeout)
        {
            return SendAndWaitForResultAsync<int>(serializer: null, timeout);
        }

        /// <summary>
        /// Sends the request, waits for a response and tries to convert the result to a specified type.
        /// </summary>
        /// <typeparam name="T">The type of result to expect.</typeparam>
        /// <param name="timeout">The timeout to start and wait for a service connection.</param>
        /// <returns>A <see cref="Task{TResult}"/> with the expected result, if successful.</returns>
        /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
        public Task<T> SendAndWaitForResultAsync<T>(TimeSpan timeout)
        {
            return SendAndWaitForResultAsync<T>(serializer: null, timeout)!;
        }

        /// <summary>
        /// Sends the request, waits for a response and tries to convert the result to a specified type.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
        /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
        /// <param name="serializer">The serializer to use to serialize the return value.</param>
        /// <param name="timeout">The timeout to start and wait for a service connection.</param>
        /// <returns>A <see cref="Task{TResult}"/> with the expected result, if successful.</returns>
        /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
        public Task<TResult?> SendAndWaitForResultAsync<TSerializer, TResult>(TSerializer serializer, TimeSpan timeout)
            where TSerializer : IValueSetSerializer<TResult>
        {
            return SendAndWaitForResultAsync(serializer, timeout);
        }

        /// <summary>
        /// Sends the request, waits for a response and tries to convert the result to a specified type.
        /// </summary>
        /// <typeparam name="T">The type of result to expect.</typeparam>
        /// <param name="serializer">The serializer, if available.</param>
        /// <param name="timeout">The timeout to start and wait for a service connection.</param>
        /// <returns>A <see cref="Task{TResult}"/> with the expected result, if successful.</returns>
        /// <exception cref="AppServiceException">Thrown if the request fails.</exception>
        private async Task<T?> SendAndWaitForResultAsync<T>(IValueSetSerializer<T>? serializer, TimeSpan timeout)
        {
            // If a progress has been configured, add it to the trackers set for the current host.
            // The id also needs to be sent, as it'll be used by the component to report progress back.
            if (_progressId != Guid.Empty)
            {
                _valueSet.Add(ProgressKey, _progressId);

                _host.AddProgressTracker(_progressId, _progress!);
            }

            // If a cancellation token has been configured
            if (_cancellationTokenId != Guid.Empty)
            {
                _valueSet.Add(CancellationKey, _cancellationTokenId);
            }

            try
            {
                return await _host.SendAppServiceRequestAsync(
                    _requestName,
                    _valueSet,
                    _cancellationTokenId,
                    _cancellationToken,
                    serializer,
                    timeout);
            }
            finally
            {
                // Remove the progress tracker when the request is complete
                if (_progress is not null)
                {
                    _host.RemoveProgressTracker(_progressId);
                }
            }
        }
    }
}