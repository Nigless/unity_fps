using System;
using UnityEngine;

namespace ExtensionMethods
{
    public static class Vector2Extensions
    {
        public static Vector2 Lerp(this Vector2 from, Vector2 to, float by)
        {
            return Vector2.Lerp(from, to, by);
        }

        public static float Angle(this Vector2 from, Vector2 to)
        {
            return Vector2.Angle(from, to);
        }
    }
}

