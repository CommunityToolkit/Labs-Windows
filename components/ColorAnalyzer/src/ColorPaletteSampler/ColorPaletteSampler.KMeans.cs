// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace CommunityToolkit.WinUI.Helpers;

public partial class ColorPaletteSampler
{
    private static Vector3[] KMeansCluster(Span<Vector3> points, int k, out int[] counts)
    {
        // Track the assigned cluster of each point
        int[] clusterIds = new int[points.Length];

        // Track the centroids of each cluster and its member count
        Span<Vector3> centroids = stackalloc Vector3[k];
        counts = new int[k];
        
        // Split the points into arbitrary clusters
        Split(k, clusterIds);

        bool converged = false;
        while (!converged)
        {
            // Assume we've converged. If we haven't, we'll assign converged
            // to false when adjust the clusters
            converged = true;

            // Calculate/Recalculate centroids
            CalculateCentroidsAndPrune(ref centroids, ref counts, points, clusterIds);

            // Move each point's clusterId to the nearest cluster centroid
            for (int i = 0; i < points.Length; i++)
            {
                var nearestIndex = FindNearestClusterIndex(points[i], centroids);

                // The nearest cluster hasn't changed. Do nothing
                if (clusterIds[i] == nearestIndex)
                    continue;

                // Update the cluster id and note that we have not converged
                clusterIds[i] = nearestIndex;
                converged = false;
            }
        }

        return centroids.ToArray();
    }

    /// <summary>
    /// Assigns arbitrary clusterIds for each point
    /// </summary>
    private static void Split(int k, int[] clusterIds)
    {
        // Mathematically true random sampling
#if NET6_0_OR_GREATER
        var offset = Random.Shared.Next(k); 
#else
        var rand = new Random();
        var offset = rand.Next(k);
#endif

        // Assign each clusters id
        for (int i = 0; i < clusterIds.Length; i++)
            clusterIds[i] = (i + offset) % k;
    }

    /// <summary>
    /// Calculates the centroid of each cluster, and prunes empty clusters.
    /// </summary>
    internal static void CalculateCentroidsAndPrune(ref Span<Vector3> centroids, ref int[] counts, Span<Vector3> points, int[] clusterIds)
    {
        // Clear centroids and counts before recalculation
        for (int i = 0; i < centroids.Length; i++)
        {
            centroids[i] = Vector3.Zero;
            counts[i] = 0;
        }
        
        // Accumulate step in centroid calculation
        for (int i = 0; i < clusterIds.Length; i++)
        {
            int id = clusterIds[i];
            centroids[id] += points[i];
            counts[id]++;
        }
        
        // Prune empty clusters
        // All empty clusters are swapped to the end of the span
        // then a slice is taken with only the remaining populated clusters
        int pivot = counts.Length;
        for (int i = 0; i < pivot;)
        {
            // Increment and continue if populated
            if (counts[i] != 0)
            {
                i++;
                continue;
            }
        
            // The item is not populated. Swap to end and move pivot
            // NOTE: This is a one-way "swap". We're discarding the 0s anyways.
            pivot--;
            centroids[i] = centroids[pivot];
            counts[i] = counts[pivot];
        }
        
        // Perform slice
#if !WINDOWS_UWP
        counts = counts[..pivot];
        centroids = centroids[..pivot];
#elif WINDOWS_UWP
        Array.Resize(ref counts, pivot);
        centroids = centroids.Slice(0, pivot);
#endif
        
        // Division step in centroid calculation
        for (int i = 0; i < centroids.Length; i++)
            centroids[i] /= counts[i];
    }

    /// <summary>
    /// Finds the index of the centroid nearest the point.
    /// </summary>
    private static int FindNearestClusterIndex(Vector3 point, Span<Vector3> centroids)
    {
        // Track the nearest centroid's distance and the index of that centroid
        float nearestDistance = float.PositiveInfinity;
        int nearestIndex = -1;
        
        for (int j = 0; j < centroids.Length; j++)
        {
            // Compare the point to the jth centroid
            float distance = Vector3.DistanceSquared(point, centroids[j]);
        
            // Skip the cluster if further than the nearest seen cluster 
            if (nearestDistance < distance)
                continue;
        
            // This is the nearest cluster
            // Update the distance and index
            nearestDistance = distance;
            nearestIndex = j;
        }

        return nearestIndex;
    }
}
