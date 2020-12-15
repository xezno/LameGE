using System;
using System.Numerics;

namespace Engine.Utils.MathUtils
{
    public struct Vector3d : IVector3<double, Vector3d>
    {
        /// <summary>
        /// The point at which the vector resides on the X axis
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The point at which the vector resides on the Y axis
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The point at which the vector resides on the Z axis
        /// </summary>
        public double Z { get; set; }

        public double Magnitude => Math.Sqrt(SqrMagnitude);
        public double SqrMagnitude => X * X + Y * Y + Z * Z;

        public Vector3d Normalized
        {
            get
            {
                if (Math.Abs(Magnitude) < 0.0001f)
                    return new Vector3d(0, 0, 0);
                return this / Magnitude;
            }
        }

        public void Normalize() => this = Normalized;

        /// <summary>
        /// Construct a <see cref="Vector3d"/> with three initial values.
        /// </summary>
        /// <param name="x">The initial x coordinate</param>
        /// <param name="y">The initial y coordinate</param>
        /// <param name="z">The initial z coordinate</param>
        public Vector3d(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static Vector3d operator *(Vector3d a, Vector3d b) => new Vector3d(a.X * b.X,
                                                                                a.Y * b.Y,
                                                                                a.Z * b.Z);
        public static Vector3d operator -(Vector3d a, Vector3d b) => new Vector3d(a.X - b.X,
                                                                                a.Y - b.Y,
                                                                                a.Z - b.Z);
        public static Vector3d operator /(Vector3d a, Vector3d b) => new Vector3d(a.X / b.X,
                                                                                a.Y / b.Y,
                                                                                a.Z / b.Z);
        public static Vector3d operator +(Vector3d a, Vector3d b) => new Vector3d(a.X + b.X,
                                                                                a.Y + b.Y,
                                                                                a.Z + b.Z);

        public static Vector3d operator *(Vector3d a, double b) => new Vector3d(a.X * b,
                                                                            a.Y * b,
                                                                            a.Z * b);
        public static Vector3d operator -(Vector3d a, double b) => new Vector3d(a.X - b,
                                                                            a.Y - b,
                                                                            a.Z - b);
        public static Vector3d operator /(Vector3d a, double b) => new Vector3d(a.X / b,
                                                                            a.Y / b,
                                                                            a.Z / b);

        public static Vector3d operator +(Vector3d a, double b) => new Vector3d(a.X + b,
                                                                            a.Y + b,
                                                                            a.Z + b);

        public static Vector3d operator %(Vector3d a, double b) => new Vector3d(a.X % b,
                                                                            a.Y % b,
                                                                            a.Z % b);

        public static Vector3d up = new Vector3d(0, 1, 0);
        public static Vector3d right = new Vector3d(1, 0, 0);
        public static Vector3d forward = new Vector3d(0, 0, 1);
        public static Vector3d one = new Vector3d(1, 1, 1);

        /// <summary>
        /// Get all values within the <see cref="Vector3d"/> as a string.
        /// </summary>
        /// <returns>All coordinates (<see cref="X"/>, <see cref="Y"/> and <see cref="Z"/>) concatenated as a string.</returns>
        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }

        public static Vector3d ConvertFromNumerics(System.Numerics.Vector3 numericsVector3d)
        {
            return new Vector3d(numericsVector3d.X, numericsVector3d.Y, numericsVector3d.Z);
        }

        public System.Numerics.Vector3 ConvertToNumerics()
        {
            return new System.Numerics.Vector3((float)X, (float)Y, (float)Z);
        }

        public Vector3f ToVector3f()
        {
            return new Vector3f((float)X, (float)Y, (float)Z);
        }
    }
}
