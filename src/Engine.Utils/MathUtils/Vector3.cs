using System;

namespace Engine.Utils.MathUtils
{
    public struct Vector3
    {
        /// <summary>
        /// The point at which the vector resides on the X axis
        /// </summary>
        public float x;

        /// <summary>
        /// The point at which the vector resides on the Y axis
        /// </summary>
        public float y;

        /// <summary>
        /// The point at which the vector resides on the Z axis
        /// </summary>
        public float z;

        public float Magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

        public Vector3 Normalized
        {
            get
            {
                if (Math.Abs(Magnitude) < 0.0001f)
                    return new Vector3(0, 0, 0);
                return this / Magnitude;
            }
        }

        /// <summary>
        /// Construct a <see cref="Vector3"/> with three initial values.
        /// </summary>
        /// <param name="x">The initial x coordinate</param>
        /// <param name="y">The initial y coordinate</param>
        /// <param name="z">The initial z coordinate</param>
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.x * b.x,
                                                                                a.y * b.y,
                                                                                a.z * b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x,
                                                                                a.y - b.y,
                                                                                a.z - b.z);
        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.x / b.x,
                                                                                a.y / b.y,
                                                                                a.z / b.z);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x,
                                                                                a.y + b.y,
                                                                                a.z + b.z);

        public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.x * b,
                                                                            a.y * b,
                                                                            a.z * b);
        public static Vector3 operator -(Vector3 a, float b) => new Vector3(a.x - b,
                                                                            a.y - b,
                                                                            a.z - b);
        public static Vector3 operator /(Vector3 a, float b) => new Vector3(a.x / b,
                                                                            a.y / b,
                                                                            a.z / b);

        public static Vector3 operator +(Vector3 a, float b) => new Vector3(a.x + b,
                                                                            a.y + b,
                                                                            a.z + b);

        public static Vector3 operator %(Vector3 a, float b) => new Vector3(a.x % b,
                                                                            a.y % b,
                                                                            a.z % b);

        public static Vector3 up = new Vector3(0, 1, 0);
        public static Vector3 right = new Vector3(1, 0, 0);
        public static Vector3 forward = new Vector3(0, 0, 1);

        /// <summary>
        /// Get all values within the <see cref="Vector3"/> as a string.
        /// </summary>
        /// <returns>All coordinates (<see cref="x"/>, <see cref="y"/> and <see cref="z"/>) concatenated as a string.</returns>
        public override string ToString()
        {
            return $"{x}, {y}, {z}";
        }

        public static Vector3 ConvertFromNumerics(System.Numerics.Vector3 numericsVector3)
        {
            return new Vector3(numericsVector3.X, numericsVector3.Y, numericsVector3.Z);
        }

        public System.Numerics.Vector3 ConvertToNumerics()
        {
            return new System.Numerics.Vector3(x, y, z);
        }
    }
}
