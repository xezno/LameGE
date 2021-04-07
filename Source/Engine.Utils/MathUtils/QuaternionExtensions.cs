using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Engine.Utils.MathUtils
{
    public static class QuaternionExtensions
    {
        public static Quaternion CreateFromEulerAngles(Vector3 euler)
        {
            return Quaternion.CreateFromYawPitchRoll(euler.Y, euler.X, euler.Z);
        }
    }
}
