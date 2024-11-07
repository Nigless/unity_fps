using System;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector3Extensions
    {
        public static Vector3 Lerp(this Vector3 from, Vector3 to, float by)
        {
            return Vector3.Lerp(from, to, by);
        }

        public static float Angle(this Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }
    }
}