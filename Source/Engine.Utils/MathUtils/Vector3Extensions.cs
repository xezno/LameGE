using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Engine.Utils.MathUtils
{
    public static class Vector3Extensions
    {
        public static Vector3 Normalized(this Vector3 vector)
        {
            var normalizedVector = vector;
            if (normalizedVector.Length() == 0f)
            {
                return normalizedVector;
            }

            normalizedVector = Vector3.Normalize(normalizedVector);
            return normalizedVector;
        }
    }
}
