// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace CommunityToolkit.WinUI.Helpers;

public partial class AccentAnalyzer
{
    private static Vector3[] KMeansCluster(Span<Vector3> points, int k, out int[] counts)
    {
        // Track the assigned cluster of each point
        int[] clusterIds = new int[points.Length];

        // Track the centroids of each cluster and its member count
        // TODO: stackalloc is great here, but pooling should be thresholded
        // just in case
        Span<Vector3> centroids = stackalloc Vector3[k];
        counts = new int[k];

        // Split the points into arbitrary clusters
        // NOTE: Can this be rearranged to converge faster?
        var offset = Random.Shared.Next(k); // Mathematically true random sampling
        for (int i = 0; i < clusterIds.Length; i++)
            clusterIds[i] = (i + offset) % k;

        bool converged = false;
        while (!converged)
        {
            // Assume we've converged. If we haven't, we'll assign converged
            // to false when adjust the clusters
            converged = true;

            // KMeans Loop Step 1:
            // Calculate/Recalculate the centroids of each cluster

            // Clear centroids and counts before recalculation
            for(int i = 0; i < centroids.Length; i++)
            {
                centroids[i] = Vector3.Zero;
                counts[i] = 0;
            }

            // Accumulate step in centroid calculation
            for(int i = 0; i < clusterIds.Length; i++)
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
                // NOTE: This is a oneway swap. We're discarding the 0 anyways.
                pivot--;
                counts[i] = counts[pivot];
            }
            counts = counts[..pivot];
            centroids = centroids[..pivot];

            // Division step in centroid calculation
            for (int i = 0; i < centroids.Length; i++)
                centroids[i] /= counts[i];
            
            // KMeans Loop Step 2:
            // Move each point's clusterId to the nearest cluster centroid
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 point = points[i];
                var oldId = clusterIds[i];

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

                // The nearest cluster hasn't changed. Do nothing
                if (oldId == nearestIndex)
                    continue;

                // Update the cluster id and note that we have not converged
                clusterIds[i] = nearestIndex;
                converged = false;
            }
        }

        return centroids.ToArray();
    }

    private static float FindColorfulness(Vector3 color)
    {
        var rg = color.X - color.Y;
        var yb = ((color.X + color.Y) / 2) - color.Z;
        return 0.3f * new Vector2(rg, yb).Length();
    }

    private static float FindColorfulness(Vector3[] colors)
    {
        // Isolate rg and yb
        var rg = colors.Select(x => Math.Abs(x.X - x.Y));
        var yb = colors.Select(x => Math.Abs(0.5f * (x.X + x.Y) - x.Z));

        // Evaluate rg and yb mean and std
        var rg_std = FindStandardDeviation(rg, out var rg_mean);
        var yb_std = FindStandardDeviation(yb, out var yb_mean);

        // Combine means and standard deviations
        var std = new Vector2(rg_mean, yb_mean).Length();
        var mean = new Vector2(rg_std, yb_std).Length();

        // Return colorfulness
        return std + (0.3f * mean);
    }

    private static float FindStandardDeviation(IEnumerable<float> data, out float avg)
    {
        var average = data.Average();
        avg = average;
        var sumOfSquares = data.Select(x => (x - average) * (x - average)).Sum();
        return (float)Math.Sqrt(sumOfSquares / data.Count());
    }
}
