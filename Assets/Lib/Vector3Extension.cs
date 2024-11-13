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

        public static Vector3 MoveTowards(this Vector3 from, Vector3 to, float by)
        {
            return Vector3.MoveTowards(from, to, by);
        }

        public static float Dot(this Vector3 lhs, Vector3 rhs)
        {
            return Vector3.Dot(lhs, rhs);
        }

        public static float Angle(this Vector3 from, Vector3 to)
        {
            return Vector3.Angle(from, to);
        }

        public static Vector3 ProjectOnPlane(this Vector3 from, Vector3 to)
        {
            return Vector3.ProjectOnPlane(from, to);
        }
    }
}