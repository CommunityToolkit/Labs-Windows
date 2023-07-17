// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System;
using Microsoft.CodeAnalysis;
using CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Helpers;

namespace CommunityToolkit.Extensions.DependencyInjection.SourceGenerators.Extensions;

/// <summary>
/// Extension methods for the <see cref="IncrementalValuesProvider{TValues}"/> type.
/// </summary>
internal static class IncrementalValuesProviderExtensions
{
    /// <summary>
    /// Concatenates two <see cref="IncrementalValuesProvider{TValues}"/> sources into one.
    /// </summary>
    /// <typeparam name="T">The type of items to combine.</typeparam>
    /// <param name="left">The first source.</param>
    /// <param name="right">The second source.</param>
    /// <returns>The resulting sequence combining items from both <paramref name="left"/> and then <paramref name="right"/>.</returns>
    public static IncrementalValuesProvider<T> Concat<T>(this IncrementalValuesProvider<T> left, IncrementalValuesProvider<T> right)
    {
        IncrementalValueProvider<ImmutableArray<T>> leftItems = left.Collect();
        IncrementalValueProvider<ImmutableArray<T>> rightItems = right.Collect();
        IncrementalValueProvider<(ImmutableArray<T> Left, ImmutableArray<T> Right)> allItems = leftItems.Combine(rightItems);

        return allItems.SelectMany((item, token) =>
        {
            ImmutableArray<T>.Builder builder = ImmutableArray.CreateBuilder<T>(item.Left.Length + item.Right.Length);

            builder.AddRange(item.Left);
            builder.AddRange(item.Right);

            return builder.MoveToImmutable();
        });
    }

    /// <summary>
    /// Groups items in a given <see cref="IncrementalValuesProvider{TValue}"/> sequence by a specified key.
    /// </summary>
    /// <typeparam name="T">The type of items in the source.</typeparam>
    /// <typeparam name="TKey">The type of resulting key elements.</typeparam>
    /// <typeparam name="TElement">The type of resulting projected elements.</typeparam>
    /// <param name="source">The input <see cref="IncrementalValuesProvider{TValues}"/> instance.</param>
    /// <param name="keySelector">The key selection <see cref="Func{T, TResult}"/>.</param>
    /// <returns>An <see cref="IncrementalValuesProvider{TValues}"/> with the grouped results.</returns>
    public static IncrementalValuesProvider<T> GroupBy<T, TKey, TElement>(
        this IncrementalValuesProvider<T> source,
        Func<T, TKey> keySelector,
        Func<T, ImmutableArray<TElement>> elementsSelector,
        Func<TKey, ImmutableArray<TElement>, T> resultSelector)
        where T : IEquatable<T>
        where TKey : IEquatable<TKey>
        where TElement : IEquatable<TElement>
    {
        return source.Collect().SelectMany((item, token) =>
        {
            Dictionary<TKey, ImmutableArrayBuilder<TElement>> map = new();

            // For each input item, extract the key and the items and group them
            foreach (T entry in item)
            {
                TKey key = keySelector(entry);
                ImmutableArray<TElement> items = elementsSelector(entry);

                // Get or create a builder backed by a pooled array
                if (!map.TryGetValue(key, out ImmutableArrayBuilder<TElement> builder))
                {
                    builder = ImmutableArrayBuilder<TElement>.Rent();

                    map.Add(key, builder);
                }

                // Aggregate all items for the current key
                builder.AddRange(items.AsSpan());
            }

            token.ThrowIfCancellationRequested();

            ImmutableArray<T>.Builder result = ImmutableArray.CreateBuilder<T>(map.Count);

            // For each grouped pair, create the resulting item
            foreach (KeyValuePair<TKey, ImmutableArrayBuilder<TElement>> entry in map)
            {
                // Copy the aggregated items to a new array
                ImmutableArray<TElement> elements = entry.Value.ToImmutable();

                // Manually dispose the rented buffer to return the array to the pool
                entry.Value.Dispose();

                result.Add(resultSelector(entry.Key, elements));
            }

            return result;
        });
    }
}