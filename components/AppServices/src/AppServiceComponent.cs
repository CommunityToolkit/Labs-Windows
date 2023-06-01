// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.AppServices.Helpers;

#pragma warning disable CA2213, CA1063

namespace CommunityToolkit.AppServices;

/// <summary>
/// A base type for an app service component (replying to requests from a host).
/// </summary>
public abstract class AppServiceComponent : IDisposable
{
    /// <summary>
    /// The name of the app service.
    /// </summary>
    private readonly string _appServiceName;

    /// <summary>
    /// The mapping of available endpoints for this component.
    /// </summary>
    private readonly Dictionary<string, Func<AppServiceParameters, Task<object?>>> _endpoints = new();

    /// <summary>
    /// <para>
    /// The mapping of cancellation keys to <see cref="CancellationTokenSource"/> instances. This is used to associate
    /// an executing request with its cancellation key, so that if the host sends a cancellation request, the component
    /// can lookup its associated <see cref="CancellationTokenSource"/> instance and propagate the cancellation.
    /// </para>
    /// <para>
    /// This mapping is populated when a request is received from the host, and cleared right after it completes or is
    /// canceled. Each individual request is responsible for adding and removing its own cancellation pair.
    /// </para>
    /// </summary>
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _cancellationSources = new();

    /// <summary>
    /// The <see cref="AppServiceConnection"/> instance to use.
    /// </summary>
    private AppServiceConnection? _connection;

    /// <summary>
    /// Raised whenever an app service connection fails to connect.
    /// </summary>
    /// <remarks>When this event is raised, usually a full trust process is expected to terminate.</remarks>
    public event EventHandler? ConnectionFailed;

    /// <summary>
    /// Raised whenever an app service connection is closed.
    /// </summary>
    /// <remarks>When this event is raised, usually a full trust process is expected to terminate.</remarks>
    public event EventHandler? ConnectionClosed;

    /// <summary>
    /// Creates a new <see cref="AppServiceComponent"/> instance with the specified parameters.
    /// </summary>
    /// <param name="appServiceName">The name of the app service.</param>
    protected AppServiceComponent(string appServiceName)
    {
        _appServiceName = appServiceName;
    }

    /// <summary>
    /// Initializes the app service.
    /// </summary>
    public async Task InitializeAppService()
    {
        Dispose();

        AppServiceConnection appServiceConnection = new()
        {
            PackageFamilyName = Package.Current.Id.FamilyName,
            AppServiceName = _appServiceName,
        };

        appServiceConnection.RequestReceived += Connection_RequestReceived;

        AppServiceConnectionStatus status = await appServiceConnection.OpenAsync();

        if (status != AppServiceConnectionStatus.Success)
        {
            ConnectionFailed?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            // Resets the connection and closes the application
            void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
            {
                _ = Interlocked.CompareExchange(ref _connection, null, sender);

                ConnectionClosed?.Invoke(this, EventArgs.Empty);
            }

            appServiceConnection.ServiceClosed += Connection_ServiceClosed;

            _connection = appServiceConnection;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        AppServiceConnection? connection = Interlocked.CompareExchange(ref _connection, null, null);

        if (connection is not null)
        {
            connection.Dispose();
        }
    }

    /// <summary>
    /// Registers a synchronous endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint(Action endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, _ =>
        {
            try
            {
                endpoint();

                return Task.FromResult<object?>(0);
            }
            catch (Exception e)
            {
                // Normalize exceptions for callers invoking the endpoint
                return Task.FromException<object?>(e);
            }
        });
    }

    /// <summary>
    /// Registers a synchronous endpoint.
    /// </summary>
    /// <typeparam name="T">The type of return value for the endpoint.</typeparam>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<T>(Func<T> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, _ =>
        {
            try
            {
                return Task.FromResult(ValueSetMarshaller.ToObject(endpoint()));
            }
            catch (Exception e)
            {
                // Normalize exceptions for callers invoking the endpoint
                return Task.FromException<object?>(e);
            }
        });
    }

    /// <summary>
    /// Registers a synchronous endpoint.
    /// </summary>
    /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
    /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
    /// <param name="serializer">The serializer to use to serialize the return value.</param>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<TSerializer, TResult>(TSerializer serializer, Func<TResult?> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
        where TSerializer : IValueSetSerializer<TResult>
    {
        _endpoints.Add(endpointName!, _ =>
        {
            try
            {
                return Task.FromResult<object?>(serializer.Serialize(endpoint()));
            }
            catch (Exception e)
            {
                // Normalize exceptions for callers invoking the endpoint
                return Task.FromException<object?>(e);
            }
        });
    }

    /// <summary>
    /// Registers a synchronous endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint(Action<AppServiceParameters> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, parameters =>
        {
            try
            {
                endpoint(parameters);

                return Task.FromResult<object?>(0);
            }
            catch (Exception e)
            {
                // Normalize exceptions for callers invoking the endpoint
                return Task.FromException<object?>(e);
            }
        });
    }

    /// <summary>
    /// Registers a synchronous endpoint.
    /// </summary>
    /// <typeparam name="T">The type of return value for the endpoint.</typeparam>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<T>(Func<AppServiceParameters, T> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, parameters =>
        {
            try
            {
                return Task.FromResult(ValueSetMarshaller.ToObject(endpoint(parameters)));
            }
            catch (Exception e)
            {
                // Normalize exceptions for callers invoking the endpoint
                return Task.FromException<object?>(e);
            }
        });
    }

    /// <summary>
    /// Registers a synchronous endpoint.
    /// </summary>
    /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
    /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
    /// <param name="serializer">The serializer to use to serialize the return value.</param>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<TSerializer, TResult>(TSerializer serializer, Func<AppServiceParameters, TResult?> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
        where TSerializer : IValueSetSerializer<TResult>
    {
        _endpoints.Add(endpointName!, parameters =>
        {
            try
            {
                return Task.FromResult<object?>(serializer.Serialize(endpoint(parameters)));
            }
            catch (Exception e)
            {
                // Normalize exceptions for callers invoking the endpoint
                return Task.FromException<object?>(e);
            }
        });
    }

    /// <summary>
    /// Registers an asynchronous endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint(Func<Task> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, async _ =>
        {
            await endpoint();

            return 0;
        });
    }

    /// <summary>
    /// Registers an asynchronous endpoint.
    /// </summary>
    /// <typeparam name="T">The type of return value for the endpoint.</typeparam>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<T>(Func<Task<T>> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, async _ => ValueSetMarshaller.ToObject(await endpoint()));
    }

    /// <summary>
    /// Registers an asynchronous endpoint.
    /// </summary>
    /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
    /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
    /// <param name="serializer">The serializer to use to serialize the return value.</param>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<TSerializer, TResult>(TSerializer serializer, Func<Task<TResult?>> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
        where TSerializer : IValueSetSerializer<TResult>
    {
        _endpoints.Add(endpointName!, async _ => serializer.Serialize(await endpoint()));
    }

    /// <summary>
    /// Registers an asynchronous endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint(Func<AppServiceParameters, Task> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, async parameters =>
        {
            await endpoint(parameters);

            return 0;
        });
    }

    /// <summary>
    /// Registers an asynchronous endpoint.
    /// </summary>
    /// <typeparam name="T">The type of return value for the endpoint.</typeparam>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<T>(Func<AppServiceParameters, Task<T>> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
    {
        _endpoints.Add(endpointName!, async parameters => ValueSetMarshaller.ToObject(await endpoint(parameters)));
    }

    /// <summary>
    /// Registers an asynchronous endpoint.
    /// </summary>
    /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
    /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
    /// <param name="serializer">The serializer to use to serialize the return value.</param>
    /// <param name="endpoint">The endpoint function.</param>
    /// <param name="endpointName">
    /// The endpoint name (it uses <see cref="CallerArgumentExpressionAttribute"/> targeting <paramref name="endpoint"/>,
    /// so a method group expression can be used to automatically pick up the method name as endpoint name).
    /// </param>
    protected void RegisterEndpoint<TSerializer, TResult>(TSerializer serializer, Func<AppServiceParameters, Task<TResult?>> endpoint, [CallerArgumentExpression("endpoint")] string? endpointName = null)
        where TSerializer : IValueSetSerializer<TResult>
    {
        _endpoints.Add(endpointName!, async parameters => serializer.Serialize(await endpoint(parameters)));
    }

    /// <summary>
    /// Handles an incoming app service request.
    /// </summary>
    /// <param name="sender">The <see cref="AppServiceConnection"/> instance for the request.</param>
    /// <param name="args">The request arguments.</param>
    private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
    {
        // Check if this is a cancellation request for a pending operation, and not an actual request.
        // If that is the case, request cancellation and then return, as there is no more work to do.
        try
        {
            if (args.Request.Message.TryGetValue(AppServiceHost.CancellationKey, out object? cancellationTokenKeyObj) &&
                cancellationTokenKeyObj is Guid cancellationKey)
            {
                RemoveCancellationSource(cancellationKey, requestCancellation: true);

                return;
            }
        }
        catch
        {
            // Exceptions shouldn't happen when just requesting cancellation, but if they do, they can just be ignored
            return;
        }

        AppServiceDeferral deferral = args.GetDeferral();

        try
        {
            // Extract the command name and the ValueSet instance with the arguments
            if (args.Request.Message.TryGetValue(AppServiceHost.CommandKey, out object? command) && command is string commandStr &&
                args.Request.Message.TryGetValue(AppServiceHost.ArgsKey, out var argsObj) && argsObj is ValueSet parameters)
            {
                object? response;

                try
                {
                    // Try to get the registered endpoint with the command name, and invoke it
                    if (_endpoints.TryGetValue(commandStr, out Func<AppServiceParameters, Task<object?>> endpoint))
                    {
                        response = await endpoint(new AppServiceParameters(this, sender, parameters));
                    }
                    else
                    {
                        throw new EntryPointNotFoundException();
                    }
                }
                catch (EntryPointNotFoundException ex)
                {
                    // The endpoint was not registered
                    ValueSet errorResponseContainer = new()
                    {
                        [AppServiceHost.StatusKey] = (int)AppServiceStatus.ActionNotFound,
                        [AppServiceHost.ReasonKey] = ex.Message
                    };

                    _ = await args.Request.SendResponseAsync(errorResponseContainer);

                    return;
                }
                catch (OperationCanceledException)
                {
                    // The operation was canceled, so it shouldn't be reported as an error
                    ValueSet errorResponseContainer = new() { [AppServiceHost.StatusKey] = (int)AppServiceStatus.Canceled };

                    _ = await args.Request.SendResponseAsync(errorResponseContainer);

                    return;
                }
                catch (Exception e)
                {
                    // Some exception was thrown by one of the registered endpoints
                    ValueSet errorResponseContainer = new()
                    {
                        [AppServiceHost.StatusKey] = (int)AppServiceStatus.Error,
                        [AppServiceHost.ReasonKey] = e.Message,
                        [AppServiceHost.HResultKey] = e.HResult
                    };

                    _ = await args.Request.SendResponseAsync(errorResponseContainer);

                    return;
                }

                // The endpoint was found and invoked successfully, return its result
                ValueSet responseContainer = new()
                {
                    [AppServiceHost.StatusKey] = (int)AppServiceStatus.Ok,
                    [AppServiceHost.ValueKey] = response
                };

                _ = await args.Request.SendResponseAsync(responseContainer);
            }
            else
            {
                // The input arguments to invoke the command were not present
                ValueSet errorResponseContainer = new()
                {
                    [AppServiceHost.StatusKey] = (int)AppServiceStatus.InvalidRequest
                };

                _ = await args.Request.SendResponseAsync(errorResponseContainer);
            }
        }
        catch (Exception ex)
        {
            // Some unknown exception was thrown in the whole response handling
            ValueSet errorResponseContainer = new()
            {
                [AppServiceHost.StatusKey] = (int)AppServiceStatus.Error,
                [AppServiceHost.ReasonKey] = ex.Message,
                [AppServiceHost.HResultKey] = ex.HResult
            };

            _ = await args.Request.SendResponseAsync(errorResponseContainer);
        }
        finally
        {
            // Regardless of the result, also remove the cancellation source, if one was available
            if (args.Request.Message.TryGetValue(AppServiceHost.CancellationKey, out object? cancellationTokenKeyObj) &&
                cancellationTokenKeyObj is Guid cancellationKey)
            {
                RemoveCancellationSource(cancellationKey, requestCancellation: false);
            }

            deferral.Complete();
        }
    }

    /// <summary>
    /// Adds a new cancellation source to the set managed by this component.
    /// </summary>
    /// <param name="id">The cancellation source id.</param>
    /// <param name="cancellationTokenSource">The <see cref="CancellationTokenSource"/> instance.</param>
    private void AddCancellationSource(Guid id, CancellationTokenSource cancellationTokenSource)
    {
        _ = _cancellationSources.TryAdd(id, cancellationTokenSource);
    }

    /// <summary>
    /// Removes a cancellation source from the set managed by this component and optionally cancels it.
    /// </summary>
    /// <param name="id">The cancellation source id.</param>
    /// <param name="requestCancellation">Whether to request cancellation when removing the cancellation source.</param>
    private void RemoveCancellationSource(Guid id, bool requestCancellation)
    {
        if (_cancellationSources.TryRemove(id, out CancellationTokenSource? cancellationTokenSource) && requestCancellation)
        {
            cancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    /// An object that can be used to retrieve parameters
    /// </summary>
    protected readonly struct AppServiceParameters
    {
        /// <summary>
        /// The <see cref="AppServiceComponent"/> owner for the current parameters.
        /// </summary>
        private readonly AppServiceComponent _component;

        /// <summary>
        /// The <see cref="AppServiceConnection"/> instance used for the current request.
        /// </summary>
        private readonly AppServiceConnection _connection;

        /// <summary>
        /// The <see cref="ValueSet"/> object with the available parameters.
        /// </summary>
        private readonly ValueSet _valueSet;

        /// <summary>
        /// Creates a new <see cref="AppServiceParameters"/> instance with the specified parameters.
        /// </summary>
        /// <param name="component">The <see cref="AppServiceComponent"/> owner for the current parameters.</param>
        /// <param name="connection">The <see cref="AppServiceConnection"/> instance used for the current request.</param>
        /// <param name="valueSet">The <see cref="ValueSet"/> object with the available parameters.</param>
        public AppServiceParameters(AppServiceComponent component, AppServiceConnection connection, ValueSet valueSet)
        {
            _component = component;
            _connection = connection;
            _valueSet = valueSet;
        }

        /// <summary>
        /// Gets a parameter of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of parameter to retrieve.</typeparam>
        /// <param name="parameter">The resulting parameter.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="InvalidOperationException">Thrown if there is no parameter with the specified name.</exception>
        public void GetParameter<T>(out T parameter, [CallerArgumentExpression("parameter")] string? parameterName = null)
        {
            FixupParameterName(ref parameterName);

            // Try to get the requested parameter
            if (!_valueSet.TryGetValue(parameterName, out object? value) ||
                !ValueSetMarshaller.TryGetValue(value, out T? result))
            {
                throw new InvalidOperationException($"Failed to retrieve parameter with name \"{parameterName}\" and type \"{typeof(T)}\".");
            }

            parameter = result;
        }

        /// <summary>
        /// Gets a parameter of a specified type.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
        /// <typeparam name="TParameter">The type of parameter to retrieve.</typeparam>
        /// <param name="serializer">The serializer to use to load <paramref name="parameter"/>.</param>
        /// <param name="parameter">The resulting parameter.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <exception cref="InvalidOperationException">Thrown if there is no parameter with the specified name.</exception>
        /// <exception cref="Exception">Thrown if the deserialization failed.</exception>
        public void GetParameter<TSerializer, TParameter>(TSerializer serializer, out TParameter? parameter, [CallerArgumentExpression("parameter")] string? parameterName = null)
            where TSerializer: IValueSetSerializer<TParameter>
        {
            FixupParameterName(ref parameterName);

            // Try to get the requested parameter
            if (!_valueSet.TryGetValue(parameterName, out object? value) ||
                value is not (null or ValueSet))
            {
                throw new InvalidOperationException($"Failed to retrieve parameter with name \"{parameterName}\" and type \"{typeof(ValueSet)}\".");
            }

            parameter = serializer.Deserialize((ValueSet?)value);
        }

        /// <summary>
        /// Gets an <see cref="IProgress{T}"/> instance of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of progress values being used.</typeparam>
        /// <param name="progress">The resulting <see cref="IProgress{T}"/> instance.</param>
        /// <exception cref="InvalidOperationException">Thrown if the current request has no progress support.</exception>
        public void GetProgress<T>(out IProgress<T> progress)
        {
            // Try to get the progress id to send values back to the host
            if (!_valueSet.TryGetValue(AppServiceHost.ProgressKey, out object? progressKeyObj) ||
                progressKeyObj is not Guid progressKey)
            {
                throw new InvalidOperationException("The current request does not support progress reporting.");
            }

            AppServiceConnection connection = _connection;

            async void ReportProgress(T value)
            {
                try
                {
                    await connection.SendMessageAsync(new ValueSet
                    {
                        [AppServiceHost.ProgressKey] = progressKey,
                        [AppServiceHost.ProgressValue] = ValueSetMarshaller.ToObject(value)
                    });
                }
                catch
                {
                    // If a progress message is lost, it can just be ignored. The operation
                    // should not fail nor the component should crash if that happens.
                }
            }

            progress = new Progress<T>(ReportProgress);
        }

        /// <summary>
        /// Gets an <see cref="IProgress{T}"/> instance of a specified type.
        /// </summary>
        /// <typeparam name="TSerializer">The type of serializer to use.</typeparam>
        /// <typeparam name="TResult">The type of return value for the endpoint.</typeparam>
        /// <param name="serializer">The serializer to use to serialize the return value.</param>
        /// <param name="progress">The resulting <see cref="IProgress{T}"/> instance.</param>
        /// <exception cref="InvalidOperationException">Thrown if the current request has no progress support.</exception>
        public void GetProgress<TSerializer, TResult>(TSerializer serializer, out IProgress<TResult?> progress)
            where TSerializer : IValueSetSerializer<TResult>
        {
            // Try to get the progress id to send values back to the host
            if (!_valueSet.TryGetValue(AppServiceHost.ProgressKey, out object? progressKeyObj) ||
                progressKeyObj is not Guid progressKey)
            {
                throw new InvalidOperationException("The current request does not support progress reporting.");
            }

            AppServiceConnection connection = _connection;

            async void ReportProgress(TResult? value)
            {
                try
                {
                    await connection.SendMessageAsync(new ValueSet
                    {
                        [AppServiceHost.ProgressKey] = progressKey,
                        [AppServiceHost.ProgressValue] = serializer.Serialize(value)
                    });
                }
                catch
                {
                    // If a progress message is lost, it can just be ignored. The operation
                    // should not fail nor the component should crash if that happens.
                }
            }

            progress = new Progress<TResult?>(ReportProgress);
        }

        /// <summary>
        /// Gets a <see cref="CancellationToken"/> instance from the current parameters.
        /// </summary>
        /// <param name="cancellationToken">The resulting <see cref="CancellationToken"/> instance.</param>
        /// <exception cref="InvalidOperationException">Thrown if the current request has no cancellation support.</exception>
        public void GetCancellationToken(out CancellationToken cancellationToken)
        {
            // Try to get the cancellation id to lookup the local CancellationTokenSource instance
            if (!_valueSet.TryGetValue(AppServiceHost.CancellationKey, out object? cancellationTokenKeyObj) ||
                cancellationTokenKeyObj is not Guid cancellationTokenKey)
            {
                throw new InvalidOperationException("The current request does not support cancellation.");
            }

            CancellationTokenSource cancellationTokenSource = new();

            _component.AddCancellationSource(cancellationTokenKey, cancellationTokenSource);

            cancellationToken = cancellationTokenSource.Token;
        }

        /// <summary>
        /// Applies the necessary fixup to a parameter name.
        /// </summary>
        /// <param name="parameterName">The parameter name to fixup.</param>
        private static void FixupParameterName([NotNull] ref string? parameterName)
        {
            // If the parameter name has no spaces, it means it's an explicit name. In that case
            // the valuer doesn't need to be parsed, and it can be used directly as value name.
            if (parameterName!.IndexOf(' ') != -1)
            {
                int leftSeparatorIndex = parameterName.TrimEnd().LastIndexOf(' ');
                int rightSeparatorIndex = parameterName.TrimEnd().Length - 1;

                // The input expression will be in the form: "out <TYPE> <NAME>". We need to get the <NAME> part.
                // So we trim the end and get the last index of ' ' (ie. the space on the left), and then also
                // trim and calculate the actual end of the parameter name. Then we slice the input name with that.
                parameterName = parameterName!.Substring(leftSeparatorIndex + 1, rightSeparatorIndex - leftSeparatorIndex);
            }
        }
    }
}
