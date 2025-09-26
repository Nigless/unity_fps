using System;
using UnityEngine;

namespace ExtensionMethods
{
    public static class BoundsExtensions
    {
        public static Bounds? Intersection(this Bounds self, Bounds bounds)
        {
            if (!self.Intersects(bounds)) return null;

            Vector3 minIntersection = Vector3.Max(self.min, bounds.min);
            Vector3 maxIntersection = Vector3.Min(self.max, bounds.max);
            Vector3 sizeIntersection = maxIntersection - minIntersection;

            return new Bounds(minIntersection + sizeIntersection / 2, sizeIntersection);
        }

        public static float Volume(this Bounds self)
        {
            Vector3 size = self.size;
            return size.x * size.y * size.z;
        }
    }
}