// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CommunityToolkit.GeneratedDependencyProperty.Helpers;
using Microsoft.CodeAnalysis;

namespace CommunityToolkit.GeneratedDependencyProperty.Extensions;

/// <summary>
/// Extension methods for <see cref="IncrementalValuesProvider{TValues}"/>.
/// </summary>
internal static class IncrementalValuesProviderExtensions
{
    /// <summary>
    /// Groups items in a given <see cref="IncrementalValuesProvider{TValue}"/> sequence by a specified key.
    /// </summary>
    /// <typeparam name="TValues">The type of value that this source provides access to.</typeparam>
    /// <typeparam name="TKey">The type of grouped key elements.</typeparam>
    /// <typeparam name="TElement">The type of projected elements.</typeparam>
    /// <typeparam name="TResult">The type of resulting items.</typeparam>
    /// <param name="source">The input <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
    /// <param name="keySelector">The key selection <see cref="Func{T, TResult}"/>.</param>
    /// <param name="elementSelector">The element selection <see cref="Func{T, TResult}"/>.</param>
    /// <param name="resultSelector">The result selection <see cref="Func{T, TResult}"/>.</param>
    /// <returns>An <see cref="IncrementalValuesProvider{TValues}"/> with the grouped results.</returns>
    public static IncrementalValuesProvider<TResult> GroupBy<TValues, TKey, TElement, TResult>(
        this IncrementalValuesProvider<TValues> source,
        Func<TValues, TKey> keySelector,
        Func<TValues, TElement> elementSelector,
        Func<(TKey Key, EquatableArray<TElement> Values), TResult> resultSelector)
        where TValues : IEquatable<TValues>
        where TKey : IEquatable<TKey>
        where TElement : IEquatable<TElement>
        where TResult : IEquatable<TResult>
    {
        return source.Collect().SelectMany((item, token) =>
        {
            Dictionary<TKey, ImmutableArray<TElement>.Builder> map = [];

            foreach (TValues value in item)
            {
                TKey key = keySelector(value);
                TElement element = elementSelector(value);

                if (!map.TryGetValue(key, out ImmutableArray<TElement>.Builder builder))
                {
                    builder = ImmutableArray.CreateBuilder<TElement>();

                    map.Add(key, builder);
                }

                builder.Add(element);
            }

            token.ThrowIfCancellationRequested();

            using ImmutableArrayBuilder<TResult> result = new();

            foreach (KeyValuePair<TKey, ImmutableArray<TElement>.Builder> entry in map)
            {
                result.Add(resultSelector((entry.Key, entry.Value.ToImmutable())));
            }

            return result.ToImmutable();
        });
    }
}
