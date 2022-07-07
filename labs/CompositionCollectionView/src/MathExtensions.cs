#nullable enable
using Windows.Foundation;

namespace CompositionCollectionView
{
    internal static class MathExtensions
    {
        public static double DistanceTo(this Point p1, Point p2) => System.Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
    }
}