// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace CommunityToolkit.WinUI.Helpers;

public partial class ColorPaletteSampler
{
    private ref struct DBScan
    {
        private const int Unclassified = -1;

        public static Vector3[] Cluster(Span<Vector3> points, float epsilon, int minPoints, ref float[] weights)
        {
            var centroids = new List<Vector3>();
            var newWeights = new List<float>();

            // Create context
            var context = new DBScan(points, weights, epsilon, minPoints);

            // Attempt to create a cluster around each point,
            // skipping that point if already classified
            for (int i = 0; i < points.Length; i++)
            {
                // Already classified, skip
                if (context.PointClusterIds[i] is not Unclassified)
                    continue;

                // Attempt to create cluster
                if(context.CreateCluster(i, out var centroid, out var weight))
                {
                    centroids.Add(centroid);
                    newWeights.Add(weight);
                }
            }

            weights = newWeights.ToArray();
            return centroids.ToArray();
        }

        private bool CreateCluster(int originIndex, out Vector3 centroid, out float weight)
        {
            weight = 0;
            centroid = Vector3.Zero;
            var seeds = GetSeeds(originIndex, out bool isCore);

            // Not enough seeds to be a core point.
            // Cannot create a cluster around it
            if (!isCore)
            {
                return false;
            }

            ExpandCluster(seeds, out centroid, out weight);
            ClusterId++;

            return true;
        }

        private void ExpandCluster(Queue<int> seeds, out Vector3 centroid, out float weight)
        {
            weight = 0;
            centroid = Vector3.Zero;
            while(seeds.Count > 0)
            {
                var seedIndex = seeds.Dequeue();

                // Skip duplicate seed entries
                if (PointClusterIds[seedIndex] is not Unclassified)
                    continue;

                // Assign this seed's id to the cluster
                PointClusterIds[seedIndex] = ClusterId;
                var w = Weights[seedIndex];
                centroid += Points[seedIndex] * w;
                weight += w;

                // Check if this seed is a core point
                var grandSeeds = GetSeeds(seedIndex, out var seedIsCore);
                if (!seedIsCore)
                    continue;

                // This seed is a core point. Enqueue all its seeds
                foreach(var grandSeedIndex in grandSeeds)
                    if (PointClusterIds[grandSeedIndex] is Unclassified)
                        seeds.Enqueue(grandSeedIndex);
            }

            centroid /= weight;
        }

        private Queue<int> GetSeeds(int originIndex, out bool isCore)
        {
            var origin = Points[originIndex];

            // NOTE: Seeding could be done using a spatial data structure to improve traversal
            // speeds. However currently DBSCAN is run after KMeans with a maximum of 8 points.
            // There is no need.

            var seeds = new Queue<int>();
            for (int i = 0; i < Points.Length; i++)
            {
                if (Vector3.DistanceSquared(origin, Points[i]) <= Epsilon2)
                    seeds.Enqueue(i);
            }

            // Count includes self, so compare without checking equals
            isCore = seeds.Count > MinPoints;
            return seeds;
        }

        private DBScan(Span<Vector3> points, Span<float> weights, float epsilon, int minPoints)
        {
            Points = points;
            Weights = weights;
            Epsilon2 = epsilon * epsilon;
            MinPoints = minPoints;

            ClusterId = 0;
            PointClusterIds = new int[points.Length];
            for(int i  = 0; i < points.Length; i++)
                PointClusterIds[i] = Unclassified;
        }

        /// <summary>
        /// Gets the points being clustered.
        /// </summary>
        public Span<Vector3> Points { get; }

        /// <summary>
        /// Gets the weights of the points.
        /// </summary>
        public Span<float> Weights { get; }

        /// <summary>
        /// Gets or sets the id of the currently evaluating cluster.
        /// </summary>
        public int ClusterId { get; set; }

        /// <summary>
        /// Gets an array containing the id of the cluster each point belongs to.
        /// </summary>
        public int[] PointClusterIds { get; }

        /// <summary>
        /// Gets epsilon squared. Where epsilon is the max distance to consider two points connected.
        /// </summary>
        /// <remarks>
        /// This is cached as epsilon squared to skip a sqrt operation when comparing distances to epsilon.
        /// </remarks>
        public double Epsilon2 { get; }

        /// <summary>
        /// Gets the minimum number of points required to make a core point.
        /// </summary>
        public int MinPoints { get; }
    }
}
